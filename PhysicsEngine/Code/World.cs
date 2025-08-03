using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PhysicsEngine.Bodies;
using PhysicsEngine.Render;
using System.Collections.Generic;
using System.Linq;

namespace PhysicsEngine
{
    internal class World : Game
    {
        private readonly Color BACKGROUND_COLOR = new Color(21, 21, 21);

        private readonly Vector2 GRAVITY = new Vector2(0, 30);

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
            this.graphics.PreferredBackBufferHeight = 750;
            this.graphics.PreferredBackBufferWidth = 1280;
            this.graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            this.spriteBatch = new SpriteBatch(GraphicsDevice);

            Body bodyQuad = new BodyBuilder()
                .SetShape(new Quad(20, 20, 0, Color.White, this))
                .SetPosition(new Vector2(200, 200))
                .Build();

            Body bodyCircle = new BodyBuilder()
                .SetShape(new Circle(20, 0, Color.Red, this, 50))
                .SetPosition(new Vector2(500, 400))
                .Build();

            this.ShapeList.Add(bodyQuad);
            this.ShapeList.Add(bodyCircle);
        }

        protected override void Update(GameTime gameTime)
        {
            double deltaTime = gameTime.ElapsedGameTime.TotalSeconds;

            foreach (Body body in this.ShapeList)
            {
                if (!body.DoesPhysicsAffect) continue;

                body.Velocity += GRAVITY * (float)deltaTime;
                body.Position += body.Velocity * (float)deltaTime;
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
