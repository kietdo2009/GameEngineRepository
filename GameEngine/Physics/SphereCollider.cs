using System;
using Microsoft.Xna.Framework;

namespace CPI311.GameEngine
{
    public class SphereCollider : Collider
    {
        public float Radius { get; set; }
        public override bool Collides(Collider other, out Vector3 normal)
        {
            if (other is SphereCollider)
            {
                SphereCollider collider = other as SphereCollider;
                if ((Transform.Position - collider.Transform.Position).LengthSquared()
                    < System.Math.Pow(Radius + collider.Radius, 2))
                {
                    normal = Vector3.Normalize
                        (Transform.Position - collider.Transform.Position);
                    return true;
                }
            }
            else if (other is BoxCollider) return other.Collides(this, out normal);

            return base.Collides(other, out normal);
        }
        public bool SweptCollides(Collider other, Vector3 otherLastPosition, Vector3 lastPosition, out Vector3 normal)
        {
            if (other is SphereCollider)
            {
                SphereCollider collider = other as SphereCollider;
                // calculate the vectors for two spheres
                Vector3 vp = Transform.Position - lastPosition;
                Vector3 vq = collider.Transform.Position - otherLastPosition;
                // calculate the A and B (refer to the white board)
                Vector3 A = lastPosition - otherLastPosition;
                Vector3 B = vp - vq;
                // calculate the a, b, and c
                float a = Vector3.Dot(B, B);
                float b = 2 * Vector3.Dot(A, B);
                float c = Vector3.Dot(A, A) - (Radius + collider.Radius) * (Radius
                + collider.Radius);
                float disc = (float)(b * b - 4.0 * a * c); // discriminant (b^2 –4ac)
            if (disc >= 0)
                {
                    float t = (float)((-b + Math.Sqrt(disc)) / 2.0 * a);
                    Vector3 p = lastPosition + t * vp;
                    Vector3 q = otherLastPosition + t * vq;
                    Vector3 intersect = Vector3.Lerp(
                    p, q, this.Radius / (this.Radius + collider.Radius));
                    normal = Vector3.Normalize(p - q);
                    return true;
                }
            }
            else if (other is BoxCollider)
                return other.Collides(this, out normal);
            return base.Collides(other, out normal);
        }
        public override float? Intersects(Ray ray)
        {
            // Transform the ray from world space to this collider's object space
            Matrix worldInv = Matrix.Invert(Transform.World);

            ray.Position = Vector3.Transform(ray.Position, worldInv);
            ray.Direction = Vector3.TransformNormal(ray.Direction, worldInv);

            // This part is crucial for spheres!
            float length = ray.Direction.Length();
            ray.Direction /= length; // Normalize the direction

            // Now do the intersection test in object space
            float? p = new BoundingSphere(Vector3.Zero, Radius).Intersects(ray);

            // If it hit, we must scale the resulting distance 'p'
            // back by the length we normalized it by.
            if (p != null)
                return (float)p; // Rescale the intersection distance

            return null;
        }

    }
}
