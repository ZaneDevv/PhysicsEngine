using PhysicsEngine.Bodies;
using PhysicsEngine.Render;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace PhysicsEngine.Physics
{
    internal static class Physics
    {
        private readonly static Vector2 GRAVITY = new Vector2(0, 200);
        private readonly static int ITERATIONS = 4;

        private readonly static double EPSILON = 0.0005;

        private readonly static Vector2[] contactPoints = new Vector2[2];
        private readonly static Vector2[] impulses = new Vector2[2];
        private readonly static Vector2[] frictionImpulses = new Vector2[2];
        private readonly static Vector2[] raList = new Vector2[2];
        private readonly static Vector2[] rbList = new Vector2[2];
        private readonly static double[] impulseMagnitudes = new double[2];

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

                Physics.ApplyGravity(body, deltaTime);
            }

            // Removes unneccessary objects
            foreach (Body body in BodiesToRemove)
            {
                bodies.Remove(body);
            }

            // Solve collisions
            for (int index = 0; index < ITERATIONS; index++)
            {
                Physics.DoCollisions(bodies);
            }
        }

        /// <summary>
        /// Applies gravity to a body
        /// </summary>
        /// <param name="body">Body to apply gravity</param>
        /// <param name="deltaTime">Time passed since the last frame</param>
        private static void ApplyGravity(Body body, double deltaTime)
        {
            body.ApplyForce(GRAVITY * (float)body.Mass);
            body.Update(deltaTime);
        }

        /// <summary>
        /// Checks and solves the collisions
        /// </summary>
        /// <param name="bodies">Bodies to check collisions</param>
        private static void DoCollisions(List<Body> bodies)
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

                        areColliding = Collisions.Collision.Polygon_VS_Polygon(polygon1, polygon2, ref normal, ref depth);
                    }
                    else if (body1.BodyType is BodyType.Circle && body2.BodyType is BodyType.Circle)
                    {
                        Circle circle1 = (Circle)body1.Shape;
                        Circle circle2 = (Circle)body2.Shape;

                        Vector3 position1 = new Vector3(circle1.Position.X, circle1.Position.Y, 0);
                        Vector3 position2 = new Vector3(circle2.Position.X, circle2.Position.Y, 0);

                        areColliding = Collisions.Collision.Circle_VS_Circle(position1, circle1.Radius, position2, circle2.Radius, ref normal, ref depth);
                    }
                    else
                    {
                        Quad polygon = (Quad)(body1.BodyType == BodyType.Quad ? body1.Shape : body2.Shape);
                        Circle circle = (Circle)(body1.BodyType == BodyType.Circle ? body1.Shape : body2.Shape);

                        areColliding = Collisions.Collision.Circle_VS_Polygon(polygon.Vertices, new Vector3(circle.Position.X, circle.Position.Y, 0), circle.Radius, ref normal, ref depth);
                    }

                    if (!areColliding) continue;

                    Physics.SolveCollisions(body1, body2, new Vector2(normal.X, normal.Y), depth);
                }
            }
        }

        /// <summary>
        /// Solves the given collision
        /// </summary>
        /// <param name="body1">First collided body</param>
        /// <param name="body2">Second collided body</param>
        /// <param name="normal">Collision normal vector</param>
        /// <param name="depth">How much are the bodies overlaping</param>
        private static void SolveCollisions(Body body1, Body body2, Vector2 normal, double depth)
        {
            if (body1.DoesPhysicsAffect)
            {
                body1.Position -= normal * (float)depth / (body2.DoesPhysicsAffect ? 2 : 1);
            }
            if (body2.DoesPhysicsAffect)
            {
                body2.Position += normal * (float)depth / (body1.DoesPhysicsAffect ? 2 : 1);
            }

            double minRestitution = Math.Min(body1.Restitution, body2.Restitution);

            PhysicsEngine.Collisions.Collision.GetContactCollisionPoints(body1, body2, out short contactPointsAmount, out Vector3 contactPoint1, out Vector3 contactPoint2);

            Physics.contactPoints[0] = new Vector2(contactPoint1.X, contactPoint1.Y);
            Physics.contactPoints[1] = new Vector2(contactPoint2.X, contactPoint2.Y);

            for (byte index = 0; index < contactPointsAmount; index++)
            {
                Vector2 ra = Physics.contactPoints[index] - body1.Position;
                Vector2 rb = Physics.contactPoints[index] - body2.Position;

                Vector2 raPerpendicular = new Vector2(-ra.Y, ra.X);
                Vector2 rbPerpendicular = new Vector2(-rb.Y, rb.X);

                Vector2 angularVelocityDirection1 = raPerpendicular * (float)body1.AngularVelocity;
                Vector2 angularVelocityDirection2 = rbPerpendicular * (float)body2.AngularVelocity;

                Vector2 relativeVelocity = body2.LinearVelocity + angularVelocityDirection2 - body1.LinearVelocity - angularVelocityDirection1;

                double contactVelocityMagnitude = Vector2.Dot(normal, relativeVelocity);

                double raPerpendicularDotNormal = Vector2.Dot(raPerpendicular, normal);
                double rbPerpendicularDotNormal = Vector2.Dot(rbPerpendicular, normal);

                double nominator = -(1 + minRestitution) * contactVelocityMagnitude;
                double denominator = 1 / body1.Mass + 1 / body2.Mass +
                    (raPerpendicularDotNormal * raPerpendicularDotNormal) / body1.RotationalIntertia +
                    (rbPerpendicularDotNormal * rbPerpendicularDotNormal) / body2.RotationalIntertia;

                double impulseMagnitude = nominator / denominator / contactPointsAmount;

                Physics.impulseMagnitudes[index] = impulseMagnitude;
                Physics.impulses[index] = (float)impulseMagnitude * normal;
                Physics.raList[index] = ra;
                Physics.rbList[index] = rb;
            }

            for (byte index = 0; index < contactPointsAmount; index++)
            {
                if (body1.DoesPhysicsAffect)
                {
                    body1.AngularVelocity -= Physics.Determinant(Physics.raList[index], Physics.impulses[index]) / body1.RotationalIntertia;
                    body1.LinearVelocity -= Physics.impulses[index] / (float)body1.Mass;
                }
                if (body2.DoesPhysicsAffect)
                {
                    body2.AngularVelocity += Physics.Determinant(Physics.rbList[index], Physics.impulses[index]) / body2.RotationalIntertia;
                    body2.LinearVelocity += Physics.impulses[index] / (float)body2.Mass;
                }
            }

            for (byte index = 0; index < contactPointsAmount; index++)
            {
                double staticFriction = (body1.StaticFriction + body2.StaticFriction) / 2;
                double dynamicFriction = (body1.DynamicFriction + body2.DynamicFriction) / 2;

                Vector2 ra = Physics.raList[index];
                Vector2 rb = Physics.rbList[index];

                Vector2 raPerpendicular = new Vector2(-ra.Y, ra.X);
                Vector2 rbPerpendicular = new Vector2(-rb.Y, rb.X);

                Vector2 angularVelocityDirection1 = raPerpendicular * (float)body1.AngularVelocity;
                Vector2 angularVelocityDirection2 = rbPerpendicular * (float)body2.AngularVelocity;

                Vector2 relativeVelocity = body2.LinearVelocity + angularVelocityDirection2 - body1.LinearVelocity - angularVelocityDirection1;

                Vector2 tangent = relativeVelocity - Vector2.Dot(relativeVelocity, normal) * normal;
                if (tangent.LengthSquared() <= Physics.EPSILON * Physics.EPSILON) continue;

                tangent = Vector2.Normalize(tangent);

                double contactVelocityMagnitude = Vector2.Dot(tangent, relativeVelocity);

                double raPerpendicularDotTangent = Vector2.Dot(raPerpendicular, tangent);
                double rbPerpendicularDotTangent = Vector2.Dot(rbPerpendicular, tangent);

                double denominator = 1 / body1.Mass + 1 / body2.Mass +
                    (raPerpendicularDotTangent * raPerpendicularDotTangent) / body1.RotationalIntertia +
                    (rbPerpendicularDotTangent * rbPerpendicularDotTangent) / body2.RotationalIntertia;

                double frictionMagnitude = -contactVelocityMagnitude / denominator / contactPointsAmount;

                Vector2 frictionImpulse = Math.Abs(frictionMagnitude) <= Physics.impulseMagnitudes[index] ?
                    frictionImpulse = (float)frictionMagnitude * tangent :
                    (float)(Physics.impulseMagnitudes[index] * dynamicFriction) * tangent;

                Physics.frictionImpulses[index] = frictionImpulse;
            }

            for (byte index = 0; index < contactPointsAmount; index++)
            {
                if (body1.DoesPhysicsAffect)
                {
                    body1.AngularVelocity -= Physics.Determinant(Physics.raList[index], Physics.frictionImpulses[index]) / body1.RotationalIntertia;
                    body1.LinearVelocity -= Physics.frictionImpulses[index] / (float)body1.Mass;
                }
                if (body2.DoesPhysicsAffect)
                {
                    body2.AngularVelocity += Physics.Determinant(Physics.rbList[index], Physics.frictionImpulses[index]) / body2.RotationalIntertia;
                    body2.LinearVelocity += Physics.frictionImpulses[index] / (float)body2.Mass;
                }
            }

            //double minRestitution = Math.Min(body1.Restitution, body2.Restitution);
            //double p = -minRestitution * Vector2.Dot(body2.LinearVelocity - body1.LinearVelocity, normal) / (1 / body1.Mass + 1 / body2.Mass);

            //if (body1.DoesPhysicsAffect)
            //{
            //    body1.Position -= normal * (float)depth / (body2.DoesPhysicsAffect ? 2 : 1);
            //    body1.LinearVelocity -= normal * (float)(p / body1.Mass);
            //}
            //if (body2.DoesPhysicsAffect)
            //{
            //    body2.Position += normal * (float)depth / (body1.DoesPhysicsAffect ? 2 : 1);
            //    body2.LinearVelocity += normal * (float)(p / body2.Mass);
            //}
        }

        /// <summary>
        /// Calculates the determinant of two vectors
        /// </summary>
        /// <param name="vector1">First vector</param>
        /// <param name="vector2">Second vector</param>
        /// <returns>The determinant of the vectors</returns>
        private static double Determinant(Vector2 vector1, Vector2 vector2) => vector1.X * vector2.Y - vector2.X * vector1.Y;
    }
}
