using CPI311.GameEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Lab08
{
    public class Lab08 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // *** Declared variables from Section F ***
        SoundEffect gunSound;
        // SoundEffectInstance soundInstance; // <-- Removed, will use local instance
        Model model;
        Camera camera, topDownCamera;
        List<Transform> transforms;
        List<Collider> colliders;
        List<Camera> cameras;
        Effect effect;
        Texture2D texture;


        public Lab08()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _graphics.GraphicsProfile = GraphicsProfile.HiDef;

            // *** Initialize lists ***
            transforms = new List<Transform>();
            colliders = new List<Collider>();
            cameras = new List<Camera>();
        }

        protected override void Initialize()
        {
            // *** Initialize static managers ***
            Time.Initialize();
            InputManager.Initialize();
            ScreenManager.Initialize(_graphics);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // *** Load assets ***
            model = Content.Load<Model>("Sphere");
            effect = Content.Load<Effect>("SimpleShading (1)");
            texture = Content.Load<Texture2D>("Square");
            gunSound = Content.Load<SoundEffect>("Gun");

            // *** Lab 8 Item: Setup ScreenManager ***
            ScreenManager.Setup(false, 1920, 1080); // Using 'false' for windowed mode

            // *** Setup single object (transform and collider) ***
            Transform transform = new Transform();
            SphereCollider sphereCollider = new SphereCollider();
            sphereCollider.Radius = 1.0f;
            sphereCollider.Transform = transform;

            transforms.Add(transform);
            colliders.Add(sphereCollider);

            // *** Setup main camera (left side of screen) ***
            camera = new Camera();
            camera.Transform = new Transform();
            camera.Transform.LocalPosition = Vector3.Backward * 5;
            camera.Position = new Vector2(0f, 0f);    // Start at top-left
            camera.Size = new Vector2(0.5f, 1f);      // 50% width, 100% height
            camera.AspectRatio = camera.Viewport.AspectRatio;

            // *** Setup top-down camera (right side of screen) ***
            topDownCamera = new Camera();
            topDownCamera.Transform = new Transform();
            topDownCamera.Transform.LocalPosition = Vector3.Up * 10;
            topDownCamera.Transform.Rotate(Vector3.Right, -MathHelper.PiOver2); // Look down
            topDownCamera.Position = new Vector2(0.5f, 0f); // Start at horizontal midpoint
            topDownCamera.Size = new Vector2(0.5f, 1f);     // 50% width, 100% height
            topDownCamera.AspectRatio = topDownCamera.Viewport.AspectRatio;

            cameras.Add(topDownCamera);
            cameras.Add(camera);

            // Set a default color before the first Update
            effect.Parameters["DiffuseColor"].SetValue(Color.Blue.ToVector3());
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // *** Update static managers ***
            Time.Update(gameTime);
            InputManager.Update();

            // *** Raycasting logic from Section C ***
            Ray ray = camera.ScreenPointToWorldRay(InputManager.GetMousePosition());
            float nearest = Single.MaxValue; // Start with highest value
            float? p;
            Collider target = null; // Assume no intersection

            foreach (Collider collider in colliders)
                if ((p = collider.Intersects(ray)) != null)
                {
                    float q = (float)p;
                    if (q < nearest)
                        nearest = q;
                    target = collider;
                }

            // *** Color-change logic from Section F ***
            if (target != null && nearest < camera.FarPlane)
            {
                // Hit! Set color to Red
                effect.Parameters["DiffuseColor"].SetValue(Color.Red.ToVector3());
                SoundEffectInstance instance = gunSound.CreateInstance();
                instance.Volume = 0.5f;
                instance.Play();
            }
            else
            {
                // No hit. Set color to Blue
                effect.Parameters["DiffuseColor"].SetValue(Color.Blue.ToVector3());
            }

            // *** Sound playback logic from Section D ***
            if (InputManager.IsKeyPressed(Keys.Space))
            {
                // Create a local instance that plays once and disposes
                SoundEffectInstance instance = gunSound.CreateInstance();
                instance.Volume = 0.5f;
                instance.Play();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // *** Multi-camera rendering loop ***
            foreach (Camera cam in cameras) // Renamed to 'cam' to avoid conflict
            {
                GraphicsDevice.DepthStencilState = new DepthStencilState(); // Enable Z-buffering
                GraphicsDevice.Viewport = cam.Viewport; // Set the viewport

                Matrix view = cam.View;
                Matrix projection = cam.Projection;

                effect.CurrentTechnique = effect.Techniques[1];
                effect.Parameters["View"].SetValue(view);
                effect.Parameters["Projection"].SetValue(projection);
                effect.Parameters["LightPosition"].SetValue(Vector3.Backward * 10 + Vector3.Right * 5);
                effect.Parameters["CameraPosition"].SetValue(cam.Transform.Position);
                effect.Parameters["Shininess"].SetValue(20f);
                effect.Parameters["AmbientColor"].SetValue(new Vector3(.2f, .2f, .2f));
                effect.Parameters["SpecularColor"].SetValue(new Vector3(1, 1, 1));
                // 'DiffuseColor' is set in Update()
                effect.Parameters["DiffuseTexture"].SetValue(texture);

                foreach (Transform transform in transforms)
                {
                    effect.Parameters["World"].SetValue(transform.World);
                    foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                    {
                        pass.Apply();
                        foreach (ModelMesh mesh in model.Meshes)
                            foreach (ModelMeshPart part in mesh.MeshParts)
                            {
                                GraphicsDevice.SetVertexBuffer(part.VertexBuffer);
                                GraphicsDevice.Indices = part.IndexBuffer;
                                GraphicsDevice.DrawIndexedPrimitives(
                                    PrimitiveType.TriangleList,
                                    part.VertexOffset,
                                    0,
                                    part.PrimitiveCount);
                            }
                    }
                }
            }

            // Must be called ONCE, outside the loop
            base.Draw(gameTime);
        }
    }
}