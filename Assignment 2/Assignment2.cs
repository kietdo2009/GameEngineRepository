using CPI311.GameEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic; // Keep this if you add more objects later

namespace Assignment2
{
    public class Assignment2 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // Models for our objects
        Model sphereModel; // For Sun, Earth, Mercury, Luna
        Model planeModel;  // For the ground
        Model playerModel; // For the player character

        // Transforms for all game objects
        Transform sunTransform;
        Transform mercuryTransform;
        Transform earthTransform;
        Transform lunaTransform;
        Transform planeTransform;
        Transform playerTransform;
        Transform cameraTransform;
        Transform modelTransform;

        // Two cameras as required
        Camera firstPersonCamera;
        Camera thirdPersonCamera;
        Camera activeCamera; // This is the one we are currently using
        Camera camera; // Add this line

        // Control variables
        float simulationSpeed = 1.0f;
        float playerMoveSpeed = 5.0f;
        float playerRotationSpeed = 2.0f;
        float zoomSpeed = 0.5f; // For field of view zoom

        // Solar system animation speeds
        float sunRotationSpeed = 0.1f;
        float mercuryRevolutionSpeed = 0.5f;
        float earthRevolutionSpeed = 0.3f;
        float earthRotationSpeed = 0.2f;
        float lunaRevolutionSpeed = 1.0f;


