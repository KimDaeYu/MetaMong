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
    public void ClickCmtCancel(){
        CmtPanel.SetActive(false);
    }

    public void ClickCmtOK(){
        CmtInputPanel.SetActive(true);
    }


    Vector3 flag = new Vector3(1,1,1);

    public void AddNewCmt(){
        //Content
        CmtPrefab.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = CmtContent.GetComponent<TMP_InputField>().text;
        
        var info = CmtPrefab.transform.GetChild(1);
        //Date
        info.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = System.DateTime.Now.ToString("yyyy-MM-dd");
        //Name
        info.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = "ID " + Random.Range(1000,2000).ToString();
        
        var NewObject = Instantiate(CmtPrefab);
        NewObject.transform.SetParent(CmtList.transform);
        NewObject.transform.localScale = flag;

        var Count = TargetStory.transform.GetChild(2).GetChild(1).GetChild(1).gameObject;
        var num = int.Parse(Count.GetComponent<TextMeshPro>().text) + 1;
        Count.GetComponent<TextMeshPro>().text = "00" + num.ToString();
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
