using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using TMPro;

public class FeedBackControl : MonoBehaviour
{
    public ARRaycastManager arRaycaster;
    public GameObject CommentPanel;

    [SerializeField] GameObject UIManager;
    // Update is called once per frame
    void Update()
    {
        Touched();
    }


    private bool touched = false;
    Transform target;
    GameObject Story;

    private void Touched(){
        if(Input.GetMouseButton(0) && !touched){
            touched = true;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray,out hit, 10)){
                //Comment Slected
                if(hit.collider.gameObject.CompareTag("Comment")){
                    Story = hit.collider.gameObject.transform.parent.parent.gameObject;
                    GameObject Info = Story.transform.GetChild(1).gameObject;
                    GameObject Name = Info.transform.GetChild(1).gameObject;

                    Debug.Log(Name.GetComponent<TextMeshPro>().text.Substring(3));
                    
                    GameObject.Find("Log").GetComponent<TextMeshProUGUI>().text += System.DateTime.Now.ToString() + " " + Story.name + " Hit!!!\n";
                    CommentPanel.SetActive(true);
                    UIManager.GetComponent<Comment>().TargetStory = Story;
                    UIManager.GetComponent<Comment>().ClearCmtList();
                    //댓글 만드는 오브젝트 함수
                }

                if(hit.collider.gameObject.CompareTag("Like")){
                    Story = hit.collider.gameObject.transform.parent.parent.gameObject;
                    GameObject Info = Story.transform.GetChild(1).gameObject;
                    GameObject Name = Info.transform.GetChild(1).gameObject;

                    Debug.Log(Name.GetComponent<TextMeshPro>().text.Substring(3));
                    GameObject.Find("Log").GetComponent<TextMeshProUGUI>().text += System.DateTime.Now.ToString() + " " + Story.name + " Hit!!!\n";
                    
                    var Like = Story.transform.GetChild(2).GetChild(0).gameObject;
                    if(Like.transform.GetChild(0).gameObject.activeSelf == true){ 
                        var Count = Like.transform.GetChild(1).gameObject;
                        var num = int.Parse(Count.GetComponent<TextMeshPro>().text) + 1;
                        Count.GetComponent<TextMeshPro>().text = "00" + num.ToString();
                        Like.transform.GetChild(0).gameObject.SetActive(false);
                        Like.transform.GetChild(2).gameObject.SetActive(true);
                    }else{
                        var Count = Like.transform.GetChild(1).gameObject;
                        var num = int.Parse(Count.GetComponent<TextMeshPro>().text) - 1;
                        Count.GetComponent<TextMeshPro>().text = "00" + num.ToString();
                        Like.transform.GetChild(0).gameObject.SetActive(true);
                        Like.transform.GetChild(2).gameObject.SetActive(false);
                    }
                    //UIManager.GetComponent<Comment>().TargetStory = Story;
                    //댓글 만드는 오브젝트 함수
                }
            }
        }

        if(Input.GetMouseButtonUp(0)){
            touched = false;
        }
    }
}
