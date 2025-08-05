using PhysicsEngine.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace PhysicsEngine.Bodies
{
    internal class Body
    {
        #region ATTRIBUTES

        private double mass;
        private double restitution;
        private double inertia;

        private bool isCollideable = true;
        private bool doesPhysicsAffect = true;

        private RenderShape shape;
        private BodyType bodyType;

        private Vector2 force;
        private Vector2 linearVelocity;
        private Vector2 position;

        private double angularVelocity;
        private double rotationalIntertia;
        private double rotation;

        #endregion


        internal Body(
                double mass, double restitution, double inertia,
                bool isCollideable, bool doesPhysicsAffect,
                BodyType bodyType, RenderShape shape, Vector2 position, double rotation
            )
        {
            this.mass = mass;
            this.restitution = restitution;
            this.inertia = inertia;

            this.isCollideable = isCollideable;
            this.doesPhysicsAffect = doesPhysicsAffect;

            this.shape = shape;
            this.bodyType = bodyType;
            this.position = position;

            this.rotation = rotation;

            this.shape.Position = this.position;
            this.shape.Rotation = this.rotation;

            this.force = Vector2.Zero;
            this.linearVelocity = Vector2.Zero;

            this.angularVelocity = 0;
            this.rotationalIntertia = this.GetRotationalInertia();
        }

        /// <summary>
        /// Updates the body's physics
        /// </summary>
        /// <param name="deltaTime">Time passed by since the last frame</param>
        internal void Update(double deltaTime)
        {
            this.linearVelocity += this.force * (float)(deltaTime / this.mass);

            this.Rotation += this.angularVelocity * (float)deltaTime;
            this.Position += this.linearVelocity * (float)deltaTime;

            this.force = Vector2.Zero;
        }

        /// <summary>
        /// Applies a force to the body
        /// </summary>
        /// <param name="force">Force applied</param>
        internal void ApplyForce(Vector2 force) => this.force += force;

        /// <summary>
        /// Calculates the rotational intertia according to the size and mass of the body
        /// </summary>
        /// <returns>Rotational inertia of the body</returns>
        private double GetRotationalInertia()
        {
            if (this.bodyType == BodyType.Circle)
            {
                Circle circle = (Circle)this.shape;
                return this.mass * circle.Radius * circle.Radius / 2;
            }

            Quad quad = (Quad)this.shape;
            return this.mass * (quad.Size.X * quad.Size.X + quad.Size.Y * quad.Size.Y) / 2;
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

        internal double Restitution
        {
            get => this.restitution;
            set => this.restitution = value;
        }

        internal double Inertia
        {
            get => this.inertia;
            set => this.inertia = value;
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

        internal Vector2 LinearVelocity
        {
            get => this.linearVelocity;
            set => this.linearVelocity = value;
        }

        internal double AngularVelocity
        {
            get => this.angularVelocity;
            set => this.angularVelocity = value;
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
