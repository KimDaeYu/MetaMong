using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using TMPro;
public class StoryTouchSize : MonoBehaviour
{
    public ARRaycastManager arRaycaster;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Touched();
        //ARTouched();
    }

    private void ARTouched(){
        if(Input.GetMouseButton(0)){
            Touch touch = Input.GetTouch(0);

            List<ARRaycastHit> hits = new List<ARRaycastHit>();
            if(arRaycaster.Raycast(touch.position,hits, UnityEngine.XR.ARSubsystems.TrackableType.FeaturePoint)){
                GameObject.Find("Log").GetComponent<TextMeshProUGUI>().text += System.DateTime.Now.ToString() + " " + hits[0].pose.position.ToString() + " AR Hit!!\n";
                GameObject.Find("StoryManager").GetComponent<NewStory>().targetPos = hits[0].pose.position;
                //hits[0]
            }
        }
        //arRaycaster.Raycast(?,hits,UnityEngine.XR.ARSubsystems.TrackableType.);
    }

    // private void Touched22(){
    //     if(Input.touchCount > 0){
    //         Debug.Log("Touched!");
    //         Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
    //         RaycastHit hit;
    //         if(Physics.Raycast(ray,out hit, 10)){
    //             Debug.Log(hit.collider.gameObject.name);
    //         }
    //     }
    // }

    private bool touched = false;
    Transform target;
    Vector3 targetSize = new Vector3(0.5f,0.5f,0.5f);
    private void Touched(){
        if(Input.GetMouseButton(0) && !touched){
            touched = true;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray,out hit, 10)){
                //Story Slected
                if(hit.collider.gameObject.transform.parent.CompareTag("Story")){
                    GameObject.Find("Log").GetComponent<TextMeshProUGUI>().text += System.DateTime.Now.ToString() + " " + hit.collider.gameObject.transform.parent.name + hit.collider.gameObject.transform.position.ToString() + " Hit!!\n";
                    //GameObject.Find("StoryManager").GetComponent<NewStory>().targetPos = hit.collider.gameObject.transform.position;
                    target = hit.collider.gameObject.transform.parent.transform;
                    var feedback = hit.collider.gameObject.transform.parent.transform.GetChild(2).gameObject;
                    if(target.localScale[0] > 0.9f){
                        //small
                        targetSize = new Vector3(0.3f,0.3f,0.3f);
                        feedback.SetActive(false);
                    }else{
                        //origin
                        targetSize = new Vector3(1,1,1);
                        feedback.SetActive(true);
                    }

                }else{
                    GameObject.Find("Log").GetComponent<TextMeshProUGUI>().text += System.DateTime.Now.ToString() + " " + hit.collider.gameObject.name + " Hit!!\n";
                }
            }
        }

        if(Input.GetMouseButtonUp(0)){
            touched = false;
        }
        
        if(target != null){
            target.localScale = Vector3.Lerp(target.localScale, targetSize, Time.deltaTime * 12f);
        }
    }
}
