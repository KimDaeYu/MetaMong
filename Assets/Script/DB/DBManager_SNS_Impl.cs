using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using Firebase.Functions;
using Cysharp.Threading.Tasks;

public partial class DBManager : MonoBehaviour
{
    FirebaseFunctions functions;
    HttpsCallableReference addComment;
    HttpsCallableReference setLike;

    public partial class Post
    {
        public Post(string id, string userId, string userName, System.DateTime date, ContentType type, object content, Vector3 position, int likes, int comments, bool like)
        {
            this.id = id;
            this.userId = userId;
            this.userName = userName;
            this.date = date;
            this.type = type;
            this.content = content;
            this.position = position;
            this.likes = likes;
            this.comments = comments;
            this.like = like;
        }
    }

    double GetDoubleValue(DataSnapshot snapshot)
    {
        if (snapshot.Value is double)
        {
            return (double)snapshot.Value;
        }
        return 0;
    }

    async UniTask<Post> SnapshotToPost(DataSnapshot snapshot)
    {
        float x = (float)GetDoubleValue(snapshot.Child("x"));
        float y = (float)GetDoubleValue(snapshot.Child("y"));
        float z = (float)GetDoubleValue(snapshot.Child("z"));

        ContentType type = (ContentType)(long)snapshot.Child("type").Value;
        object content;
        if (type == ContentType.Text)
        {
            content = snapshot.Child("content").Value;
        }
        else
        {
            string path = snapshot.Child("content").Value as string;
            content = await DownloadImage(path);
        }
        
        return new Post(
            snapshot.Key,
            snapshot.Child("userId").Value as string,
            snapshot.Child("userName").Value as string,
            System.DateTime.FromBinary((long)snapshot.Child("date").Value),
            type,
            content,
            new Vector3(x, y, z),
            (int)(long)snapshot.Child("likes").Value,
            (int)(long)snapshot.Child("comments").Value,
            likes.Contains(snapshot.Key)
        );
    }

    System.Action<bool> connectARSpaceNodeCallback;
    DatabaseReference currentSpaceRef;
    System.Action<Post> postAddedListener;
    DatabaseReference likesRef;
    HashSet<string> likes;

    void InitSNS()
    {
        functions = FirebaseFunctions.GetInstance(app, "asia-northeast3");
        addComment = functions.GetHttpsCallable("addComment");
        setLike = functions.GetHttpsCallable("setLike");
        likes = new HashSet<string>();
    }

    void HandlePostAdded(object sender, ChildChangedEventArgs args)
    {
        SnapshotToPost(args.Snapshot).ContinueWith(postAddedListener);
    }

    /// <summary>
    /// AR공간에 연결 시도 <br/>
    /// 다른 공간에 연결된 상태에서 호출시 연결 해제 후 연결 시도
    /// </summary>
    /// <param name="spaceId">연결할 공간 ID</param>
    /// <param name="postAddedListener">공간에 포스트가 추가되었을 때 호출될 함수</param>
    /// <param name="callback">연결 성공/실패시 호출될 함수. 성공 여부가 인자로 주어짐</param>
    public partial void ConnectARSpaceNode(string spaceId, System.Action<Post> postAddedListener, System.Action<bool> callback)
    {
        DisconnectARSpaceNode();
        connectARSpaceNodeCallback = callback;
        this.postAddedListener = postAddedListener;

        likesRef = db.GetReference("likes/" + AuthManager.Instance.user.userId + "/" + spaceId);
        likesRef.ValueChanged += HandleLikesChanged;

        currentSpaceRef = db.GetReference("posts/" + spaceId);  // 쿼리로 얻어진 존재하는 spaceId만 주어진다고 가정하고 체크 생략
    }

    /// <summary>
    /// AR공간 연결 해제 <br/>
    /// 연결되어 있지 않은 상태에서 호출시 아무 일도 하지 않음
    /// </summary>
    public partial void DisconnectARSpaceNode()
    {
        if (currentSpaceRef != null)
        {
            currentSpaceRef.ChildAdded -= HandlePostAdded;
        }
        if (likesRef != null)
        {
            likesRef.ValueChanged -= HandleLikesChanged;
            likesRef.ChildAdded -= HandleLikeAdded;
            likesRef.ChildRemoved -= HandleLikeRemoved;
        }
        currentSpaceRef = null;
        currentSpaceId = null;
        likesRef = null;
        postAddedListener = null;
        likes.Clear();
    }

