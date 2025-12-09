using CPI311.GameEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace FinalProject
{
    public class Credits : GameScene
    {
        SpriteFont font;
        float scrollY = 0;
        float scrollSpeed = 40f; // Pixels per second
        List<string> lines;

        public Credits(Game game) : base(game)
        {
            lines = new List<string>();
        }

        public override void LoadContent()
        {
            font = game.Content.Load<SpriteFont>("Font");

            // --- THE CREDITS LIST ---
            lines.Clear();
            lines.Add("TELETOKEN");
            lines.Add(""); 
            lines.Add("A Game By");
            lines.Add("Kiet Do");
            lines.Add("");
            lines.Add("--- Art & Assets ---");
            lines.Add("Kenny Art (Kenney.nl)");
            lines.Add("Unity FBX Exporter");
            lines.Add("");
            lines.Add("--- Engine ---");
            lines.Add("CPI311 Game Engine");
            lines.Add("MonoGame Framework");
            lines.Add("");
            lines.Add("--- Special Thanks ---");
            lines.Add("Professor Yoshi");
            lines.Add("TA Von");
            lines.Add("");
            lines.Add("");
            lines.Add("Thanks for playing!");

            // Start the text below the screen
            scrollY = -game.GraphicsDevice.Viewport.Height;
        }

        public override void Update()
        {
            // Scroll the text UP
            scrollY += scrollSpeed * Time.ElapsedGameTime;

            // Allow player to exit early or restart
            if (InputManager.IsKeyReleased(Keys.Escape) || InputManager.IsKeyReleased(Keys.Space))
            {
                ((FinalProject)game).SwitchScene("Menu");
            }

            // Loop back to menu automatically when finished
            // Calculate total height of text block approx (Lines * 30px)
            if (scrollY > (lines.Count * 40) + game.GraphicsDevice.Viewport.Height)
            {
                ((FinalProject)game).SwitchScene("Menu");
            }
        }

        public override void Draw()
        {
            game.GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            Vector2 viewport = new Vector2(game.GraphicsDevice.Viewport.Width, game.GraphicsDevice.Viewport.Height);

            for (int i = 0; i < lines.Count; i++)
            {
                string line = lines[i];
                Vector2 size = font.MeasureString(line);

                // Center the text horizontally
                Vector2 center = (viewport - size) / 2;

                // Calculate Y position based on scroll
                // We add (i * 40) to space lines out
                float yPos = center.Y + (i * 40) - scrollY + (viewport.Y / 2);

                // Only draw if visible on screen
                if (yPos > -50 && yPos < viewport.Y + 50)
                {
                    Color color = Color.White;
                    if (i == 0) color = Color.Yellow; // Make Title Yellow
                    if (line.StartsWith("---")) color = Color.Red; // Make Headers Red

                    spriteBatch.DrawString(font, line, new Vector2(center.X, yPos), color);
                }
            }

            spriteBatch.End();
        }
    }
}