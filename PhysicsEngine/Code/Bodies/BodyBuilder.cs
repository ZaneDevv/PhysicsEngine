using PhysicsEngine.Render;
using Microsoft.Xna.Framework;

namespace PhysicsEngine.Bodies
{
    internal sealed class BodyBuilder
    {
        private double mass = 1;

        private bool isCollideable = true;
        private bool doesPhysicsAffect = true;

        private RenderShape shape;

        private Vector2 position = Vector2.Zero;

        internal BodyBuilder SetMass(double mass)
        {
            this.mass = mass;
            return this;
        }

        internal BodyBuilder SetCollisions(bool isCollideable)
        {
            this.isCollideable = isCollideable;
            return this;
        }

        internal BodyBuilder SetPhysics(bool doesPhysicsAffect)
        {
            this.doesPhysicsAffect = doesPhysicsAffect;
            return this;
        }

        internal BodyBuilder SetShape(RenderShape shape)
        {
            this.shape = shape;
            return this;
        }

        internal BodyBuilder SetPosition(Vector2 position)
        {
            this.position = position;
            return this;
        }

        internal Body Build() => new Body(
            this.mass,
            this.isCollideable, this.doesPhysicsAffect,
            this.shape, this.position);
    }
}