using CPI311.GameEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Design;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Net;


namespace Lab11
{
    public class Lab11 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Color background = Color.Blue;
        SpriteFont Font;
        Button exitButton;
        Dictionary<String, Scene> scenes;
        Scene currentScene;
        List <GUIElement> guiElements = new List<GUIElement>();
        public class Scene
        {
            public delegate void CallMethod();
            public CallMethod Update;
            public CallMethod Draw;
            public Scene(CallMethod update, CallMethod draw)
            { Update = update; Draw = draw; }
        }

        public Lab11()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            Time.Initialize();
            InputManager.Initialize();
            ScreenManager.Initialize(_graphics);

            scenes = new Dictionary<string, Scene>();
            

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            scenes.Add("Menu", new Scene(MainMenuUpdate, MainMenuDraw));
            scenes.Add("Play", new Scene(PlayUpdate, PlayDraw));
            currentScene = scenes["Menu"];


            Texture2D texture = Content.Load<Texture2D>("Square");
            Font = Content.Load<SpriteFont>("Font");

            //exitButton = new Button();
            //exitButton.Texture = texture;
            //exitButton.Text = "Exit";
            //exitButton.Bounds = new Rectangle(50, 50, 300, 20);
            //exitButton.Action += ExitGame;

            Button fullButton = new Button();
            fullButton.Texture = texture;
            fullButton.Text = "FullScreen";
            fullButton.Bounds = new Rectangle(50, 50, 300, 20);
            fullButton.Action += FullScreen;


            CheckBox checkBox = new CheckBox();
            checkBox.Texture = texture;
            checkBox.Text = "Switch Scene";
            checkBox.Bounds = new Rectangle(50, 100, 300, 20);
            checkBox.Action += SwitchScene;
            guiElements.Add(checkBox);
            guiElements.Add(fullButton);
            //guiElements.Add(exitButton);

        }

        protected override void Update(GameTime gameTime)
        {
           // if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
              //  Exit();
            Time.Update(gameTime);
            InputManager.Update();
            currentScene.Update();
            //exitButton.Update();
            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(background);
            _spriteBatch.Begin();
            //exitButton.Draw(_spriteBatch, Font);
            _spriteBatch.End();

            currentScene.Draw();
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
        void MainMenuUpdate()
        {
            foreach (GUIElement element in guiElements)
                element.Update();
        }
        void MainMenuDraw()
        {
            _spriteBatch.Begin();
            foreach (GUIElement element in guiElements)
                element.Draw(_spriteBatch, Font);
            _spriteBatch.End();
        }
        void PlayUpdate()
        {
            if (InputManager.IsKeyReleased(Keys.Escape))
                currentScene = scenes["Menu"];
        }
        void PlayDraw()
        {

            _spriteBatch.Begin();
            _spriteBatch.DrawString(Font, "Play Mode! Press \"Esc\" to go back",
            Vector2.Zero, Color.Black);
            _spriteBatch.End();

        }
        void ExitGame(GUIElement element)
        {
            background = (background == Color.White ? Color.Blue : Color.White);
        }
        void FullScreen(GUIElement element)
        {
            ScreenManager.Setup(!ScreenManager.IsFullScreen);

        }
        void SwitchScene(GUIElement element)
        {
            // Cast the generic GUIElement back to CheckBox to access the Checked property
            CheckBox checkBox = element as CheckBox;

            if (checkBox != null)
            {
                // If the checkbox is checked, switch to "Play", otherwise switch to "Menu"
                currentScene = checkBox.Checked ? scenes["Play"] : scenes["Menu"];
            }

        }
    }
}
