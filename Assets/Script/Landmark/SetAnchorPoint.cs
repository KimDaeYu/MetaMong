using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using TMPro;
using ARLocation;

public class SetAnchorPoint : MonoBehaviour
{
    public ARRaycastManager arRaycaster;
    public GameObject Origin;
    public GameObject line;

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

                measured_dis = Vector3.Distance(hitPose.position, Camera.main.transform.position);
                
                var space = GameObject.Find("Space").GetComponent<SpaceInstance>();
                space.compass = (float)locationProvider.CurrentHeading.heading;
                space.tilt = (float)Camera.main.transform.localEulerAngles.x * -1;
                space.distance = measured_dis;

                GameObject.Find("Log").GetComponent<TextMeshProUGUI>().text += space.compass.ToString() + " compass\n";
                GameObject.Find("Log").GetComponent<TextMeshProUGUI>().text += space.tilt.ToString() + " tilt\n";
                GameObject.Find("Log").GetComponent<TextMeshProUGUI>().text += space.distance.ToString() + " distance\n";

                line.transform.Find("Line").gameObject.GetComponent<LineRenderer>().SetPosition(0, hitPose.position);
                line.transform.Find("Line").gameObject.GetComponent<LineRenderer>().SetPosition(1, Camera.main.transform.position);
                line.transform.Find("Start").position = hitPose.position;
                line.transform.Find("End").position = Camera.main.transform.position;
                line.transform.Find("Length").position = (Camera.main.transform.position + hitPose.position) / 2;
        
                GameObject.Find("AnchorImage").GetComponent<SetAnchorImage>().TakeScreenShot();

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
