using CPI311.GameEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics;
using System.Threading;
namespace Lab07
{
    public class Lab07 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        // Lab 6 *********
        BoxCollider boxCollider;
        SphereCollider sphere1, sphere2;
        List<Transform> transforms;
        List<Rigidbody> rigidbodies;
        List<Collider> colliders;
        // *************************
        List<Renderer> renderers;
        //**************************
        Random random;
        Model model;
        Camera camera;
        Transform cameraTransform;
        // **************************
        Light light;
        Transform lightTransform;
        // **************************
        // Lab 7 *****************
        int numberCollisions = 0;
        SpriteFont font;
        bool haveThreadRunning = true;
        int lastSecondCollisions = 0;
        // ***********************
        public Lab07()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            // ***************************************************
            _graphics.GraphicsProfile = GraphicsProfile.HiDef;
            // ***************************************************
        }
        protected override void Initialize()
        {
            // ***********************
            Time.Initialize();
            InputManager.Initialize();
            //*************************
            // *** Lab 7 *************************************************
            ThreadPool.QueueUserWorkItem(new WaitCallback(CollisionReset));
            //************************************************************
            random = new Random();
            transforms = new List<Transform>();
            rigidbodies = new List<Rigidbody>();
            colliders = new List<Collider>();
            // *******************************
            renderers = new List<Renderer>();
            // *******************************
            boxCollider = new BoxCollider();
            boxCollider.Size = 10;
            base.Initialize();
        }
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("Font");
            model = Content.Load<Model>("Sphere");
            cameraTransform = new Transform();
            cameraTransform.LocalPosition = Vector3.Backward * 20; // (0, 0, -20)
            camera = new Camera();
            camera.Transform = cameraTransform;
            // ***********************************
            lightTransform = new Transform();
            lightTransform.LocalPosition = Vector3.Backward * 20 + Vector3.Up * 20
            + Vector3.Right * 20;
            light = new Light();
            light.Transform = lightTransform;
            //***********************************
        }
        protected override void Update(GameTime gameTime)
        {
            // ***********************
            Time.Update(gameTime);
            InputManager.Update();
            //************************
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back ==
            ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            if (InputManager.IsKeyPressed(Keys.Space))
            {
                AddSphere();
            }
            foreach (Rigidbody rigidbody in rigidbodies)
                rigidbody.Update();
            Vector3 normal; // it is updated if a collision happens
            for (int i = 0; i < transforms.Count; i++)
            {
                if (boxCollider.Collides(colliders[i], out normal))
                {
                    numberCollisions++;
                    if (Vector3.Dot(normal, rigidbodies[i].Velocity) < 0)
                        rigidbodies[i].Impulse +=
                        Vector3.Dot(normal, rigidbodies[i].Velocity) * -2 *
                        normal;
                }
                for (int j = i + 1; j < transforms.Count; j++)
                {
                    if (colliders[i].Collides(colliders[j], out normal)) { }
                    //numberCollisions++;
                    Vector3 velocityNormal = Vector3.Dot(normal,
                    rigidbodies[i].Velocity - rigidbodies[j].Velocity) * -2
                    * normal * rigidbodies[i].Mass * rigidbodies[j].Mass;
                    rigidbodies[i].Impulse += velocityNormal / 2;
                    rigidbodies[j].Impulse += -velocityNormal / 2;
                }
            }
            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            //foreach (Transform transform in transforms)
            // model.Draw(transform.World, camera.View, camera.Projection);
            //for (int i = 0; i < transforms.Count; i++)
            //{
            // Transform transform = transforms[i];
            // float speed = rigidbodies[i].Velocity.Length();
            // float speedValue = MathHelper.Clamp(speed / 20f, 0, 1);
            // (model.Meshes[0].Effects[0] as BasicEffect).DiffuseColor =
            // new Vector3(speedValue, speedValue,
            //1);
            // model.Draw(transform.World, camera.View, camera.Projection);
            //}
            for (int i = 0; i < renderers.Count; i++)
                renderers[i].Draw();
            _spriteBatch.Begin();
            _spriteBatch.DrawString(font, "Collision: " + lastSecondCollisions,
            Vector2.Zero, Color.Black);
            _spriteBatch.End();
            base.Draw(gameTime);
        }
        // **** Private Lab7 Functions
        private void CollisionReset(Object obj)
        {
            while (haveThreadRunning)
            {
                lastSecondCollisions = numberCollisions;
                numberCollisions = 0;
                System.Threading.Thread.Sleep(1000);
            }
        }
        private void AddSphere()
        {
            // *** Make a transform
            Transform transform = new Transform();
            transform.Position += Vector3.Right * 10 * (float)random.NextDouble();
            // *** Make a rigidbody
            Rigidbody rigidbody = new Rigidbody();
            rigidbody.Transform = transform;
            rigidbody.Mass = 1;
            Vector3 direction = new Vector3(
            (float)random.NextDouble(), (float)random.NextDouble(),
            (float)random.NextDouble());
            direction.Normalize();
            rigidbody.Velocity =
            direction * ((float)random.NextDouble() * 5 + 5);
            // *** Make a shperecollider
            SphereCollider sphereCollider = new SphereCollider();
            sphereCollider.Radius = 1.0f * transform.LocalScale.Y;
            sphereCollider.Transform = transform;
            // **** Make a rendere
            Texture2D texture = Content.Load<Texture2D>("Square (1)");
            Renderer renderer = new Renderer(model, transform, camera, Content,
            GraphicsDevice, light, 1, "SimpleShading", 20f,
            texture);
            transforms.Add(transform);
            colliders.Add(sphereCollider);
            rigidbodies.Add(rigidbody);
            // ***********************
            renderers.Add(renderer);
            // ***********************
        }
    }
}