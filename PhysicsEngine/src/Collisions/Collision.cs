using System;
using PhysicsEngine.Bodies;
using PhysicsEngine.Render;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace PhysicsEngine.Collisions
{
    internal struct Collision
    {
        private static readonly double Epsilon = 0.0005;

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
        internal static bool Circle_VS_Circle(Vector3 center1, double radius1, Vector3 center2, double radius2, ref Vector3 normal, ref double depth)
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
            normal = Vector3.Normalize(center2 - center1);

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
        internal static bool Polygon_VS_Polygon(Quad quad1, Quad quad2, ref Vector3 normal, ref double depth)
        {
            normal = Vector3.Zero;
            depth = double.MaxValue;

            Vector3[] vertices1 = quad1.Vertices;
            Vector3[] vertices2 = quad2.Vertices;

            List<Vector3> axes = new List<Vector3>();

            AddAxesFromVertices(ref axes, vertices1);
            AddAxesFromVertices(ref axes, vertices2);

            foreach (Vector3 axis in axes)
            {
                VerticesProjectionOntoAxis(axis, vertices1, out double min1, out double max1);
                VerticesProjectionOntoAxis(axis, vertices2, out double min2, out double max2);

                if (!AABB.AreOverlaping(min1, max1, min2, max2)) return false;

                double minDepth = Math.Min(max1 - min2, max2 - min1);

                if (minDepth < depth)
                {
                    depth = minDepth;
                    normal = axis;
                }
            }

            if (normal.X * (quad2.Position.X - quad1.Position.X) + normal.Y * (quad2.Position.Y - quad1.Position.Y) < 0)
            {
                normal = -normal;
            }

            return true;
        }

        /// <summary>
        /// Checks and gives information about the possible collision betwee a polygon and a circle
        /// </summary>
        /// <param name="polygonVertices">Polygon's vertices</param>
        /// <param name="circlePosition">Circle's center position</param>
        /// <param name="circleRadius">Circle's radius</param>
        /// <param name="normal">The collisions' normal vector</param>
        /// <param name="depth">How much are the polygons overlaping</param>
        /// <returns></returns>
        internal static bool Circle_VS_Polygon(Vector3[] polygonVertices, Vector3 circlePosition, double circleRadius, ref Vector3 normal, ref double depth)
        {
            normal = Vector3.Zero;
            depth = double.MaxValue;

            List<Vector3> axes = new List<Vector3>();
            AddAxesFromVertices(ref axes, polygonVertices);

            double closestDistance = double.MaxValue;
            Vector3 closestPoint = Vector3.Zero;

            foreach (Vector3 vertex in polygonVertices)
            {
                double squaredDistance = Vector3.DistanceSquared(vertex, circlePosition);

                if (squaredDistance < closestDistance)
                {
                    closestDistance = squaredDistance;
                    closestPoint = vertex;
                }
            }

            axes.Add(Vector3.Normalize(closestPoint - circlePosition));

            foreach (Vector3 axis in axes)
            {

                Vector3[] circlePseudovertices = new Vector3[] {
                    new Vector3(circlePosition.X, circlePosition.Y, 0) - axis * (float)circleRadius,
                    new Vector3(circlePosition.X, circlePosition.Y, 0) + axis * (float)circleRadius
                };

                VerticesProjectionOntoAxis(axis, polygonVertices, out double min1, out double max1);
                VerticesProjectionOntoAxis(axis, circlePseudovertices, out double min2, out double max2);

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

        /// <summary>
        /// Adds the normals of each edge to a list for axes
        /// </summary>
        /// <param name="axesList">Axes list</param>
        /// <param name="vertices">Vertices to calculate the normals and axes</param>
        private static void AddAxesFromVertices(ref List<Vector3> axesList, Vector3[] vertices)
        {
            for (short index = 0; index < vertices.Length; index++)
            {
                Vector3 currentVertex = vertices[index];
                Vector3 nextVertex = vertices[(index + 1) % vertices.Length];
                Vector3 edge = nextVertex - currentVertex;
                Vector3 axis = new Vector3(-edge.Y, edge.X, 0);

                axesList.Add(Vector3.Normalize(axis));
            }
        }

        /// <summary>
        /// Projects the vertices of a shape onto an axis
        /// </summary>
        /// <param name="axis">The axis to project onto</param>
        /// <param name="vertices">The vertices to project</param>
        /// <param name="min">The minimum value of the projection</param>
        /// <param name="max">The maximum value of the projection</param>
        private static void VerticesProjectionOntoAxis(Vector3 axis, Vector3[] vertices, out double min, out double max)
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

        /// <summary>
        /// Calculates the contact points of the collision
        /// </summary>
        /// <param name="body1">First collided body</param>
        /// <param name="body2">Second collided body</param>
        /// <param name="pointsAmount">Contact points amount [1, 2]</param>
        /// <param name="contactPoint1">First contact point</param>
        /// <param name="contactPoint2">Possible second contact point</param>
        internal static void GetContactCollisionPoints(Body body1, Body body2, ref short pointsAmount, ref Vector3 contactPoint1, ref Vector3 contactPoint2)
        {
            if (body1.BodyType is BodyType.Circle && body2.BodyType is BodyType.Circle)
            {
                Circle circle1 = (Circle)body1.Shape;
                Circle circle2 = (Circle)body2.Shape;

                Vector3 position = new Vector3(circle1.Position.X, circle1.Position.Y, 0);
                Vector3 direction = new Vector3(body2.Position.X - body1.Position.X, body2.Position.Y - body1.Position.Y, 0);
                direction = Vector3.Normalize(direction);

                pointsAmount = 1;
                contactPoint1 = position +  direction * (float)circle1.Radius;
            }
            else if (body1.BodyType is BodyType.Quad && body2.BodyType is BodyType.Quad)
            {
                Quad quad1 = (Quad)body1.Shape;
                Quad quad2 = (Quad)body2.Shape;

                double minimumDistance = double.MaxValue;

                void GetContactPoints(Vector3[] vertices1, Vector3[] vertices2, ref short pointsAmount, ref Vector3 contactPoint1, ref Vector3 contactPoint2)
                {
                    for (short i = 0; i < vertices1.Length; i++)
                    {
                        Vector3 vertex1 = vertices1[i];

                        for (short j = 0; j < vertices2.Length; j++)
                        {
                            Vector3 vertex2 = vertices2[j];
                            Vector3 nextVertex2 = vertices2[(j + 1) % vertices2.Length];

                            double squaredDistance = Collision.SquaredDistancePointSegment(vertex1, vertex2, nextVertex2, out Vector3 pointInLine);

                            if (Collision.AreNumbersClose(squaredDistance, minimumDistance))
                            {
                                bool arePointsCloseInX = Collision.AreNumbersClose(contactPoint1.X, pointInLine.X);
                                bool arePointsCloseInY = Collision.AreNumbersClose(contactPoint1.Y, pointInLine.Y);

                                if (arePointsCloseInX && arePointsCloseInY) continue;

                                pointsAmount = 2;
                                contactPoint2 = pointInLine;
                            }
                            else if (squaredDistance < minimumDistance)
                            {
                                pointsAmount = 1;
                                minimumDistance = squaredDistance;
                                contactPoint1 = pointInLine;
                            }
                        }
                    }

                }
                GetContactPoints(quad1.Vertices, quad2.Vertices, ref pointsAmount, ref contactPoint1, ref contactPoint2);
                GetContactPoints(quad2.Vertices, quad1.Vertices, ref pointsAmount, ref contactPoint1, ref contactPoint2);
            }
            else
            {
                pointsAmount = 1;

                Quad quad = (Quad)(body1.BodyType is BodyType.Quad ? body1.Shape : body2.Shape);
                Circle circle = (Circle)(body1.BodyType is BodyType.Circle ? body1.Shape : body2.Shape);

                Vector3 circlePosition = new Vector3(circle.Position.X, circle.Position.Y, 0);

                double closestSuqaredDistance = double.MaxValue;

                for (short i = 0; i < quad.Vertices.Length; i++)
                {
                    Vector3 vertex = quad.Vertices[i];
                    Vector3 nextVertex = quad.Vertices[(i + 1) % quad.Vertices.Length];

                    double squaredDistance = Collision.SquaredDistancePointSegment(circlePosition, vertex, nextVertex, out Vector3 pointInLine);

                    if (squaredDistance < closestSuqaredDistance)
                    {
                        closestSuqaredDistance = squaredDistance;
                        contactPoint1 = pointInLine;
                    }
                }
            }
        }

        /// <summary>
        /// Calculates the squared distance to the closets point in a line enclosed by two points
        /// </summary>
        /// <param name="point">Point to get the distance</param>
        /// <param name="segmentPoint1">First point to define the line</param>
        /// <param name="segmentPoint2">Second point to define the line</param>
        /// <returns>Closets squared distance to the point from any point in the line</returns>
        private static double SquaredDistancePointSegment(Vector3 point, Vector3 segmentPoint1, Vector3 segmentPoint2, out Vector3 pointInLine)
        {
            Vector3 line = segmentPoint2 - segmentPoint1;

            double projection = Vector3.Dot(line, point - segmentPoint1);
            double normalizedPorjection = projection / (double)line.LengthSquared();

            pointInLine = normalizedPorjection < 0 ? segmentPoint1 : normalizedPorjection > 1 ? segmentPoint2 : segmentPoint1 + line * (float)normalizedPorjection;

            return Vector3.DistanceSquared(point, pointInLine);
        }

        /// <summary>
        /// Checks if two numbers are close enough to each other
        /// </summary>
        /// <param name="x">First number to compare</param>
        /// <param name="y">Second number to compare</param>
        /// <returns>Returns if the numbers are close to each other</returns>
        private static bool AreNumbersClose(double x, double y) => Math.Abs(x - y) <= Collision.Epsilon;
    }
}