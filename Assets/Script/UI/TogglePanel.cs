using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TogglePanel : MonoBehaviour
{
    public GameObject SelectPanel;

    public void OnValueChanged(bool isOn){
        if(GameObject.Find("Canvas").transform.Find("CmtPanel").gameObject.activeSelf == true){ 
            GameObject.Find("Canvas").transform.Find("CmtPanel").gameObject.SetActive(false);
        }
        if(GameObject.Find("Canvas").transform.Find("CmtInputPanel").gameObject.activeSelf == true){ 
            GameObject.Find("Canvas").transform.Find("CmtInputPanel").gameObject.SetActive(false);
        }
        if(GameObject.Find("ObjectCanvas").transform.Find("OPanel").gameObject.activeSelf == true){ 
            GameObject.Find("Canvas").transform.Find("NewObject").gameObject.GetComponent<Toggle>().isOn = false;
        }

        SelectPanel.SetActive(isOn);
        if(!isOn){
            gameObject.GetComponent<Image>().color = new Color32(40,40,40,150);
        }else{
            gameObject.GetComponent<Image>().color = new Color32(40,40,40,255);
        }
    }
}
