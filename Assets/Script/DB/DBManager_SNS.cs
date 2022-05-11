using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public partial class DBManager : MonoBehaviour
{
    public partial class Post
    {
        public string id;
        public string userId;
        public string userName;
        public System.DateTime date;
        public ContentType type;

        ///<summary>
        ///<see cref="type"/>이 <see cref="ContentType.Text"/>이면 <see cref="string"/><br/>
        ///<see cref="type"/>이 <see cref="ContentType.Image"/>이면 <see cref="Texture2D"/>
        ///</summary>
        // type이 Text이면 string, Image이면 Texture2D
        public object content;

        public Vector3 position;

        ///<summary>좋아요 수</summary>
        public int likes;

        ///<summary>댓글 수</summary>
        public int comments;

        ///<summary>사용자가 좋아요 했는지 여부</summary>
        public bool like;
    }

    public enum ContentType
    {
        Text,
        Image
    }

    public class Comment
    {
        public string id;
        public string userId;
        public string userName;
        public System.DateTime date;
        public string content;
    }

    /// <summary>
    /// 현재 연결된 공간의 ID <br/>
    /// 공간에 연결되지 않은 경우 null
    /// </summary>
    public string currentSpaceId { get; private set; }

    // AR공간에 연결 시도
    // spaceId: 연결할 공간 ID
    // postAddrListener: 공간에 포스트가 추가되었을 때 호출될 함수
    // callback: 연결 성공/실패시 호출될 함수. 성공 여부가 인자로 주어짐
    // 다른 공간에 연결된 상태에서 호출시 연결 해제 후 연결 시도
    public partial void ConnectARSpaceNode(string spaceId, System.Action<Post> postAddedListener, System.Action<bool> callback);

    // AR공간 연결 해제
    // 연결되어 있지 않은 상태에서 호출시 아무 일도 하지 않음.
    public partial void DisconnectARSpaceNode();


    /* 이하 함수들은 먼저 AR공간에 연결한 뒤에 호출해야 함 */

    // 현재 연결 된 공간에 포스트 추가
    // content: 포스트 내용(string or Texture2D)
    //          Texture2D인 경우 isReadable이 true여야 함
    //          NativeGallery 사용하는 경우 Load할 때 markTextureNonReadable: false 를 설정
    // position: 좌표값
    // return: 성공시 추가된 포스트, 실패시 null
    public partial UniTask<Post> AddPost(string content, Vector3 position);
    public partial UniTask<Post> AddPost(Texture2D content, Vector3 position);


    // 포스트에 댓글 추가
    // postId: 포스트 ID
    // content: 댓글 내용
    // return: 성공시 포스트의 전체 댓글 배열, 실패시 null
    public partial UniTask<Comment[]> AddComment(string postId, string content);

    // 포스트의 전체 댓글 가져오기
    // postId: 포스트 ID
    // return: 성공시 포스트의 전체 댓글 배열, 실패시 null
    public partial UniTask<Comment[]> GetComments(string postId);


    // 포스트에 좋아요/해제
    // postId: 포스트 ID
    // like: 좋아요 - true, 해제 - false
    // return: 성공시 포스트의 전체 좋아요 수, 실패시 -1
    public partial UniTask<int> SetLike(string postId, bool like);


    // 포스트의 전체 좋아요 수 가져오기
    // postId: 포스트 ID
    // return: 성공시 포스트의 전체 댓글 배열, 실패시 null
    public partial UniTask<int> GetLikes(string postId);
}
