using CPI311.GameEngine;
using FinalProject;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct2D1;
using System.Collections.Generic;

namespace FinalProject
{
    public class MainMenu : GameScene
    {
        SpriteFont font;
        List<GUIElement> guiElements;
        Texture2D background;

        public MainMenu(Game game) : base(game)
        {
            guiElements = new List<GUIElement>();
        }

        public override void LoadContent()
        {
            font = game.Content.Load<SpriteFont>("Font");
            Texture2D guiTexture = game.Content.Load<Texture2D>("Square");
            background = game.Content.Load<Texture2D>("Square"); // Or a specific Menu BG

            // --- Play Button ---
            Button playButton = new Button();
            playButton.Texture = guiTexture;
            playButton.Text = "Start Tutorial";
            playButton.Bounds = new Rectangle(350, 200, 100, 50);
            playButton.Action += (x) => ((FinalProject)game).SwitchScene("Tutorial");

            // --- Exit Button ---
            Button exitButton = new Button();
            exitButton.Texture = guiTexture;
            exitButton.Text = "Exit";
            exitButton.Bounds = new Rectangle(350, 300, 100, 50);
            exitButton.Action += (x) => game.Exit();

            guiElements.Add(playButton);
            guiElements.Add(exitButton);
        }

        public override void Update()
        {
            game.IsMouseVisible = true; // Always show mouse in menu
            foreach (GUIElement element in guiElements)
                element.Update();
        }

        public override void Draw()
        {
            game.GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();

            // Draw Title
            spriteBatch.DrawString(font, "TeleToken", new Vector2(350, 100), Color.White);

            foreach (GUIElement element in guiElements)
                element.Draw(spriteBatch, font);

            spriteBatch.End();
        }
    }
}