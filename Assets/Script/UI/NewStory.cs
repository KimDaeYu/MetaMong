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

    public List<Vector3> testset;
    public GameObject SNSManager;
    public float distance = 0.8f;

    public Vector3 targetPos = new Vector3(0,0,0); 
    public Sprite targetImage;

    public Texture2D targetTexture2D;
    public bool landscape = true;
    public void AddNewStoryText(Vector3 Pos){
        SNSManager.GetComponent<SNSManager>().AddPost(Content.GetComponent<TMP_InputField>().text, GameObject.Find("ARLocationRoot").transform.InverseTransformPoint(Pos));
    }




    public void ClickTextOK(){
        var TmpPos = new Vector3(0,0,0);
        GameObject.Find("Log").GetComponent<TextMeshProUGUI>().text += System.DateTime.Now.ToString() + " target " + targetPos.ToString() + "\n";
        if(targetPos == new Vector3(0,0,0)){
            TmpPos = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, distance));
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
        SNSManager.GetComponent<SNSManager>().AddPost(duplicateTexture(targetTexture2D), GameObject.Find("ARLocationRoot").transform.InverseTransformPoint(Pos));
    }
    Texture2D duplicateTexture(Texture2D source)
    {
        RenderTexture renderTex = RenderTexture.GetTemporary(
                    source.width,
                    source.height,
                    0,
                    RenderTextureFormat.Default,
                    RenderTextureReadWrite.Linear);

        Graphics.Blit(source, renderTex);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = renderTex;
        Texture2D readableText = new Texture2D(source.width, source.height);
        readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
        readableText.Apply();
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTex);
        return readableText;
    }
    public void ClickImageOK(){
        var TmpPos = new Vector3(0,0,0);
        GameObject.Find("Log").GetComponent<TextMeshProUGUI>().text += System.DateTime.Now.ToString() + " target " + targetPos.ToString() + "\n";
        if(targetPos == new Vector3(0,0,0)){
            TmpPos = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, distance));
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

    public void MakeInstance(){
        foreach (var item in testset)
        {
            var NewObject = Instantiate(StoryText, item, Quaternion.identity);
            NewObject.name = "Story" + Random.Range(1000,2000).ToString();
            NewObject.transform.SetParent(StoryView.transform);
            //NewObject.transform.GetChild(1).gameObject.transform.localPosition = item;
        }
    }

}
