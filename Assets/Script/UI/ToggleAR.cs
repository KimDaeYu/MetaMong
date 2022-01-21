using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;

public class ToggleAR : MonoBehaviour
{
    public ARPlaneManager planeManager;
    public ARPointCloudManager pointCloudManager;

    public void OnValueChanged(bool isOn){
        VisualizePlanes(isOn);
        VisualizePoints(isOn);
        if(!isOn){
            gameObject.GetComponent<Image>().color = new Color32(255,255,255,100);
        }else{
            gameObject.GetComponent<Image>().color = new Color32(255,255,255,255);
        }
    }

    void VisualizePlanes(bool active){
        planeManager.enabled = active;
        foreach (ARPlane plane in planeManager.trackables){
            plane.gameObject.SetActive(active);
        }
    }

    void VisualizePoints(bool active){
        pointCloudManager.enabled = active;
        foreach (ARPointCloud point in pointCloudManager.trackables){
            point.gameObject.SetActive(active);
        }
    }

}
