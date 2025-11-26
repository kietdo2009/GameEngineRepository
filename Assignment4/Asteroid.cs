using CPI311.GameEngine;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;  
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment4
{
    public class Asteroid : GameObject
    {
        public bool isActive;

    public Asteroid(ContentManager Content, Camera camera, GraphicsDevice graphicsDevice, Light light) : base()
    {
            // Add Rigidbody, Collider, Renderer components
            // *** Add Rigidbody
            Rigidbody rigidbody = new Rigidbody();
            rigidbody.Transform = Transform;
            rigidbody.Mass = 1;
            Add<Rigidbody>(rigidbody);
            // *** Add Renderer
            Texture2D texture = Content.Load<Texture2D>("Square");
            Renderer renderer = new Renderer(Content.Load<Model>("asteroid4"),
            Transform, camera, Content, graphicsDevice, light, 1, null, 20f, texture);
            Add<Renderer>(renderer);
            // *** Add collider
            SphereCollider sphereCollider = new SphereCollider();
            sphereCollider.Radius = renderer.ObjectModel.Meshes[0].BoundingSphere.Radius;
            sphereCollider.Transform = Transform;
            Add<Collider>(sphereCollider);
            //*** Additional Property (for Asteroid, isActive = true)
            isActive = true; 
        }
        // In Asteroid.cs

        public override void Update()
        {
            if (!isActive) return;

            // 1. Move the Asteroid (CRITICAL: This applies the Velocity)
            base.Update();

            // 2. Screen Wrapping (Optional but recommended)
            // If asteroid goes too far right, warp it to the left, etc.
            if (Transform.Position.X > GameConstants.PlayfieldSizeX)
                Transform.LocalPosition -= Vector3.Right * 2 * GameConstants.PlayfieldSizeX;

            if (Transform.Position.X < -GameConstants.PlayfieldSizeX)
                Transform.LocalPosition += Vector3.Right * 2 * GameConstants.PlayfieldSizeX;

            if (Transform.Position.Z > GameConstants.PlayfieldSizeY)
                Transform.LocalPosition -= Vector3.Forward * 2 * GameConstants.PlayfieldSizeY;

            if (Transform.Position.Z < -GameConstants.PlayfieldSizeY)
                Transform.LocalPosition += Vector3.Forward * 2 * GameConstants.PlayfieldSizeY;

            // Fix for CS1612: Cannot modify the return value of 'Rigidbody.Velocity' because it is not a variable
            Vector3 velocity = Rigidbody.Velocity;
            velocity.X += (float)Math.Sin(Time.TotalGameTime.TotalSeconds * 2) * 5f;
            Rigidbody.Velocity = velocity;
        }
        public override void Draw()
        {
            if (isActive) base.Draw();
        }
    }
}
