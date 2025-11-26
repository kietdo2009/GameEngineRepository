using CPI311.GameEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;


namespace Assignment4
{
    public class Ship : GameObject
    {
        public Model Model;
        public Matrix[] Transforms;

        public Vector3 Position = Vector3.Zero;

        public Vector3 Velocity = Vector3.Zero;

        public Matrix RotationMatrix = Matrix.Identity;
        public float rotation;
        public Ship(ContentManager Content, Camera camera, GraphicsDevice graphicsDevice, Light light) : base()
        {
            // Add Rigidbody, Collider, Renderer components
            // *** Add Rigidbody
            Rigidbody rigidbody = new Rigidbody();
            rigidbody.Transform = Transform;
            rigidbody.Mass = 1;
            Add<Rigidbody>(rigidbody);

            // *** Add Renderer
            Texture2D texture = Content.Load<Texture2D>("wedge_p1_diff_v1");
            Renderer renderer = new Renderer(Content.Load<Model>("p1_wedge"),
            Transform, camera, Content, graphicsDevice, light, 1, null, 20f, texture);
            Add<Renderer>(renderer);

            // *** Add collider
            SphereCollider sphereCollider = new SphereCollider();
            sphereCollider.Radius = renderer.ObjectModel.Meshes[0].BoundingSphere.Radius;
            sphereCollider.Transform = Transform;
            Add<Collider>(sphereCollider);
        }
        public float Rotation
        {
            get { return rotation; }
            set
            {
                float newVal = value;
                while (newVal >= MathHelper.TwoPi)
                    newVal -= MathHelper.TwoPi;
                while (newVal < 0)
                {
                    newVal += MathHelper.TwoPi;
                }

                if (rotation != newVal)
                {
                    rotation = newVal;
                    RotationMatrix = Matrix.CreateRotationY(rotation);
                }
            }
        }
        // Inside Ship.cs

        public override void Update()
        {
            // 1. Handle Rotation
            if (InputManager.IsKeyDown(Keys.A))
                Transform.LocalPosition += Transform.Left * Time.ElapsedGameTime * GameConstants.VelocityScale * 500;

            if (InputManager.IsKeyDown(Keys.D))
                Transform.LocalPosition -= Transform.Left* Time.ElapsedGameTime * GameConstants.VelocityScale * 500;

            // 2. Handle Movement
            // Use Transform.Forward so it moves in the direction it's facing
            if (InputManager.IsKeyDown(Keys.W))
                Transform.LocalPosition += Transform.Forward * Time.ElapsedGameTime * GameConstants.VelocityScale*500;

            if (InputManager.IsKeyDown(Keys.S))
                Transform.LocalPosition -= Transform.Forward * Time.ElapsedGameTime * GameConstants.VelocityScale* 500;

            Position = Transform.LocalPosition;

            base.Update();
        }

    }
}
