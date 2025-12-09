using CPI311.GameEngine;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Assignment5
{
    public class Agent : GameObject
    {
        public AStarSearch search;
        public List<Vector3> path;
        private float speed = 5f; //moving speed
        private int gridSize = 20; //grid size
        private TerrainRenderer Terrain;
        public Agent(TerrainRenderer terrain, ContentManager Content,
        Camera camera, GraphicsDevice graphicsDevice, Light light) : base()
        {
            Terrain = terrain;
            path = null;
            Rigidbody rigidbody = new Rigidbody();
            rigidbody.Transform = Transform;
            rigidbody.Mass = 1;
            Add<Rigidbody>(rigidbody);
            // Add a Sphere renderer so we can see the agent
            Texture2D texture = Content.Load<Texture2D>("Square");
            Renderer renderer = new Renderer(Content.Load<Model>("Sphere"), Transform, camera, Content, graphicsDevice, light, 1, "SimpleShading", 20f, texture);
            Add<Renderer>(renderer);

            // Initialize position safely on the terrain
            this.Transform.LocalPosition = new Vector3(0, 0, 20);

            search = new AStarSearch(gridSize, gridSize);
            float gridW = Terrain.size.X / gridSize;
            float gridH = Terrain.size.Y / gridSize;
            for (int i = 0; i < gridSize; i++)
                for (int j = 0; j < gridSize; j++)
                {
                    Vector3 pos = new Vector3(gridW * i + gridW / 2 - Terrain.size.X / 2, 0, gridH * j + gridH / 2 - Terrain.size.Y / 2);
                    if (Terrain.GetAltitude(pos) > 0.01f)
                        search.Nodes[j, i].Passable = false;

                }
        }
        public override void Update()
        {
            // 1. Check if we have a path to follow
            if (path != null && path.Count > 0)
            {
                Vector3 currP = Transform.Position;
                Vector3 destP = GetGridPosition(path[0]); // Safe to access path[0] now

                // Ignore Y height for distance check (movement is 2D top-down)
                currP.Y = 0;
                destP.Y = 0;

                // Move Agent
                Vector3 direction = Vector3.Distance(currP, destP) == 0 ? Vector3.Zero : Vector3.Normalize(destP - currP);
                this.Rigidbody.Velocity = new Vector3(direction.X, 0, direction.Z) * speed;

                // 2. Check if we reached the current node
                if (Vector3.Distance(currP, destP) < 1f)
                {
                    path.RemoveAt(0); // Remove the node we just reached so we go to the next one

                    // If that was the last node, we are done
                    if (path.Count == 0)
                    {
                        path = null;
                        this.Rigidbody.Velocity = Vector3.Zero; // Stop moving
                    }
                }
            }
            else
            {
                RandomPathFinding();

                // Teleport to the new start position so we don't walk through walls to get there
                // Note: AStarNode uses (Column, Row) -> (X, Y) in Vector3 usually
                this.Transform.LocalPosition = GetGridPosition(new Vector3(search.Start.Col, search.Start.Row, 0));
            }

            // 4. Snap Y position to Terrain Height (Visuals)
            this.Transform.LocalPosition = new Vector3(
                this.Transform.LocalPosition.X,
                Terrain.GetAltitude(this.Transform.LocalPosition),
                this.Transform.LocalPosition.Z
            ) + Vector3.Up;

            Transform.Update();
            base.Update();
        }
        private Vector3 GetGridPosition(Vector3 gridPos)
        {
            float gridW = Terrain.size.X / search.Cols;
            float gridH = Terrain.size.Y / search.Rows;
            return new Vector3(
        gridW * gridPos.X + gridW / 2 - Terrain.size.X / 2,
        0,
        gridH * gridPos.Y + gridH / 2 - Terrain.size.Y / 2
    );
        }
        public void RandomPathFinding()
        {
            Random random = new Random();
            // Keep picking random spots until we find one that isn't a wall
            while (!(search.Start = search.Nodes[random.Next(search.Rows), random.Next(search.Cols)]).Passable) ;

            // Set destination to center (10, 10)
            search.End = search.Nodes[search.Rows / 2, search.Cols / 2];

            search.Search(); // Run A*

            path = new List<Vector3>();
            AStarNode current = search.End;

            while (current != null)
            {
                path.Insert(0, current.Position); // Add node to front of list
                current = current.Parent;         // Move backwards through path
            }
        }
    }
}