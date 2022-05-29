using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;

public class SpaceNamePanel : MonoBehaviour
{
    public GameObject SelectPanel;
    public GameObject Content;
    public GameObject Image;
    DBManager db;
    GPSManager _gps;
    SpaceInstance _space;  
    void Start()
    {
        db = DBManager.Instance;
        _gps = GPSManager.Instance;
        _space = SpaceInstance.Instance;
    }
    public void OnValueChanged(bool isOn){
        SelectPanel.SetActive(isOn);
        if(!isOn){
            gameObject.GetComponent<Image>().color = new Color32(40,40,40,150);
        }else{
            gameObject.GetComponent<Image>().color = new Color32(40,40,40,255);
        }
    }

    public void ClickTextCancel(){
        //clear
        Content.GetComponent<TMP_InputField>().text = "";
        GetComponent<Toggle>().isOn = false;
    }


    public void check()
    {
        Texture2D image = Image.GetComponent<SetAnchorImage>()._screenShot;
        
        db.AddSpace(new DBManager.ARSpaceData
        {
            name = Content.GetComponent<TMP_InputField>().text,
            image = image,
            x = 88f,//_gps.current_Lat,
            y = 88f,//_gps.current_Long,
            tilt = 88f,//_space.tilt,
            distance = 88f,//_space.distance,
            compass = 88f,//_space.compass,
            radius = 50,
        }).ContinueWith((space) =>
        {
            if (space == null)
            {
                Debug.Log("Failed to add space");
                //Debug.Log(space_gps);
            }
            else
            {
                Debug.Log(space);
            }

        }).Forget();
        
    }
}
