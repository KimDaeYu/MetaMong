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
        추후에, 통신이나 json을 불러오는 것으로 수정.
        https://docs.mapbox.com/unity/maps/guides/add-markers/#use-the-spawnonmap-script

    */
		[SerializeField]
		AbstractMap _map;

		[SerializeField]
		[Geocode]
		string[] _locationStrings;
		Vector2d[] _locations;

		[SerializeField]
		float _spawnScale = 100f;

		[SerializeField]
		GameObject _markerPrefab;

		List<GameObject> _spawnedObjects;


		private float onemeter_scale = 2f;
		void Start()
		{
			_locations = new Vector2d[_locationStrings.Length];
			_spawnedObjects = new List<GameObject>();
			for (int i = 0; i < _locationStrings.Length; i++)
			{
				var locationString = _locationStrings[i];
				_locations[i] = Conversions.StringToLatLon(locationString);
				var instance = Instantiate(_markerPrefab);

				instance.transform.localPosition = _map.GeoToWorldPosition(_locations[i], true);

                Debug.Log("m당 유니티 스케일 : "+GetOneMapMeterInUnityMeters(_map,instance.transform.localPosition));
				//_spawnScale=GetOneMapMeterInUnityMeters(_map,instance.transform.localPosition);

                instance.transform.position = new Vector3(instance.transform.position.x,
                instance.transform.position.y+2.5f,
                instance.transform.position.z); // 지도 위에 띄우기 위해 y값 조절

				instance.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);
				_spawnedObjects.Add(instance);
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
            var oneMapMeterInUnityMeters = (worldPosOneMeterAway - targetPlayerPos).magnitude;
            return oneMapMeterInUnityMeters;
        }


		private void Update()
		{
			int count = _spawnedObjects.Count;
            //계속 업데이트를 해야하나?
            //차후 이 부분을 거리에 따라 object를 바꾸는 것으로 교체.
			for (int i = 0; i < count; i++)
			{
				var spawnedObject = _spawnedObjects[i];
				var location = _locations[i];
				spawnedObject.transform.localPosition = _map.GeoToWorldPosition(location, true);

				onemeter_scale=GetOneMapMeterInUnityMeters(_map,spawnedObject.transform.localPosition);
				if(onemeter_scale is not float.NaN){
					_spawnScale=onemeter_scale*100;
				}

                spawnedObject.transform.position = new Vector3(spawnedObject.transform.position.x,
                spawnedObject.transform.position.y+3.0f,
                spawnedObject.transform.position.z); // 지도 위에 띄우기 위해 y값 조절

				spawnedObject.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);
				
                Debug.Log("m당 유니티 스케일 : "+GetOneMapMeterInUnityMeters(_map,spawnedObject.transform.localPosition));
			}
		}
}
