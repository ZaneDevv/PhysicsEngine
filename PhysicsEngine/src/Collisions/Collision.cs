using Microsoft.Xna.Framework;

namespace PhysicsEngine.Collisions
{
    internal struct Collision
    {
        /// <summary>
        /// Checks ahd gives information about the possible collision of two circles
        /// </summary>
        /// <param name="center1">Center position of the frist circle</param>
        /// <param name="radius1">Radius of the first circle</param>
        /// <param name="center2">Center position of the second circle</param>
        /// <param name="radius2">Radius of the second circle</param>
        /// <param name="normal">The normal of the collision</param>
        /// <param name="depth">How much are the circles overlaping</param>
        /// <returns>Returns if the two circles are overlaping</returns>
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
