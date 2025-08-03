namespace PhysicsEngine.Collisions
{
    internal struct AABB
    {
        double minX, maxX, minY, maxY;

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
            bool aabb1Overlaping = aabb1.minX < aabb2.maxX && aabb1.maxX > aabb2.minX;
            bool aabb2Overlaping = aabb1.minY < aabb2.maxY && aabb1.maxY > aabb2.minY;

            return aabb1Overlaping || aabb2Overlaping;
        }
    }
}
