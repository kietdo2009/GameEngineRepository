using CPI311.GameEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace FinalProject
{
    public class Flag : GameObject
    {
        private Transform poleTransform;
        private Transform clothTransform;

        private Renderer poleRenderer;
        private Renderer clothRenderer;

        public Flag(ContentManager content, Camera camera, GraphicsDevice graphicsDevice, Light light) : base()
        {
            Model model = content.Load<Model>("Sphere");
            Texture2D texture = content.Load<Texture2D>("Square");

            // --- 1. SETUP THE POLE ---
            poleTransform = new Transform();
            poleRenderer = new Renderer(model, poleTransform, camera, content, graphicsDevice, light, 1, "SimpleShading", 20f, texture);

            poleRenderer.Material.Diffuse = Color.Gray.ToVector3();
            poleRenderer.Material.Specular = Color.White.ToVector3();

            // NOTE: We do NOT add them to the base list using Add<Renderer> anymore.
            // We will manage them manually to ensure both draw.

            // --- 2. SETUP THE CLOTH ---
            clothTransform = new Transform();
            clothRenderer = new Renderer(model, clothTransform, camera, content, graphicsDevice, light, 1, "SimpleShading", 20f, texture);

            clothRenderer.Material.Diffuse = Color.Red.ToVector3();
            clothRenderer.Material.Specular = Color.Red.ToVector3();
        }

        public override void Update()
        {
            // Sync Pole Position (Centered)
            poleTransform.LocalPosition = this.Transform.LocalPosition;
            poleTransform.Rotation = this.Transform.Rotation;
            poleTransform.LocalScale = new Vector3(0.1f, 4.0f, 0.1f);

            // Sync Cloth Position (Offset near top)
            Vector3 offset = new Vector3(0, 1.5f, 0);

            // Waving Animation
            float wave = (float)Math.Sin(Time.TotalGameTime.TotalSeconds * 5.0f) * 0.2f;
            Quaternion waveRot = Quaternion.CreateFromAxisAngle(Vector3.Up, -MathHelper.PiOver2 + wave);

            clothTransform.LocalPosition = this.Transform.LocalPosition + offset;
            clothTransform.Rotation = this.Transform.Rotation * waveRot;

            // Offset cloth so edge touches pole
            clothTransform.LocalPosition += clothTransform.Forward * 0.8f; // Adjusted to 0.8 to fit better
            clothTransform.LocalScale = new Vector3(0.1f, 1.0f, 1.5f);

            base.Update();
        }

        public override void Draw()
        {
            // Manually draw both parts!
            if (poleRenderer != null) poleRenderer.Draw();
            if (clothRenderer != null) clothRenderer.Draw();

            // We don't call base.Draw() because we handled the drawing here.
        }
    }
}