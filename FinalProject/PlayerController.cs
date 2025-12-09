using CPI311.GameEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FinalProject
{
    public class PlayerController : GameObject
    {
        public int Tokens { get; set; } = 0; // Resource count
        public Camera Camera { get; set; }
        public TerrainRenderer Terrain { get; set; }

        // Settings
        public float MoveSpeed { get; set; } = 15f;
        public float MouseSensitivity { get; set; } = 0.005f;

        // Rotation Tracking
        private float pitch = 0f; // Up/Down
        private float yaw = 0f;   // Left/Right
        private Vector2 previousMousePos;

        // Add a field to store the GraphicsDevice reference
        private GraphicsDevice graphicsDevice;

        public PlayerController(TerrainRenderer terrain, Camera camera, ContentManager content, GraphicsDevice graphicsDevice, Light light) : base()
        {
            Terrain = terrain;
            Camera = camera;
            this.graphicsDevice = graphicsDevice; // Store the reference

            Tokens = 0;

            // 1. Setup Physical Body (Rigidbody)
            Rigidbody rigidbody = new Rigidbody();
            rigidbody.Transform = Transform;
            rigidbody.Mass = 1;
            Add<Rigidbody>(rigidbody);

            // 2. Setup Collision (So we don't walk through walls)
            SphereCollider collider = new SphereCollider();
            collider.Radius = 1.0f;
            collider.Transform = Transform;
            Add<Collider>(collider);

            // 3. Initialize rotation based on current camera
            previousMousePos = InputManager.GetMousePosition();
        }

        public override void Update()
        {
            // --- 1. MOUSE LOOK (Rotation) ---
            Vector2 currentMousePos = InputManager.GetMousePosition();
            Vector2 mouseDelta = currentMousePos - previousMousePos; // How much moved?

            // Standard FPS requires locking mouse to center, but since InputManager 
            // might not support that easily, we just use the delta.
            // Note: For a true FPS, you usually reset mouse to center screen here.

            // Adjust Yaw (Left/Right) - Rotates the PLAYER BODY
            yaw -= mouseDelta.X * MouseSensitivity;
            this.Transform.Rotation = Quaternion.CreateFromAxisAngle(Vector3.Up, yaw);

            // Adjust Pitch (Up/Down) - Rotates the CAMERA only
            pitch -= mouseDelta.Y * MouseSensitivity;
            pitch = MathHelper.Clamp(pitch, -1.5f, 1.5f); // Stop head from spinning 360 vertically

            // Apply rotation to Camera (Camera is a child of Player effectively)
            Camera.Transform.LocalPosition = this.Transform.Position + Vector3.Up * 2f; // Eye level
            Camera.Transform.Rotation = Quaternion.CreateFromYawPitchRoll(yaw, pitch, 0);

            previousMousePos = currentMousePos;

            // --- 2. WASD MOVEMENT ---
            Vector3 movement = Vector3.Zero;

            // Move relative to where we are facing
            if (InputManager.IsKeyDown(Keys.W)) movement += this.Transform.Forward;
            if (InputManager.IsKeyDown(Keys.S)) movement += this.Transform.Backward;
            if (InputManager.IsKeyDown(Keys.A)) movement += this.Transform.Left;
            if (InputManager.IsKeyDown(Keys.D)) movement += this.Transform.Right;

            // Normalize so diagonal movement isn't faster
            if (movement != Vector3.Zero) movement.Normalize();

            // Apply Physics Velocity (Keep Y velocity for gravity if you add jumping later)
            this.Rigidbody.Velocity = new Vector3(movement.X * MoveSpeed, this.Rigidbody.Velocity.Y, movement.Z * MoveSpeed);

            // --- 3. TERRAIN HEIGHT & BOUNDS ---
            // Keep player on the ground
            Vector3 newPos = this.Transform.LocalPosition;
            float groundHeight = Terrain.GetAltitude(newPos);

            // Simple gravity/snap-to-floor
            if (newPos.Y < groundHeight)
                newPos.Y = groundHeight;

            this.Transform.LocalPosition = newPos;
            // Reset mouse to center of screen to allow infinite rotation
            Mouse.SetPosition(graphicsDevice.Viewport.Width / 2, graphicsDevice.Viewport.Height / 2);
            previousMousePos = new Vector2(graphicsDevice.Viewport.Width / 2, graphicsDevice.Viewport.Height / 2);

            base.Update();
        }


        private Vector3? GetClickPosition()
        {
            // Get Mouse Position
            Vector2 mousePos = InputManager.GetMousePosition();
            Viewport viewport = graphicsDevice.Viewport; // Use the instance field

            // Calculate Ray from Screen to World
            Vector3 nearPoint = viewport.Unproject(new Vector3(mousePos, 0), Camera.Projection, Camera.View, Matrix.Identity);
            Vector3 farPoint = viewport.Unproject(new Vector3(mousePos, 1), Camera.Projection, Camera.View, Matrix.Identity);
            Vector3 direction = Vector3.Normalize(farPoint - nearPoint);
            Ray ray = new Ray(nearPoint, direction);

            // Intersect Ray with Ground Plane (Y = 0)
            // We use a mathematical plane because it's faster/easier than checking every terrain pixel
            Plane groundPlane = new Plane(Vector3.Up, 0f);
            float? distance = ray.Intersects(groundPlane);

            if (distance != null)
            {
                Vector3 worldPos = ray.Position + direction * distance.Value;
                return worldPos;
            }

            return null; // Clicked on the sky
        }
    }
}