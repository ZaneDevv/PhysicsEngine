using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

/*
    Class destinated to create circles in the screen with a certain position, rotation, size and color. 
*/

namespace PhysicsEngine.Render
{
    internal class Circle : RenderShape
    {
        #region ATTRIBUTES

        private Vector3[] Vertices;
        private VertexPositionColor[] VerticesColor;
        private short[] Indices;

        private int segments;

        #endregion

        /// <summary>
        /// Creates a brand new circle from scratch
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <param name="radius">Radius of the circle</param>
        /// <param name="rotation">Angle of rotation</param>
        /// <param name="color">Circle's color</param>
        /// <param name="world">Circle's world</param>
        /// <param name="segments">Number of segments (smoothness)</param>
        internal Circle(int x, int y, float radius, double rotation, Color color, World world, int segments = 36)
        {
            this.color = color;
            this.world = world;
            this.segments = Math.Max(3, segments);

            this.position = new Vector3(x, y, 0);
            this.size = new Vector2(radius, radius);

            this.rotation = rotation;
            this.sin = Math.Sin(this.rotation);
            this.cos = Math.Cos(this.rotation);

            this.VerticesColor = new VertexPositionColor[this.segments + 1];
            this.Vertices = new Vector3[this.segments + 1];


            this.SetIndices();
            this.SetEffect();
            this.UpdateShape();
        }

        /// <summary>
        /// Copies an existing circle
        /// </summary>
        /// <param name="circle">Circle to copy</param>
        internal Circle(Circle circle)
        {
            this.color = circle.Color;
            this.world = circle.World;

            this.position = new Vector3(circle.Position.X, circle.Position.Y, 0);
            this.size = new Vector2(circle.Size.X, circle.Size.Y);

            this.rotation = circle.Rotation;
            this.sin = Math.Sin(this.rotation);
            this.cos = Math.Cos(this.rotation);

            this.segments = circle.segments;

            this.VerticesColor = new VertexPositionColor[this.segments + 1];
            this.Vertices = new Vector3[this.segments + 1];

            this.SetIndices();
            this.SetEffect();
            this.UpdateShape();
        }

        /// <summary>
        /// Updates the shape
        /// </summary>
        protected override void UpdateShape()
        {
            UpdateVertices();
            UpdateVertexPositionColor();
        }

        /// <summary>
        /// Generates all vertices to form a circle using a triangle fan
        /// </summary>
        private void UpdateVertices()
        {
            Vertices[0] = this.position;

            double angleStep = MathHelper.Tau / segments;

            for (int index = 0; index < segments; index++)
            {
                double angle = index * angleStep;

                double x = Math.Cos(angle) * size.X;
                double y = Math.Sin(angle) * size.Y;

                double rotatedX = x * cos - y * sin;
                double rotatedY =  x * sin + y * cos;

                Vertices[index + 1] = new Vector3((int)rotatedX, (int)rotatedY, 0) + position;
            }
        }

        /// <summary>
        /// Sets up all the indices for the render of the shape
        /// </summary>
        private void SetIndices()
        {
            Indices = new short[this.segments * 3];

            for (int index = 0; index < this.segments; index++)
            {
                Indices[index * 3] = 0;
                Indices[index * 3 + 1] = (short)(index + 1);
                Indices[index * 3 + 2] = (short)((index + 2 > this.segments) ? 1 : index + 2);
            }
        }

        /// <summary>
        /// Updates color
        /// </summary>
        protected override void UpdateVertexPositionColor()
        {
            for (short index = 0; index < this.Vertices.Length; index++)
            {
                this.VerticesColor[index] = new VertexPositionColor(this.Vertices[index], this.color);
            }
        }

        /// <summary>
        /// Renders the circle to the screen
        /// </summary>
        internal override void Draw()
        {
            base.Draw();

            foreach (EffectPass pass in this.effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                this.world.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(
                    PrimitiveType.TriangleList,
                    this.VerticesColor,
                    0,
                    this.VerticesColor.Length,
                    this.Indices,
                    0,
                    segments
                );
            }
        }
    }
}
