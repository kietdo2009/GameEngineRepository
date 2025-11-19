using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace CPI311.GameEngine
{
    public class TerrainRenderer : Component, IRenderable
    {
        // *** 1. ADD A FIELD TO STORE THE GRAPHICS DEVICE ***
        private GraphicsDevice graphicsDevice;

        private VertexPositionTexture[] Vertices { get; set; }
        private int[] Indices { get; set; }

        private float[] heights;
        private Texture2D HeightMap { get; set; }
        public Texture2D NormalMap { get; set; }
        private Vector2 size;

        // *** 2. ADD 'GraphicsDevice' TO THE CONSTRUCTOR ***
        public TerrainRenderer(Texture2D texture, Vector2 size, Vector2 res, GraphicsDevice graphicsDevice)
        {
            // *** 3. STORE THE GRAPHICS DEVICE ***
            this.graphicsDevice = graphicsDevice;

            HeightMap = texture;
            this.size = size;

            CreateHeights();

            int rows = (int)res.Y + 1;
            int cols = (int)res.X + 1;

            Vector3 offset = new Vector3(-size.X / 2, 0, -size.Y / 2);
            float stepX = size.X / res.X;
            float stepZ = size.Y / res.Y;

            Vertices = new VertexPositionTexture[rows * cols];
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    Vector2 texCoords = new Vector2(c / res.X, r / res.Y);
                    float height = GetHeight(texCoords);

                    Vertices[r * cols + c] = new VertexPositionTexture(
                        offset + new Vector3(c * stepX, height, r * stepZ),
                        texCoords);
                }
            }

            Indices = new int[(rows - 1) * (cols - 1) * 6];
            int index = 0;
            for (int r = 0; r < rows - 1; r++)
            {
                for (int c = 0; c < cols - 1; c++)
                {
                    Indices[index++] = r * cols + c;
                    Indices[index++] = r * cols + c + 1;
                    Indices[index++] = (r + 1) * cols + c;

                    Indices[index++] = (r + 1) * cols + c;
                    Indices[index++] = r * cols + c + 1;
                    Indices[index++] = (r + 1) * cols + c + 1;
                }
            }
        }

        // ... (CreateHeights and GetHeight methods are unchanged) ...
        private void CreateHeights()
        {
            Color[] data = new Color[HeightMap.Width * HeightMap.Height];
            HeightMap.GetData<Color>(data);
            heights = new float[HeightMap.Width * HeightMap.Height];
            for (int i = 0; i < heights.Length; i++)
                heights[i] = data[i].G / 255f;
        }

        public float GetHeight(Vector2 tex)
        {
            tex = Vector2.Clamp(tex, Vector2.Zero, Vector2.One) *
                  new Vector2(HeightMap.Width - 1, HeightMap.Height - 1);

            int x = (int)tex.X; float u = tex.X - x;
            int y = (int)tex.Y; float v = tex.Y - y;

            int x1 = Math.Min(x + 1, HeightMap.Width - 1);
            int y1 = Math.Min(y + 1, HeightMap.Height - 1);

            return heights[y * HeightMap.Width + x] * (1 - u) * (1 - v) +
                   heights[y * HeightMap.Width + x1] * u * (1 - v) +
                   heights[y1 * HeightMap.Width + x] * (1 - u) * v +
                   heights[y1 * HeightMap.Width + x1] * u * v;
        }


        public float GetAltitude(Vector3 position)
        {
            position = Vector3.Transform(position, Matrix.Invert(Transform.World));

            if (position.X > -size.X / 2 && position.X < size.X / 2 &&
                position.Z > -size.Y / 2 && position.Z < size.Y / 2)
            {
                Vector2 texCoords = new Vector2(
                    (position.X + size.X / 2) / size.X,
                    (position.Z + size.Y / 2) / size.Y
                );

                return GetHeight(texCoords) * Transform.LocalScale.Y;
            }
            return -1;
        }

        public void Draw()
        {
            if (Vertices == null || Indices == null)
                return;

            this.graphicsDevice.DrawUserIndexedPrimitives
                <VertexPositionTexture>(PrimitiveType.TriangleList,
                Vertices, 0, Vertices.Length, // Fixed typo here
                Indices, 0, Indices.Length / 3);
        }
    }
}