using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;

public class GpsManager : MonoBehaviour
{

    /*
    사용안함.
    */
    public static double first_Lat;//최초 경도
    public static double first_Long;//최초 경도
    public static double first_Height;//최초 고도
    public static double current_Lat;//현재 위도
    public static double current_Long;//현재 경도
    public static double current_Height;//현재 고도

    private static WaitForSeconds second;// 기다리게 하는건가?

    private static bool gpsStarted = false;

    private static LocationInfo location;      
    
    // Start is called before the first frame update

    public Text[] text_data =new Text[4];

    private void Awake(){
        second = new WaitForSeconds(1.0f);

    }
    void start(){
        Debug.Log("gps manager start");
        StartCoroutine(gps_man());   
    }

    IEnumerator gps_man(){
        //참고 1. https://www.youtube.com/watch?v=tMRoXrR7m6o
        //참고 2. https://m.blog.naver.com/PostView.naver?isHttpsRedirect=true&blogId=2983934&logNo=221258680165
        //위치정보 permission
        //궁금한것. 유니티 안드로이드 permission만 해도 되는지?
        if(!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);//위치정보가 없으면 팝업창 띄우기
            while(!Permission.HasUserAuthorizedPermission(Permission.FineLocation))//받을때 까지 대기 하는것.
            {
                yield return null; 
            }
            
        }
        // 유저가 GPS 사용중인지 최초 체크
        if (!Input.location.isEnabledByUser) {

            Debug.Log ("GPS is not enabled");
            text_data[3].text = "GPS 장치가 꺼져있습니다.";
            yield break;
        }
 
        //GPS 서비스 시작
        Input.location.Start ();
        Debug.Log ("Awaiting initialization");
 
        //활성화될 때 까지 대기
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0) {
            yield return second; //시간만큼 딜레이 주는 방법. 다시 여기로 돌아옴.
            maxWait -= 1;
        }
 
        //20초 지날경우 활성화 중단
        if (maxWait < 1) {
            Debug.Log ("Timed out. GPS가 활성화되지 않음.");
            text_data[3].text = "Timed out. GPS가 활성화되지 않음.";
            yield break;
        }
 
        //연결 실패
        if (Input.location.status == LocationServiceStatus.Failed ||Input.location.status == LocationServiceStatus.Stopped ) {
            Debug.Log ("Unable to determine device location");
            text_data[3].text = "위치 정보를 가져오는데 실패했습니다.";
            yield break;
 
        } else {
            //접근 허가됨, 최초 위치 정보 받아오기
            location = Input.location.lastData;
            first_Lat = location.latitude * 1.0d;
            first_Long = location.longitude * 1.0d;
            first_Height = location.altitude * 1.0d;

            text_data[0].text = "위도 : "+first_Lat.ToString();
            text_data[1].text = "경도 : "+first_Lat.ToString();
            text_data[2].text = "고도 : "+first_Height.ToString();
            text_data[3].text = "위치 정보 수신완료.";



            gpsStarted = true;
 
            //현재 위치 갱신
            while (gpsStarted) {
                location = Input.location.lastData;
                current_Lat = location.latitude * 1.0d;//정확하게 만들기위해 DOUBLE을 곱.
                current_Long = location.longitude * 1.0d;
                current_Height = location.altitude * 1.0d;

                text_data[0].text = "위도 : "+current_Lat.ToString();
                text_data[1].text = "경도 : "+current_Long.ToString();
                text_data[2].text = "고도 : "+current_Height.ToString();
                text_data[3].text = "위치 정보 수신완료.";
                yield return second;
            }
        }
    }
    //위치 서비스 종료
    public static void StopGPS () {
        if (Input.location.isEnabledByUser) {
            gpsStarted = false;
            Input.location.Stop ();
        }
    }
    // Update is called once per frame
    void Update()
    {
        
        
    }
}
