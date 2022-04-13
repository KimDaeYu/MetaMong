using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using TMPro;

public class ARPointCheck : MonoBehaviour
{
    public ARRaycastManager arRaycaster;

    public GameObject line;
    private Vector3 mousePos;

    private int currLines = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ARTouched();
    }

    private bool ARtouch = false;
    private bool seleceted = false;
    private void ARTouched(){
        if(Input.GetMouseButton(0) && !ARtouch && !seleceted){
            ARtouch = true;
            Touch touch = Input.GetTouch(0);

            List<ARRaycastHit> hits = new List<ARRaycastHit>();

            if(arRaycaster.Raycast(touch.position,hits, UnityEngine.XR.ARSubsystems.TrackableType.FeaturePoint)){
            //if(arRaycaster.Raycast(touch.position,hits, UnityEngine.XR.ARSubsystems.TrackableType.Planes)){
                Pose hitPose = hits[0].pose;
                line.transform.Find("Line").gameObject.GetComponent<LineRenderer>().SetPosition(0, hitPose.position);
                line.transform.Find("Line").gameObject.GetComponent<LineRenderer>().SetPosition(1, Camera.main.transform.position);
                line.transform.Find("Start").position = hitPose.position;
                line.transform.Find("End").position = Camera.main.transform.position;
                line.transform.Find("Length").position = (Camera.main.transform.position + hitPose.position) / 2;

                float dis = Vector3.Distance(hitPose.position, Camera.main.transform.position);
                GameObject.Find("Log").GetComponent<TextMeshProUGUI>().text += dis.ToString() + " measured!!\n";
                line.transform.Find("Length").gameObject.GetComponent<TextMeshPro>().SetText(dis.ToString() + "m");
                
                //GameObject spawnObject;
                
                GameObject.Find("Log").GetComponent<TextMeshProUGUI>().text += System.DateTime.Now.ToString() + " " + hits[0].pose.position.ToString() + " AR Hit!!\n";
            }
        }
        if(Input.GetMouseButtonUp(0)){
            ARtouch = false;
        }
    }

}
