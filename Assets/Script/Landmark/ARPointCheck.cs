using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using TMPro;
using ARLocation;

public class ARPointCheck : MonoBehaviour
{
    public ARRaycastManager arRaycaster;
    public GameObject Origin;
    public GameObject line;
    public GameObject SetCoordBtn;

    private ARLocation.ARLocationProvider locationProvider;

    // Start is called before the first frame update
    void Start()
    {
        locationProvider = ARLocationProvider.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        ARTouched();
    }

    private bool ARtouch = false;
    private bool seleceted = false;

    public float measured_dis;
    public Vector3 LastHitPose; 
    private void ARTouched(){
        if(Input.GetMouseButton(0) && !ARtouch && !seleceted){
            ARtouch = true;
            Touch touch = Input.GetTouch(0);

            List<ARRaycastHit> hits = new List<ARRaycastHit>();

            if(arRaycaster.Raycast(touch.position,hits, UnityEngine.XR.ARSubsystems.TrackableType.FeaturePoint)){
            //if(arRaycaster.Raycast(touch.position,hits, UnityEngine.XR.ARSubsystems.TrackableType.Planes)){
                Pose hitPose = hits[0].pose;
                LastHitPose = hitPose.position;
                Origin.transform.position = LastHitPose;
                SetCoordBtn.SetActive(true);
                measured_dis = Vector3.Distance(hitPose.position, Camera.main.transform.position);

                GameObject.Find("Log").GetComponent<TextMeshProUGUI>().text += measured_dis.ToString() + " measured!!\n";
                //line.transform.Find("Length").gameObject.GetComponent<TextMeshPro>().SetText(measured_dis.ToString() + "m");
                
                //GameObject spawnObject;
                GameObject.Find("Log").GetComponent<TextMeshProUGUI>().text += System.DateTime.Now.ToString() + " " + hits[0].pose.position.ToString() + " AR Hit!!\n";
            }
        }
        if(Input.GetMouseButtonUp(0)){
            ARtouch = false;
        }
    }

}
