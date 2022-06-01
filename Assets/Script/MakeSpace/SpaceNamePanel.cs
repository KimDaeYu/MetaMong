using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;
using static DBManager;

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
        Texture2D image = (Texture2D)Image.GetComponent<SetAnchorImage>()._screenShot;
        
        //Texture2D image = Image.GetComponent<Image>().texture;

        Debug.Log(_gps);
        Debug.Log(_space);
        Debug.Log(image);
        
        db.AddSpace(new DBManager.ARSpaceData
        {
            name = Content.GetComponent<TMP_InputField>().text,
            image = image,
            x = _gps.current_Long,
            y = _gps.current_Lat,
            tilt = _space.tilt,
            distance = _space.distance,
            compass = _space.compass,
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
