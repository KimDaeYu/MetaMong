using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;
public class SNSManager : MonoBehaviour
{
    public GameObject ui;
    public string spaceId;
    public Button connectButton;
    public TMP_Text statusText;

    public GameObject TextPostTemplate;
    public GameObject ImagePostTemplate;
    public GameObject PImagePostTemplate;
    public Transform postParent;

    public TMP_InputField postInput;
    public Button addPostButton;

    AuthManager auth;
    DBManager db;

    [SerializeField] GameObject CmtList;
    [SerializeField] GameObject CmtContent;
    [SerializeField] GameObject CmtPrefab;

    void Start()
    {
        //ui.SetActive(false);

        connectButton.onClick.AddListener(ConnectSpace);
        //addPostButton.onClick.AddListener(AddPost);

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
                        //ui.SetActive(true);
                        Debug.Log("Connect Success");
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

        if (string.IsNullOrWhiteSpace(spaceId))
        {
            Debug.Log("spacetext is null or whitespace");
            return;
        }

        connectButton.interactable = false;
        db.ConnectARSpaceNode(spaceId, PostAddedListener, (connected) => { 
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

    bool landscape = true;
    void PostAddedListener(DBManager.Post post)
    {  
        if(post.type == DBManager.ContentType.Image){
            Debug.Log("Add Image!");
            Texture2D content = post.content as Texture2D;
            if(content.width > content.height){
                GameObject IpostObj = Instantiate(ImagePostTemplate, postParent);
                IpostObj.transform.Find("Image").GetComponent<RectTransform>().sizeDelta = new Vector2(800,content.height * 800 / content.width );
                landscape = true;
                InitializePost(IpostObj, post);
                IpostObj.SetActive(true);
            }else{
                GameObject IpostObj = Instantiate(PImagePostTemplate, postParent);
                IpostObj.transform.Find("Image").GetComponent<RectTransform>().sizeDelta = new Vector2(content.width * 800 / content.height ,800);
                landscape = false;
                InitializePost(IpostObj, post);
                IpostObj.SetActive(true);
            }
            GameObject postObj = Instantiate(ImagePostTemplate, postParent);
            
        }else{
            GameObject postObj = Instantiate(TextPostTemplate, postParent);
            InitializePost(postObj, post);
            postObj.SetActive(true);
        }
    }

    void InitializePost(GameObject postObj, DBManager.Post post)
    {
        if(post.type == DBManager.ContentType.Image){
            Vector3 InfoPos;
            Texture2D content = post.content as Texture2D;
            Rect rect = new Rect(0, 0, content.width, content.height);
            Sprite img = Sprite.Create(content, rect, new Vector2(0.5f, 0.5f));
            var Temp = postObj.transform.Find("Image").GetComponent<SpriteRenderer>();
            Temp.sprite = img;
            if(landscape){
                Temp.size = new Vector2(3,img.bounds.size[1] * 3 / img.bounds.size[0]);
                InfoPos = new Vector3(0,( (2f - Temp.size[1]) / 2f), 0);
                //SelectedPrefabs.transform.GetChild(0).gameObject.transform.localPosition = new Vector3(0,((2f - Temp.size[1]) / 2f) * -1, 0);
            }else{
                Temp.size = new Vector2(img.bounds.size[0] * 3 / img.bounds.size[1],3);
                InfoPos = new Vector3(0.3f + ((2f - Temp.size[0]) / 2f),0,0);
                //SelectedPrefabs.transform.GetChild(0).gameObject.transform.localPosition = new Vector3( -0.3f - ((2f - Temp.size[0]) / 2f),0,0);
            }
            Temp.flipX = true;
            postObj.transform.Find("Info").transform.localPosition = InfoPos;
            if(landscape){
                postObj.transform.Find("FeedBack").transform.localPosition = InfoPos;
            }
        }else{
            TextMeshPro content = postObj.transform.Find("Content").GetComponent<TextMeshPro>();
            content.text = post.content as string;
        }
        
        TextMeshPro date = postObj.transform.Find("Info/Date").GetComponent<TextMeshPro>();
        TextMeshPro user = postObj.transform.Find("Info/Name").GetComponent<TextMeshPro>();
        TextMeshPro like_count = postObj.transform.Find("FeedBack/Like/Count").GetComponent<TextMeshPro>();
        TextMeshPro comment_count = postObj.transform.Find("FeedBack/Comment/Count").GetComponent<TextMeshPro>();
        
        var Like = postObj.transform.Find("FeedBack/Like");
        if(post.like){ 
            Like.transform.GetChild(0).gameObject.SetActive(false);
            Like.transform.GetChild(2).gameObject.SetActive(true);
        }else{
            Like.transform.GetChild(0).gameObject.SetActive(true);
            Like.transform.GetChild(2).gameObject.SetActive(false);
        }

        postObj.name = "Story" + post.id;
        //content.text = post.content;
        date.text = String.Format("{0:yyyy-MM-dd}", post.date);
        user.text = "By " + post.userName;
        like_count.text = String.Format("{0:000}", post.likes);
        comment_count.text = String.Format("{0:000}", post.comments);

        //position update
        postObj.transform.position = post.position;
    }

    public int ClickedLike(string postid, bool like){
        Debug.Log("Try!");
        Debug.Log(postid);
        if (db.currentSpaceId == null)
        {
            Debug.Log("null2!");
            return -1;
        }
        var Count = 0;
        db.SetLike(postid, like).ContinueWith((result) =>
        {
            Debug.Log("end!!");
            Debug.Log(result);
            if (result != -1)
            {
                //likesBtn.GetComponentInChildren<TMP_Text>().text = string.Format("{0}/{1}", result, !post.like);
            }
            Count = result;
        });
        return Count;
    }

    public void GetComments(string postid){
        Debug.Log("Try!");
        Debug.Log(postid);
        if (db.currentSpaceId == null)
        {
            Debug.Log("null!");
            return;
        }
        var result = db.GetComments(postid).ContinueWith((comments) =>
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
                foreach (var cmt in comments)
                {
                    var info = CmtPrefab.transform.GetChild(1);
                    //Content
                    CmtPrefab.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = cmt.content;
                    //Date
                    info.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = String.Format("{0:yyyy-MM-dd}", cmt.date);;
                    //Name
                    info.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = "By " + cmt.userName;

                    var NewObject = Instantiate(CmtPrefab);
                    NewObject.transform.SetParent(CmtList.transform);
                    NewObject.transform.localScale = new Vector3(1,1,1);
                }
            } 
        });
        Debug.Log(result);
        return;
    }


    public void AddPost(string content, Vector3 pos)
    {
        if (db.currentSpaceId == null)
        {
            return;
        }
        if (string.IsNullOrWhiteSpace(content))
        {
            Debug.Log("null or whitespace post");
        }

        db.AddPost(content, pos).ContinueWith((post) =>
        {
            if (post == null)
            {
                Debug.Log("Failed to add post");
            }
            else
            {
                Debug.Log(post);
            }
        });
    }

    public void AddPost(Texture2D content, Vector3 pos)
    {
        if (db.currentSpaceId == null)
        {
            return;
        }
        if (content == null)
        {
            Debug.Log("null or whitespace post");
        }

        db.AddPost(content, pos).ContinueWith((post) =>
        {
            if (post == null)
            {
                Debug.Log("Failed to add post");
            }
            else
            {
                Debug.Log(post);
            }
        });
    }

    
    public void AddComment(string postid, string content)
    {
        if (db.currentSpaceId == null)
        {
            return;
        }
        db.AddComment(postid, content).ContinueWith((comments) =>
        {
            if (comments != null)
            {
                GameObject.Find("Story"+postid).transform.Find("FeedBack/Comment/Count").GetComponentInChildren<TMP_Text>().text = String.Format("{0:000}", comments.Length);
                GameObject.Find("UIManager").GetComponent<Comment>().ClearCmtList();
                GameObject.Find("SNSManager").GetComponent<SNSManager>().GetComments(postid);
            }
            else
            {
                Debug.Log("Failed to add comment");
            }
        });
    }
}
