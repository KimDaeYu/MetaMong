using UnityEngine;
using System.Collections;

using Mapbox.Utils;
using Mapbox.Unity.Map;
using Mapbox.Unity.MeshGeneration.Factories;
using Mapbox.Unity.Utilities;

public class Circle : MonoBehaviour {
 
    [SerializeField]
    AbstractMap _map;

    public int segments;
    public float xradius;
    public float yradius;

    public Vector2d Pos;
    LineRenderer line;
 
    void Start () {
        line = gameObject.GetComponent<LineRenderer> ();
        line.SetVertexCount (segments + 1);
        line.useWorldSpace = false;
        CreatePoints();    
    }
    
    void Update(){
        CreatePoints();
        gameObject.transform.position = GetGPSToWorld(_map, Pos) + new Vector3(0,5,0);
    }

    void CreatePoints () {
        float x;
        float y;
        float z = 0f;
 
        float angle = 20f;
        var t = GetGPSToRadius(_map, new Vector2d(60.19191360d, 24.96843000d));
        for(int i=0;i<(segments+1);i++) {
            x = Mathf.Cos(Mathf.Deg2Rad*angle) * xradius * t;
            y = Mathf.Sin(Mathf.Deg2Rad*angle) * xradius * t;
 
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
        var LandPosInMeters1MeterAway = LandMeters + new Vector2d(2, 0); 
        var LandMeters_one = Conversions.MetersToLatLon(LandPosInMeters1MeterAway);
        var worldPosOneMeterAway = map.GeoToWorldPosition(LandMeters_one);
        
        var oneMapMeterInUnityMeters = (worldPosOneMeterAway - LandPos).magnitude;

        return oneMapMeterInUnityMeters;
    }

}