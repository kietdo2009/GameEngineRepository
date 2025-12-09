using CPI311.GameEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace FinalProject
{
    public class Wall : GameObject
    {
        // Add this Property so we can access it later
        public Collider Collider { get; set; }

        public Wall(ContentManager content, Camera camera, GraphicsDevice graphicsDevice, Light light) : base()
        {
            Model model = content.Load<Model>("wall");
            Texture2D texture;
            try { texture = content.Load<Texture2D>("cobblestone"); }
            catch { texture = content.Load<Texture2D>("Square"); }

            Renderer renderer = new Renderer(model, Transform, camera, content, graphicsDevice, light, 1, "SimpleShading", 20f, texture);
            Add<Renderer>(renderer);

            BoxCollider boxCollider = new BoxCollider();
            boxCollider.Size = 1f; 
            boxCollider.Transform = Transform;
            Add<Collider>(boxCollider);

            // SAVE the reference to the property!
            this.Collider = boxCollider;
        }
    }
}