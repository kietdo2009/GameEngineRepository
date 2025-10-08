using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CPI311.GameEngine 
{
    public class AnimatedSprite : Sprite
    {
        public int TotalFrames { get; set; }
        public float CurrentFrame { get; set; }
        public float Speed { get; set; }

        public int FrameWidth { get; set; }
        public int FrameHeight { get; set; }
        public int CurrentAnimation { get; set; }
        public AnimatedSprite(Texture2D texture, int totalFrames = 1, int totalAnimations = 1)
    : base(texture)
        {
            TotalFrames = totalFrames;
            CurrentFrame = 0;
            Speed = 10f;
            CurrentAnimation = 0; // Default to the first row

            // Calculate frame dimensions using the new 'totalAnimations' parameter
            FrameWidth = texture.Width / totalFrames;
            FrameHeight = texture.Height / totalAnimations;

            // Set the initial source rectangle to the first frame of the first row
            Source = new Rectangle(0, 0, FrameWidth, FrameHeight);
        }

        public virtual void Update()
        {
            CurrentFrame += Speed * Time.ElapsedGameTime;

            if (CurrentFrame >= TotalFrames)
                CurrentFrame = 0;

            int frame = (int)CurrentFrame;

            // This is the corrected part.
            // Instead of using '0' for the Y value, we use the row
            // that was selected in the constructor.
            Source = new Rectangle(
                frame * FrameWidth,
                CurrentAnimation * FrameHeight, // Use the stored row
                FrameWidth,
                FrameHeight);
        }
    }
}