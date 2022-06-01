using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;

public class TogglePointAR : MonoBehaviour
{
    public ARPointCloudManager pointCloudManager;

    public void OnValueChanged(bool isOn){
        VisualizePoints(isOn);
        if(!isOn){
            gameObject.GetComponent<Image>().color = new Color32(255,255,255,150);
        }else{
            gameObject.GetComponent<Image>().color = new Color32(255,255,255,255);
        }
    }
    public void VisualizePoints(bool active){
        pointCloudManager.enabled = active;
        foreach (ARPointCloud point in pointCloudManager.trackables){
            point.gameObject.SetActive(active);
        }
    }

}
