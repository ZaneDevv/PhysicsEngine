using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PhysicsEngine.Render
{
    internal abstract class RenderShape
    {
        protected Vector3 position;
        protected Vector2 size;

        protected BasicEffect effect;
        protected Color color;

        protected World world;

        protected abstract void UpdateShape();

        protected void SetEffect()
        {
            this.effect = new BasicEffect(this.world.GraphicsDevice);
            this.effect.FogEnabled = false;
            this.effect.LightingEnabled = false;
            this.effect.TextureEnabled = false;
            this.effect.VertexColorEnabled = true;
            this.effect.PreferPerPixelLighting = false;
        }

        internal virtual void Draw()
        {
            Viewport viewport = this.world.GraphicsDevice.Viewport;

            this.effect.Projection = Matrix.CreateOrthographicOffCenter(0, viewport.Width, viewport.Height, 0, 0, 1);
            this.effect.View = Matrix.Identity;
            this.effect.World = Matrix.Identity;
        }
    }
}
