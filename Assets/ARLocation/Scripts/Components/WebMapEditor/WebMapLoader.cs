using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using UnityEngine;

namespace ARLocation {
    public class WebMapLoader : MonoBehaviour
    {

        public class DataEntry
        {
            public int id;
            public double lat;
            public double lng;
            public double altitude;
            public string altitudeMode;
            public string name;
            public string meshId;
            public float movementSmoothing;
            public int maxNumberOfLocationUpdates;
            public bool useMovingAverage;
            public bool hideObjectUtilItIsPlaced;

            public AltitudeMode getAltitudeMode()
            {
                if (altitudeMode == "GroundRelative") {
                    return AltitudeMode.GroundRelative;
                } else if (altitudeMode == "DeviceRelative") {
                    return AltitudeMode.DeviceRelative;
                } else if (altitudeMode == "Absolute") {
                    return AltitudeMode.Absolute;
                } else {
                    return AltitudeMode.Ignore;
                }
            }
        }

        /// <summary>
        ///   The `PrefabDatabase` ScriptableObject, containing a dictionary of Prefabs with a string ID.
        /// </summary>
        public PrefabDatabase PrefabDatabase;

        /// <summary>
        ///   The XML data file download from the Web Map Editor (htttps://editor.unity-ar-gps-location.com)
        /// </summary>
        public TextAsset XmlDataFile;

        /// <summary>
        ///   If true, enable DebugMode on the `PlaceAtLocation` generated instances.
        /// </summary>
        public bool DebugMode;

        private List<DataEntry> _dataEntries = new List<DataEntry>();
        private List<GameObject> _stages = new List<GameObject>();
        private List<PlaceAtLocation> _placeAtComponents = new List<PlaceAtLocation>();

        // Start is called before the first frame update
        void Start()
        {
            LoadXmlFile();
            BuildGameObjects();
        }

        void BuildGameObjects()
        {
            foreach (var entry in _dataEntries)
            {
                var Prefab = PrefabDatabase.GetEntryById(entry.meshId);

                if (!Prefab)
                {
                    Debug.LogWarning($"[ARLocation#WebMapLoader]: Prefab {entry.meshId} not found.");
                    continue;
                }

                var PlacementOptions = new PlaceAtLocation.PlaceAtOptions()
                    {
                        MovementSmoothing = entry.movementSmoothing,
                        MaxNumberOfLocationUpdates = entry.maxNumberOfLocationUpdates,
                        UseMovingAverage = entry.useMovingAverage,
                        HideObjectUntilItIsPlaced = entry.hideObjectUtilItIsPlaced
                    };

                var location = new Location()
                    {
                        Latitude = entry.lat,
                        Longitude = entry.lng,
                        Altitude = entry.altitude,
                        AltitudeMode = entry.getAltitudeMode(),
                        Label = entry.name
                    };

                var instance = PlaceAtLocation.CreatePlacedInstance(Prefab,
                                                                    location,
                                                                    PlacementOptions,
                                                                    DebugMode);

                _stages.Add(instance);
            }
        }

        // Update is called once per frame
        void LoadXmlFile()
        {
            var xmlString = XmlDataFile.text;

            Debug.Log(xmlString);

            XmlDocument xmlDoc = new XmlDocument();

            try {
                xmlDoc.LoadXml(xmlString);
            } catch(XmlException e) {
                Debug.LogError("[ARLocation#WebMapLoader]: Failed to parse XML file: " + e.Message);
            }

            var root = xmlDoc.FirstChild;
            var nodes = root.ChildNodes;
            foreach (XmlNode node in nodes)
            {
                Debug.Log(node.InnerXml);
                Debug.Log(node["id"].InnerText);

                int id = int.Parse(node["id"].InnerText);
                double lat = double.Parse(node["lat"].InnerText, CultureInfo.InvariantCulture);
                double lng = double.Parse(node["lng"].InnerText, CultureInfo.InvariantCulture);
                double altitude = double.Parse(node["altitude"].InnerText, CultureInfo.InvariantCulture);
                string altitudeMode = node["altitudeMode"].InnerText;
                string name = node["name"].InnerText;
                string meshId = node["meshId"].InnerText;
                float movementSmoothing = float.Parse(node["movementSmoothing"].InnerText, CultureInfo.InvariantCulture);
                int maxNumberOfLocationUpdates = int.Parse(node["maxNumberOfLocationUpdates"].InnerText);
                bool useMovingAverage = bool.Parse(node["useMovingAverage"].InnerText);
                bool hideObjectUtilItIsPlaced = bool.Parse(node["hideObjectUtilItIsPlaced"].InnerText);

                DataEntry entry = new DataEntry() {
                    id = id,
                    lat = lat,
                    lng = lng,
                    altitudeMode = altitudeMode,
                    altitude = altitude,
                    name = name,
                    meshId = meshId,
                    movementSmoothing = movementSmoothing,
                    maxNumberOfLocationUpdates = maxNumberOfLocationUpdates,
                    useMovingAverage =useMovingAverage,
                    hideObjectUtilItIsPlaced = hideObjectUtilItIsPlaced };

                _dataEntries.Add(entry);

                Debug.Log($"{id}, {lat}, {lng}, {altitude}, {altitudeMode}, {name}, {meshId}, {movementSmoothing}, {maxNumberOfLocationUpdates}, {useMovingAverage}, {hideObjectUtilItIsPlaced}");
            }
        }
    }
}
