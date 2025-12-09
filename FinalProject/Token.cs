using CPI311.GameEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace FinalProject
{
    public class Token : GameObject
    {
        public bool IsActive { get; set; } = true;

        public Token(ContentManager content, Camera camera, GraphicsDevice graphicsDevice, Light light) : base()
        {
            // Setup the visual model (Using Sphere for now, you can swap for a coin model later)
            Rigidbody rigidbody = new Rigidbody();
            rigidbody.Transform = Transform;
            rigidbody.Mass = 1;
            Add<Rigidbody>(rigidbody);

            Texture2D texture = content.Load<Texture2D>("Coin"); // Replace with "Coin" if you have it
            Renderer renderer = new Renderer(content.Load<Model>("Sphere"), Transform, camera, content, graphicsDevice, light, 1, "SimpleShading", 20f, texture);
            Add<Renderer>(renderer);

            // Create a collider for detection
            SphereCollider collider = new SphereCollider();
            collider.Radius = 1.0f;
            collider.Transform = Transform;
            Add<Collider>(collider);
        }

        public override void Update()
        {
            if (!IsActive) return;

            // Rotate the token to make it look fancy
            Transform.Rotate(Vector3.Up, Time.ElapsedGameTime * 2f);

            base.Update();
        }
    }
}