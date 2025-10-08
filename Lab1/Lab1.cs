using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;




namespace Lab1
{
    public class Lab1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;


        private SpriteFont font;
        private Fraction a = new Fraction(3, 4);
        private Fraction b = new Fraction(8, 3);

        public Lab1()
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
            font = Content.Load<SpriteFont>("Font.spritefont");
            // TODO: use this.Content to load your game content here
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
            //_spriteBatch.DrawString(font, Fraction.Multiply(a, b).ToString,
            //    new Vector2(50, 150), Color.Black);
            _spriteBatch.DrawString(font, a + " * " + b + " = " + (a * b),
            new Vector2(50, 50), Color.Black);

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
