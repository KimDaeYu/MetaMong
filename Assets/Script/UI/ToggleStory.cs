using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ToggleStory : MonoBehaviour
{
    public GameObject StoryView;

    public void OnValueChanged(bool isOn){
        StoryView.SetActive(isOn);
        if(!isOn){
            gameObject.GetComponent<Image>().color = new Color32(255,255,255,100);
        }else{
            gameObject.GetComponent<Image>().color = new Color32(255,255,255,255);
        }
    }
}
