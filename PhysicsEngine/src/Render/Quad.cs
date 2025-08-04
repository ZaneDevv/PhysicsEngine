using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

/*
    Class destinated to create quads in the screen with a certain position, rotation, size and color. 
*/

namespace PhysicsEngine.Render
{
    internal class Quad : RenderShape
    {
        #region ATTRIBUTES

        private Vector3[] vertices = new Vector3[4];
        private VertexPositionColor[] verticesColor = new VertexPositionColor[4];
        private short[] indices = new short[6];
        
        #endregion

        /// <summary>
        /// Createss a brand new quad from scratch
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <param name="width">Size in X</param>
        /// <param name="height">Size in Y</param>
        /// <param name="rotation">Angle of rotation</param>
        /// <param name="color">Quad's color</param>
        /// <param name="world">Quad's world</param>
        internal Quad(int width, int height, double rotation, Color color, World world)
        {
            this.color = color;
            this.world = world;

            this.position = Vector3.Zero;
            this.size = new Vector2(width / 2, height / 2);

            this.rotation = rotation;
            this.sin = Math.Sin(this.rotation);
            this.cos = Math.Cos(this.rotation);

            this.SetEffect();

            this.SetIndices();
            this.UpdateShape();
        }

        /// <summary>
        /// Copies an existent quad
        /// </summary>
        /// <param name="quad">Quad to copy</param>
        internal Quad(Quad quad)
        {
            this.color = quad.Color;
            this.world = quad.World;

            this.position = new Vector3(quad.Position.X, quad.Position.Y, 0);
            this.size = new Vector2(quad.Size.X, quad.Size.Y);

            this.rotation = quad.Rotation;
            this.sin = Math.Sin(this.rotation);
            this.cos = Math.Cos(this.rotation);

            this.SetEffect();

            this.SetIndices();
            this.UpdateShape();
        }

        /// <summary>
        /// Sets all the indices for the triangles making
        /// </summary>
        private void SetIndices()
        {
            this.indices[0] = 0;
            this.indices[1] = 1;
            this.indices[2] = 2;
            this.indices[3] = 2;
            this.indices[4] = 3;
            this.indices[5] = 0;
        }

        /// <summary>
        /// Updates the whole shape when the rotation, size or position changes
        /// </summary>
        protected override void UpdateShape()
        {
            UpdateVertices();
            UpdateVertexPositionColor();
        }

        /// <summary>
        /// Updates all the vertices of the quad when a property changes
        /// </summary>
        private void UpdateVertices()
        {
            this.vertices[0] = new Vector3(-this.size.X, -this.size.Y, 0);
            this.vertices[1] = new Vector3(this.size.X, -this.size.Y, 0);
            this.vertices[2] = new Vector3(this.size.X, this.size.Y, 0);
            this.vertices[3] = new Vector3(-this.size.X, this.size.Y, 0);

            for (short index = 0; index < 4; index++)
            {
                double newX = this.vertices[index].X * this.cos - this.vertices[index].Y * this.sin;
                double newY = this.vertices[index].X * this.sin + this.vertices[index].Y * this.cos;

                this.vertices[index] = new Vector3((int)newX, (int)newY, 0);
                this.vertices[index] += this.position;
            }
        }

        /// <summary>
        /// Updates all the vertex colors when either the vertices or the color change
        /// </summary>
        protected override void UpdateVertexPositionColor()
        {
            this.verticesColor[0] = new VertexPositionColor(this.vertices[0], this.color);
            this.verticesColor[1] = new VertexPositionColor(this.vertices[1], this.color);
            this.verticesColor[2] = new VertexPositionColor(this.vertices[2], this.color);
            this.verticesColor[3] = new VertexPositionColor(this.vertices[3], this.color);
        }

        /// <summary>
        /// Renders then quead in the screen
        /// </summary>
        internal override void Draw()
        {
            base.Draw();

            foreach (EffectPass pass in this.effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                this.world.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(
                    PrimitiveType.TriangleList,
                    this.verticesColor,
                    0,
                    4,
                    this.indices,
                    0,
                    2
                );
            }
        }


        #region GETTERS & SETTERS

        internal Vector3[] Vertices
        {
            get => this.vertices;
            private set => this.vertices = value;
        }

        #endregion
    }
}