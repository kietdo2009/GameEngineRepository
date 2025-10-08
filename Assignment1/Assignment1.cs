using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CPI311.GameEngine; // Make sure to use your game engine namespace
using System; // Needed for the Random class

namespace Assignment1
{
    public class Assignment1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // ****** Game Objects ******
        AnimatedSprite player;
        Sprite timeBomb;
        ProgressBar timeBar;
        ProgressBar distanceBar;
        SpriteFont font;
        // ****** Game Variables ******
        Random random;
        float totalTime = 60f; // Total time in seconds
        float currentTime;
        float distanceWalked = 0f;
        const float maxDistance = 5000f; // The total distance the player needs to walk

        public Assignment1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // Initialize your game engine managers
            InputManager.Initialize();
            Time.Initialize();

            random = new Random();
            currentTime = totalTime;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("Font");

            var playerTexture = Content.Load<Texture2D>("explorer");
            var squareTexture = Content.Load<Texture2D>("Square (1)");


            player = new AnimatedSprite(playerTexture, 8, 5);
            player.Position = new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2);

            // Set the origin to the center of a SINGLE frame for proper rotation
            player.Origin = new Vector2(player.FrameWidth / 2, player.FrameHeight / 2);

            timeBomb = new Sprite(squareTexture);
            timeBomb.Position = new Vector2(random.Next(0, _graphics.PreferredBackBufferWidth), random.Next(0, _graphics.PreferredBackBufferHeight));
            timeBomb.Color = Color.Black;

            timeBar = new ProgressBar(squareTexture);
            timeBar.Position = new Vector2(150, 20);
            timeBar.Color = Color.White;
            timeBar.FillColor = Color.Red;
            timeBar.Value = 1f;
            timeBar.Scale = new Vector2(2f, 1f);

            distanceBar = new ProgressBar(squareTexture);
            distanceBar.Position = new Vector2(480, 20);
            distanceBar.Color = Color.White;
            distanceBar.FillColor = Color.Green;
            distanceBar.Scale = new Vector2(2f, 1f);
        }
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Update your game engine managers at the start of the frame
            InputManager.Update();
            Time.Update(gameTime);

            // --- Player Movement & Animation Logic ---
            float rotationSpeed = 2.5f;
            float moveSpeed = 100f;
            bool isMoving = false;

            // Set a default animation (e.g., row 0 for facing down)
            player.CurrentAnimation = 0;

            // Rotate left (animation on row 1)
            if (InputManager.IsKeyDown(Keys.Left))
            {
                player.Rotation -= rotationSpeed * Time.ElapsedGameTime;
                player.CurrentAnimation = 1;
            }
            // Rotate right (animation on row 2)
            if (InputManager.IsKeyDown(Keys.Right))
            {
                player.Rotation += rotationSpeed * Time.ElapsedGameTime;
                player.CurrentAnimation = 2;
            }

            // Move forward (animation for "walk up" is on row 3)
            if (InputManager.IsKeyDown(Keys.Up))
            {
                isMoving = true;
                player.CurrentAnimation = 4;
                // Calculate direction based on rotation. The PiOver2 adjusts for the sprite's orientation.
                Vector2 direction = new Vector2((float)Math.Cos(player.Rotation - MathHelper.PiOver2), (float)Math.Sin(player.Rotation - MathHelper.PiOver2));
                player.Position += direction * moveSpeed * Time.ElapsedGameTime;
                distanceWalked += moveSpeed * Time.ElapsedGameTime;
            }

            // Move backward (animation for "walk down" is on row 0)
            if (InputManager.IsKeyDown(Keys.Down))
            {
                isMoving = true;
                player.CurrentAnimation = 0;
                Vector2 direction = new Vector2((float)Math.Cos(player.Rotation - MathHelper.PiOver2), (float)Math.Sin(player.Rotation - MathHelper.PiOver2));
                player.Position -= direction * moveSpeed * Time.ElapsedGameTime;
                // Optionally, you can decide if moving backward affects the distance walked
                // distanceWalked -= moveSpeed * Time.ElapsedGameTime; 
            }
            if (InputManager.IsKeyDown(Keys.A))
            {
                isMoving = true;
                player.CurrentAnimation = 2;
                Vector2 right = new Vector2((float)Math.Cos(player.Rotation), (float)Math.Sin(player.Rotation));
                player.Position -= right * moveSpeed * Time.ElapsedGameTime;
                distanceWalked += moveSpeed * Time.ElapsedGameTime;
            }
            if (InputManager.IsKeyDown(Keys.D))
            {
                isMoving = true;
                player.CurrentAnimation = 3;
                Vector2 right = new Vector2((float)Math.Cos(player.Rotation), (float)Math.Sin(player.Rotation));
                player.Position += right * moveSpeed * Time.ElapsedGameTime;
                distanceWalked += moveSpeed * Time.ElapsedGameTime;
            }

            // Only update the animation frames if the player is moving
            if (isMoving)
            {
                player.Update();
            }

            // --- Game State & Logic ---

            // Make the time bar countdown
            currentTime -= Time.ElapsedGameTime;
            if (currentTime < 0) currentTime = 0; // Prevent time from going below zero

            // Check for collision with the time bomb
            if (Vector2.Distance(player.Position, timeBomb.Position) < 50) // 50 is the "pickup distance"
            {
                currentTime += 10f; // Add 10 seconds
                if (currentTime > totalTime) currentTime = totalTime; // Don't let time exceed the max

                // Move bomb to a new random location
                timeBomb.Position = new Vector2(random.Next(0, GraphicsDevice.Viewport.Width), random.Next(0, GraphicsDevice.Viewport.Height));
            }

            // --- Update UI ---

            // Update the values of the progress bars
            timeBar.Value = currentTime / totalTime;
            distanceBar.Value = distanceWalked / maxDistance;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Teal);

            _spriteBatch.Begin();

            // Draw game objects
            player.Draw(_spriteBatch);
            timeBomb.Draw(_spriteBatch);
            timeBar.Draw(_spriteBatch);
            distanceBar.Draw(_spriteBatch);

            // Draw the UI text labels for the progress bars
            _spriteBatch.DrawString(font, "Time Remaining:", new Vector2(10, 20), Color.White);
            _spriteBatch.DrawString(font, "Distance Walked:", new Vector2(330, 20), Color.White);

            // Draw the new controls text at the bottom
            string controls = "Controls:\n" +
                              "Up/Down: Move Forward/Backward\n" +
                              "Left/Right Arrows: Rotate\n" +
                              "A/D: Strafe Left/Right";
            Vector2 controlsPosition = new Vector2(10, GraphicsDevice.Viewport.Height - 80);
            _spriteBatch.DrawString(font, controls, controlsPosition, Color.White);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}