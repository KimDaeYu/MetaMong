using UnityEngine;
using System.Collections;

using Mapbox.Utils;
using Mapbox.Unity.Map;
using Mapbox.Unity.MeshGeneration.Factories;
using Mapbox.Unity.Utilities;

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
    LineRenderer line;

    private Vector2d unit_pos=new Vector2d(60.19191360d, 24.96843000d);// 유니티 좌표로 변환하기위한 데이터 상수.
 
    void Start () {
        line = gameObject.GetComponent<LineRenderer> ();
        line.SetVertexCount(segments + 1);
        line.useWorldSpace = false;
        CreatePoints();    
    }
    
    void Update(){
        CreatePoints();
        gameObject.transform.position = GetGPSToWorld(_map, Pos) + new Vector3(0,5,0); // 5만큼 높이 올려서 생성.
    }

    void CreatePoints () {
        /*
            원 그려주는 함수.
            xradius를 통해 반지름 조절.
        */
        float x;
        float y;
        float z = 0f;
 
        float angle = 20f;
        var unityMeter_Per1Meter = GetGPSToRadius(_map, unit_pos); //m당 현재 유니티화면에서 단위.
        for(int i=0;i<(segments+1);i++) {
            x = Mathf.Cos(Mathf.Deg2Rad*angle) * xradius * unityMeter_Per1Meter * 2;// *2는 스케일이 반으로 출력되는 문제가 있어서.
            y = Mathf.Sin(Mathf.Deg2Rad*angle) * xradius * unityMeter_Per1Meter * 2;
 
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