using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PhysicsEngine.Bodies;
using PhysicsEngine.Render;
using System.Collections.Generic;

namespace PhysicsEngine
{
    internal class World : Game
    {
        private readonly Color BACKGROUND_COLOR = new Color(21, 21, 21);

        private readonly Vector2 GRAVITY = new Vector2(0, 100);

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private List<Body> ShapeList = new List<Body>();


        internal World()
        {
            this.graphics = new GraphicsDeviceManager(this);
            this.Content.RootDirectory = "Content";
            this.IsMouseVisible = true;
        }


        protected override void Initialize()
        {
            this.graphics.PreferredBackBufferWidth = 1280;
            this.graphics.PreferredBackBufferHeight = 750;
            this.graphics.ApplyChanges();

            base.Initialize();
        }


        protected override void LoadContent()
        {
            this.spriteBatch = new SpriteBatch(GraphicsDevice);

            this.ShapeList.Add(new BodyBuilder()
                .SetBodyType(BodyType.Quad)
                .SetShape(new Quad(1000, 40, 0, Color.DarkGreen, this))
                .SetPosition(new Vector2(600, 680))
                .SetPhysics(false)
                .Build());

            this.ShapeList.Add(new BodyBuilder()
                .SetBodyType(BodyType.Quad)
                .SetShape(new Quad(400, 20, 0, Color.DarkGreen, this))
                .SetPosition(new Vector2(200, 300))
                .SetRotation(MathHelper.Pi / 6)
                .SetPhysics(false)
                .Build());

            this.ShapeList.Add(new BodyBuilder()
                .SetBodyType(BodyType.Quad)
                .SetShape(new Quad(20, 20, 0, Color.White, this))
                .SetPosition(new Vector2(200, 200))
                .Build());

            this.ShapeList.Add(new BodyBuilder()
                .SetBodyType(BodyType.Circle)
                .SetShape(new Circle(20, 0, Color.Red, this, 50))
                .SetPosition(new Vector2(500, 400))
                .Build());
        }


        protected override void Update(GameTime gameTime)
        {
            double deltaTime = gameTime.ElapsedGameTime.TotalSeconds;

            List<Body> BodiesToRemove = new List<Body>();

            foreach (Body body in this.ShapeList)
            {
                bool isTooDown = body.Position.Y > this.graphics.PreferredBackBufferHeight + body.Shape.Size.Y;
                bool isTooRight = body.Position.X > this.graphics.PreferredBackBufferWidth + body.Shape.Size.X;
                bool isTooLeft = body.Position.X < -body.Shape.Size.X;
                bool willBeRemoved = isTooDown || isTooLeft || isTooRight;

                if (willBeRemoved)
                {
                    BodiesToRemove.Add(body);
                    continue;
                }

                if (!body.DoesPhysicsAffect) continue;

                body.Velocity += GRAVITY * (float)deltaTime;
                body.Position += body.Velocity * (float)deltaTime;
            }

            foreach (Body body in BodiesToRemove)
            {
                this.ShapeList.Remove(body);
            }

            for (int i = 0; i < this.ShapeList.Count; i++)
            {
                Body body1 = this.ShapeList[i];
                if (!body1.IsCollideable) continue;

                for (int j = 0; j < this.ShapeList.Count; j++)
                {
                    Body body2 = this.ShapeList[j];
                    if (!body2.IsCollideable) continue;

                    Vector3 normal = Vector3.Zero;
                    double depth = 0;
                    bool areColliding = false;

                    bool areBothPolygons = body1.BodyType == BodyType.Quad && body2.BodyType == BodyType.Quad;

                    if (areBothPolygons)
                    {
                        Quad polygon1 = (Quad)body1.Shape;
                        Quad polygon2 = (Quad)body2.Shape;

                        areColliding = Collisions.Collision.Polygon_VS_Polygon(polygon1.Vertices, polygon2.Vertices, out normal, out depth);
                    }

                    if (!areColliding) continue;

                    Vector2 normal2D = new Vector2(normal.X, normal.Y);

                    if (body1.DoesPhysicsAffect)
                    {
                        body1.Position += normal2D * (float)depth / (body2.DoesPhysicsAffect ? 2 : 1);
                    }
                    if (body2.DoesPhysicsAffect)
                    {
                        body2.Position -= normal2D * (float)depth / (body1.DoesPhysicsAffect ? 2 : 1);
                    }
                }
            }

            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            this.GraphicsDevice.Clear(BACKGROUND_COLOR);

            this.spriteBatch.Begin();

            foreach (Body body in this.ShapeList)
            {
                body.Shape.Draw();
            }

            this.spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
