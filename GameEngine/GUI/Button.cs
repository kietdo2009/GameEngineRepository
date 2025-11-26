using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
namespace CPI311.GameEngine
{
    public class Button : GUIElement
    {
        public override void Update()
        {
            if (InputManager.IsMouseReleased(0) &&
            Bounds.Contains(InputManager.GetMousePosition()))
                OnAction();
        }
        public override void Draw(SpriteBatch spriteBatch, SpriteFont font)
        {
            base.Draw(spriteBatch, font);
            spriteBatch.DrawString(font, Text,
            new Vector2(Bounds.X, Bounds.Y), Color.Black);
        }
    }
}
