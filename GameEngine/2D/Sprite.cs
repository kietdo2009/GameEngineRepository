using System;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
namespace CPI311.GameEngine
{
    public class Sprite
    {
        //Sprite Constructor **********
        public Sprite (Texture2D texture)
        {
            Texture = texture; 
            Position = new Vector2 (0, 0);
            Color = Color.White;
            //Source = new Rectangle (0,0, texture.Width, texture.Height);
            Rotation = 0;
            Origin = new Vector2(0, 0);
            Layer = 1;
            Scale = new Vector2 (1,1);
            Effect = SpriteEffects.None;
        }
        //****** Sprite Properties
        public Color Color {  get; set; }
        
        public Vector2 Position { get; set; }
        public Rectangle? Source { get; set; }
        public Single Rotation { get; set; }
        public Vector2 Scale { get; set; }
        public SpriteEffects Effect { get; set; }
        public Vector2 Origin { get; set; }
        public Single Layer {  get; set; }

        public Texture2D Texture { get; set; }
        /*
         * {
         * get{return Texture;}
         * set {Texture = value;}
         * }
         */
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, Source, Color, Rotation, Origin, Scale, Effect, Layer);
        }
    }
}
