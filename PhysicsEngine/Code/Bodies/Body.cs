using PhysicsEngine.Render;
using Microsoft.Xna.Framework;


namespace PhysicsEngine.Bodies
{
    internal class Body
    {
        private double mass;

        private bool isCollideable = true;
        private bool doesPhysicsAffect = true;

        private RenderShape shape;

        private Vector2 position;

        internal Body(
                double mass,
                bool isCollideable, bool doesPhysicsAffect,
                RenderShape shape, Vector2 position
            )
        {
            this.mass = mass;

            this.isCollideable = isCollideable;
            this.doesPhysicsAffect = doesPhysicsAffect;

            this.shape = shape;
            this.position = position;

            this.shape.Position = this.position;
        }

        internal bool IsCollideable
        {
            get => this.isCollideable;
            set => this.isCollideable = value; 
        }

        internal bool DoesPhysicsAffect
        {
            get => this.doesPhysicsAffect;
            set => this.doesPhysicsAffect = value;
        }

        internal double Mass
        {
            get => this.mass;
            private set => this.mass = value;
        }

        internal RenderShape Shape
        {
            get => this.shape;
            private set => this.shape = value;
        }
    }
}
