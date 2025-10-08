using CPI311.GameEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Reflection;

namespace Lab04
{
    public class Lab04 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // Models + transforms
        public Model torus;
        public Transform torusTransform;

        public Model sphere;
        public Transform sphereTransform;

        // Camera
        public Camera camera;
        public Transform cameraTransform;

        public Lab04()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load models
            torus = Content.Load<Model>("Torus");
            sphere = Content.Load<Model>("Sphere");

            // Parent object (sphere)
            sphereTransform = new Transform();
            sphereTransform.LocalPosition = Vector3.Zero;

            // Child object (torus)
            torusTransform = new Transform();
            torusTransform.LocalPosition = Vector3.Right * 5f;
            torusTransform.Parent = sphereTransform;  // Parent-child relationship

            // Camera setup
            camera = new Camera();
            cameraTransform = new Transform();
            cameraTransform.LocalPosition = Vector3.Backward * 10f; // (0, 0, 10)
            camera.Transform = cameraTransform;
            // ***ADDED*** Lighting setup
            foreach (ModelMesh mesh in torus.Meshes)
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;
                }
            foreach (ModelMesh mesh in sphere.Meshes)
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;
                }
        }

        protected override void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            KeyboardState ks = Keyboard.GetState();

            // --- Camera movement ---
            if (ks.IsKeyDown(Keys.W))
                cameraTransform.LocalPosition += cameraTransform.Forward * dt * 5f;
            if (ks.IsKeyDown(Keys.S))
                cameraTransform.LocalPosition += cameraTransform.Backward * dt * 5f;
            if (ks.IsKeyDown(Keys.A))
                cameraTransform.LocalPosition += cameraTransform.Left * dt * 5f;
            if (ks.IsKeyDown(Keys.D))
                cameraTransform.LocalPosition += cameraTransform.Right * dt * 5f;
            if (ks.IsKeyDown(Keys.Q))
                cameraTransform.LocalPosition += cameraTransform.Up * dt * 5f;
            if (ks.IsKeyDown(Keys.E))
                cameraTransform.LocalPosition += cameraTransform.Down * dt * 5f;

            // --- Rotate parent sphere ---
            if (ks.IsKeyDown(Keys.Left))
                sphereTransform.Rotate(Vector3.Up, dt);
            if (ks.IsKeyDown(Keys.Right))
                sphereTransform.Rotate(Vector3.Up, -dt);
            if (ks.IsKeyDown(Keys.Up))
                sphereTransform.Rotate(Vector3.Right, dt);
            if (ks.IsKeyDown(Keys.Down))
                sphereTransform.Rotate(Vector3.Right, -dt);

            // --- Rotate child torus independently ---
            if (ks.IsKeyDown(Keys.Space))
                torusTransform.Rotate(Vector3.Forward, dt * 2f);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Draw parent and child
            sphere.Draw(sphereTransform.World, camera.View, camera.Projection);
            torus.Draw(torusTransform.World, camera.View, camera.Projection);

            base.Draw(gameTime);
        }
    }
}
