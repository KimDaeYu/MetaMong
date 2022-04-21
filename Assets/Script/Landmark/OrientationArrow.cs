using UnityEngine;
using UnityEngine.UI;

using ARLocation;
public class OrientationArrow : MonoBehaviour
{
    public GameObject Arrow_cone;
    public GameObject SetCoordBtn;
    public GameObject Origin;

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

        Debug.Log(horizondiff.ToString()+ "//" +verticladiff.ToString());
        if(horizondiff < precision && verticladiff < precision){
            Arrow_cone.GetComponent<MeshRenderer>().materials[0].color = Color.green;
            SetCoordBtn.SetActive(true);
        }else{
            Arrow_cone.GetComponent<MeshRenderer>().materials[0].color = Color.red;
            SetCoordBtn.SetActive(false);
            ARreset();
        }
    }
    public void ARreset(){
        //AR/DIST reset active

    }

    public void SetOrigin(){
        Origin.transform.position = gameObject.GetComponent<ARPointCheck>().LastHitPose;
    }

    public void TargetUpdate(Vector3 _Target, Vector3 flag){
        Target = _Target + Vector3.Scale(Target,(Vector3.one - flag));
    }
}

