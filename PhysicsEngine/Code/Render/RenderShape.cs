using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace PhysicsEngine.Render
{
    internal abstract class RenderShape
    {
        #region ATTRIBUTES

        protected Vector3 position;
        protected Vector2 size;

        protected BasicEffect effect;
        protected Color color;

        protected World world;

        protected double rotation;
        protected double sin;
        protected double cos;

        #endregion


        /// <summary>
        /// Updates the whole shape as the vertices to apply size, rotation and position
        /// </summary>
        protected abstract void UpdateShape();

        /// <summary>
        /// Updates 
        /// </summary>
        protected abstract void UpdateVertexPositionColor();


        /// <summary>
        /// Updates all the vertex colors when either the vertices or the color change
        /// </summary>
        protected void SetEffect()
        {
            this.effect = new BasicEffect(this.world.GraphicsDevice);
            this.effect.FogEnabled = false;
            this.effect.LightingEnabled = false;
            this.effect.TextureEnabled = false;
            this.effect.VertexColorEnabled = true;
            this.effect.PreferPerPixelLighting = false;
        }

        /// <summary>
        /// Renders then shape in the screen
        /// </summary>
        internal virtual void Draw()
        {
            Viewport viewport = this.world.GraphicsDevice.Viewport;

            this.effect.Projection = Matrix.CreateOrthographicOffCenter(0, viewport.Width, viewport.Height, 0, 0, 1);
            this.effect.View = Matrix.Identity;
            this.effect.World = Matrix.Identity;
        }


        #region SETTERS & GETTERS

        internal Color Color
        {
            get => this.color;
            set
            {
                this.color = value;
                this.UpdateVertexPositionColor();
            }
        }
        internal World World
        {
            get => this.world;
            private set { this.world = value; }
        }

        internal Vector2 Position
        {
            get => new Vector2(this.position.X, this.position.Y);
            set
            {
                Vector2 givenVector = value;
                this.position = new Vector3(givenVector.X, givenVector.Y, 0);

                this.UpdateShape();
            }
        }
        internal Vector2 Size
        {
            get => this.size;
            set
            {
                this.size = value;
                this.UpdateShape();
            }
        }

        internal double Rotation
        {
            get => this.rotation;
            set
            {
                this.rotation = value;

                this.sin = Math.Sin(this.rotation);
                this.cos = Math.Cos(this.rotation);

                this.UpdateShape();
            }
        }

        #endregion
    }
}
