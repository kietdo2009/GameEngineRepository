using CPI311.GameEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Assignment3
{
    // Enum to manage rendering modes
    public enum RenderMode { Texture, SpeedColor }

    public class Assignment3 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // *** Refactored List (Assignment 3) ***
        List<GameObject> gameObjects; //

        // *** Simulation Objects ***
        BoxCollider boxCollider;
        Random random;
        Model model;
        Camera camera;
        Transform cameraTransform;
        Light light;
        Transform lightTransform;

        // *** UI and Controls ***
        SpriteFont font;
        private float simulationSpeed = 1.0f;
        private RenderMode renderMode = RenderMode.Texture;
        private bool showUI = true;
        private int frameCounter = 0;
        private float tenSecondTimer = 0f;
        private float averageFPS = 0f;

        // *** Collision Threading ***
        int numberCollisions = 0;
        bool haveThreadRunning = true;
        int lastSecondCollisions = 0;

        //Tiling Task
        private Vector2 textureTiling = Vector2.One;
        private Vector2 textureOffset = Vector2.Zero;

        public Assignment3()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _graphics.GraphicsProfile = GraphicsProfile.HiDef;
        }

        protected override void Initialize()
        {
            // Initialize Managers
            Time.Initialize();
            InputManager.Initialize();

            // Start collision counting thread
            ThreadPool.QueueUserWorkItem(new WaitCallback(CollisionReset));

            random = new Random();

            // *** Use new GameObject list ***
            gameObjects = new List<GameObject>(); //

            // Setup the containing box
            boxCollider = new BoxCollider();
            boxCollider.Size = 10;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("Font");
            model = Content.Load<Model>("Sphere");

            // Setup Camera
            cameraTransform = new Transform();
            cameraTransform.LocalPosition = Vector3.Backward * 20;
            camera = new Camera();
            camera.Transform = cameraTransform;

            // Setup Light
            lightTransform = new Transform();
            lightTransform.LocalPosition = Vector3.Backward * 20 + Vector3.Up * 20 + Vector3.Right * 20;
            light = new Light();
            light.Transform = lightTransform;

            
            for (int i = 0; i < 5; i++)
                AddGameObject();
        }

        
        private void AddGameObject()
        {
            // 1. Create the GameObject
            GameObject gameObject = new GameObject();

            // 2. Create and add the Rigidbody component
            Rigidbody rigidbody = new Rigidbody();
            rigidbody.Mass = 1 + (float)random.NextDouble() * 2f; // Random mass [cite: 720]

            Vector3 direction = new Vector3(
                (float)random.NextDouble() - 0.5f,
                (float)random.NextDouble() - 0.5f,
                (float)random.NextDouble() - 0.5f);
            direction.Normalize();
            rigidbody.Velocity = direction * ((float)random.NextDouble() * 5 + 5);

            gameObject.Add<Rigidbody>(rigidbody);

            // 3. Create and add the Collider component
            SphereCollider sphereCollider = new SphereCollider();
            sphereCollider.Radius = 1.0f; // All spheres same size [cite: 720]

            gameObject.Add<Collider>(sphereCollider);

            // 4. Create and add the Renderer component
            Texture2D texture = Content.Load<Texture2D>("Square (1)");
            // We assume Technique 0 is for Textures, Technique 1 is for SpeedColor
            int technique = (renderMode == RenderMode.Texture) ? 0 : 1;
            Renderer renderer = new Renderer(model, gameObject.Transform, camera, Content,
                GraphicsDevice, light, technique, "SimpleShading", 20f, texture);

            gameObject.Add<Renderer>(renderer);

            // 5. Set random position
            gameObject.Transform.LocalPosition = new Vector3(
                ((float)random.NextDouble() - 0.5f) * boxCollider.Size,
                ((float)random.NextDouble() - 0.5f) * boxCollider.Size,
                ((float)random.NextDouble() - 0.5f) * boxCollider.Size
            );

            // 6. Add the new GameObject to the main list
            gameObjects.Add(gameObject);
        }

        // *** New method for Assignment 3 ***
        private void RemoveGameObject()
        {
            if (gameObjects.Count > 0)
            {
                gameObjects.RemoveAt(gameObjects.Count - 1);
            }
        }

        protected override void Update(GameTime gameTime)
        {

            // e.g. public static void Update(GameTime gameTime, float speed)
            //      { DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds * speed; }
            Time.Update(gameTime, simulationSpeed); // Update(gameTime);
            InputManager.Update();

            
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            // *** 2. ADD WASD CONTROLS (USING Time.ElapsedGameTime) ***
            
            float moveSpeed = 0.5f; // Adjust this to make it faster/slower
            if (InputManager.IsKeyDown(Keys.W))
                textureOffset.Y -= Time.ElapsedGameTime * moveSpeed;
            if (InputManager.IsKeyDown(Keys.S))
                textureOffset.Y += Time.ElapsedGameTime * moveSpeed;
            if (InputManager.IsKeyDown(Keys.A))
                textureOffset.X -= Time.ElapsedGameTime * moveSpeed;
            if (InputManager.IsKeyDown(Keys.D))
                textureOffset.X += Time.ElapsedGameTime * moveSpeed;

            // Add/Remove Spheres
            if (InputManager.IsKeyPressed(Keys.Up))
                AddGameObject(); // 
            if (InputManager.IsKeyPressed(Keys.Down))
                RemoveGameObject(); // 

            // Change Simulation Speed
            if (InputManager.IsKeyDown(Keys.Left))
                simulationSpeed = Math.Max(0.1f, simulationSpeed - 0.1f); // 
            if (InputManager.IsKeyDown(Keys.Right))
                simulationSpeed += 0.1f; // 

            // Toggle UI
            if (InputManager.IsKeyPressed(Keys.LeftShift) || InputManager.IsKeyPressed(Keys.RightShift))
                showUI = !showUI; // 

            // Change Render Mode
            if (InputManager.IsKeyPressed(Keys.Space))
                renderMode = RenderMode.SpeedColor; // [cite: 734]
            if (InputManager.IsKeyPressed(Keys.LeftAlt) || InputManager.IsKeyPressed(Keys.RightAlt))
                renderMode = RenderMode.Texture; // [cite: 735]


           
            foreach (GameObject gameObject in gameObjects)
            {
                // Update component (e.g., Rigidbody)
                gameObject.Update();

                if (gameObject.Renderer?.Material != null)
                {
                    gameObject.Renderer.Material.Tiling = textureTiling;
                    gameObject.Renderer.Material.Offset = textureOffset;
                }
                // Update renderer technique based on user input
                int newTechnique = (renderMode == RenderMode.Texture) ? 0 : 1;
                gameObject.Renderer.CurrentTechnique = newTechnique;

                // (Optional) Update material color based on speed
                if (renderMode == RenderMode.SpeedColor)
                {
                    float speed = gameObject.Rigidbody.Velocity.Length();
                    float speedValue = MathHelper.Clamp(speed / 10f, 0, 1);
                    gameObject.Renderer.Material.Diffuse = new Vector3(speedValue, 1 - speedValue, 0);
                }
                else
                {
                    gameObject.Renderer.Material.Diffuse = Vector3.One; // Reset to white for texture
                }
            }

            // *** Updated Collision Loop (uses components) ***
            Vector3 normal;
            for (int i = 0; i < gameObjects.Count; i++)
            {
                Rigidbody rb_i = gameObjects[i].Rigidbody;
                Collider col_i = gameObjects[i].Collider;

                if (rb_i == null || col_i == null) continue; // Safety check

                
                if (boxCollider.Collides(col_i, out normal))
                {
                    numberCollisions++;
                    if (Vector3.Dot(normal, rb_i.Velocity) < 0)
                        rb_i.Impulse += Vector3.Dot(normal, rb_i.Velocity) * -2 * normal;
                }

                for (int j = i + 1; j < gameObjects.Count; j++)
                {
                    Rigidbody rb_j = gameObjects[j].Rigidbody;
                    Collider col_j = gameObjects[j].Collider;

                    if (rb_j == null || col_j == null) continue;

                    if (col_i.Collides(col_j, out normal))
                    {
                        numberCollisions++; // Count sphere-sphere collisions

                        // Simple (imperfect) impulse resolution
                        Vector3 velocityNormal = Vector3.Dot(normal,
                            rb_i.Velocity - rb_j.Velocity) * -2
                            * normal * rb_i.Mass * rb_j.Mass;

                        rb_i.Impulse += velocityNormal / (rb_i.Mass + rb_j.Mass);
                        rb_j.Impulse += -velocityNormal / (rb_i.Mass + rb_j.Mass);
                    }
                }
            }

           
            tenSecondTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            frameCounter++;
            if (tenSecondTimer >= 10.0f)
            {
                averageFPS = frameCounter / 10.0f;
                frameCounter = 0;
                tenSecondTimer = 0f;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

           
            foreach (GameObject gameObject in gameObjects)
                gameObject.Draw();

            
            if (showUI)
            {
                _spriteBatch.Begin();
                _spriteBatch.DrawString(font, "Atoms: " + gameObjects.Count,
                    new Vector2(10, 10), Color.White);
                _spriteBatch.DrawString(font, "Collisions/sec: " + lastSecondCollisions,
                    new Vector2(10, 30), Color.White);
                _spriteBatch.DrawString(font, "Avg FPS (10s): " + averageFPS.ToString("F2"),
                    new Vector2(10, 50), Color.White);
                _spriteBatch.DrawString(font, "Speed: " + simulationSpeed.ToString("F1") + "x",
                    new Vector2(10, 70), Color.White);
                _spriteBatch.DrawString(font, "Mode: " + renderMode.ToString(),
                    new Vector2(10, 90), Color.White);
                _spriteBatch.End();
            }

            base.Draw(gameTime);
        }

        // *** Collision counting thread (unchanged from Lab07) ***
        private void CollisionReset(Object obj)
        {
            while (haveThreadRunning)
            {
                lastSecondCollisions = numberCollisions;
                numberCollisions = 0;
                System.Threading.Thread.Sleep(1000); // Runs every 1 second
            }
        }

        
    }
}