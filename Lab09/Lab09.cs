using CPI311.GameEngine;
using Microsoft.VisualBasic.ApplicationServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Lab09
{
    public class Lab09 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // *** Lab 9 *****
        Model cube;
        Model sphere;
        AStarSearch search;
        List<Vector3> path;

        int size = 100;
        Random random = new Random();
        Camera camera;
        //*****************


        public Lab09()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // ******************************
            Time.Initialize();
            InputManager.Initialize();
            ScreenManager.Initialize(_graphics);
            //********************************

            search = new AStarSearch(size, size);

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
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            cube = Content.Load<Model>("cube");
            sphere = Content.Load<Model>("Sphere");


            camera = new Camera();
            camera.Transform = new Transform();
            camera.Transform.LocalPosition =  
                Vector3.Right * 50 + Vector3.Backward * 50 + Vector3.Up * 70;
            camera.Transform.Rotate(Vector3.Right, -MathHelper.PiOver2);
            camera.Position = new Vector2(0, 0);
            camera.Size = new Vector2(1.0f, 1.0f);
            camera.AspectRatio = camera.Viewport.AspectRatio;
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // **********************
            Time.Update(gameTime);
            InputManager.Update();
            //************************

            if (InputManager.IsKeyPressed(Keys.Space))
            {
                // *** Set the start and end node randomly
                while (!(search.Start = search.Nodes[
                    random.Next(search.Cols), random.Next(search.Rows)]).Passable) ;
                while (!(search.End = search.Nodes[
                    random.Next(search.Cols), random.Next(search.Rows)]).Passable) ;
                // *** Search again 
                search.Search(); // A search is made here.
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


            foreach (AStarNode node in search.Nodes)
                if (!node.Passable)
                    cube.Draw(Matrix.CreateScale(1f, 1f, 1f) *
                       Matrix.CreateTranslation(node.Position), camera.View, camera.Projection);

            foreach (Vector3 position in path)
                sphere.Draw(Matrix.CreateScale(0.5f, 0.5f, 0.5f) *
                     Matrix.CreateTranslation(position), camera.View, camera.Projection);

            base.Draw(gameTime);
        }
    }
}
