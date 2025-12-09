using CPI311.GameEngine;
using FinalProject;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace FinalProject
{
    public class Tutorial : GameScene
    {
        // Game Objects
        Camera camera;
        Light light;
        TerrainRenderer terrain;
        PlayerController player;
        List<Token> tokens;
        Effect effect;
        SpriteFont font;

        public Tutorial(Game game) : base(game)
        {
            tokens = new List<Token>();
        }

        public override void LoadContent()
        {
            font = game.Content.Load<SpriteFont>("Font");

            // --- 3D Setup ---
            camera = new Camera();
            camera.Transform = new Transform();
            camera.Transform.LocalPosition = new Vector3(0, 50, 50);
            camera.Transform.Rotate(Vector3.Right, -MathHelper.PiOver4);
            camera.FarPlane = 1000f;

            light = new Light();
            light.Transform = new Transform();
            light.Transform.LocalPosition = new Vector3(0, 50, 0);

            // Terrain
            Texture2D heightMap = game.Content.Load<Texture2D>("mazeH");
            Texture2D normalMap = game.Content.Load<Texture2D>("mazeN");
            effect = game.Content.Load<Effect>("TerrainShader");

            // Setup Shader
            effect.Parameters["AmbientColor"].SetValue(light.Ambient.ToVector3());
            effect.Parameters["DiffuseColor"].SetValue(light.Diffuse.ToVector3());
            effect.Parameters["SpecularColor"].SetValue(light.Specular.ToVector3());
            effect.Parameters["Shininess"].SetValue(32f);
            effect.Parameters["DiffuseTexture"].SetValue(heightMap);

            terrain = new TerrainRenderer(heightMap, new Vector2(40, 40), new Vector2(20, 20));
            terrain.NormalMap = normalMap;
            terrain.Effect = effect;
            if (terrain.Transform == null) terrain.Transform = new Transform();
            terrain.Transform.LocalScale = Vector3.One;

            // Player
            player = new PlayerController(terrain, camera, game.Content, game.GraphicsDevice, light);
            player.Transform.LocalPosition = new Vector3(0, 5, 0);

            // Tokens
            System.Random rnd = new System.Random();
            for (int i = 0; i < 5; i++)
            {
                Token t = new Token(game.Content, camera, game.GraphicsDevice, light);
                t.Transform.LocalPosition = new Vector3(rnd.Next(-15, 15), 2, rnd.Next(-15, 15));
                tokens.Add(t);
            }
        }

        public override void Update()
        {
            // Escape to go back to Menu
            if (InputManager.IsKeyReleased(Keys.Escape))
            {
                ((FinalProject)game).SwitchScene("Menu");
                return;
            }

            // FPS Mode usually hides cursor, but if you need to click, keep it visible
            game.IsMouseVisible = false;

            player.Update();

            // Token Logic
            for (int i = tokens.Count - 1; i >= 0; i--)
            {
                float dist = Vector3.Distance(player.Transform.LocalPosition, tokens[i].Transform.LocalPosition);

                // Print distance to the Output window so we can see it!
                System.Diagnostics.Debug.WriteLine($"Token {i} Distance: {dist}");

                tokens[i].Update();
                if (Vector3.Distance(player.Transform.LocalPosition, tokens[i].Transform.LocalPosition) < 5.0f)
                {
                    player.Tokens++;
                    tokens.RemoveAt(i);
                }
            }
        }

        public override void Draw()
        {
            game.GraphicsDevice.Clear(Color.CornflowerBlue);

            // Reset States for 3D
            game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            game.GraphicsDevice.BlendState = BlendState.Opaque;

            // Draw 3D
            effect.Parameters["View"].SetValue(camera.View);
            effect.Parameters["Projection"].SetValue(camera.Projection);
            effect.Parameters["World"].SetValue(terrain.Transform.World);
            effect.Parameters["CameraPosition"].SetValue(camera.Transform.Position);
            terrain.Draw();

            // player.Draw(); // Don't draw player in FPS mode (or add body later)
            foreach (Token t in tokens) t.Draw();

            // Draw UI
            spriteBatch.Begin();
            spriteBatch.DrawString(font, "TUTORIAL LEVEL", new Vector2(10, 10), Color.Yellow);
            spriteBatch.DrawString(font, "Tokens: " + player.Tokens, new Vector2(10, 30), Color.White);

            // Crosshair
            Vector2 center = new Vector2(game.GraphicsDevice.Viewport.Width / 2, game.GraphicsDevice.Viewport.Height / 2);
            spriteBatch.DrawString(font, "+", center, Color.Red);

            spriteBatch.End();
        }
    }
}