using CPI311.GameEngine;
using CPI311.GameEngine.CPI311.GameEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;


namespace Assignment4
{
    public class Assignment4 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        
        Ship ship;
        BoxCollider boxCollider;
        Random random;
        Model model;
        Camera camera;
        Transform cameraTransform;
        Light light;
        Transform lightTransform;
        ParticleManager particleManager; // Add this
        Texture2D particleTex;           // Add this
        Effect particleEffect;           // Add this
        int score;                       // Add this
        SoundEffect gunSound;            // Add this

        Asteroid[] asteroidList = new Asteroid[GameConstants.NumAsteroids];
        Bullet[] bulletList = new Bullet[GameConstants.NumBullets];
        private SoundEffectInstance soundInstance;

        public Assignment4()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            //Audio components
            //SoundEffect gunSound;
            //SoundEffectInstance soundInstance;
            ////Visual components
            //Ship ship;
            //Asteroid[] asteroidList = new Asteroid[GameConstants.NumAsteroids];
            //Bullet[] bulletList = new Bullet[GameConstants.NumBullets];
            ////Score & background
            //int score;
            //Texture2D stars;
            //SpriteFont lucidaConsole;
            //Vector2 scorePosition = new Vector2(100, 50);
            //// Particles
            //ParticleManager particleManager;
            //Texture2D particleTex;
            //Effect particleEffect;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            Time.Initialize();
            InputManager.Initialize();
            random = new Random();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            // Inside Assignment4.cs -> LoadContent()

            cameraTransform = new Transform();
            // 1. Move Camera WAY up (GameConstants says 25,000, let's try 5,000 for now so you can see)
            cameraTransform.LocalPosition = new Vector3(0, 5000, 0);

            // 2. Rotate Camera to look DOWN (90 degrees on the X axis)
            cameraTransform.Rotation = Quaternion.CreateFromAxisAngle(Vector3.Right, -MathHelper.PiOver2);

            camera = new Camera();
            camera.Transform = cameraTransform;
            camera.FarPlane = 30000f;
            // Setup Light
            lightTransform = new Transform();
            lightTransform.LocalPosition = Vector3.Backward * 20 + Vector3.Up * 20 + Vector3.Right * 20;
            light = new Light();
            light.Transform = lightTransform;
            //Initialize Ship
            ship = new Ship(Content, camera, GraphicsDevice, light);
            //Initialize Bullets
            for (int i = 0; i < GameConstants.NumBullets; i++)
                bulletList[i] = new Bullet(Content, camera, GraphicsDevice, light);
            // 3. Initialize Asteroids 
            ResetAsteroids();

            model = Content.Load<Model>("p1_wedge");
            gunSound = Content.Load<SoundEffect>("explosion2");
            //particleManager = new ParticleManager(GraphicsDevice, 100);
            //particleEffect = Content.Load<Effect>("ParticleShader-complete");
            //particleTex = Content.Load<Texture2D>("Textures/fire");
            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            Time.Update(gameTime); // Update(gameTime);
            InputManager.Update();
            // TODO: Add your update logic here
            ship.Update();
            for (int i = 0; i < GameConstants.NumBullets; i++) bulletList[i].Update();
            for (int i = 0; i < GameConstants.NumAsteroids; i++) asteroidList[i].Update();

            if (InputManager.IsKeyPressed(Keys.Space))
            {
                for (int i = 0; i < GameConstants.NumBullets; i++)
                {
                    // Find a bullet that isn't currently being used
                    if (!bulletList[i].isActive)
                    {
                        // 1. Set velocity in the direction the ship is facing
                        bulletList[i].Rigidbody.Velocity = ship.Transform.Forward * GameConstants.BulletSpeedAdjustment*20;

                        // 2. Place the bullet at the ship's position
                        // We add a little offset (Forward * 200) so it doesn't spawn inside the ship
                        bulletList[i].Transform.LocalPosition = ship.Transform.Position + (200 * ship.Transform.Forward);

                        // 3. Activate it
                        bulletList[i].isActive = true;

                        
                        soundInstance = gunSound.CreateInstance();
                        soundInstance.Play();

                        // 5. Exit loop so we only fire ONE bullet per press
                        break;
                    }
                }
            }
            
            Vector3 normal; // This stores the angle of impact, required by the Collides method

            for (int i = 0; i < GameConstants.NumAsteroids; i++)
            {
                // Only check active asteroids
                if (asteroidList[i].isActive)
                {
                    for (int j = 0; j < GameConstants.NumBullets; j++)
                    {
                        // Only check active bullets
                        if (bulletList[j].isActive)
                        {
                            // Check if the two objects are touching
                            if (asteroidList[i].Collider.Collides(bulletList[j].Collider, out normal))
                            {
                                // 1. Destroy both objects
                                asteroidList[i].isActive = false;
                                bulletList[j].isActive = false;

                                // 2. Add to Score (optional)
                                // score += GameConstants.KillBonus;

                                
                                            // (Only runs if you have particleManager set up correctly)
                                if (particleManager != null)
                                {
                                    Particle particle = particleManager.getNext();
                                    particle.Position = asteroidList[i].Transform.Position;
                                    particle.Velocity = new Vector3(
                                        random.Next(-5, 5), 2, random.Next(-50, 50));
                                    particle.Acceleration = new Vector3(0, 3, 0);
                                    particle.MaxAge = random.Next(1, 6);
                                    particle.Init();
                                }

                                // 4. Break out of the bullet loop (one bullet can't hit two asteroids)
                                break;
                            }
                        }
                    }
                }
            }
            //particleManager.Update();
            // --- ADD THESE DEBUG PRINT STATEMENTS ---
            Debug.WriteLine($"Ship Position: {ship.Transform.LocalPosition}");
            Debug.WriteLine($"Camera Position: {camera.Transform.LocalPosition}");
            Debug.WriteLine("------------------------------------------");
            // ----------------------------------------
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            ship.Draw();
         
            for (int i = 0; i < GameConstants.NumBullets; i++) bulletList[i].Draw();
            for (int i = 0; i < GameConstants.NumAsteroids; i++) asteroidList[i].Draw();
            base.Draw(gameTime);
        }
        private void ResetAsteroids()
        {
            float xStart;
            float yStart;
            for (int i = 0; i < GameConstants.NumAsteroids; i++)
            {
                if (random.Next(2) == 0)
                    xStart = (float)-GameConstants.PlayfieldSizeX;
                else
                    xStart = (float)GameConstants.PlayfieldSizeX;

                yStart = (float)random.NextDouble() * GameConstants.PlayfieldSizeY;

                asteroidList[i] = new Asteroid(Content, camera, GraphicsDevice, light);
                asteroidList[i].Transform.Position = new Vector3(0, 0.0f, 0);

                double angle = random.NextDouble() * 2 * Math.PI;
                asteroidList[i].Rigidbody.Velocity = new Vector3(
                    -(float)Math.Sin(angle), 0, (float)Math.Cos(angle)) *
                    (GameConstants.AsteroidMinSpeed + (float)random.NextDouble() *
                    GameConstants.AsteroidMaxSpeed);

                asteroidList[i].isActive = true;
            }
        }
    }
}
