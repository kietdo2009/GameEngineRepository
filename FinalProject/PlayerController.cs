using CPI311.GameEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace FinalProject
{
    public class PlayerController : GameObject
    {
        public int Tokens { get; set; } = 0;
        public Camera Camera { get; set; }
        public TerrainRenderer Terrain { get; set; }
        public List<TerrainRenderer> Terrains { get; set; } = new List<TerrainRenderer>();
        public List<Wall> Walls { get; set; } = new List<Wall>();
        public Collider Collider { get; set; } // Add this
        public int TeleportsUsed { get; set; } = 0;


        // Settings
        public float MoveSpeed { get; set; } = 15f;
        public float MouseSensitivity { get; set; } = 0.005f;
        public float Gravity { get; set; } = 20f; // Gravity strength
        public float FallLimit { get; set; } = -20f; // Y-Level to respawn at

        // Rotation & State
        private float pitch = 0f;
        private float yaw = 0f;
        private Vector2 previousMousePos;
        private Vector3 spawnPoint; // Where to respawn

        private GraphicsDevice graphicsDevice;

        public PlayerController(TerrainRenderer terrain, Camera camera, ContentManager content, GraphicsDevice graphicsDevice, Light light) : base()
        {
            Terrain = terrain;
            Camera = camera;
            this.graphicsDevice = graphicsDevice;
            Tokens = 0;
            Terrains.Add(terrain);

            // Define a safe spawn point (matches your Tutorial.cs load position)
            spawnPoint = new Vector3(0, 5, 0);

            // 1. Setup Rigidbody
            Rigidbody rigidbody = new Rigidbody();
            rigidbody.Transform = Transform;
            rigidbody.Mass = 1;
            Add<Rigidbody>(rigidbody);

            // 2. Setup Collision
            SphereCollider collider = new SphereCollider();
            collider.Radius = 1.0f;
            collider.Transform = Transform;
            Add<Collider>(collider);

            this.Collider = collider; 
            Camera = camera;
            this.graphicsDevice = graphicsDevice;

            previousMousePos = InputManager.GetMousePosition();
        }

        public override void Update()
        {
            // --- 1. MOUSE LOOK ---
            Vector2 currentMousePos = InputManager.GetMousePosition();
            Vector2 mouseDelta = currentMousePos - previousMousePos;

            yaw -= mouseDelta.X * MouseSensitivity;
            this.Transform.Rotation = Quaternion.CreateFromAxisAngle(Vector3.Up, yaw);

            pitch -= mouseDelta.Y * MouseSensitivity;
            pitch = MathHelper.Clamp(pitch, -1.5f, 1.5f);

            Camera.Transform.LocalPosition = this.Transform.Position + Vector3.Up * 2f;
            Camera.Transform.Rotation = Quaternion.CreateFromYawPitchRoll(yaw, pitch, 0);

            previousMousePos = currentMousePos;

            // --- 2. MOVEMENT & GRAVITY ---
            Vector3 movement = Vector3.Zero;
            if (InputManager.IsKeyDown(Keys.W)) movement += this.Transform.Forward;
            if (InputManager.IsKeyDown(Keys.S)) movement += this.Transform.Backward;
            if (InputManager.IsKeyDown(Keys.A)) movement += this.Transform.Left;
            if (InputManager.IsKeyDown(Keys.D)) movement += this.Transform.Right;

            if (movement != Vector3.Zero) movement.Normalize();

            // Calculate Vertical Velocity (Gravity)
            // We preserve the current Y velocity and subtract gravity over time
            float newYVelocity = this.Rigidbody.Velocity.Y - (Gravity * Time.ElapsedGameTime);

            // Apply Velocity (X and Z move by keys, Y moves by gravity)
            this.Rigidbody.Velocity = new Vector3(movement.X * MoveSpeed, newYVelocity, movement.Z * MoveSpeed);

            // --- 3. UPDATED TERRAIN BOUNDS CHECK ---
            Vector3 playerPos = this.Transform.LocalPosition;
            bool onGround = false;
            float groundHeight = -999f;

            // Check EVERY terrain in the list
            foreach (TerrainRenderer terrain in Terrains)
            {
                // Convert Player World Position to Terrain Local Position
                // This fixes the issue where terrain2 is at (60, -10, 0)
                Vector3 localPos = playerPos - terrain.Transform.LocalPosition;

                float halfWidth = terrain.size.X / 2f;
                float halfLength = terrain.size.Y / 2f;

                // Check if player is INSIDE this specific terrain's boundaries
                if (localPos.X > -halfWidth && localPos.X < halfWidth &&
                    localPos.Z > -halfLength && localPos.Z < halfLength)
                {
                    // Get height relative to the terrain
                    float localGroundHeight = terrain.GetAltitude(localPos);

                    // Convert back to World Height
                    float worldGroundHeight = localGroundHeight + terrain.Transform.LocalPosition.Y;

                    if (playerPos.Y < worldGroundHeight)
                    {
                        playerPos.Y = worldGroundHeight;
                        this.Rigidbody.Velocity = new Vector3(this.Rigidbody.Velocity.X, 0, this.Rigidbody.Velocity.Z);
                        onGround = true;
                    }
                }
            }
            // B. Check Walls
            foreach (Wall wall in Walls)
            {
                // 1. Get the specific collider types
                BoxCollider wallCollider = wall.Collider as BoxCollider;
                SphereCollider playerCollider = this.Collider as SphereCollider;

                if (wallCollider != null && playerCollider != null)
                {
                    // 2. Create standard XNA bounding shapes to calculate the physics
                    //    (We calculate these on the fly based on current positions)

                    BoundingSphere sphere = new BoundingSphere(
                        this.Transform.Position,
                        playerCollider.Radius
                    );

                    // Calculate Box Min/Max based on Wall Position and Size
                    Vector3 halfSize = wallCollider.Size * wall.Transform.LocalScale / 2f;
                    BoundingBox box = new BoundingBox(
                        wall.Transform.Position - halfSize,
                        wall.Transform.Position + halfSize
                    );

                    // 3. Check for Intersection
                    if (sphere.Intersects(box))
                    {
                        // Collision detected!
                        // Check if we are falling onto it from above
                        if (this.Transform.Position.Y > wall.Transform.Position.Y)
                        {
                            // Snap to top: Wall Y + Half Height + Player Radius (approx)
                            float wallTop = wall.Transform.Position.Y - halfSize.Y;
                            if (groundHeight < wallTop) groundHeight = wallTop;
                            onGround = true;
                        }
                    }
                }
            }

            // Apply the position update
            this.Transform.LocalPosition = playerPos;

            // --- 4. RESPAWN CHECK ---
            if (this.Transform.LocalPosition.Y < FallLimit)
            {
                // Reset Position
                this.Transform.LocalPosition = spawnPoint;
                // Reset Velocity (so we don't keep falling instantly)
                this.Rigidbody.Velocity = Vector3.Zero;
                TeleportsUsed = 0;
            }

            // Uncomment to lock mouse
            Mouse.SetPosition(graphicsDevice.Viewport.Width / 2, graphicsDevice.Viewport.Height / 2);
            previousMousePos = new Vector2(graphicsDevice.Viewport.Width / 2, graphicsDevice.Viewport.Height / 2);

            // --- TELEPORT INPUT ---
            if (InputManager.IsMousePressed(0)) // Left Click
            {
                // Rule: Can only teleport if usage is less than tokens collected
                if (TeleportsUsed < Tokens)
                {
                    Vector3? target = GetClickPosition();
                    if (target.HasValue)
                    {
                        SoundManager.Play("teleport");
                        this.Transform.LocalPosition = target.Value + Vector3.Up;
                        this.Rigidbody.Velocity = Vector3.Zero;
                        TeleportsUsed++; // Count the usage
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Not enough tokens to teleport!");
                }
            }

            base.Update();
        }

        private Vector3? GetClickPosition()
        {
            Vector2 mousePos = InputManager.GetMousePosition();
            Viewport viewport = graphicsDevice.Viewport;

            Vector3 nearPoint = viewport.Unproject(new Vector3(mousePos, 0), Camera.Projection, Camera.View, Matrix.Identity);
            Vector3 farPoint = viewport.Unproject(new Vector3(mousePos, 1), Camera.Projection, Camera.View, Matrix.Identity);
            Vector3 direction = Vector3.Normalize(farPoint - nearPoint);
            Ray ray = new Ray(nearPoint, direction);

            // Logic to support clicking on multiple levels
            Plane topPlane = new Plane(Vector3.Up, 0f);
            float? distTop = ray.Intersects(topPlane);

            Plane bottomPlane = new Plane(Vector3.Up, 10f); // For the lower island
            float? distBot = ray.Intersects(bottomPlane);

            Vector3 pointTop = distTop.HasValue ? ray.Position + direction * distTop.Value : Vector3.Zero;
            Vector3 pointBot = distBot.HasValue ? ray.Position + direction * distBot.Value : Vector3.Zero;

            // If clicking the void/gap area, prioritize the bottom plane
            if (distTop.HasValue && pointTop.X > 20) return pointBot;
            if (distTop.HasValue) return pointTop;

            return null;
        }
    }
}