using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CPI311.GameEngine;

namespace Lab02
{
    public class Lab02 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // Sprite sprite; // Commented out for spiral implementation
        public SpiralMover spiralMover; // Using SpiralMover instead of simple Sprite
                                        // KeyboardState prevState; // Commented out - using InputManager now

        public Lab02()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            //****************************
            InputManager.Initialize();
            Time.Initialize();
            // ***************************
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            // sprite = new Sprite(Content.Load<Texture2D>("Square")); // Commented out for spiral
            // prevState = Keyboard.GetState(); // Commented out - using InputManager

            // Create spiral mover at center of screen
            Vector2 center = new Vector2(
                GraphicsDevice.Viewport.Width / 2,
                GraphicsDevice.Viewport.Height / 2);

            spiralMover = new SpiralMover(Content.Load<Texture2D>("Square"), center);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            //*****************************
            InputManager.Update();
            Time.Update(gameTime);
            //****************************

            // Simple sprite movement code (commented out for spiral implementation)
            /*
            sprite.Rotation += Time.ElapsedGameTime;

            if (InputManager.IsKeyDown(Keys.Right))
            {
                sprite.Position += Vector2.UnitX * 5;
            }
            if (InputManager.IsKeyDown(Keys.Left))
            {
                sprite.Position += Vector2.UnitX * -5;
            }
            if (InputManager.IsKeyDown(Keys.Up))
            {
                sprite.Position += Vector2.UnitY * -5;
            }
            if (InputManager.IsKeyDown(Keys.Down))
            {
                sprite.Position += Vector2.UnitY * 5;
            }
            if (InputManager.IsKeyDown(Keys.Space))
            {
                sprite.Rotation += .05f;
            }
            */

            /********* LAB 2 Simple Input Handling (commented out)
            KeyboardState currentState = Keyboard.GetState();
            if (currentState.IsKeyDown(Keys.Left) &&
            prevState.IsKeyUp(Keys.Left))
                sprite.Position += Vector2.UnitX * -5;
            if (currentState.IsKeyDown(Keys.Right) &&
            prevState.IsKeyUp(Keys.Right))
                sprite.Position += Vector2.UnitX * 5;
            if (currentState.IsKeyDown(Keys.Up) &&
            prevState.IsKeyUp(Keys.Up))
                sprite.Position += Vector2.UnitY * -5;
            if (currentState.IsKeyDown(Keys.Down) &&
            prevState.IsKeyUp(Keys.Down))
                sprite.Position += Vector2.UnitY * 5;
            if (currentState.IsKeyDown(Keys.Space))
                sprite.Rotation += 0.05f;
            
            prevState = currentState;
            */

            // Update the spiral mover instead of simple sprite movement
            spiralMover.Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();
            // sprite.Draw(_spriteBatch); // Commented out for spiral
            spiralMover.Draw(_spriteBatch); // Using SpiralMover instead
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}