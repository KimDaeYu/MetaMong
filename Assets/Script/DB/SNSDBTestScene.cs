using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;
public class SNSDBTestScene : MonoBehaviour
{
    public GameObject ui;
    public TMP_InputField spaceInput;
    public Button connectButton;
    public TMP_Text statusText;

    public GameObject postTemplate;
    public Transform postParent;

    public TMP_InputField postInput;
    public Button addPostButton;

    AuthManager auth;
    DBManager db;

    void Start()
    {
        ui.SetActive(false);

        connectButton.onClick.AddListener(ConnectSpace);
        addPostButton.onClick.AddListener(AddPost);

        auth = AuthManager.Instance;
        db = DBManager.Instance;

        auth.Load((loaded) =>
        {
            if (!loaded)
            {
                Debug.Log("Failed to load auth manager");
            }
            else
            {
                string email = "user1@test.com";
                string password = "121212";
                auth.SignIn(email, password, (error) =>
                {
                    if (error == AuthManager.SignInError.None)
                    {
                        ui.SetActive(true);
                    }
                    else
                    {
                        Debug.Log(error);
                    }
                });
            }
        });
    }

    void ConnectSpace()
    {
        if (db.currentSpaceId != null)
        {
            db.DisconnectARSpaceNode();
            postParent.DetachChildren();
            statusText.text = "disconnected";
            return;
        }

        if (string.IsNullOrWhiteSpace(spaceInput.text))
        {
            Debug.Log("spacetext is null or whitespace");
            return;
        }

        connectButton.interactable = false;
        db.ConnectARSpaceNode(spaceInput.text, PostAddedListener, (connected) => { 
            if (connected)
            {
                statusText.text = "connected to " + db.currentSpaceId;
            }
            else
            {
                Debug.Log("connection failed");
            }
            connectButton.interactable = true;
        });
    }

    void PostAddedListener(DBManager.Post post)
    {
        GameObject postObj = Instantiate(postTemplate, postParent);
        InitializePost(postObj, post);
        postObj.SetActive(true);
    }

    void InitializePost(GameObject postObj, DBManager.Post post)
    {
        var text = postObj.transform.Find("Text").GetComponent<TMP_Text>();
        var likesBtn = postObj.transform.Find("LikesButton").GetComponent<Button>();
        var commentsBtn = postObj.transform.Find("CommentsButton").GetComponent<Button>();
        var randomCommentBtn = postObj.transform.Find("RandomCommentButton").GetComponent<Button>();

        text.text = string.Format("{0}\nuser: {1}\ndate: {2}\n{3}", post.id, post.userId, post.date, post.content);
        likesBtn.GetComponentInChildren<TMP_Text>().text = string.Format("{0}/{1}", post.likes, post.like);
        likesBtn.onClick.AddListener(() =>
        {
            if (db.currentSpaceId == null)
            {
                return;
            }
            likesBtn.interactable = false;
            db.SetLike(post.id, !post.like).ContinueWith((result) =>
            {
                if (result != -1)
                {
                    likesBtn.GetComponentInChildren<TMP_Text>().text = string.Format("{0}/{1}", result, !post.like);
                }
                likesBtn.interactable = true;
            });
        });
        commentsBtn.GetComponentInChildren<TMP_Text>().text = post.comments.ToString();
        commentsBtn.onClick.AddListener(() =>
        {
            if (db.currentSpaceId == null)
            {
                return;
            }
            commentsBtn.interactable = false;
            db.GetComments(post.id).ContinueWith((comments) =>
            {
                if (comments == null)
                {
                    Debug.Log("Failed to load comments");
                }
                else
                {
                    Debug.Log(comments.Length);
                    if (comments.Length > 0)
                    {
                        Debug.Log(comments[comments.Length - 1].id);
                    }
                }
                commentsBtn.interactable = true;
            });
        });
        randomCommentBtn.onClick.AddListener(() => {
            if (db.currentSpaceId == null)
            {
                return;
            }
            randomCommentBtn.interactable = false;
            db.AddComment(post.id, System.DateTime.Now.ToString()).ContinueWith((comments) =>
            {
                if (comments != null)
                {
                    commentsBtn.GetComponentInChildren<TMP_Text>().text = comments.Length.ToString();
                    //Debug.Log(string.Format("{0}\n{1}\n{2}\n{3}", comment.id, comment.userId, comment.date, comment.content));
                }
                else
                {
                    Debug.Log("Failed to add comment");
                }
                randomCommentBtn.interactable = true;
            });
        });
    }

    void AddPost()
    {
        if (db.currentSpaceId == null)
        {
            return;
        }
        if (string.IsNullOrWhiteSpace(postInput.text))
        {
            Debug.Log("null or whitespace post");
        }

        addPostButton.interactable = false;
        db.AddPost(postInput.text, Vector3.zero).ContinueWith((post) =>
        {
            if (post == null)
            {
                Debug.Log("Failed to add post");
            }
            else
            {
                Debug.Log(post);
            }
            addPostButton.interactable = true;
        });
    }
}
