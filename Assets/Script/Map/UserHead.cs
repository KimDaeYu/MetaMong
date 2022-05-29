using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class UserHead : MonoBehaviour 
{
 
    public static float magneticHeading;
    public static float trueHeading;


    public bool _useNegativeAngle=true;
  
    private void Awake () {
        Input.location.Start (); //위치 서비스 시작
        Input.compass.enabled = true; //나침반 활성화
    }
 
    IEnumerator Start () {
        while (true) {
 
            //헤딩 값 가져오기
            if (Input.compass.headingAccuracy == 0 || Input.compass.headingAccuracy > 0) {
                magneticHeading = Input.compass.magneticHeading;
                trueHeading = Input.compass.trueHeading;
            }
 
            yield return new WaitForSeconds (0.1f);
        }
    }

    void Update() {
        

        gameObject.transform.localRotation =  Quaternion.Euler(0, + Mathf.Floor(trueHeading/10f)*10f,0);
        //Quaternion.Lerp(transform.localRotation, _targetRotation, Time.deltaTime * _rotationFollowFactor);
    }
}
