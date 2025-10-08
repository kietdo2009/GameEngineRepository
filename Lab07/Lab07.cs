using CPI311.GameEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Threading;
namespace Lab07
{
    public class Lab07 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        //**********************************************LAB 6
        BoxCollider boxCollider;
        SphereCollider sphere1, sphere2;
        List<Transform> transforms;
        List<Rigidbody> rigidbodies;
        List<Collider> colliders;
        Random random;
        public Camera camera;
        public Transform cameraTransform;
        public Model model;
        
        int lastSecondCollisions = 0;
        //**********************************************LAB 6


        // Lab 7 ******************************
        int numberCollisions = 0;
        SpriteFont font;
        bool haveTreadRunning = false;
        int lastSecondCollision = 0;
        List<Renderer> renderers;
        
        Light light;
        Transform lightTransform;

        // Lab 7 ******************************
        public Lab07()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            // Initialize your game engine managers
            InputManager.Initialize();
            Time.Initialize();
            // Lab 7 ******************************
            ThreadPool.QueueUserWorkItem(new WaitCallback(CollisionReset));
            List<Renderer> renderers = new List<Renderer>();

            random = new Random();
            transforms = new List<Transform>();
            rigidbodies = new List<Rigidbody>();
            colliders = new List<Collider>();
            boxCollider = new BoxCollider();
            boxCollider.Size = 10;
            Transform boxTransform = new Transform();
            boxCollider.Transform = boxTransform;


            for (int i = 0; i < 2; i++)
            {
                Transform transform = new Transform();
                transform.Position += Vector3.Right * 3 * i; //Fix in lab 6 
                Rigidbody rigidbody = new Rigidbody();
                rigidbody.Transform = transform;
                rigidbody.Mass = 1;
                Vector3 direction = new Vector3(
                (float)random.NextDouble(), (float)random.NextDouble(),
                (float)random.NextDouble());
                direction.Normalize();
                rigidbody.Velocity =
                direction * ((float)random.NextDouble() * 5 + 5);
                SphereCollider sphereCollider = new SphereCollider();
                sphereCollider.Radius = 1.0f * transform.LocalScale.Y;
                sphereCollider.Transform = transform;
                transforms.Add(transform);
                colliders.Add(sphereCollider);
                rigidbodies.Add(rigidbody);
            }

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            //same as lab 5 load content
            font = Content.Load<SpriteFont>("Font");
            model = Content.Load<Model>("Sphere");
            camera = new Camera();
            cameraTransform = new Transform();
            cameraTransform.LocalPosition = Vector3.Backward * 50f; // (0, 0, 10)
            camera.Transform = cameraTransform;

            lightTransform = new Transform();
            lightTransform.LocalPosition = Vector3.Up * 10 + Vector3.Forward * 10;
            light = new Light();
            light.Transform = lightTransform;

            
            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            if (InputManager.IsKeyPressed(Keys.Space))
                AddSphere();

            // TODO: Add your update logic here
            foreach (Rigidbody rigidbody in rigidbodies) rigidbody.Update();
            Vector3 normal; // it is updated if a collision happens
            for (int i = 0; i < transforms.Count; i++)
            {
                if (boxCollider.Collides(colliders[i], out normal))
                {
                    //numberCollisions++;
                    if (Vector3.Dot(normal, rigidbodies[i].Velocity) < 0)
                        rigidbodies[i].Impulse +=
                        Vector3.Dot(normal, rigidbodies[i].Velocity) * -2 * normal;
                }
                for (int j = i + 1; j < transforms.Count; j++)
                {
                    if (colliders[i].Collides(colliders[j], out normal))
                    {
                        //numberCollisions++;
                        Vector3 velocityNormal = Vector3.Dot(normal,
                        rigidbodies[i].Velocity - rigidbodies[j].Velocity) * -2
                        * normal * rigidbodies[i].Mass * rigidbodies[j].Mass;
                        rigidbodies[i].Impulse += velocityNormal / 2;
                        rigidbodies[j].Impulse += -velocityNormal / 2;
                    }
                }
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            //foreach (Transform transform in transforms)
            //    model.Draw(transform.World, camera.View, camera.Projection);
            for (int i = 0; i < transforms.Count; i++)
            {
                Transform transform = transforms[i];
                float speed = rigidbodies[i].Velocity.Length();
                float speedValue = MathHelper.Clamp(speed / 20f, 0, 1);
                (model.Meshes[0].Effects[0] as BasicEffect).DiffuseColor =
                new Vector3(speedValue, speedValue, 1);
                model.Draw(transform.World, camera.View, camera.Projection);
            }
            _spriteBatch.Begin();
            _spriteBatch.DrawString(font, "Collisions: " + lastSecondCollisions, new Vector2(10, 10), Color.White);
            _spriteBatch.End();
            base.Draw(gameTime);
        }
        private void CollisionReset(Object obj)
        {
            while (haveTreadRunning)
            {
                lastSecondCollisions = numberCollisions;
                numberCollisions = 0;
                System.Threading.Thread.Sleep(1000);
            }
        }
        private void AddSphere()
        {
            Transform transform = new Transform();
            transform.Position = Vector3.Right * (float)random.NextDouble();
            Rigidbody sphere = new Rigidbody();
            sphere.Mass = 1;
            Vector3 direction = new Vector3(
                (float)random.NextDouble(), (float)random.NextDouble(),
                (float)random.NextDouble());
            direction.Normalize();
            sphere.Velocity = direction * ((float)random.NextDouble() * 5 + 5);
            Texture2D texture = Content.Load<Texture2D>("Square");
            Renderer renderer = new Renderer(model, transform, camera, Content, GraphicsDevice, light, "SimpleShading", 1, 20f, texture);
            
            //make sphere collider
            SphereCollider sphereCollider = new SphereCollider();
            sphereCollider.Radius = 1.0f * transform.LocalScale.Y;
            sphereCollider.Transform = transform;
            transforms.Add(transform);
            colliders.Add(sphereCollider);
            renderers.Add(renderer);

            
        }
    }
}

