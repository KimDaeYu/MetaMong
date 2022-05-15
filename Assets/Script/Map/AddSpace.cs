	using UnityEngine;
	using Mapbox.Utils;
	using Mapbox.Unity.Map;
	using Mapbox.Unity.MeshGeneration.Factories;
	using Mapbox.Unity.Utilities;
	using System.Collections.Generic;

public class AddSpace : MonoBehaviour
{    
    /*
        AR SPACE를 불러오는 CLASS.
		여러 circle instance를 만들어준다.
        추후에, 통신이나 json을 불러오는 것으로 수정.
        https://docs.mapbox.com/unity/maps/guides/add-markers/#use-the-spawnonmap-script

    */
		[SerializeField]
		AbstractMap _map;

		//  전송받은 데이터 담는 리스트들.
		public List<Vector2d> ArSpace_gps_list;

		public List<string> ArSpace_name_list; 


		//

		public GameObject ArSpace_prefab;//circle
		private List<GameObject> ArSpace_object_List = new List<GameObject>();

		public GameObject Location_provider;

		bool user_in_flag=false;//for문에서 한번이라도 ar space에 접근시 true.

		private Vector2d user_position = new Vector2d();

		private int current_index = -1;//현재 위치해있는 인덱스.


		GameObject target_object=null;
		public GameObject user_arrow;


		void Start()
		{
			//이 부분에 firebase 추가.
			for(int index = 0; index< ArSpace_gps_list.Count; index++){

				Debug.Log("index: "+index);
				GameObject arInstance = Instantiate(ArSpace_prefab);
				arInstance.GetComponent<Circle>().Pos=ArSpace_gps_list[index]; // gps 삽입. 
				arInstance.GetComponent<Circle>()._map=_map;
				arInstance.GetComponent<Circle>().Space_title = ArSpace_name_list[index]; //  장소 명 삽입.
				ArSpace_object_List.Add(arInstance);

			}

		}

        public static float GetOneMapMeterInUnityMeters(AbstractMap map, Vector3 targetPlayerPos)
        {

            // hackish way of doing the conversion, since there is no obvious way of doing it in the API
            // taking a position X meters away, converting to map coords, and then back into unity world coords.
            var playerPosGps = map.WorldToGeoPosition(targetPlayerPos);
            var playerPosInMeters = Conversions.LatLonToMeters(playerPosGps);
            var playerPosInMeters1MeterAway = playerPosInMeters + new Vector2d(1, 0); // 직접 1미터를 더해서 바꿔오기.
            var oneMeterAwayLatLon = Conversions.MetersToLatLon(playerPosInMeters1MeterAway);
            var worldPosOneMeterAway = map.GeoToWorldPosition(oneMeterAwayLatLon);
            var oneMapMeterInUnityMeters = (worldPosOneMeterAway - targetPlayerPos).magnitude;//1미터의 비율값. 
            return oneMapMeterInUnityMeters;

        }

		public GameObject get_current_space(){
			//현재 어디 스페이스에 위치해있는지 index를 출력해주는 함수.
			//arrow manager에 사용.
			return target_object;
		}

		public bool is_zoom(){
			//현재 줌 상태인지 확인. space안에 위치해있는지.
			return user_in_flag;
		}

		

		
		float targetzoom = 18f;
		float prezoom = 16f;
		private void Update()
		{
			//업데이트에서 통신이 담겨야함.

			//프레임마다 유저 gps 값을 불러오고 들어왔는지 확인.
			user_position.x=Location_provider.GetComponent<MetamongLogLocationProvider>().Get_userX();
			user_position.y=Location_provider.GetComponent<MetamongLogLocationProvider>().Get_userY();
			Debug.Log(user_position);
			

			if(user_position.x==0){
				Debug.Log("유저 좌표가 설정되지 않았습니다.");
				return;
			}

			if(user_in_flag){
				prezoom  = Mathf.Lerp(prezoom, 18f, Time.deltaTime*2);
				_map.GetComponent<AbstractMap>().SetZoom(prezoom);
				_map.GetComponent<AbstractMap>().UpdateMap();

				//화살표 나타내기
				if(!user_arrow.activeSelf){
					user_arrow.SetActive(true);
				}
				

			}else{
				prezoom  = Mathf.Lerp(prezoom, 16f, Time.deltaTime*2);
				_map.GetComponent<AbstractMap>().SetZoom(prezoom);
				_map.GetComponent<AbstractMap>().UpdateMap();
				target_object=null;

			}
			//prezoom = targetzoom;
			
			user_in_flag=false;
			//불러오고 나서 포함되는지 여부 확인
			foreach(GameObject arspace in ArSpace_object_List ){
				var howfar =(Conversions.LatLonToMeters(user_position) - Conversions.LatLonToMeters(arspace.GetComponent<Circle>().Pos)).magnitude;	
				if(howfar<50){
					Debug.Log("in");
					Debug.Log(howfar);
					//원의 크기로 봤을때는 반지름이 50m 넘어보인다. 어디가 문제인지.
					
					//진입 여부에 따라 다른 동작 수행.
					
					//진입했으면 zoom , 주변 나침반 on
					//16 -> 18.1
					user_in_flag=true;
					target_object=arspace;



					//1. ar space 2m 반경이면
					if(howfar<2){
						Debug.Log("Deep in");
						Debug.Log(howfar);

					}




				}
				//_map.GetComponent<AbstractMap>().SetZoom(16.1f);
			}

			// if(user_in_flag==false && user_in==true){
			// 	//나간것으로 판정하고 다시 축소
			// 	_map.GetComponent<AbstractMap>().SetZoom(16f);
			// 	_map.GetComponent<AbstractMap>().UpdateMap();
			// 	user_in=false;
			// }

			



		}
}
