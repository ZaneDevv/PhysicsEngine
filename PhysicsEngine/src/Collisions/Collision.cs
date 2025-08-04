using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace PhysicsEngine.Collisions
{
    internal struct Collision
    {
        /// <summary>
        /// Checks and gives information about the possible collision betwee two circles
        /// </summary>
        /// <param name="center1">Center position of the frist circle</param>
        /// <param name="radius1">Radius of the first circle</param>
        /// <param name="center2">Center position of the second circle</param>
        /// <param name="radius2">Radius of the second circle</param>
        /// <param name="normal">The collisions' normal vector</param>
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

        /// <summary>
        /// Checks and gives information about the possible collision betwee two polygons
        /// </summary>
        /// <param name="vertices1">First polygon's vertices</param>
        /// <param name="vertices2">Second polygon's vertices</param>
        /// <param name="normal">The collisions' normal vector</param>
        /// <param name="depth">How much are the polygons overlaping</param>
        /// <returns>Returns if the two polygons are overlaping</returns>
        internal static bool Polygon_VS_Polygon(Vector3[] vertices1, Vector3[] vertices2, out Vector3 normal, out double depth)
        {
            normal = Vector3.Zero;
            depth = double.MaxValue;

            List<Vector3> axes = new List<Vector3>();

            void AddAxesFromVertices(Vector3[] vertices)
            {
                for (short index = 0; index < vertices.Length; index++)
                {
                    Vector3 currentVertex = vertices[index];
                    Vector3 nextVertex = vertices[(index + 1) % vertices.Length];
                    Vector3 edge = Vector3.Normalize(nextVertex - currentVertex);
                    Vector3 axis = new Vector3(-edge.Y, edge.X, 0);

                    axes.Add(axis);
                }
            }

            void VerticesProjectionOntoAxis(Vector3 axis, Vector3[] vertices, out double min, out double max)
            {
                min = double.MaxValue;
                max = double.MinValue;

                foreach (Vector3 vertex in vertices)
                {
                    double projection = Vector3.Dot(vertex, axis);

                    if (min > projection) min = projection;
                    if (max < projection) max = projection;   
                }
            }

            AddAxesFromVertices(vertices1);
            AddAxesFromVertices(vertices2);

            foreach (Vector3 axis in axes)
            {
                double min1, max1;
                double min2, max2;

                VerticesProjectionOntoAxis(axis, vertices1, out min1, out max1);
                VerticesProjectionOntoAxis(axis, vertices2, out min2, out max2);

                if (!AABB.AreOverlaping(min1, max1, min2, max2)) return false;

                double minDepth = Math.Min(max1 - min2, max2 - min1);

                if (minDepth < depth)
                {
                    depth = minDepth;
                    normal = axis;
                }
            }

            return true;
        }

        internal static bool Circle_VS_Polygon()
        {
            return true;
        }
    }
}
