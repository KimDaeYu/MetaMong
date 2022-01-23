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

    public void printCmt(string _content, string _date, string _id){
        var info = CmtPrefab.transform.GetChild(1);
        //Content
        CmtPrefab.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = _content;
        //Date
        info.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = _date;
        //Name
        info.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = _id;

        var NewObject = Instantiate(CmtPrefab);
        NewObject.transform.SetParent(CmtList.transform);
        NewObject.transform.localScale = flag;
    }

    public void AddNewCmt(){
        //Content
        string _content = CmtContent.GetComponent<TMP_InputField>().text;
        CmtPrefab.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = _content;
        item.content = _content;

        var info = CmtPrefab.transform.GetChild(1);
        //Date
        string _date = System.DateTime.Now.ToString("yyyy-MM-dd");
        info.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = _date;
        item.date = _date;
        
        //Name
        string _id = "ID " + Random.Range(1000,2000).ToString();
        info.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = _id;
        item.id = _id;

        var NewObject = Instantiate(CmtPrefab);
        NewObject.transform.SetParent(CmtList.transform);
        NewObject.transform.localScale = flag;

        var Count = TargetStory.transform.GetChild(2).GetChild(1).GetChild(1).gameObject;
        var num = int.Parse(Count.GetComponent<TextMeshPro>().text) + 1;
        Count.GetComponent<TextMeshPro>().text = "00" + num.ToString();

        var dict = GameObject.Find("SelectionManager").GetComponent<FeedBackControl>().itemMap;
        if (dict.ContainsKey(TargetStory.name)){
            dict[TargetStory.name].Add(new CmtItem(_content,_date,_id));
        }else{
            List<CmtItem> __item = new List<CmtItem>();
            __item.Add(new CmtItem(_content,_date,_id));
            dict.Add(TargetStory.name, __item);
        }
        

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
