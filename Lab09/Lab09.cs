using CPI311.GameEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
namespace Lab09
{
    public class Lab09 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private AStarSearch search;
        private List<Vector3> path;
        private int size;
        Random random = new Random();
        Camera camera;
        List<Camera> cameras;
        Model sphere;
        Model cube;

        public Lab09()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            //LAB 9 **********************
            Model cube;
            Model sphere;
            AStarSearch search;
            List<Vector3> path;
            size = 10;
            cameras = new List<Camera>();
            //*****************************
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            Time.Initialize();
            InputManager.Initialize();
            ScreenManager.Initialize(_graphics);
            
            search = new AStarSearch(size, size); // size of grid
            foreach (AStarNode node in search.Nodes)
                if (random.NextDouble() < 0.2)
                    search.Nodes[random.Next(size), random.Next(size)].Passable = false;
            search.Start = search.Nodes[0, 0];
            search.Start.Passable = true;
            search.End = search.Nodes[size - 1, size - 1];
            search.End.Passable = true;
            
            search.Search(); // A search is made here.
            path = new List<Vector3>();
            AStarNode current = search.End;
            while (current != null)
            {
                path.Insert(0, current.Position);
                current = current.Parent;
            }


            base.Initialize();
        }

        protected override void LoadContent()
        {
            sphere = Content.Load<Model>("Sphere");
            cube = Content.Load<Model>("cube");
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            // *** Setup main camera (left side of screen) ***
            camera = new Camera();
            camera.Transform = new Transform();
            camera.Transform.LocalPosition = Vector3.Backward * 5;
            camera.Position = new Vector2(0f, 0f);    // Start at top-left
            camera.Size = new Vector2(0.5f, 1f);      // 50% width, 100% height
            camera.AspectRatio = camera.Viewport.AspectRatio;
            // TODO: use this.Content to load your game content here

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            Time.Update(gameTime);
            InputManager.Update();
            // TODO: Add your update logic here
            if (InputManager.IsKeyPressed(Keys.Space))
            {
                while(!(search.Start = search.Nodes[random.Next(search.Cols), random.Next(search.Rows)]).Passable) ;
                search.Search();
                path = new List<Vector3>();
                AStarNode current = search.End;
                while (current != null)
                {
                    path.Insert(0, current.Position);
                    current = current.Parent;
                }
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            foreach (AStarNode node in search.Nodes)
                if (!node.Passable)
                    cube.Draw(Matrix.CreateScale(0.5f, 0.05f, 0.5f) *
                    Matrix.CreateTranslation(node.Position), camera.View, camera.Projection);
            foreach (Vector3 position in path)
                sphere.Draw(Matrix.CreateScale(0.1f, 0.1f, 0.1f) *
                Matrix.CreateTranslation(position), camera.View, camera.Projection);
            base.Draw(gameTime);
        }
    }
}
