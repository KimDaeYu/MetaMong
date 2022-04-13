using UnityEngine;
using UnityEngine.UI;

namespace ARLocation.UI
{
    public class OrientationArrow : MonoBehaviour
    {
        private GameObject Arrow;
        private GameObject redArrow;
        private GameObject trueNorthLabel;
        private GameObject magneticNorthLabel;
        private GameObject headingAccuracyLabel;
        private GameObject compassImage;
        private ARLocationProvider locationProvider;
        private GameObject mainCamera;
        private bool isMainCameraNull;
        private Text text;
        private Text text1;
        private Text text2;
        private RectTransform rectTransform;
        private RectTransform rectTransform1;
        
        public Vector3 ArrowVec; 
        // Use this for initialization
        void Start()
        {
            mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            isMainCameraNull = mainCamera == null;

            locationProvider = ARLocationProvider.Instance;

            Arrow = GameObject.Find(gameObject.name + "Arrow/rotate/Cone");

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
            Arrow.transform.localRotation = Quaternion.Euler(-(float)currentHeading,0, -(float)mainCamera.transform.localEulerAngles.x);
        }
    }
}