    void HandleLikeAdded(object sender, ChildChangedEventArgs args)
    {
        likes.Add(args.Snapshot.Key);
    }

    void HandleLikeRemoved(object sender, ChildChangedEventArgs args)
    {
        likes.Remove(args.Snapshot.Key);
    }

    // DatabaseError 핸들링?
    void HandleLikesChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.Log(args.DatabaseError);
            DisconnectARSpaceNode();
            connectARSpaceNodeCallback(false);
        }
        if (args.Snapshot != null)
        {
            foreach (var snapshot in args.Snapshot.Children)
            {
                likes.Add(snapshot.Key);
            }
        }
        likesRef.ChildAdded += HandleLikeAdded;
        likesRef.ChildRemoved += HandleLikeRemoved;
        likesRef.ValueChanged -= HandleLikesChanged;
        currentSpaceRef.ChildAdded += HandlePostAdded;
        currentSpaceId = currentSpaceRef.Key;
        connectARSpaceNodeCallback(true);
    }

    static Post NewPost(string id, ContentType contentType, object content, Vector3 position)
    {
        var user = AuthManager.Instance.user;   // 로그인 된 상태에서만 호출된다고 가정하고 체크 생략
        return new Post(id, user.userId, user.name, System.DateTime.Now, contentType, content, position, 0, 0, false);
    }

    async UniTask<Post> AddPost(ContentType contentType, object content, Vector3 position)
    {
        // Space에 Connect된 경우에만 호출된다고 가정하고 체크 생략

        var newRef = currentSpaceRef.Push();
        Post post = NewPost(newRef.Key, contentType, content, position);

        var data = new Dictionary<string, object>
        {
            {"userId", post.userId },
            {"userName", post.userName },
            {"date", post.date.ToBinary() },
            {"type", (int)post.type },
            {"x", post.position.x },
            {"y", post.position.y },
            {"z", post.position.z },
            {"likes", post.likes },
            {"comments", post.comments },
        };

        if (contentType == ContentType.Image)
        {
            string imageLocation = "posts/" + currentSpaceRef.Key + "/" + newRef.Key;
            if (! await UploadImage(imageLocation, content as Texture2D))
            {
                return null;
            }
            data.Add("content", imageLocation);
        }
        else
        {
            data.Add("content", content);
        }

        if (!await SetValue(newRef, data))
        {
            return null;
        }

        return post;
    }

    /// <summary>
    /// 현재 연결 된 공간에 포스트 추가
    /// </summary>
    /// <param name="content">포스트 내용</param>
    /// <param name="position">좌표값</param>
    /// <returns>성공시 추가된 포스트, 실패시 null</returns>
    public partial async UniTask<Post> AddPost(string content, Vector3 position)
    {
        return await AddPost(ContentType.Text, content, position);
    }

    /// <summary>
    /// 현재 연결 된 공간에 포스트 추가
    /// </summary>
    /// <param name="content">
    /// 포스트 내용 <br/>
    /// isRedable이 true여야 함
    /// </param>
    /// <param name="position">좌표값</param>
    /// <returns>성공시 추가된 포스트, 실패시 null</returns>
    public partial async UniTask<Post> AddPost(Texture2D content, Vector3 position)
    {
        return await AddPost(ContentType.Image, content, position);
    }

    /*public partial void AddPost(string content, Vector3 position, System.Action<Post> callback)
    {
        AddPost(ContentType.Text, content, position).ContinueWith(callback);
    }

    public partial void AddPost(Texture2D content, Vector3 position, System.Action<Post> callback)
    {
        AddPost(ContentType.Image, content, position).ContinueWith(callback);
    }*/

    static Comment NewComment(string content)
    {
        var user = AuthManager.Instance.user;   // 로그인 된 상태에서만 호출된다고 가정하고 체크 생략
        return new Comment
        {
            userId = user.userId,
            userName = user.name,
            date = System.DateTime.Now,
            content = content,
        };
    }

    /// <summary>
    /// 포스트에 댓글 추가
    /// </summary>
    /// <param name="postId">포스트 ID</param>
    /// <param name="content">댓글 내용</param>
    /// <returns>성공시 포스트의 전체 댓글 배열, 실패시 null</returns>
    public partial async UniTask<Comment[]> AddComment(string postId, string content)
    {
        string spaceId = currentSpaceRef.Key;   // Space에 Connect된 경우에만 호출된다고 가정하고 체크 생략

        Comment comment = NewComment(content);
        var data = new Dictionary<string, object>
        {
            {"userId", comment.userId },
            {"spaceId", spaceId },
            {"postId", postId },
            {"userName", comment.userName },
            {"date", comment.date.ToBinary() },
            {"content", comment.content },
        };

        var result = await CallFunction(addComment, data);
        if (result == null)
        {
            return null;
        }

        var comments = await GetComments(postId);

        return comments;
    }

    /*public partial void AddComment(string postId, string content, System.Action<Comment[]> callback)
    {
        AddComment(postId, content).ContinueWith(callback);
    }*/

    static Comment SnapshotToComment(DataSnapshot snapshot)
    {
        return new Comment
        {
            id = snapshot.Key,
            userId = snapshot.Child("userId").Value as string,
            userName = snapshot.Child("userName").Value as string,
            date = System.DateTime.FromBinary((long)snapshot.Child("date").Value),
            content = snapshot.Child("content").Value as string,
        };
    }

    /// <summary>
    /// 포스트의 전체 댓글 가져오기
    /// </summary>
    /// <param name="postId">포스트 ID</param>
    /// <returns>성공시 포스트의 전체 댓글 배열, 실패시 null</returns>
    public partial async UniTask<Comment[]> GetComments(string postId)
    {
        // Space에 Connect된 경우에만 호출된다고 가정하고 체크 생략

        string path = path = "comments/" + currentSpaceRef.Key + "/" + postId;
        var snapshot = await GetValue(db.GetReference(path));
        if (snapshot == null)
        {
            return null;
        }

        var comments = new Comment[snapshot.ChildrenCount];
        int i = 0;
        foreach (var commentSnapshot in snapshot.Children)
        {
            comments[i++] = SnapshotToComment(commentSnapshot);
        }
        return comments;
    }

    /*public partial void GetComments(string postId, System.Action<Comment[]> callback)
    {
        GetComments(postId).ContinueWith(callback);
    }*/

    /// <summary>
    /// 포스트에 좋아요/해제
    /// </summary>
    /// <param name="postId">포스트 ID</param>
    /// <param name="like">좋아요 - true, 해제 - false</param>
    /// <returns>성공시 포스트의 전체 좋아요 수, 실패시 -1</returns>
    public partial async UniTask<int> SetLike(string postId, bool like)
    {
        if (likes.Contains(postId) == like)
        {
            return -1;
        }

        var data = new Dictionary<string, object>
        {
            {"userId", AuthManager.Instance.user.userId },
            {"spaceId", currentSpaceRef.Key },
            {"postId", postId },
            {"like", like },
        };
        var result = await CallFunction(setLike, data);
        if (result == null || (long)result == 0)
        {
            return -1;
        }

        return await GetLikes(postId);
    }

    /*public partial void SetLike(string postId, bool like, System.Action<int> callback)
    {
        SetLike(postId, like).ContinueWith(callback);
    }*/

    /// <summary>
    /// 포스트의 전체 좋아요 수 가져오기
    /// </summary>
    /// <param name="postId">포스트 ID</param>
    /// <returns>성공시 포스트의 전체 좋아요 수, 실패시 -1</returns>
    public partial async UniTask<int> GetLikes(string postId)
    {
        var result = await GetValue(currentSpaceRef.Child(postId).Child("likes"));
        if (result == null)
        {
            return -1;
        }

        return (int)(long)result.Value;
    }
}
