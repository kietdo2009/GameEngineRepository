using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CPI311.GameEngine
{
    public class Camera : Component // <-- Component is added at Lab6
    { 
        // *** Properties ***
        public float FieldOfView { get; set; }  // For Projection Matrix
        public float AspectRatio { get; set; } // For Projection Matrix
        public float NearPlane { get; set; } // For Projection Matrix
        public float FarPlane { get; set; } // For Projection Matrix
        //Lab 8******************************
        public Vector2 Position { get; set; } //mouse positiscreen lefttopcorner
        public Vector2 Size { get; set; } // Screen Size + 1
        public Viewport Viewport
        {
            get
            {
                return new Viewport((int)(ScreenManager.Width * Position.X),
                (int)(ScreenManager.Height * Position.Y),
                (int)(ScreenManager.Width * Size.X),
                (int)(ScreenManager.Height * Size.Y));
            }
        }
        public Ray ScreenPointToWorldRay(Vector2 position)
        {
            Vector3 start = Viewport.Unproject(new Vector3(position, 0),
            Projection, View, Matrix.Identity);
            Vector3 end = Viewport.Unproject(new Vector3(position, 1),
            Projection, View, Matrix.Identity);
            return new Ray(start, end - start);
        }
        //**************************************
        public Transform Transform { get; set; } // For View Matrix

        public Matrix Projection
        {
            get { return Matrix.CreatePerspectiveFieldOfView(
                FieldOfView, AspectRatio, NearPlane, FarPlane); }
        }
        public Matrix View
        {
            get { return Matrix.CreateLookAt(
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
