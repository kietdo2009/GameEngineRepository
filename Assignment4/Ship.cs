using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using CPI311.GameEngine;


namespace Assignment4
{
    public class Ship : GameObject
    {
        public Model Model;
        public Matrix[] Transforms;

        public Vector3 Position = Vector3.Zero;

        public Vector3 Velocity = Vector3.Zero;

        public Matrix RotationMatrix = Matrix.Identity;
        public float rotation;

        public float Rotation
        {
            get { return rotation; }
            set
            {
                float newVal = value;
                while (newVal >= MathHelper.TwoPi)
                    newVal -= MathHelper.TwoPi;
                while (newVal < 0)
                {
                    newVal += MathHelper.TwoPi;
                }

                if (rotation != newVal)
                {
                    rotation = newVal;
                    RotationMatrix = Matrix.CreateRotationY(rotation);
                }
            }
        }
        public void Update(GamePadState controllerState)
        {
            Rotation -= controllerState.ThumbSticks.Left.X * 0.1f;

            Velocity += RotationMatrix.Forward * 1.0f * controllerState.Triggers.Right;

        }
    }
}
