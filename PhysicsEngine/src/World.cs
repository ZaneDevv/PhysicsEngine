using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PhysicsEngine.Bodies;
using PhysicsEngine.Render;
using PhysicsEngine.Physics;
using System.Collections.Generic;

namespace PhysicsEngine
{
    internal class World : Game
    {
        private readonly Color BACKGROUND_COLOR = new Color(21, 21, 21);

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
                .SetShape(new Circle(30, 0, Color.Beige, this, 50))
                .SetMass(500)
                .SetPosition(new Vector2(700, 400))
                .Build());

            this.ShapeList.Add(new BodyBuilder()
                .SetBodyType(BodyType.Circle)
                .SetShape(new Circle(15, 0, Color.Red, this, 50))
                .SetPosition(new Vector2(200, 100))
                .Build());
        }


        protected override void Update(GameTime gameTime)
        {
            double deltaTime = gameTime.ElapsedGameTime.TotalSeconds;
            Physics.Physics.UpdatePhysics(ref this.ShapeList, deltaTime, this.graphics);

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
