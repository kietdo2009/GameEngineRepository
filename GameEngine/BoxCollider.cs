// BoxCollider.cs

using Microsoft.Xna.Framework;
using System;

namespace CPI311.GameEngine
{
    public class BoxCollider : Collider
    {
        public float Size { get; set; }

        private static Vector3[] normals = {
            Vector3.Up, Vector3.Down, Vector3.Right,
            Vector3.Left,Vector3.Forward, Vector3.Backward,
        };

        private static Vector3[] vertices = {
            new Vector3(-1,-1,1), new Vector3(1,-1,1), new Vector3(1,-1,-1), new Vector3(-1,-1,-1),
            new Vector3(-1,1,1), new Vector3(1,1,1), new Vector3(1,1,-1), new Vector3(-1,1,-1),
        };

        private static int[] indices = {
            0,1,2,  0,2,3, // Down
            5,4,7,  5,7,6, // Up
            1,5,6,  1,6,2, // Right
            4,0,3,  4,3,7, // Left
            4,5,1,  4,1,0, // Front
            3,2,6,  3,6,7, // Back
        };

        public BoxCollider() { Size = 1.0f; }

        // In BoxCollider.cs

        public override bool Collides(Collider other, out Vector3 normal)
        {
            if (other is SphereCollider)
            {
                SphereCollider collider = other as SphereCollider;
                normal = Vector3.Zero;
                Vector3 accumulatedNormal = Vector3.Zero; // Use a temporary vector
                bool isColliding = false;

                // Transform sphere's world position to the box's local space
                Matrix inverseTransform = Matrix.Invert(this.Transform.World);
                Vector3 sphereLocalPos = Vector3.Transform(collider.Transform.Position, inverseTransform);

                for (int i = 0; i < 6; i++) // For each face
                {
                    for (int j = 0; j < 2; j++) // For each triangle on the face
                    {
                        int baseIndex = i * 6 + j * 3;
                        Vector3 a = vertices[indices[baseIndex]] * this.Size;
                        Vector3 b = vertices[indices[baseIndex + 1]] * this.Size;
                        Vector3 c = vertices[indices[baseIndex + 2]] * this.Size;

                        // *** FIX: Use the corrected normals array ***
                        Vector3 n = normals[i];

                        float signedDist = Vector3.Dot(sphereLocalPos - a, n);

                        if (Math.Abs(signedDist) < collider.Radius)
                        {
                            Vector3 pointOnPlane = sphereLocalPos - n * signedDist;

                            // *** FIX: A more robust point-in-triangle test ***
                            // This works regardless of the triangle's winding order.
                            float area1 = Vector3.Dot(Vector3.Cross(b - a, pointOnPlane - a), n);
                            float area2 = Vector3.Dot(Vector3.Cross(c - b, pointOnPlane - b), n);
                            float area3 = Vector3.Dot(Vector3.Cross(a - c, pointOnPlane - c), n);

                            if ((area1 >= 0 && area2 >= 0 && area3 >= 0) || (area1 <= 0 && area2 <= 0 && area3 <= 0))
                            {
                                accumulatedNormal += n;
                                isColliding = true;
                                goto EndOfLoops; // Exit loops once a collision is confirmed for simplicity
                            }
                        }
                    }
                }

            EndOfLoops:
                if (isColliding)
                {
                    // *** FIX: Only transform and normalize if a collision actually happened ***
                    normal = Vector3.TransformNormal(accumulatedNormal, this.Transform.World);
                    normal.Normalize();
                }

                return isColliding;
            }
            return base.Collides(other, out normal);
        }
    }
}