using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ARLocation;
public class OrientationArrow : MonoBehaviour
{
    public GameObject Arrow_cone;
    public GameObject InfoText;
    public GameObject ViewAR;

    private ARLocation.ARLocationProvider locationProvider;
    private GameObject mainCamera;
    

    private bool isMainCameraNull;
    
    public int precision = 10;
    public Vector3 Target; //compass, updown, distance
    // Use this for initialization
    void Start()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        isMainCameraNull = mainCamera == null;
        locationProvider = ARLocationProvider.Instance;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isMainCameraNull)
        {
            return;
        }

        var currentHeading = locationProvider.CurrentHeading.heading;
        var currentMagneticHeading = locationProvider.CurrentHeading.magneticHeading;
        var currentAccuracy = locationProvider.Provider.CurrentHeading.accuracy;
        
        //Arrow.transform.localRotation = Quaternion.Euler(ArrowVec);
        Arrow_cone.transform.localRotation = Quaternion.Euler(-(float)currentHeading + Target.x, 0, -(float)mainCamera.transform.localEulerAngles.x - Target.y);
        
        float horizondiff = ((float)currentHeading - Target.x) % 360;
        if(horizondiff < 0)
            horizondiff *= -1;
        float verticladiff = ((float)mainCamera.transform.localEulerAngles.x + Target.y) % 360;
        if(verticladiff < 0)
            verticladiff *= -1;

        if(horizondiff > 180){
            horizondiff = (360 + horizondiff * -1);   
        }
        if(verticladiff > 180){
            verticladiff = (360 + verticladiff * -1);   
        }

        //Debug.Log(horizondiff.ToString()+ "//" +verticladiff.ToString());
        if(horizondiff < precision && verticladiff < precision){
            Arrow_cone.GetComponent<MeshRenderer>().materials[0].color = Color.green;
            if (InfoText != null){
                InfoText.GetComponent<TextMeshProUGUI>().text = "해당 부분을 클릭해 고정해주세요!";
                gameObject.GetComponent<ARPointCheck>().enabled = true;
                ViewAR.GetComponent<ToggleAR>().VisualizePoints(true);
                ViewAR.GetComponent<Image>().color = new Color32(255,255,255,255);
            }
        }else{
            Arrow_cone.GetComponent<MeshRenderer>().materials[0].color = Color.red;
            
            if (InfoText != null){
                InfoText.GetComponent<TextMeshProUGUI>().text = "해당위치를 찾아주세요!";
                gameObject.GetComponent<ARPointCheck>().enabled = false;
                gameObject.GetComponent<ARPointCheck>().SetCoordBtn.SetActive(false);
                ViewAR.GetComponent<ToggleAR>().VisualizePoints(false);
                ViewAR.GetComponent<Image>().color = new Color32(255,255,255,100);
            }

            //ViewAR.GetComponent<Toggle>().isOn = false;
            ARreset();
        }
    }
    public void ARreset(){
        //AR/DIST reset active

    }

    public void TargetUpdate(Vector3 _Target, Vector3 flag){
        Target = _Target + Vector3.Scale(Target,(Vector3.one - flag));
    }
}

