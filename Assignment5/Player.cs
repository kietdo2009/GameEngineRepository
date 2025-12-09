using CPI311.GameEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Assignment5
{
    public class Player : GameObject
    {
        public TerrainRenderer Terrain { get; set; }

        public Player(TerrainRenderer terrain, ContentManager Content, Camera camera,
                      GraphicsDevice graphicsDevice, Light light) : base()
        {
            Terrain = terrain;

            
            Rigidbody rigidbody = new Rigidbody();
            rigidbody.Transform = Transform;
            rigidbody.Mass = 1;
            Add<Rigidbody>(rigidbody);

            // Add a Sphere renderer so we can see the player
            // We use a simple sphere model for the player representation
            Texture2D texture = Content.Load<Texture2D>("Square"); 
            Renderer renderer = new Renderer(Content.Load<Model>("Sphere"), Transform, camera, Content, graphicsDevice, light, 1, "SimpleShading", 20f, texture);
            Add<Renderer>(renderer);

            // Initialize position safely on the terrain
            this.Transform.LocalPosition = new Vector3(0, 0, 0);
        }

        public override void Update()
        {
            
            Vector3 newPosition = this.Transform.LocalPosition;

            // Move Forward
            if (InputManager.IsKeyDown(Keys.W))
                newPosition += this.Transform.Forward * Time.ElapsedGameTime * 10f; // Speed 10

            // Move Backward
            if (InputManager.IsKeyDown(Keys.S))
                newPosition -= this.Transform.Forward * Time.ElapsedGameTime * 10f;

            // Rotate Left
            if (InputManager.IsKeyDown(Keys.A))
                this.Transform.Rotate(Vector3.Up, Time.ElapsedGameTime * 2f);

            // Rotate Right
            if (InputManager.IsKeyDown(Keys.D))
                this.Transform.Rotate(Vector3.Up, -Time.ElapsedGameTime * 2f);

            
            float newAltitude = Terrain.GetAltitude(newPosition);

            // Assuming walls are higher than 1.0f or 0.5f. 
            // If it is a valid path (low altitude), allow the move.
            if (newAltitude < 0.5f)
            {
                this.Transform.LocalPosition = newPosition;
            }

            
            this.Transform.LocalPosition = new Vector3(
                this.Transform.LocalPosition.X,
                Terrain.GetAltitude(this.Transform.LocalPosition),
                this.Transform.LocalPosition.Z) + Vector3.Up; // + Vector3.Up lifts player slightly above ground

            base.Update();
        }

    }
}