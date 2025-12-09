using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalProject
{
    public abstract class GameScene
    {
        protected Game game;
        protected SpriteBatch spriteBatch;

        public GameScene(Game game)
        {
            this.game = game;
            this.spriteBatch = new SpriteBatch(game.GraphicsDevice);
        }

        public abstract void LoadContent();
        public abstract void Update();
        public abstract void Draw();
    }
}