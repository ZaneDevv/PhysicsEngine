using PhysicsEngine.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace PhysicsEngine.Bodies
{
    internal class Body
    {
        private double mass;

        private bool isCollideable = true;
        private bool doesPhysicsAffect = true;

        private RenderShape shape;
        private BodyType bodyType;

        private Vector2 velocity;
        private Vector2 position;

        private double rotation;

        internal Body(
                double mass,
                bool isCollideable, bool doesPhysicsAffect,
                BodyType bodyType, RenderShape shape, Vector2 position, double rotation
            )
        {
            this.mass = mass;

            this.isCollideable = isCollideable;
            this.doesPhysicsAffect = doesPhysicsAffect;

            this.shape = shape;
            this.bodyType = bodyType;
            this.position = position;

            this.rotation = rotation;

            this.shape.Position = this.position;
            this.shape.Rotation = this.rotation;

            this.velocity = Vector2.Zero;
        }



        #region GETTERS & SETTERS

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

        internal BodyType BodyType
        {
            get => this.bodyType;
            private set => this.bodyType = value;
        }

        internal RenderShape Shape
        {
            get => this.shape;
            private set => this.shape = value;
        }

        internal Vector2 Position
        {
            get => this.position;
            set
            {
                this.position = value;
                this.shape.Position = this.position;
            }
        }

        internal Vector2 Velocity
        {
            get => this.velocity;
            set => this.velocity = value;
        }

        internal double Rotation
        {
            get => this.rotation;
            set
            {
                this.rotation = value;
                this.shape.Rotation = this.rotation;
            }
        }

        #endregion
    }
}
