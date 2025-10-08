using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CPI311.GameEngine // Or whatever your game engine namespace is
{
    public class ProgressBar : Sprite
    {
        public Color FillColor { get; set; }

        private float _value;
        public float Value
        {
            get { return _value; }
            set { _value = MathHelper.Clamp(value, 0, 1); }
        }

        // CORRECTED CONSTRUCTOR: Default value is no longer hard-coded to 1.
        public ProgressBar(Texture2D texture) : base(texture)
        {
            FillColor = Color.Green;
            // The default value for a float is 0, which is what we want.
        }

       
        public override void Draw(SpriteBatch spriteBatch)
        {
            // Get the full outer dimensions of the bar
            int width = (int)(Texture.Width * Scale.X);
            int height = (int)(Texture.Height * Scale.Y);

            // 1. Draw the background rectangle using the Sprite's Color property
            Rectangle backgroundRect = new Rectangle((int)Position.X, (int)Position.Y, width, height);
            spriteBatch.Draw(Texture, backgroundRect, Color);

            // 2. Define padding to create the border
            int padding = 2;

            // 3. Calculate the inner foreground rectangle's width based on the Value
            int fillWidth = (int)((width - padding * 2) * Value);
            if (fillWidth > 0)
            {
                Rectangle foregroundRect = new Rectangle(
                    (int)Position.X + padding,
                    (int)Position.Y + padding,
                    fillWidth,
                    height - padding * 2);

                // 4. Draw the foreground rectangle using the FillColor property
                spriteBatch.Draw(Texture, foregroundRect, FillColor);
            }
        }
    }
}