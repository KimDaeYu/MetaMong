using UnityEngine;

namespace ARLocation
{

    public class MockLocationProvider : AbstractLocationProvider
    {
        public override string Name => "MockLocationProvider";

        public override bool IsCompassEnabled => true;

        public Location mockLocation = new Location();

        protected override HeadingReading? ReadHeading()
        {
            var mainCamera = ARLocationManager.Instance.MainCamera;

            var transform = mainCamera.transform;

            var localEulerAngles = transform.localEulerAngles;
            return new HeadingReading
            {
                heading = localEulerAngles.y,
                magneticHeading = localEulerAngles.y,
                accuracy = 0,
                isMagneticHeadingAvailable = true,
                timestamp = (long)(Time.time * 1000)
            };
        }

        protected override LocationReading? ReadLocation()
        {
            return new LocationReading
            {
                latitude = mockLocation.Latitude,
                longitude = mockLocation.Longitude,
                altitude = mockLocation.Altitude,
                accuracy = 0.0,
                floor = -1,
                timestamp = (long)(Time.time * 1000)
            };
        }

        private bool requested = true;

        protected override void RequestLocationAndCompassUpdates()
        {
            requested = true;
        }

        protected override void UpdateLocationRequestStatus()
        {
            if (requested)
            {
                Status = LocationProviderStatus.Initializing;
                requested = false;
            }

            if (Status == LocationProviderStatus.Initializing)
            {
                Status = LocationProviderStatus.Started;
            }
        }
    }
}
