using CPI311.GameEngine;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework; // Add this if not present
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment4
{
    public class Asteroid : GameObject
    {
        bool isActive;

    public Asteroid(ContentManager Content, Camera camera, GraphicsDevice graphicsDevice, Light light) : base()
    {
        // Add Rigidbody, Collider, Renderer components
        Rigidbody rigidbody = Add<Rigidbody>();
        Collider collider = Add<Collider>();
        Renderer renderer = Add<Renderer>();

        // Example setup (customize as needed)
        renderer.g = graphicsDevice;
        renderer.Camera = camera;
        renderer.Light = light;

        isActive = true;
    }
        public override void Update()
        {
            //... move back to the space if it is out of game space
        }
        public override void Draw()
        {
            if (isActive) base.Draw();
        }
    }
}
