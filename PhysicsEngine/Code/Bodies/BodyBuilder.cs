using PhysicsEngine.Render;
using Microsoft.Xna.Framework;

namespace PhysicsEngine.Bodies
{
    internal sealed class BodyBuilder
    {
        #region ATTRIBUTES

        private double mass = 1;

        private bool isCollideable = true;
        private bool doesPhysicsAffect = true;

        private RenderShape shape;

        private Vector2 position = Vector2.Zero;

        #endregion

        /// <summary>
        /// Sets the mass for the body
        /// </summary>
        /// <param name="mass">Body's mass</param>
        /// <returns>The proper class</returns>
        internal BodyBuilder SetMass(double mass)
        {
            this.mass = mass;
            return this;
        }

        /// <summary>
        /// Sets if the body has collisions
        /// </summary>
        /// <param name="isCollideable"></param>
        /// <returns>The proper class</returns>
        internal BodyBuilder SetCollisions(bool isCollideable)
        {
            this.isCollideable = isCollideable;
            return this;
        }

        /// <summary>
        /// Sets if the physics should affect to this body
        /// </summary>
        /// <param name="doesPhysicsAffect"></param>
        /// <returns>The proper class</returns>
        internal BodyBuilder SetPhysics(bool doesPhysicsAffect)
        {
            this.doesPhysicsAffect = doesPhysicsAffect;
            return this;
        }

        /// <summary>
        /// Sets the shape of the body
        /// </summary>
        /// <param name="shape">Body's shape</param>
        /// <returns>The proper class</returns>
        internal BodyBuilder SetShape(RenderShape shape)
        {
            this.shape = shape;
            return this;
        }

        /// <summary>
        /// Sets the body's position
        /// </summary>
        /// <param name="position">Body's position</param>
        /// <returns>The proper class</returns>
        internal BodyBuilder SetPosition(Vector2 position)
        {
            this.position = position;
            return this;
        }

        /// <summary>
        /// Creates a new body according to the specified properties
        /// </summary>
        /// <returns>Body created</returns>
        internal Body Build() => new Body(
            this.mass,
            this.isCollideable, this.doesPhysicsAffect,
            this.shape, this.position);
    }
}