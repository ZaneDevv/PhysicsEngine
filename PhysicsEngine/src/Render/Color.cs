using Microsoft.Xna.Framework;
using System;

namespace PhysicsEngine.Render
{
    internal struct ColorUtils
    {
        internal static Color HSVToRGB(float h, float s, float v)
        {
            float c = v * s;
            float x = c * (1 - Math.Abs((h / 60f) % 2 - 1));
            float m = v - c;

            float r1 = 0, g1 = 0, b1 = 0;

            if (h < 60)
            {
                r1 = c; g1 = x; b1 = 0;
            }
            else if (h < 120)
            {
                r1 = x; g1 = c; b1 = 0;
            }
            else if (h < 180)
            {
                r1 = 0; g1 = c; b1 = x;
            }
            else if (h < 240)
            {
                r1 = 0; g1 = x; b1 = c;
            }
            else if (h < 300)
            {
                r1 = x; g1 = 0; b1 = c;
            }
            else
            {
                r1 = c; g1 = 0; b1 = x;
            }

            byte r = (byte)(MathHelper.Clamp((r1 + m) * 255, 0, 255));
            byte g = (byte)(MathHelper.Clamp((g1 + m) * 255, 0, 255));
            byte b = (byte)(MathHelper.Clamp((b1 + m) * 255, 0, 255));

            return new Color(r, g, b);
        }
    }
}
