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

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private List<RenderShape> ShapeList = new List<RenderShape>();


        internal World()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            graphics.PreferredBackBufferHeight = 750;
            graphics.PreferredBackBufferWidth = 1280;
            graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            ShapeList.Add(new Quad(30, 30, 200, 200, 0, Color.White, this));
            ShapeList.Add(new Circle(30, 30, 100, 0, Color.Red, this, 50));
        }

        protected override void Update(GameTime gameTime)
        {

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(BACKGROUND_COLOR);

            spriteBatch.Begin();

            foreach (RenderShape shape in ShapeList)
            {
                shape.Draw();
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
