using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

/*
    Class destinated to create queds in the screen with a certain position, rotation, size and color. 
*/

namespace PhysicsEngine.Render {
    internal class Quad
    {
        private Texture2D texture;
        private bool textureInitialized = false;

        private Vector3[] Vertices = new Vector3[4];
        private VertexPositionColor[] VerticesColor = new VertexPositionColor[4];
        private short[] Indices = new short[6];

        private BasicEffect effect;
        private Color color;

        private World world;

        private Vector3 position;
        private Vector2 size;

        private double rotation;
        private double sin;
        private double cos;

        /// <summary>
        /// Create a brand new quad from scratch
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <param name="width">Size in X</param>
        /// <param name="height">Size in Y</param>
        /// <param name="rotation">Angle of rotation</param>
        /// <param name="color">Quad's color</param>
        /// <param name="world">Quad's world</param>
        internal Quad(int x, int y, int width, int height, double rotation, Color color, World world)
        {
            this.color = color;
            this.world = world;

            this.effect = new BasicEffect(this.world.GraphicsDevice);
            this.effect.FogEnabled = false;
            this.effect.LightingEnabled = false;
            this.effect.TextureEnabled = false;
            this.effect.VertexColorEnabled = true;
            this.effect.PreferPerPixelLighting = false;

            this.position = new Vector3(x, y, 0);
            this.size = new Vector2(width / 2, height / 2);

            this.rotation = rotation;
            this.sin = Math.Sin(this.rotation);
            this.cos = Math.Cos(this.rotation);

            this.UpdateShape();
            this.SetIndices();
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

            this.effect = new BasicEffect(this.world.GraphicsDevice);
            this.effect.FogEnabled = false;
            this.effect.LightingEnabled = false;
            this.effect.TextureEnabled = false;
            this.effect.VertexColorEnabled = true;
            this.effect.PreferPerPixelLighting = false;

            this.UpdateShape();
            this.SetIndices();
        }

        /// <summary>
        /// Sets all the indices for the triangles making
        /// </summary>
        private void SetIndices()
        {
            this.Indices[0] = 0;
            this.Indices[1] = 1;
            this.Indices[2] = 2;
            this.Indices[3] = 2;
            this.Indices[4] = 3;
            this.Indices[5] = 0;
        }

        /// <summary>
        /// Updates the whole shape when the rotation, size or position changes
        /// </summary>
        private void UpdateShape()
        {
            UpdateVertices();
            UpdateVertexPositionColor();
        }

        /// <summary>
        /// Updates all the vertices of the quad when a property changes
        /// </summary>
        private void UpdateVertices()
        {
            this.Vertices[0] = new Vector3(-this.size.X, -this.size.Y, 0);
            this.Vertices[1] = new Vector3(this.size.X, -this.size.Y, 0);
            this.Vertices[2] = new Vector3(this.size.X, this.size.Y, 0);
            this.Vertices[3] = new Vector3(-this.size.X, this.size.Y, 0);

            for (short index = 0; index < 4; index++)
            {
                double newX = this.Vertices[index].X * this.cos - this.Vertices[index].Y * this.sin;
                double newY = this.Vertices[index].X * this.sin + this.Vertices[index].Y * this.cos;

                this.Vertices[index] = new Vector3((int)newX, (int)newY, 0);
                this.Vertices[index] += this.position;
            }
        }

        /// <summary>
        /// Updates all the vertex colors when either the vertices or the color change
        /// </summary>
        private void UpdateVertexPositionColor()
        {
            this.VerticesColor[0] = new VertexPositionColor(this.Vertices[0], this.color);
            this.VerticesColor[1] = new VertexPositionColor(this.Vertices[1], this.color);
            this.VerticesColor[2] = new VertexPositionColor(this.Vertices[2], this.color);
            this.VerticesColor[3] = new VertexPositionColor(this.Vertices[3], this.color);
        }

        /// <summary>
        /// Renders then quead in the screen
        /// </summary>
        internal void Draw()
        {
            Viewport viewport = this.world.GraphicsDevice.Viewport;
            this.effect.Projection = Matrix.CreateOrthographicOffCenter(0, viewport.Width, viewport.Height, 0, 0, 1);
            this.effect.View = Matrix.Identity;
            this.effect.World = Matrix.Identity;

            foreach (EffectPass pass in this.effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                this.world.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(
                    PrimitiveType.TriangleList,
                    this.VerticesColor,
                    0,
                    4,
                    this.Indices,
                    0,
                    2
                );
            }
        }


        internal Color Color { 
            get => this.color;
            set {
                this.color = value;
                UpdateVertexPositionColor();
            }
        }
        internal World World { 
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

                UpdateShape();
            }
        }
    }
}