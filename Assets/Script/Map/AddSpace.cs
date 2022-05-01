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
		public List<Vector2d> ArSpace_gps_list;
		public GameObject ArSpace_prefab;//circle
		private List<GameObject> ArSpace_object_List = new List<GameObject>();

		public GameObject Location_provider;

		private Vector2d user_position = new Vector2d();

		void Start()
		{
			//list 생성시 문제점. 더블형 크기가 정해진다.
			foreach(Vector2d gps in ArSpace_gps_list ){
				//foreach의 문제점이 있을까? 원소 삭제해도 잘 돌아갈지.
				//Debug.Log(gps);
				GameObject arInstance = Instantiate(ArSpace_prefab);
				arInstance.GetComponent<Circle>().Pos=gps;
				arInstance.GetComponent<Circle>()._map=_map;
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

	
		private void Update()
		{
			//업데이트에서 통신이 담겨야함.

			//매 프레임마다 유저 gps 값을 불러온다.
			

			//이렇게 매프레임 가져오면, 폰에 영향을 주려나.
			
			user_position.x=Location_provider.GetComponent<MetamongLogLocationProvider>().Get_userX();
			user_position.y=Location_provider.GetComponent<MetamongLogLocationProvider>().Get_userY();
			Debug.Log(user_position);
			//불러오고 나서 포함되는지 여부 확인
			foreach(GameObject arspace in ArSpace_object_List ){
				var howfar =(Conversions.LatLonToMeters(user_position) - Conversions.LatLonToMeters(arspace.GetComponent<Circle>().Pos)).magnitude;	
				if(howfar<50){
					Debug.Log("in");
					//원의 크기로 봤을때는 반지름이 50m 넘어보인다. 어디가 문제인지.
					Debug.Log(howfar);
				}
			}



		}
}
