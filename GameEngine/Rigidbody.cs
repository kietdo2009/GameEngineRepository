// Rigidbody.cs

using Microsoft.Xna.Framework;

namespace CPI311.GameEngine
{
    public class Rigidbody : Component, IUpdateable
    {
        public Vector3 Velocity { get; set; }
        public float Mass { get; set; }
        public Vector3 Acceleration { get; set; }
        public Vector3 Impulse { get; set; }

        public Rigidbody()
        {
            Mass = 1.0f;
            Velocity = Vector3.Zero;
            Acceleration = Vector3.Zero;
            Impulse = Vector3.Zero;
        }

        public void Update()
        {
            float elapsedGameTime = 1.0f / 60.0f; // Assuming 60 FPS

            Velocity += Acceleration * elapsedGameTime + Impulse / Mass;
            Transform.LocalPosition += Velocity * elapsedGameTime;
            Impulse = Vector3.Zero;
        }
    }
}