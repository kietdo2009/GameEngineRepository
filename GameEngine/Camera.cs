using Microsoft.Xna.Framework;

namespace CPI311.GameEngine
{
    public class Camera : Component // <-- Component is added at Lab6
    {
        // *** Properties ***
        public float FieldOfView { get; set; }  // For Projection Matrix
        public float AspectRatio { get; set; } // For Projection Matrix
        public float NearPlane { get; set; } // For Projection Matrix
        public float FarPlane { get; set; } // For Projection Matrix

        public Transform Transform { get; set; } // For View Matrix

        public Matrix Projection
        {
            get
            {
                return Matrix.CreatePerspectiveFieldOfView(
                FieldOfView, AspectRatio, NearPlane, FarPlane);
            }
        }
        public Matrix View
        {
            get
            {
                return Matrix.CreateLookAt(
                Transform.LocalPosition,
                Transform.LocalPosition + Transform.Forward,
                Transform.Up);
            }
        }
        // *** Constructor ***
        public Camera()
        {
            FieldOfView = MathHelper.PiOver2;
            AspectRatio = 1.33f;
            NearPlane = 0.1f;
            FarPlane = 100f;
            Transform = null; // Need to initialize in Game Class

        }



    }
}
