using CPI311.GameEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace FinalProject
{
    public class TowerVisual : GameObject
    {
        public TowerVisual(ContentManager content, Camera camera, GraphicsDevice graphicsDevice, Light light, string textureName) : base()
        {
            Model model = content.Load<Model>("tower");
            Texture2D texture;

            // --- SMART TEXTURE LOADING ---
            // 1. Try to load the name exactly as typed (e.g. "planks")
            try
            {
                texture = content.Load<Texture2D>(textureName);
            }
            catch
            {
                // 2. If that fails, try looking in the "Textures" folder (e.g. "Textures/planks")
                try
                {
                    texture = content.Load<Texture2D>("Textures/" + textureName);
                }
                catch
                {
                    // 3. If BOTH fail, load the fallback "Square" so the game doesn't crash
                    texture = content.Load<Texture2D>("Square");
                }
            }
            // -----------------------------

            Renderer renderer = new Renderer(model, Transform, camera, content, graphicsDevice, light, 1, "SimpleShading", 20f, texture);

            renderer.Material.Specular = Vector3.Zero;

            // 2. Set Diffuse to Pure White
            // This tells the engine: "Show the texture's original colors exactly as they are"
            renderer.Material.Diffuse = Vector3.One;

            // 3. Set Ambient to Dark Grey
            // This ensures the shadowed sides aren't pitch black, but not too bright either
            renderer.Material.Ambient = new Vector3(0.2f, 0.2f, 0.2f);

            Add<Renderer>(renderer);
        }
    }
}