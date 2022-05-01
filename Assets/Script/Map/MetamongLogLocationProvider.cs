
using UnityEngine;
using Mapbox.Unity.Location;
using Mapbox.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine.UI;




public class MetamongLogLocationProvider : MonoBehaviour
{
	/*
		휴대폰에서 사용자 위치 불러오는 class

		굳이 클래스에서 사용을 해야할지. mapbox의 location에서 바로 구할 수 있는데.

	*/

		[SerializeField]
		private Text _logText;

		[SerializeField]
		private Toggle _logToggle;


		private CultureInfo _invariantCulture = CultureInfo.InvariantCulture;
		private bool _logToFile = false;
		private LocationLogWriter _logWriter = null;

		
		private double user_x;
		private double user_y;

		void Start()
		{
			Screen.sleepTimeout = SleepTimeout.NeverSleep;
			LocationProviderFactory.Instance.DefaultLocationProvider.OnLocationUpdated += LocationProvider_OnLocationUpdated;

			if (null != _logToggle)
			{
				_logToggle.onValueChanged.AddListener((isOn) => { _logToFile = isOn; });
			}
			else
			{
				Debug.LogError("no logtoggle attached, cannot log");
			}
			if (null == _logText)
			{
				Debug.LogError("no text to log to");
			}
		}

		//location.LatitudeLongitude.x, location.LatitudeLongitude.y  (경도 위도 불러오기)
		public double Get_userX()
		{
			//이걸 함수 하나로 묶을수는 없나?
			return user_x;
		}
		public double Get_userY(){
			return user_y;
		}


		void OnDestroy()
		{
			closeLogWriter();
			LocationProviderFactory.Instance.DefaultLocationProvider.OnLocationUpdated -= LocationProvider_OnLocationUpdated;
		}


		void LocationProvider_OnLocationUpdated(Location location)
		{

			/////////////// GUI logging //////////////////////
			
			//변수를 업데이트
			user_x=location.LatitudeLongitude.x;
			user_y=location.LatitudeLongitude.y;
			//
			
			StringBuilder sb = new StringBuilder();

			sb.AppendLine(string.Format("IsLocationServiceEnabled: {0}", location.IsLocationServiceEnabled));
			sb.AppendLine(string.Format("IsLocationServiceInitializing: {0}", location.IsLocationServiceInitializing));
			sb.AppendLine(string.Format("IsLocationUpdated: {0}", location.IsLocationUpdated));
			sb.AppendLine(string.Format("IsHeadingUpdated: {0}", location.IsUserHeadingUpdated));
			string locationProviderClass = LocationProviderFactory.Instance.DefaultLocationProvider.GetType().Name;
			sb.AppendLine(string.Format("location provider: {0} ({1})", location.Provider, locationProviderClass));
			sb.AppendLine(string.Format("UTC time:{0}  - device:  {1}{0}  - location:{2}", Environment.NewLine, DateTime.UtcNow.ToString("yyyyMMdd HHmmss"), UnixTimestampUtils.From(location.Timestamp).ToString("yyyyMMdd HHmmss")));
			sb.AppendLine(string.Format(_invariantCulture, "position: {0:0.00000000} / {1:0.00000000}", location.LatitudeLongitude.x, location.LatitudeLongitude.y));
			sb.AppendLine(string.Format(_invariantCulture, "accuracy: {0:0.0}m", location.Accuracy));
			sb.AppendLine(string.Format(_invariantCulture, "user heading: {0:0.0}°", location.UserHeading));
			sb.AppendLine(string.Format(_invariantCulture, "device orientation: {0:0.0}°", location.DeviceOrientation));
			sb.AppendLine(nullableAsStr<float>(location.SpeedKmPerHour, "speed: {0:0.0}km/h"));
			sb.AppendLine(nullableAsStr<bool>(location.HasGpsFix, "HasGpsFix: {0}"));
			sb.AppendLine(nullableAsStr<int>(location.SatellitesUsed, "SatellitesUsed:{0} "));
			sb.AppendLine(nullableAsStr<int>(location.SatellitesInView, "SatellitesInView:{0}"));

			if (null != _logText)
			{
				_logText.text = sb.ToString();
			}


			/////////////// file logging //////////////////////

			// start logging to file
			if (_logToFile && null == _logWriter)
			{
				Debug.Log("--- about to start logging to file ---");
				_logWriter = new LocationLogWriter();
				_logToggle.GetComponentInChildren<Text>().text = "stop logging";
			}


			// stop logging to file
			if (!_logToFile && null != _logWriter)
			{
				Debug.Log("--- about to stop logging to file ---");
				_logToggle.GetComponentInChildren<Text>().text = "start logging";
				closeLogWriter();
			}


			// write line to log file
			if (_logToFile && null != _logWriter)
			{
				_logWriter.Write(location);
			}
		}


		private string nullableAsStr<T>(T? val, string formatString = null) where T : struct
		{
			if (null == val && null == formatString) { return "[not supported by provider]"; }
			if (null == val && null != formatString) { return string.Format(_invariantCulture, formatString, "[not supported by provider]"); }
			if (null != val && null == formatString) { return val.Value.ToString(); }
			return string.Format(_invariantCulture, formatString, val);
		}


		private void closeLogWriter()
		{
			if (null == _logWriter) { return; }
			Debug.Log("closing stream writer");
			_logWriter.Dispose();
			_logWriter = null;
		}


		void Update() { }
}
