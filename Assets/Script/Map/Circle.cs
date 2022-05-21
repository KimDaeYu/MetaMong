using UnityEngine;
using System.Collections;

using Mapbox.Utils;
using Mapbox.Unity.Map;
using Mapbox.Unity.MeshGeneration.Factories;
using Mapbox.Unity.Utilities;
using TMPro;

public class Circle : MonoBehaviour {
    /*
        하나의 AR SPACE를 그려주는 CLASS.
        추후에, 통신이나 json을 불러오는 것으로 수정.

    */ 
    public AbstractMap _map;

    public int segments;
    public float xradius=50.0f;
    public float yradius;//현재 안쓰이는 것.

    public Vector2d Pos;

    public string id;

    public string Space_title="";// space 생성시 addspace에서 자동으로 입력해줌.

    LineRenderer line;

    //자식들.
    GameObject marker;
    GameObject marker_title; // title 담기는 ui
    GameObject marker_text;

    float _spawnScale = 15f;

    float _spawnScale_text_zoomIn = 40f;
    float _spawnScale_text_zoomOut = 60f;

    private Vector2d unit_pos=new Vector2d(60.19191360d, 24.96843000d);// 유니티 좌표로 변환하기위한 데이터 상수.
    
    void Start () {
        line = gameObject.GetComponent<LineRenderer> ();
        marker = transform.Find("Marker").gameObject;
        marker_text = transform.Find("CircleText").gameObject;
        marker_title = transform.Find("CircleText/Content").gameObject;


        line.SetVertexCount(segments + 1);
        line.useWorldSpace = false;
        CreatePoints();
        UpdateMarker();
    }
    
    void Update(){
        
        CreatePoints();
        UpdateMarker();

        gameObject.transform.position = GetGPSToWorld(_map, Pos) + new Vector3(0,5,0); // 5만큼 높이 올려서 생성.
    }

    void UpdateMarker(){
        //마커 정보를 수정하거나 등, 내용 업데이트 된 것을 반영.
        if(marker_title==null)Debug.Log("오류");

        var unityMeter_Per1Meter = GetGPSToRadius(_map, unit_pos); //m당 현재 유니티화면에서 단위.
        
        marker.transform.localPosition = new Vector3(0f,0f,0f); //원의 중심에 위치하도록
        marker.transform.localScale = new Vector3(_spawnScale*unityMeter_Per1Meter, _spawnScale*unityMeter_Per1Meter, _spawnScale*unityMeter_Per1Meter);
        
        marker_title.GetComponent<TextMeshPro>().text = Space_title;
        
        //텍스트창 크기 조절 
        float text_y = 30f;
        marker_text.transform.localPosition = new Vector3(0f,text_y*unityMeter_Per1Meter,0f); 


        //줌인이냐 줌아웃이냐에 따라 TEXT 크기를 달리 설정.
        bool iszoom = GameObject.Find("ArSpaceManager").GetComponent<AddSpace>().is_zoom(); 

        if(iszoom){
            marker_text.transform.localScale = new Vector3(_spawnScale_text_zoomIn*unityMeter_Per1Meter, _spawnScale_text_zoomIn*unityMeter_Per1Meter, _spawnScale_text_zoomIn*unityMeter_Per1Meter);
        }else{
            marker_text.transform.localScale = new Vector3(_spawnScale_text_zoomOut*unityMeter_Per1Meter, _spawnScale_text_zoomOut*unityMeter_Per1Meter, _spawnScale_text_zoomOut*unityMeter_Per1Meter);
        }
                
    }

    void CreatePoints () {
        /*
            원 그려주는 함수.
            xradius를 통해 반지름 조절.
        */
        float x=0f;
        float y=0f;
        float z = 0f;
 
        float angle = 20f;
        float unityMeter_Per1Meter = GetGPSToRadius(_map, unit_pos); //m당 현재 유니티화면에서 단위.

        for(int i=0;i<(segments+1);i++) {
            x = Mathf.Cos(Mathf.Deg2Rad*angle) * xradius * unityMeter_Per1Meter * 1.6f;// *1.8는 스케일이 반으로 출력되는 문제가 있어서. float 문제?
            z = Mathf.Sin(Mathf.Deg2Rad*angle) * xradius * unityMeter_Per1Meter * 1.6f;
            
 
            line.SetPosition (i,new Vector3(x,y,z));
            
            angle += (360f / segments);
        }
    }
    

    //60.19202138798248, 24.964615042211662
    public Vector3 GetGPSToWorld(AbstractMap map, Vector2d LatLon){
        var worldPos = map.GeoToWorldPosition(LatLon);
        return worldPos;
    }

    public static float GetGPSToRadius(AbstractMap map, Vector2d LatLon){ // 위도 경도
        var LandPos = map.GeoToWorldPosition(LatLon);
        var LandMeters = Conversions.LatLonToMeters(LatLon);
        var LandPosInMeters1MeterAway = LandMeters + new Vector2d(1, 0);
        var LandMeters_one = Conversions.MetersToLatLon(LandPosInMeters1MeterAway);
        var worldPosOneMeterAway = map.GeoToWorldPosition(LandMeters_one);
        
        var oneMapMeterInUnityMeters = (worldPosOneMeterAway - LandPos).magnitude;

        return oneMapMeterInUnityMeters;
    }

}