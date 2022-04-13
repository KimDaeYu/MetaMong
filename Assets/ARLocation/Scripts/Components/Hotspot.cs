using System;
using UnityEngine;
using UnityEngine.Events;
using Logger = ARLocation.Utils.Logger;

namespace ARLocation {

    [AddComponentMenu("AR+GPS/Hotspot")]
    [HelpURL("https://http://docs.unity-ar-gps-location.com/guide/#hotspot")]
    [DisallowMultipleComponent]
    public class Hotspot : MonoBehaviour
    {
        [Serializable]
        public class OnHotspotActivatedUnityEvent : UnityEvent<GameObject> {}

        [Serializable]
        public class OnHotspotLeaveUnityEvent : UnityEvent<GameObject> {}

        [Serializable]
        public enum PositionModes {
            HotspotCenter,
            CameraPosition
        };

        [Serializable]
        public class HotspotSettingsData
        {
            [Tooltip("The prefab/GameObject that will be instantiated by the Hotspot.")]
            public GameObject Prefab;

            [Tooltip("The positioning mode. 'HotspotCenter' means the object will be instantiated at the Hotpot's center geo-location. 'CameraPosition' means it will be positioned at the front of the camera.")]
            public PositionModes PositionMode;

            [Tooltip("The distance from the center that the user must be located to activate the Hotspot.")]
            public float ActivationRadius = 4.0f;

            [Tooltip("If true, align the instantiated object to face the camera (horizontally).")]
            public bool AlignToCamera = true;

            [Tooltip("If 'PositionMode' is set to 'CameraPosition', how far from the camera the instantiated object should be placed.")]
            public float DistanceFromCamera = 3.0f;

            [Tooltip("If true, use raw GPS data, instead of the filtered GPS data.")]
            public bool UseRawLocation = true;
        }

        [Serializable]
        public class StateData
        {
            public bool Activated;
            public GameObject Instance;
            public Location Location;
            public bool EmittedLeaveHotspotEvent;
        }

        public PlaceAtLocation.LocationSettingsData LocationSettings = new PlaceAtLocation.LocationSettingsData();
        public HotspotSettingsData HotspotSettings = new HotspotSettingsData();

        [Space(4.0f)]

        [Header("Debug")]
        [Tooltip("When debug mode is enabled, this component will print relevant messages to the console. Filter by 'Hotspot' in the log output to see the messages.")]
        public bool DebugMode;

        [Space(4.0f)]


        [Header("Events")]
        [Tooltip("If true, monitor distance to emit the 'OnLeaveHotspot' event. If you don't need it, keep this disabled for better performance.")]
        public bool UseOnLeaveHotspotEvent;

        [Tooltip("Event for the Hotspot is activated.")]
        public OnHotspotActivatedUnityEvent OnHotspotActivated = new OnHotspotActivatedUnityEvent();

        [Tooltip("This event will be emited only once, when the user leaves the hotspot area after it is activated.")]
        public OnHotspotLeaveUnityEvent OnHotspotLeave = new OnHotspotLeaveUnityEvent();

        private readonly StateData state = new StateData();

        private Transform root;
        private Camera arCamera;
        private double currentDistance;

        // ReSharper disable once UnusedMember.Global
        public GameObject Instance => state.Instance;

        // ReSharper disable once MemberCanBePrivate.Global
        public Location Location {
            // ReSharper disable once UnusedMember.Global
            get => state.Location;
            set => state.Location = value;
        }

        /// <summary>
        /// Returns the current user distance to the Hotspot center.
        /// </summary>
        public float CurrentDistance => (float) currentDistance;

        void Start()
        {
            var arLocationProvider = ARLocationProvider.Instance;

            if (HotspotSettings.UseRawLocation) 
            {
                arLocationProvider.Provider.LocationUpdatedRaw += Provider_LocationUpdated;
            }
            else
            {
                arLocationProvider.Provider.LocationUpdated += Provider_LocationUpdated;
            }

            root = ARLocationManager.Instance.gameObject.transform;

            if (state.Location == null)
            {
                state.Location = LocationSettings.GetLocation();
            }

            arCamera = ARLocationManager.Instance.MainCamera;

            if (arLocationProvider.IsEnabled)
            {
                Provider_LocationUpdated(arLocationProvider.CurrentLocation, arLocationProvider.LastLocation);
            }
        }

