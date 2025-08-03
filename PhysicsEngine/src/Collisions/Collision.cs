using Microsoft.Xna.Framework;

namespace PhysicsEngine.Collisions
{
    internal struct Collision
    {
        internal static bool Circle_VS_Circle(Vector3 center1, double radius1, Vector3 center2, double radius2, out Vector3 normal, out double depth)
        {
            double radiusAddition = radius1 + radius2;
            double distance = Vector3.Distance(center1, center2);

            if (distance >= radiusAddition)
            {
                normal = Vector3.Zero;
                depth = 0;

                return false;
            }

            depth = radiusAddition - distance;
            normal = Vector3.Normalize(center1 - center2);

            return true;
        }
    }
}
