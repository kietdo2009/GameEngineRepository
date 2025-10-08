using Microsoft.Xna.Framework;

namespace CPI311.GameEngine
{
    public class Camera: Component
    {
        public float FieldOfView { get; set; } // Projection Matrix
        public float AspectRatio { get; set; }
        public float NearPlane { get; set; }
        public float FarPlane { get; set; }
        public Transform Transform { get; set; }//View atrix

        //Constructor ***********************************

        public Matrix Projection
        {
            get { return Matrix.CreatePerspectiveFieldOfView(FieldOfView, AspectRatio, NearPlane, FarPlane); }
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
        public Camera()
        {

            FieldOfView = MathHelper.PiOver2;
            AspectRatio = 1.33f;
            NearPlane = .1f;
            FarPlane = 100f;
            Transform = new Transform(); //Need to initialize in Game Class
        }
    }
}
