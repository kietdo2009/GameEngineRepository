using CPI311.GameEngine;
using FinalProject;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace FinalProject
{
    public class FinalProject : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // Scene Management
        Dictionary<string, GameScene> scenes;
        GameScene currentScene;

        public FinalProject()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.GraphicsProfile = GraphicsProfile.HiDef;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            Time.Initialize();
            InputManager.Initialize();
            ScreenManager.Initialize(_graphics);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            scenes = new Dictionary<string, GameScene>();
            SoundManager.Load(Content);

            // Initialize Scenes
            scenes.Add("Menu", new MainMenu(this));
            scenes.Add("Tutorial", new Tutorial(this));
            scenes.Add("Level2", new Level2(this));
            scenes.Add("Level3", new Level3(this));
            scenes.Add("Credits", new Credits(this));


            // Load Content for ALL scenes
            foreach (var scene in scenes.Values)
                scene.LoadContent();

            // Start at Menu
            currentScene = scenes["Menu"];
        }

        // Helper to switch scenes from other classes
        public void SwitchScene(string sceneName)
        {
            if (scenes.ContainsKey(sceneName))
            {
                currentScene = scenes[sceneName];
            }
        }

        protected override void Update(GameTime gameTime)
        {
            Time.Update(gameTime);
            InputManager.Update();

            if (currentScene != null)
                currentScene.Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            if (currentScene != null)
                currentScene.Draw();

            base.Draw(gameTime);
        }
    }
}