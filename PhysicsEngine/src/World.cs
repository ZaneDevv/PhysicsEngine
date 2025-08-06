using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhysicsEngine.Bodies;
using PhysicsEngine.Render;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using System;

namespace PhysicsEngine
{
    internal class World : Game
    {
        private readonly Color BACKGROUND_COLOR = new Color(21, 21, 21);

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private List<Body> ShapeList = new List<Body>();

        private bool wasLeftClickPressedLastFrame = false;
        private bool wasRightClickPressedLastFrame = false;
        private bool wasKeyBPressedLastFrame = false;

        private Random random = new Random();


        /// <summary>
        /// Creates a brand new game
        /// </summary>
        internal World()
        {
            this.graphics = new GraphicsDeviceManager(this);
            this.Content.RootDirectory = "Content";
            this.IsMouseVisible = true;
        }

        /// <summary>
        /// Initializes the simulation
        /// </summary>
        protected override void Initialize()
        {
            this.graphics.PreferredBackBufferWidth = 1280;
            this.graphics.PreferredBackBufferHeight = 750;
            this.graphics.ApplyChanges();

            base.Initialize();
        }

        /// <summary>
        /// Loads all bodies data
        /// </summary>
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
        }

        /// <summary>
        /// Updates the whole game
        /// </summary>
        /// <param name="gameTime">Time data</param>
        protected override void Update(GameTime gameTime)
        {
            double deltaTime = gameTime.ElapsedGameTime.TotalSeconds;
            Physics.Physics.UpdatePhysics(ref this.ShapeList, deltaTime, this.graphics);

            Point mouse = Mouse.GetState().Position;

            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                wasLeftClickPressedLastFrame = true;
            }
            else if (wasLeftClickPressedLastFrame)
            {
                wasLeftClickPressedLastFrame = false;

                int sizeX = (int)(random.NextDouble() * 10 + 40);
                int sizeY = (int)(random.NextDouble() * 10 + 40);

                this.ShapeList.Add(new BodyBuilder()
                .SetBodyType(BodyType.Quad)
                .SetShape(new Quad(sizeX, sizeY, 0, this.GetRandomColor(), this))
                .SetPosition(new Vector2(mouse.X, mouse.Y))
                .SetMass((sizeX + sizeY) / 2)
                .SetRestitution(0.3)
                .Build());
            }

            if (Mouse.GetState().RightButton == ButtonState.Pressed)
            {
                wasRightClickPressedLastFrame = true;
            }
            else if (wasRightClickPressedLastFrame)
            {
                wasRightClickPressedLastFrame = false;

                float radius = (float)random.NextDouble() * 10 + 40;

                this.ShapeList.Add(new BodyBuilder()
                .SetBodyType(BodyType.Circle)
                .SetShape(new Circle(radius, 0, this.GetRandomColor(), this, 50))
                .SetPosition(new Vector2(mouse.X, mouse.Y))
                .SetMass(radius / 2)
                .Build());
            }

            if (Keyboard.GetState().IsKeyDown(Keys.B))
            {
                wasKeyBPressedLastFrame = true;
            }
            else if (wasKeyBPressedLastFrame)
            {
                wasKeyBPressedLastFrame = false;

                foreach (Body body in this.ShapeList)
                {
                    Vector2 direction = body.Position - mouse.ToVector2();
                    body.LinearVelocity += Vector2.Normalize(direction) * (float)(1e+3 * body.Mass / direction.Length());
                }
            }

            base.Update(gameTime);
        }


        /// <summary>
        /// Renders all the bodies in the screen
        /// </summary>
        /// <param name="gameTime">Time data</param>
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

        /// <summary>
        /// Computes a random color
        /// </summary>
        /// <returns>Obtained color</returns>
        private Color GetRandomColor() => ColorUtils.HSVToRGB((float)random.NextDouble() * 360, 0.75f, 0.75f);
    }
}
