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

        private List<Quad> quadList = new List<Quad>();


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
            this.spriteBatch = new SpriteBatch(this.GraphicsDevice);

            quadList.Add(new Quad(30, 30, 200, 200, 0, Color.White, this));
        }

        protected override void Update(GameTime gameTime)
        {
            MouseState mouse = Mouse.GetState();
            Quad quad = this.quadList.First();

            quad.Rotation += MathHelper.Pi / 360;
            quad.Position = new Vector2(mouse.X, mouse.Y);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            this.GraphicsDevice.Clear(BACKGROUND_COLOR);

            this.spriteBatch.Begin();

            foreach (Quad quad in quadList)
            {
                quad.Draw();
            }

            this.spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
