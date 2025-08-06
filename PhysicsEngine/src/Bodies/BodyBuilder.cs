using PhysicsEngine.Render;
using Microsoft.Xna.Framework;

namespace PhysicsEngine.Bodies
{
    internal sealed class BodyBuilder
    {
        #region ATTRIBUTES

        private double mass = 1;
        private double restitution = 1;
        private double intertia = -5;

        private bool isCollideable = true;
        private bool doesPhysicsAffect = true;

        private RenderShape shape;
        private BodyType bodyType;

        private Vector2 position = Vector2.Zero;

        private double rotation = 0;

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
        /// Sets the restitution for the body
        /// </summary>
        /// <param name="restitution">Body's restitution</param>
        /// <returns>The proper class</returns>
        internal BodyBuilder SetRestitution(double restitution)
        {
            this.restitution = restitution;
            return this;
        }

        /// <summary>
        /// Sets the intertia for the body
        /// </summary>
        /// <param name="intertia">Body's intertia</param>
        /// <returns>The proper class</returns>
        internal BodyBuilder SetIntertia(double intertia)
        {
            this.intertia = intertia;
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
        /// Sets if the body is either a circle or a quad physically
        /// </summary>
        /// <param name="bodyType">Type of the body</param>
        /// <returns>The proper class</returns>
        internal BodyBuilder SetBodyType(BodyType bodyType)
        {
            this.bodyType = bodyType;
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
        /// Sets body's rotation
        /// </summary>
        /// <param name="rotation">Body's rotation</param>
        /// <returns>The proper class</returns>
        internal BodyBuilder SetRotation(double rotation)
        {
            this.rotation = rotation;
            return this;
        }

        /// <summary>
        /// Creates a new body according to the specified properties
        /// </summary>
        /// <returns>The body created</returns>
        internal Body Build() => new Body(
            this.mass, this.restitution, this.intertia,
            this.isCollideable, this.doesPhysicsAffect,
            this.bodyType, this.shape, this.position, this.rotation);
    }
}