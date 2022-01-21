using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using TMPro;
using UnityEngine.UI;
public class CreateObject : MonoBehaviour
{   
    [SerializeField] private ARRaycastManager arRaycaster;
    [SerializeField] private GameObject SelecetedObject;

    GameObject target;
    GameObject spawnObject;
    // Update is called once per frame
    void Update()
    {
        Touched();
        ARTouched();
    }

    private bool ARtouch = false;
    private void ARTouched(){
        if(Input.GetMouseButton(0) && !ARtouch && !seleceted){
            ARtouch = true;
            Touch touch = Input.GetTouch(0);

            List<ARRaycastHit> hits = new List<ARRaycastHit>();
            //if(arRaycaster.Raycast(touch.position,hits, UnityEngine.XR.ARSubsystems.TrackableType.FeaturePoint)){
            if(arRaycaster.Raycast(touch.position,hits, UnityEngine.XR.ARSubsystems.TrackableType.Planes)){
                Pose hitPose = hits[0].pose;
                //GameObject spawnObject;
                if(target != null){
                    var spawnObject = Instantiate(target, hitPose.position, hitPose.rotation);
                    spawnObject.transform.Rotate(0,180,0);

                    if(target.name.Substring(0,4) == "Rubb"){
                        spawnObject.transform.Rotate(-90,0,0);
                    }
                    
                    //GameObject.Find("Log").GetComponent<TextMeshProUGUI>().text += System.DateTime.Now.ToString() + " " + target.name.Substring(0,4) + "!!\n";
                    GameObject.Find("Log").GetComponent<TextMeshProUGUI>().text += System.DateTime.Now.ToString() + " " + spawnObject.transform.rotation.ToString() + "\n";
                }

                GameObject.Find("Log").GetComponent<TextMeshProUGUI>().text += System.DateTime.Now.ToString() + " " + hits[0].pose.position.ToString() + " AR Hit!!\n";
                target = null;
                
            }
        }
        if(Input.GetMouseButtonUp(0)){
            ARtouch = false;
        }
    }


    private bool touched = false;
    private bool seleceted = false;
    private void Touched(){
        if(Input.GetMouseButton(0) && !touched){
            touched = true;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray,out hit, 10)){
                //Comment Slected
                if(hit.collider.gameObject.CompareTag("Object")){
                    target = hit.collider.gameObject.transform.GetChild(0).gameObject;
                    GameObject.Find("Canvas").transform.Find("NewObject").gameObject.GetComponent<Toggle>().isOn = false;

                    // var spawnObject = Instantiate(target);
                    // spawnObject.transform.SetParent(SelecetedObject.transform);
                    // spawnObject.AddComponent<RotateObject>();
                    // spawnObject.transform.localPosition = new Vector3(0,0,0);
                    // spawnObject.transform.parent.rotation = Quaternion.Euler(-90, 0, 0);
                    GameObject.Find("Log").GetComponent<TextMeshProUGUI>().text += System.DateTime.Now.ToString() + " " + hit.collider.gameObject.name + " Seleceted!!\n";
                    // GameObject Name = Info.transform.GetChild(1).gameObject;
                    seleceted = true;
                }else{
                    seleceted = false;
                }
            }
        }
        if(Input.GetMouseButtonUp(0)){
            touched = false;
        }
    }
}
