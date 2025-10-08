using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lab1._1
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont _font;
        private Fraction _a = new Fraction(3, 4);
        private Fraction _b = new Fraction(1, 3);

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _font = Content.Load<SpriteFont>("Font"); //SpriteFont is in content
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            _spriteBatch.DrawString(_font, _a + " + " + _b + " = " + (_a + _b), new Vector2(50, 50), Color.Black);
            _spriteBatch.DrawString(_font, _a + " - " + _b + " = " + (_a - _b), new Vector2(50, 80), Color.Black);
            _spriteBatch.DrawString(_font, _a + " * " + _b + " = " + (_a * _b), new Vector2(50, 110), Color.Black);
            _spriteBatch.DrawString(_font, _a + " / " + _b + " = " + (_a / _b), new Vector2(50, 140), Color.Black);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}