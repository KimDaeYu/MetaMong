using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Comment : MonoBehaviour
{
    [SerializeField] GameObject CmtPanel;
    [SerializeField] GameObject CmtInputPanel;

    [SerializeField] GameObject CmtList;
    [SerializeField] GameObject CmtContent;

    [SerializeField] GameObject CmtPrefab;

    
    public GameObject TargetStory;
    public CmtItem item = new CmtItem("","","");

    public void ClickCmtCancel(){
        CmtPanel.SetActive(false);
    }

    public void ClickCmtOK(){
        CmtInputPanel.SetActive(true);
    }


    Vector3 flag = new Vector3(1,1,1);

    public void AddNewCmt(){
        //Content
        string _content = CmtContent.GetComponent<TMP_InputField>().text;
        GameObject.Find("SNSManager").GetComponent<SNSManager>().AddComment(TargetStory.name.Substring(5),_content);
        CancelNewCmt();
    }

    public void ClearCmtList(){
        Transform[] childList = CmtList.GetComponentsInChildren<Transform>();
        if(childList != null){
            for(int i=1; i < childList.Length; i++){
                if(childList[i] != transform){
                    Destroy(childList[i].gameObject);
                }
            }
        }
    }

    public void CancelNewCmt(){
        //clear
        CmtContent.GetComponent<TMP_InputField>().text = "";
        CmtInputPanel.SetActive(false);
    }
}
