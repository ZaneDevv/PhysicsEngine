namespace PhysicsEngine.Collisions
{
    internal struct AABB
    {
        double minX, maxX;
        double minY, maxY;

        /// <summary>
        /// Creates a new AABB
        /// </summary>
        /// <param name="minX">Minimum value in X axis</param>
        /// <param name="maxX">Maximum value in X axis</param>
        /// <param name="minY">Minimum value in Y axis</param>
        /// <param name="maxY">Maximum value in Y axis</param>
        internal AABB(double minX, double maxX, double minY, double maxY)
        {
            this.minX = minX;
            this.maxX = maxX;
            this.minY = minY;
            this.maxY = maxY;
        }

        /// <summary>
        /// Checks if two AABBs are overlaping
        /// </summary>
        /// <param name="aabb1">One AABB to compare</param>
        /// <param name="aabb2">The other AABB to compare</param>
        /// <returns>Returns true if the two AABBs are overlaping and false if they are not</returns>
        internal static bool AreOverlaping(AABB aabb1, AABB aabb2)
        {
            bool aabb1Overlaping = AreOverlaping(aabb1.minX, aabb1.maxX, aabb2.minX, aabb2.maxX);
            bool aabb2Overlaping = AreOverlaping(aabb1.minY, aabb1.maxY, aabb2.minY, aabb2.maxY);

            return aabb1Overlaping || aabb2Overlaping;
        }

        /// <summary>
        /// Checks if a line intersects with another in 1D
        /// </summary>
        /// <param name="min1">The frist line minimum extreme</param>
        /// <param name="max1">The frist line maximum extreme</param>
        /// <param name="min2">The second line minimum extreme</param>
        /// <param name="max2">The second line maximum extreme</param>
        /// <returns>Returns if the lines defined by its extremes in 1D are overlaping</returns>
        internal static bool AreOverlaping(double min1, double max1, double min2, double max2) => min1 < max2 && max1 > min2;
    }
}
