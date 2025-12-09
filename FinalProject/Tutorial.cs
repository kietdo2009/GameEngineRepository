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
        Flag exitFlag;
        TerrainRenderer terrain2;
        const int WIN_TOKENS = 5;

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

            // --- 1. SETUP TERRAIN 1 (The Starting Island) ---
            terrain = new TerrainRenderer(heightMap, new Vector2(40, 40), new Vector2(20, 20));
            terrain.NormalMap = normalMap;
            terrain.Effect = effect;
            if (terrain.Transform == null) terrain.Transform = new Transform();
            terrain.Transform.LocalScale = Vector3.One;

            // --- 2. SETUP TERRAIN 2 (The Goal Island) ---
            // We reuse the same assets, but create a new instance
            terrain2 = new TerrainRenderer(heightMap, new Vector2(40, 40), new Vector2(20, 20));
            terrain2.NormalMap = normalMap;
            terrain2.Effect = effect;
            terrain2.Transform = new Transform();
            terrain2.Transform.LocalScale = Vector3.One;

            // MOVE IT: 40 units Right (X) and 10 units Down (Y)
            // Since Terrain 1 is 40 wide (extends to X=20), placing Center at X=40 puts it right next to it.
            terrain2.Transform.LocalPosition = new Vector3(60, -10, 0);
            

            // Player
            player = new PlayerController(terrain, camera, game.Content, game.GraphicsDevice, light);
            player.Transform.LocalPosition = new Vector3(0, 5, 0);
            player.Transform.LocalScale = Vector3.One * 2;
            player.Terrains.Add(terrain2);

            // Exit Flag
            exitFlag = new Flag(game.Content, camera, game.GraphicsDevice, light);
            exitFlag.Transform.LocalPosition = new Vector3(60, -8, 10); // Put it at the end of the maze

            // Tokens
            System.Random rnd = new System.Random();

            SpawnTokens();
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

            // For flag waving animation
            exitFlag.Update();

            // Token Logic
            for (int i = tokens.Count - 1; i >= 0; i--)
            {
                //float dist = Vector3.Distance(player.Transform.LocalPosition, tokens[i].Transform.LocalPosition);

                // Print distance to the Output window so we can see it!
                //System.Diagnostics.Debug.WriteLine($"Token {i} Distance: {dist}");

                tokens[i].Update();
                if (Vector3.Distance(player.Transform.LocalPosition, tokens[i].Transform.LocalPosition) < 5.0f)
                {
                    player.Tokens++;
                    tokens.RemoveAt(i);
                }
            }


            // Check Distance

            float dist = Vector3.Distance(player.Transform.LocalPosition, exitFlag.Transform.LocalPosition);
            System.Diagnostics.Debug.WriteLine($"Flag Distance: {dist}");
            if (Vector3.Distance(player.Transform.LocalPosition, exitFlag.Transform.LocalPosition) < 5.0f)
            {
        // WIN CONDITION MET!
        // We only allow exit if they have collected at least 1 token (Optional rule)   
                ((FinalProject)game).SwitchScene("Level2");
                System.Diagnostics.Debug.WriteLine("You Win!");
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
            
            //Terrain 1
            effect.Parameters["CameraPosition"].SetValue(camera.Transform.Position);
            terrain.Draw();

            //Goal Terrain
            effect.Parameters["World"].SetValue(terrain2.Transform.World);
            terrain2.Draw();

            // player.Draw(); // Don't draw player in FPS mode (or add body later)
            foreach (Token t in tokens) t.Draw();

            // Draw UI
            spriteBatch.Begin();
            spriteBatch.DrawString(font, "TUTORIAL LEVEL", new Vector2(10, 10), Color.Yellow);
            spriteBatch.DrawString(font, "Tokens: " + player.Tokens, new Vector2(10, 30), Color.White);
            // Show Tokens Needed
            spriteBatch.DrawString(font, $"Tokens: {player.Tokens} / {WIN_TOKENS} Needed", new Vector2(10, 30), Color.White);
            // Show Teleports (Green if available, Red if out)
            Color telColor = (player.TeleportsUsed < player.Tokens) ? Color.Green : Color.Red;
            spriteBatch.DrawString(font, $"Teleports Used: {player.TeleportsUsed}", new Vector2(10, 50), telColor);

            // Crosshair
            Vector2 center = new Vector2(game.GraphicsDevice.Viewport.Width / 2, game.GraphicsDevice.Viewport.Height / 2);
            spriteBatch.DrawString(font, "+", center, Color.Red);

            //Win Flag
            exitFlag.Draw();

            spriteBatch.End();
        }
        private void SpawnTokens()
        {
            tokens.Clear();
            System.Random rnd = new System.Random();

            // Create 8 tokens scattered around so player can definitely find 5
            for (int i = 0; i < 8; i++)
            {
                Token t = new Token(game.Content, camera, game.GraphicsDevice, light);
                // Randomly place on first island or near it (Adjust range as needed)
                t.Transform.LocalPosition = new Vector3(rnd.Next(-15, 15), 2, rnd.Next(-15, 15));
                tokens.Add(t);
            }
        }

        private void ResetLevel()
        {
            // 1. Reset Player Stats
            player.Transform.LocalPosition = new Vector3(0, 5, 0);
            player.Rigidbody.Velocity = Vector3.Zero;
            player.Tokens = 0;
            player.TeleportsUsed = 0;

            // 2. Respawn Tokens
            SpawnTokens();
        }
    }
}