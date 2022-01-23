using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NewStory : MonoBehaviour
{
    [SerializeField] GameObject StoryView;
    [SerializeField] GameObject StoryImage;
    [SerializeField] GameObject StoryImageP;
    [SerializeField] GameObject StoryText;
    [SerializeField] GameObject Content;

    public Vector3 targetPos = new Vector3(0,0,0); 
    public Sprite targetImage;
    public bool landscape = true;
    public void AddNewStoryText(Vector3 Pos){
        
        //Content
        StoryText.transform.GetChild(0).gameObject.GetComponent<TextMeshPro>().text = Content.GetComponent<TMP_InputField>().text;
        
        var info = StoryText.transform.GetChild(1);
        //Date
        info.transform.GetChild(0).gameObject.GetComponent<TextMeshPro>().text = System.DateTime.Now.ToString("yyyy-MM-dd");
        //Name
        info.transform.GetChild(1).gameObject.GetComponent<TextMeshPro>().text = "ID " + Random.Range(1000,2000).ToString();
        
        var NewObject = Instantiate(StoryText, Pos, Quaternion.identity);
        NewObject.name = "Story" + Random.Range(1000,2000).ToString();
        NewObject.transform.SetParent(StoryView.transform);
    }




    public void ClickTextOK(){
        var TmpPos = new Vector3(0,0,0);
        GameObject.Find("Log").GetComponent<TextMeshProUGUI>().text += System.DateTime.Now.ToString() + " target " + targetPos.ToString() + "\n";
        if(targetPos == new Vector3(0,0,0)){
            TmpPos = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 5f));
        }else{
            TmpPos = targetPos;
            targetPos = new Vector3(0,0,0);
        }

        AddNewStoryText(TmpPos);
        ClickTextCancel();
    }
    public void ClickTextCancel(){
        //clear
        Content.GetComponent<TMP_InputField>().text = "";
        GameObject.Find("InputPanel").SetActive(false);
        GameObject.Find("NewStory").GetComponent<Toggle>().isOn = false;
    }

    public void AddNewStoryImage(Vector3 Pos){
        Vector3 InfoPos;
        //Content
        var SelectedPrefabs = StoryImage;
        if(!landscape){
            SelectedPrefabs = StoryImageP;
        }
        
        var Temp = SelectedPrefabs.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
        Temp.sprite = targetImage;
        //Size 
        if(landscape){
            Temp.size = new Vector2(3,targetImage.bounds.size[1] * 3 / targetImage.bounds.size[0]);
            InfoPos = new Vector3(0,( (2f - Temp.size[1]) / 2f), 0);
            GameObject.Find("Log").GetComponent<TextMeshProUGUI>().text += "Landscape! pos " + Temp.size.ToString() + "\n";
            //SelectedPrefabs.transform.GetChild(0).gameObject.transform.localPosition = new Vector3(0,((2f - Temp.size[1]) / 2f) * -1, 0);
        }else{
            Temp.size = new Vector2(targetImage.bounds.size[0] * 3 / targetImage.bounds.size[1],3);
            InfoPos = new Vector3(0.3f + ((2f - Temp.size[0]) / 2f),0,0);
            GameObject.Find("Log").GetComponent<TextMeshProUGUI>().text += "Protrait! pos " + Temp.size.ToString() + "\n";
            //SelectedPrefabs.transform.GetChild(0).gameObject.transform.localPosition = new Vector3( -0.3f - ((2f - Temp.size[0]) / 2f),0,0);
        }
        SelectedPrefabs.transform.GetChild(0).gameObject.GetComponent<BoxCollider>().size = new Vector3(Temp.size[0],Temp.size[1],0.1f);
        //Info 
        var info = SelectedPrefabs.transform.GetChild(1);
        //Date
        info.transform.GetChild(0).gameObject.GetComponent<TextMeshPro>().text = System.DateTime.Now.ToString("yyyy-MM-dd");
        //Name
        info.transform.GetChild(1).gameObject.GetComponent<TextMeshPro>().text = "ID " + Random.Range(1000,2000).ToString();
        Temp.flipX = true;
        
        
        
        var NewObject = Instantiate(SelectedPrefabs, Pos, Quaternion.identity);
        NewObject.transform.SetParent(StoryView.transform);
        NewObject.transform.GetChild(1).gameObject.transform.localPosition = InfoPos;
        if(landscape){
            NewObject.transform.GetChild(2).gameObject.transform.localPosition = InfoPos;
        }
    }

    public void ClickImageOK(){
        var TmpPos = new Vector3(0,0,0);
        GameObject.Find("Log").GetComponent<TextMeshProUGUI>().text += System.DateTime.Now.ToString() + " target " + targetPos.ToString() + "\n";
        if(targetPos == new Vector3(0,0,0)){
            TmpPos = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 5f));
        }else{
            TmpPos = targetPos;
            targetPos = new Vector3(0,0,0);
        }

        AddNewStoryImage(TmpPos);
        ClickImageCancel();
    }
    public void ClickImageCancel(){
        //clear
        GameObject.Find("NewStory").GetComponent<Toggle>().isOn = false;
        gameObject.GetComponent<Gallery>().ImagePanel.SetActive(false);
    }
}