        public void OnDisable()
        {
            var arLocationProvider = ARLocationProvider.Instance;

            if (!arLocationProvider) return;

            if (HotspotSettings.UseRawLocation) 
            {
                arLocationProvider.Provider.LocationUpdatedRaw -= Provider_LocationUpdated;
            }
            else
            {
                arLocationProvider.Provider.LocationUpdated -= Provider_LocationUpdated;
            }
        }

        public void Restart()
        {
            state.Activated = false;
            state.EmittedLeaveHotspotEvent = false;
            state.Instance = null;

            Destroy(state.Instance);
        }


        private void Provider_LocationUpdated(LocationReading currentLocation, LocationReading lastLocation)
        {
            if (state.Activated) return;

            Logger.LogFromMethod("Hotspot", "LocationUpdatedRaw", $"({gameObject.name}): New device location {currentLocation}", DebugMode);

            currentDistance = Location.HorizontalDistance(currentLocation.ToLocation(), state.Location);

            var delta = Location.HorizontalVectorFromTo(currentLocation.ToLocation(), state.Location);

            if (currentDistance <= HotspotSettings.ActivationRadius)
            {
                ActivateHotspot(new Vector3((float) delta.x, 0, (float) delta.y));
            }
            else
            {
                Logger.LogFromMethod("Hotspot", "LocationUpdatedRaw", $"({gameObject.name}): No activation - distance = {currentDistance}", DebugMode);
            }
        }

        private void ActivateHotspot(Vector3 delta)
        {
            Logger.LogFromMethod("Hotspot", "ActivateHotspot", $"({gameObject.name}): Activating hotspot...", DebugMode);

            if (HotspotSettings.Prefab == null) return;

            state.Instance = Instantiate(HotspotSettings.Prefab, HotspotSettings.AlignToCamera ?  gameObject.transform : root);

            switch (HotspotSettings.PositionMode)
            {
                case PositionModes.HotspotCenter:
                    state.Instance.transform.position = arCamera.transform.position + delta;
                    break;
                case PositionModes.CameraPosition:
                    var transform1 = arCamera.transform;
                    var forward = transform1.forward;
                    forward.y = 0;
                    state.Instance.transform.position = transform1.position + forward * HotspotSettings.DistanceFromCamera;
                    break;
            }

            if (HotspotSettings.AlignToCamera)
            {
                state.Instance.transform.LookAt(arCamera.transform);
            }

            state.Instance.AddComponent<GroundHeight>();
            state.Instance.name = name + " (Hotspot)";

            state.Activated = true;

            Logger.LogFromMethod("Hotspot", "ActivateHotspot", $"({name}): Hotspot activated", DebugMode);

            OnHotspotActivated?.Invoke(state.Instance);
        }

        // ReSharper disable once UnusedMethodReturnValue.Global
        // ReSharper disable once MemberCanBePrivate.Global
        public static Hotspot AddHotspotComponent(GameObject go, Location location, HotspotSettingsData settings)
        {
            var hotspot = go.AddComponent<Hotspot>();
            hotspot.Location = location.Clone();
            hotspot.HotspotSettings = settings;

            return hotspot;
        }

        // ReSharper disable once UnusedMember.Global
        public static GameObject CreateHotspotGameObject(Location location, HotspotSettingsData settings,
            string name = "GPS Hotspot")
        {
            var go = new GameObject(name);

            AddHotspotComponent(go, location, settings);

            return go;
        }

        void Update()
        {
            if (UseOnLeaveHotspotEvent && state.Activated && !state.EmittedLeaveHotspotEvent)
            {
                var distance = Vector3.Distance(arCamera.transform.position, state.Instance.transform.position);
                if (distance >= HotspotSettings.ActivationRadius)
                {
                    OnHotspotLeave?.Invoke(state.Instance);
                    state.EmittedLeaveHotspotEvent = true;
                }
            }
        }
    }
}
