using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectInput : MonoBehaviour
{
    public GameObject InputPanel;
    public GameObject SelectPanel;

    public void TextClick(){
        InputPanel.SetActive(true);
        SelectPanel.SetActive(false);
    }

    public void ImageClick(){
        GameObject.Find("StoryManager").GetComponent<Gallery>().PickImage(500);
        SelectPanel.SetActive(false);
    }
}
