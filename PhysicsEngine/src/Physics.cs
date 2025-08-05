using PhysicsEngine.Bodies;
using PhysicsEngine.Render;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace PhysicsEngine.Physics
{
    internal static class Physics
    {
        private readonly static Vector2 GRAVITY = new Vector2(0, 100);

        /// <summary>
        /// Updates physics for all the specified bodies
        /// </summary>
        /// <param name="bodies">Bodies to update physcis</param>
        /// <param name="deltaTime">Time passed since the last frame</param>
        /// <param name="graphics">Graphics data</param>
        internal static void UpdatePhysics(ref List<Body> bodies, double deltaTime, GraphicsDeviceManager graphics)
        {
            List<Body> BodiesToRemove = new List<Body>();

            // Applies gravity
            foreach (Body body in bodies)
            {
                bool isTooDown = body.Position.Y > graphics.PreferredBackBufferHeight + body.Shape.Size.Y;
                bool isTooRight = body.Position.X > graphics.PreferredBackBufferWidth + body.Shape.Size.X;
                bool isTooLeft = body.Position.X < -body.Shape.Size.X;
                bool willBeRemoved = isTooDown || isTooLeft || isTooRight;

                if (willBeRemoved)
                {
                    BodiesToRemove.Add(body);
                    continue;
                }

                if (!body.DoesPhysicsAffect) continue;

                body.ApplyForce(GRAVITY * (float)body.Mass);
                body.Update(deltaTime);
            }

            // Removes unneccessary objects
            foreach (Body body in BodiesToRemove)
            {
                bodies.Remove(body);
            }

            // Solve collisions
            for (int index = 0; index < 5; index++)
            {
                Physics.SolveCollisions(ref bodies);
            }
        }

        /// <summary>
        /// Checks and solves the collisions
        /// </summary>
        /// <param name="bodies">Bodies to check collisions</param>
        private static void SolveCollisions(ref List<Body> bodies)
        {
            for (int i = 0; i < bodies.Count; i++)
            {
                Body body1 = bodies[i];
                if (!body1.IsCollideable) continue;

                for (int j = 0; j < bodies.Count; j++)
                {
                    Body body2 = bodies[j];
                    if (!body2.IsCollideable) continue;

                    if (body1 == body2) continue;

                    if (!Collisions.AABB.AreOverlaping(body1.Shape.GetAABB(), body2.Shape.GetAABB())) continue;

                    Vector3 normal = Vector3.Zero;
                    double depth = 0;
                    bool areColliding = false;

                    if (body1.BodyType is BodyType.Quad && body2.BodyType is BodyType.Quad)
                    {
                        Quad polygon1 = (Quad)body1.Shape;
                        Quad polygon2 = (Quad)body2.Shape;

                        areColliding = Collisions.Collision.Polygon_VS_Polygon(polygon1.Vertices, polygon2.Vertices, out normal, out depth);
                    }
                    else if (body1.BodyType is BodyType.Circle && body2.BodyType is BodyType.Circle)
                    {
                        Circle circle1 = (Circle)body1.Shape;
                        Circle circle2 = (Circle)body2.Shape;

                        Vector3 position1 = new Vector3(circle1.Position.X, circle1.Position.Y, 0);
                        Vector3 position2 = new Vector3(circle2.Position.X, circle2.Position.Y, 0);

                        areColliding = Collisions.Collision.Circle_VS_Circle(position1, circle1.Radius, position2, circle2.Radius, out normal, out depth);
                    }
                    else
                    {
                        Quad polygon = (Quad)(body1.BodyType == BodyType.Quad ? body1.Shape : body2.Shape);
                        Circle circle = (Circle)(body1.BodyType == BodyType.Circle ? body1.Shape : body2.Shape);

                        areColliding = Collisions.Collision.Circle_VS_Polygon(polygon.Vertices, new Vector3(circle.Position.X, circle.Position.Y, 0), circle.Radius, out normal, out depth);
                    }

                    if (!areColliding) continue;

                    short contactPointsAmount = 0;
                    Vector3 contactPoint1;
                    Vector3 contactPoint2;

                    Collisions.Collision.GetContactCollisionPoints(body1, body2, out contactPointsAmount, out contactPoint1, out contactPoint2);


                    Vector2 normal2D = new Vector2(normal.X, normal.Y);

                    double minRestitution = Math.Min(body1.Restitution, body2.Restitution);
                    double p = minRestitution * Vector2.Dot(body2.LinearVelocity - body1.LinearVelocity, normal2D) / (1 / body1.Mass + 1 / body2.Mass);

                    if (body1.DoesPhysicsAffect)
                    {
                        body1.Position += normal2D * (float)depth / (body2.DoesPhysicsAffect ? 2 : 1);
                        body1.LinearVelocity += normal2D * (float)(p / body1.Mass);
                    }
                    if (body2.DoesPhysicsAffect)
                    {
                        body2.Position -= normal2D * (float)depth / (body1.DoesPhysicsAffect ? 2 : 1);
                        body2.LinearVelocity -= normal2D * (float)(p / body2.Mass);
                    }
                }
            }
        }
    }
}
