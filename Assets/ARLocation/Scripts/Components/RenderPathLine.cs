using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace ARLocation
{
    using Utils;

    /// <summary>
    /// This component renders a LocationPath using a given LineRenderer.
    /// </summary>
    [AddComponentMenu("AR+GPS/Render Path Line")]
    [HelpURL("https://http://docs.unity-ar-gps-location.com/guide/#renderpathline")]
    public class RenderPathLine : MonoBehaviour
    {
        public MoveAlongPath.PathSettingsData PathSettings;
        public MoveAlongPath.PlacementSettingsData PlacementSettings;
        private Transform arLocationRoot;

        public void Start()
        {
            if (PathSettings.LineRenderer == null)
            {
                var lineRenderer = gameObject.GetComponent<LineRenderer>();

                if (!lineRenderer)
                {
                    throw new NullReferenceException("[AR+GPS][RenderPathLine#Start]: No Line Renderer!");
                }

                PathSettings.LineRenderer = lineRenderer;
            }

            arLocationRoot = ARLocationManager.Instance.gameObject.transform;

            var pathGameObject = new GameObject($"{gameObject.name} - RenderPathLine");

            var moveAlongPath = pathGameObject.AddComponent<MoveAlongPath>();
            moveAlongPath.PathSettings = PathSettings;
            moveAlongPath.PlacementSettings = PlacementSettings;
        }
    }
}
