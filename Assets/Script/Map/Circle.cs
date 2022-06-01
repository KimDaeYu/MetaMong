using UnityEngine;
using Mapbox.Utils;
using Mapbox.Unity.Map;
using Mapbox.Unity.MeshGeneration.Factories;
using Mapbox.Unity.Utilities;
using TMPro;

using Cysharp.Threading.Tasks;
using static DBManager; 

public class Circle : MonoBehaviour {
    /*
        하나의 AR SPACE를 그려주는 CLASS.
        추후에, 통신이나 json을 불러오는 것으로 수정.

    */ 
    public AbstractMap _map;

    public int segments;
    public float xradius=50.0f;
    public float yradius;//현재 안쓰이는 것.


    //DB 객체
    ARSpace space_data; // 넘어온 db 객체를 받은것. 이걸 다시 불러서 이미지 등 사용.


    public Vector2d Pos;

    public string id;

    public string Space_title="";// space 생성시 addspace에서 자동으로 입력해줌.

    LineRenderer line;

    //자식들.
    GameObject area;
    GameObject marker; // 작은 빨간 마커 사용 예정.
    GameObject marker_title; // title 담기는 ui
    GameObject marker_text;

    float _spawnScale = 7f; // Mesh Renderer size
    float circle_spawnScale = 1.3f; // 원의 사이즈를 조절.

    float _spawnScale_text_zoomIn = 20f;
    float _spawnScale_text_zoomOut = 60f;

    float _spawnScale_marker_zoomIn = 400f;
    private Vector2d unit_pos=new Vector2d(60.19191360d, 24.96843000d);// 유니티 좌표로 변환하기위한 데이터 상수.
    


    void Start () {
        line = gameObject.GetComponent<LineRenderer> ();
        area = transform.Find("Area").gameObject;
        marker = transform.Find("Marker").gameObject;
        marker_text = transform.Find("CircleText").gameObject;
        marker_title = transform.Find("CircleText/Content").gameObject;


        line.SetVertexCount(segments + 1);
        line.useWorldSpace = false;
        CreatePoints();
        UpdateMarker();
    }
    
    void Update(){
        
        //CreatePoints();
        UpdateMarker();

        gameObject.transform.position = GetGPSToWorld(_map, Pos) + new Vector3(0,1,0); // 5만큼 높이 올려서 생성.
    }

    public void Set_arspace_data(ARSpace in_space_data){
        space_data = in_space_data;
    }
    

    public void UpdateSpaceImg(){
        //공간 진입시, 이미지를 다운 받아서, don't destroy에 삽입.
        //캐싱처리도 해야함.
        GameObject.Find("PassData").GetComponent<SetData>().UpdateSpaceImg(space_data); 

    }
    


    float marker_prezoom=100f;
    float text_prezoom=10f;
    public void UpdateMarker(){
        /*
            1.Area component를 중앙으로 위치
            2.Marker Component 중앙 위치. 
            3.Circle Text의 확대 축소 조절.
            
        */
        
        //마커 정보를 수정하거나 등, 내용 업데이트 된 것을 반영.
        if(marker_title==null)Debug.Log("오류");

        var unityMeter_Per1Meter = GetGPSToRadius(_map, unit_pos); //m당 현재 유니티화면에서 단위.
        


        area.transform.localPosition = new Vector3(0f,0f,0f); // 매시 랜더러가 중앙에 오도록.
        //marker.transform.localPosition = new Vector3(0f,0f,0f); //원의 중심에 위치하도록

        area.transform.localScale = new Vector3(_spawnScale*unityMeter_Per1Meter, _spawnScale*unityMeter_Per1Meter, _spawnScale*unityMeter_Per1Meter); //매시 랜더러 크기 조절.
        
        marker_title.GetComponent<TextMeshPro>().text = Space_title;
        
        //텍스트창 위치 조절 
        float text_y = 30f;
        //marker_text.transform.localPosition = new Vector3(0f,text_y*unityMeter_Per1Meter,0f); 


        //줌인이냐 줌아웃이냐에 따라 TEXT 크기를 달리 설정.
        bool iszoom = GameObject.Find("ArSpaceManager").GetComponent<AddSpace>().is_zoom(); 

        if(iszoom){
            Debug.Log("확대");

            marker_prezoom  = Mathf.Lerp(marker_prezoom, 200f, Time.deltaTime*2);
            marker.transform.localScale = Vector3.one * marker_prezoom;

        	// 확대시 1.Mesh 안보이게 하기 2. LineRenderer 나타내기. 3. 마커 표시하기. 4. 씬 변환 버튼
            //  5. text 높이 변경.
            marker.SetActive(true);
            text_y = 60f;
            Debug.Log("스케일");
            Debug.Log(unityMeter_Per1Meter);

            text_prezoom  = Mathf.Lerp(text_prezoom, 14f, Time.deltaTime*2);
            marker_text.transform.localScale = Vector3.one * text_prezoom;

            marker_text.transform.localPosition = new Vector3(0f,25f,0f);
            //marker.transform.localPosition = new Vector3(0f,(text_y-30f)*30,0f);
            
            area.SetActive(false);
            line.enabled=true;

        


        }else{
            Debug.Log("축소.");
            //marker.transform.localScale = new Vector3(_spawnScale_marker_zoomOut*unityMeter_Per1Meter, _spawnScale_marker_zoomOut*unityMeter_Per1Meter, _spawnScale_marker_zoomIn*unityMeter_Per1Meter);
            //prezz=0.16f;
            marker_prezoom=10f;
            text_prezoom  = Mathf.Lerp(text_prezoom, 10f, Time.deltaTime*2);
            marker_text.transform.localScale = Vector3.one * text_prezoom;
            marker_text.transform.localPosition = new Vector3(0f,5f,0f);
            marker.SetActive(false);
            area.SetActive(true);
            line.enabled=false;
            
            //marker_text.transform.localPosition = new Vector3(0f,text_y*unityMeter_Per1Meter,0f); 

        }
                
    }


    

    public void CreatePoints() {
        /*
            원 그려주는 함수.
            xradius를 통해 반지름 조절.
        */
        float x=0f;
        float y=0f;
        float z = 0f;
 
        float angle = 20f;
        float unityMeter_Per1Meter = GetGPSToRadius(_map, unit_pos); //m당 현재 유니티화면에서 단위.

        for(int i=0;i<(segments+1);i++) 
        {
            x = Mathf.Cos(Mathf.Deg2Rad*angle) * xradius * unityMeter_Per1Meter * circle_spawnScale;// *1.8는 스케일이 반으로 출력되는 문제가 있어서. float 문제?
            z = Mathf.Sin(Mathf.Deg2Rad*angle) * xradius * unityMeter_Per1Meter * circle_spawnScale;
            
 
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


