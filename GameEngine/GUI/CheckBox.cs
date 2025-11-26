using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;



namespace CPI311.GameEngine
{
    public class CheckBox : GUIElement
    {
        public bool Checked { get; set; }
        Texture2D Box { get; set; }
        public CheckBox()
        {
            Checked = false;
        }
        public override void Update()
        {
            if (InputManager.IsMouseDown(0))
            {
                Vector2 mousePos = InputManager.GetMousePosition();
                if (Bounds.Contains((int)mousePos.X, (int)mousePos.Y))
                {
                    Checked = !Checked;
                    OnAction();
                }
            }
            base.Update();
        }
        public override void Draw(SpriteBatch spriteBatch, SpriteFont font)
        {
            base.Draw(spriteBatch, font);
            int width = Math.Min(Bounds.Width, Bounds.Height);
            spriteBatch.Draw(Texture, new Rectangle(Bounds.X, Bounds.Y, width, width),
            Checked ? Color.Red : Color.White);
            spriteBatch.DrawString(font, Text, new Vector2(Bounds.X + width,
            Bounds.Y), Color.Black);
            //base.Draw(spriteBatch, font);
            //int width = Math.Min(Bounds.Width, Bounds.Height);
            //spriteBatch.Draw(Box, new Rectangle(Bounds.X, Bounds.Y, width, width),
            //Checked ? Color.Red : Color.White);
            //spriteBatch.DrawString(font, Text, new Vector2(Bounds.X + width,
            //Bounds.Y), Color.Black);
        }
    }
}
