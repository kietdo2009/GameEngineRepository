using CPI311.GameEngine;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Input;

namespace Lab02
{
    public class SpiralMover
    {
        public Sprite Sprite { get; set; }
        public Vector2 Center { get; set; }
        public float Radius { get; set; }   // anchor radius (r)
        public float Speed { get; set; }    // rate of angle change
        public float Angle { get; set; }    // total time/angle

        public SpiralMover(Texture2D texture, Vector2 center, float radius = 100f, float speed = 1f)
        {
            Sprite = new Sprite(texture);
            Center = center;
            Radius = radius;
            Speed = speed/10;
            Angle = 0f;

            // set origin to middle so rotation looks natural
            Sprite.Origin = new Vector2(texture.Width / 2, texture.Height / 2);
        }

        public void Update()
        {
            // keyboard input
            if (InputManager.IsKeyDown(Keys.Right))
                Radius += 1f; // increase anchor radius
            if (InputManager.IsKeyDown(Keys.Left))
                Radius = Math.Max(0, Radius - 1f); // clamp at 0
            if (InputManager.IsKeyDown(Keys.Up))
                Speed += 0.001f; // faster
            if (InputManager.IsKeyDown(Keys.Down))
                Speed = Math.Max(0, Speed - 0.001f); // slower 

            // advance time/angle
            Angle += Speed * Time.ElapsedGameTime;

            // position from periodic spiral equations
            float cosAngle = (float)Math.Cos(Angle);
            float sinAngle = (float)Math.Sin(Angle);

            float x = (Radius + cosAngle) * cosAngle;
            float y = (Radius + cosAngle) * sinAngle;

            Sprite.Position = Center + new Vector2(x*2, y*2);
            Sprite.Position += new Vector2((float)Math.Cos(x)*100, (float)Math.Sin(x)*100);
           
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Sprite.Draw(spriteBatch);
        }
    }
}