        public Assignment2()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true; // Useful for debugging, can be false
        }

        protected override void Initialize()
        {
            // Initialize game engine helpers
            Time.Initialize();
            InputManager.Initialize();

            // *** 1. Set up the Solar System Hierarchy ***

            // Sun (Sol) - Center, Radius 5
            sunTransform = new Transform();
            sunTransform.LocalScale = Vector3.One * 5;

            // Mercury - Orbits Sun, Radius 2, No rotation
            mercuryTransform = new Transform();
            mercuryTransform.Parent = sunTransform;
            mercuryTransform.LocalScale = Vector3.One * 2;
            mercuryTransform.LocalPosition = Vector3.Right * 10; // 10 units from Sun

            // Earth - Orbits Sun, Radius 3, Rotates
            earthTransform = new Transform();
            earthTransform.Parent = sunTransform;
            earthTransform.LocalScale = Vector3.One * 3;
            earthTransform.LocalPosition = Vector3.Right * 20; // 20 units from Sun

            // Luna (Moon) - Orbits Earth, Radius 1, Rotates 2x for 1 revolution
            lunaTransform = new Transform();
            lunaTransform.Parent = earthTransform;
            lunaTransform.LocalScale = Vector3.One * 1;
            lunaTransform.LocalPosition = Vector3.Right * 5; // 5 units from Earth

            // *** 2. Set up the Plane and Player ***

            // Ground Plane
            planeTransform = new Transform();
            planeTransform.LocalPosition = Vector3.Down * 10; // Place it below the solar system
            planeTransform.LocalScale = Vector3.One * 100; // Make it large

            // Player
            playerTransform = new Transform();
            // Start the player on top of the plane
            playerTransform.LocalPosition = new Vector3(0, planeTransform.LocalPosition.Y + 1, 0);


            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            
            sphereModel = Content.Load<Model>("Sphere");
            planeModel = Content.Load<Model>("Sphere");
            playerModel = Content.Load<Model>("Sphere"); // e.g., a simple capsule or box

            // *** 3. Set up the Cameras ***

            // First Person Camera (Child of player)
            //firstPersonCamera = new Camera();
            //firstPersonCamera.Transform.Parent = playerTransform;
            //firstPersonCamera.Transform.LocalPosition = new Vector3(0, 0.5f, 0); // "Eye level"
            //firstPersonCamera.FarPlane = 1000f; // See far

            // Third Person Camera (Fixed, top-down)
            thirdPersonCamera = new Camera();
            thirdPersonCamera.Transform.LocalPosition = new Vector3(0, 100, 1); // High up
            // Look down at the world
            thirdPersonCamera.Transform.LocalRotation = Quaternion.CreateFromAxisAngle(
                                                            Vector3.Right, -MathHelper.PiOver2);
            thirdPersonCamera.FarPlane = 1000f;

            // Start in first-person mode
            activeCamera = firstPersonCamera;
        }

        protected override void Update(GameTime gameTime)
        {
            // Update engine helpers
            Time.Update(gameTime);
            InputManager.Update();

            // Get delta time for frame-independent movement
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // *** 4. Handle Simulation Controls ***

            // Control simulation speed (e.g., + and - keys)
            if (InputManager.IsKeyDown(Keys.OemPlus))
                simulationSpeed += 0.5f * dt;
            if (InputManager.IsKeyDown(Keys.OemMinus))
                simulationSpeed = Math.Max(0, simulationSpeed - 0.5f * dt); // Don't go below 0

            // Control zoom (e.g., [ and ] keys)
            if (InputManager.IsKeyDown(Keys.OemOpenBrackets))
                activeCamera.FieldOfView += zoomSpeed * dt;
            if (InputManager.IsKeyDown(Keys.OemCloseBrackets))
                activeCamera.FieldOfView -= zoomSpeed * dt;


            // *** 5. Handle Camera Switching ***
            if (InputManager.IsKeyPressed(Keys.Tab))
            {
                if (activeCamera == firstPersonCamera)
                    activeCamera = thirdPersonCamera;
                else
                    activeCamera = firstPersonCamera;
            }

            // *** 6. Handle Player Movement and Rotation ***

            // Player Rotation (Arrow Keys Left/Right for YAW)
            if (InputManager.IsKeyDown(Keys.Left))
                playerTransform.Rotate(Vector3.Up, playerRotationSpeed * dt);
            if (InputManager.IsKeyDown(Keys.Right))
                playerTransform.Rotate(Vector3.Up, -playerRotationSpeed * dt);

            // Camera-only Pitch (Arrow Keys Up/Down for PITCH)
            // This only rotates the first-person camera, not the player's body
            if (activeCamera == firstPersonCamera)
            {
                if (InputManager.IsKeyDown(Keys.Up))
                    firstPersonCamera.Transform.Rotate(Vector3.Right, playerRotationSpeed * dt);
                if (InputManager.IsKeyDown(Keys.Down))
                    firstPersonCamera.Transform.Rotate(Vector3.Right, -playerRotationSpeed * dt);
            }

            // Player Movement (WASD)
            Vector3 moveDirection = Vector3.Zero;
            if (InputManager.IsKeyDown(Keys.W))
                moveDirection += playerTransform.Forward;
            if (InputManager.IsKeyDown(Keys.S))
                moveDirection += playerTransform.Backward;
            if (InputManager.IsKeyDown(Keys.A))
                moveDirection += playerTransform.Left;
            if (InputManager.IsKeyDown(Keys.D))
                moveDirection += playerTransform.Right;

            // "No movement in Y-axis" constraint
            moveDirection.Y = 0;

            if (moveDirection != Vector3.Zero)
            {
                moveDirection.Normalize();
                playerTransform.LocalPosition += moveDirection * playerMoveSpeed * dt;
            }


            // *** 7. Animate the Solar System ***

            // Get the scaled delta time for the simulation
            float simDelta = dt * simulationSpeed;

            // Sun rotates
            sunTransform.Rotate(Vector3.Up, sunRotationSpeed * simDelta);

            // Mercury revolves (no rotation)
            mercuryTransform.Rotate(Vector3.Up, mercuryRevolutionSpeed * simDelta);

            // Earth revolves and rotates
            earthTransform.Rotate(Vector3.Up, earthRevolutionSpeed * simDelta);
            earthTransform.Rotate(Vector3.Up, earthRotationSpeed * simDelta);

            // Luna revolves and rotates (2x rotation per 1x revolution)
            lunaTransform.Rotate(Vector3.Up, lunaRevolutionSpeed * simDelta);
            lunaTransform.Rotate(Vector3.Up, (lunaRevolutionSpeed * 2) * simDelta);


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black); // Black for space

            // Get the matrices from the currently active camera
            Matrix view = activeCamera.View;
            Matrix projection = activeCamera.Projection;

            // Draw all the models
            sphereModel.Draw(sunTransform.World, view, projection);
            sphereModel.Draw(mercuryTransform.World, view, projection);
            sphereModel.Draw(earthTransform.World, view, projection);
            sphereModel.Draw(lunaTransform.World, view, projection);
            planeModel.Draw(planeTransform.World, view, projection);

            // "Represent the first person by a model when in third person mode"
            // "but don't display it when in first person mode"
            if (activeCamera == thirdPersonCamera)
            {
                playerModel.Draw(playerTransform.World, view, projection);
            }


            base.Draw(gameTime);
        }
    }
}