using UnityEngine;

namespace ARLocation
{
    public static class MathUtils
    {
        public static Vector2 HorizontalVector(Vector3 v)
        {
            return new Vector2(v.x, v.z);
        }

        public static Vector3 HorizontalVectorToVector3(Vector2 v, float y = 0.0f)
        {
            return new Vector3(v.x, y, v.y);
        }

        public static float HorizontalDistance(Vector3 a, Vector3 b)
        {
            return Vector2.Distance(HorizontalVector(a), HorizontalVector(b));
        }

        public static Vector3 SetY(Vector3 v, float y)
        {
            return new Vector3(v.x, y, v.z);
        }

        public static float DegreesToRadians(float degrees)
        {
            return Mathf.PI * degrees / 180.0f;
        }

        public static float RadiansToDegrees(float degrees)
        {
            return 180.0f * degrees / Mathf.PI;
        }

        public static class Double
        {
            public static double DegreesToRadians(double degrees)
            {
                return System.Math.PI * degrees / 180.0;
            }

            public static double RadiansToDegrees(double degrees)
            {
                return 180.0 * degrees / System.Math.PI;
            }
        }
    }
}
