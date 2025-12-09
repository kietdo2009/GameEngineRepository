using CPI311.GameEngine;
using FinalProject;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace FinalProject
{
    public class Level2 : GameScene
    {
        Camera camera;
        Light light;
        PlayerController player;
        List<Token> tokens;
        List<Wall> platforms; // Track the boxes
        Effect effect;
        SpriteFont font;
        Flag exitFlag;
        TerrainRenderer terrain;
        // Replaced Walls with Terrains
        TerrainRenderer startTerrain;
        TerrainRenderer goalTerrain;


        // Background terrain (the void floor)
        TerrainRenderer voidTerrain;

        public Level2(Game game) : base(game)
        {
            tokens = new List<Token>();
            platforms = new List<Wall>();
        }

        public override void LoadContent()
        {
            font = game.Content.Load<SpriteFont>("Font");

            // --- Camera & Light ---
            camera = new Camera();
            camera.Transform = new Transform();
            camera.Transform.LocalPosition = new Vector3(0, 50, 50); // High angle
            camera.Transform.Rotate(Vector3.Right, -MathHelper.PiOver4);
            camera.FarPlane = 2000f;

            light = new Light();
            light.Transform = new Transform();
            light.Transform.LocalPosition = new Vector3(0, 50, 0);

            // --- The "Void" Floor (Deep down so you can fall) ---
            Texture2D heightMap = game.Content.Load<Texture2D>("mazeH"); // Or any texture
            Texture2D texture = game.Content.Load<Texture2D>("Square");
            effect = game.Content.Load<Effect>("TerrainShader");

            // Setup Shader
            effect.Parameters["AmbientColor"].SetValue(light.Ambient.ToVector3());
            effect.Parameters["DiffuseColor"].SetValue(light.Diffuse.ToVector3());
            effect.Parameters["SpecularColor"].SetValue(light.Specular.ToVector3());
            effect.Parameters["Shininess"].SetValue(32f);
            effect.Parameters["DiffuseTexture"].SetValue(texture);

            voidTerrain = new TerrainRenderer(heightMap, new Vector2(200, 200), new Vector2(100, 100));
            voidTerrain.NormalMap = game.Content.Load<Texture2D>("mazeN");
            voidTerrain.Effect = effect;
            voidTerrain.Transform = new Transform();
            voidTerrain.Transform.LocalPosition = new Vector3(0, -30, 0); // Way down!

            // --- 1. SETUP SHADER & TEXTURES ---
            Texture2D normalMap = game.Content.Load<Texture2D>("mazeN");
            // Effect effect = game.Content.Load<Effect>("TerrainShader");

            effect.Parameters["AmbientColor"].SetValue(light.Ambient.ToVector3());
            effect.Parameters["DiffuseColor"].SetValue(light.Diffuse.ToVector3());
            effect.Parameters["SpecularColor"].SetValue(light.Specular.ToVector3());
            effect.Parameters["Shininess"].SetValue(32f);
            effect.Parameters["DiffuseTexture"].SetValue(heightMap);

            // --- 2. SETUP START TERRAIN (Under Feet) ---
            // Size (40, 40) is large and safe.
            startTerrain = new TerrainRenderer(heightMap, new Vector2(40, 40), new Vector2(20, 20));
            startTerrain.NormalMap = normalMap;
            startTerrain.Effect = effect;
            startTerrain.Transform = new Transform();
            startTerrain.Transform.LocalScale = Vector3.One;
            startTerrain.Transform.LocalPosition = Vector3.Zero; // Center

            // --- 3. SETUP GOAL TERRAIN (Under Flag) ---
            // We reuse the assets but create a new instance.
            goalTerrain = new TerrainRenderer(heightMap, new Vector2(40, 40), new Vector2(20, 20));
            goalTerrain.NormalMap = normalMap;
            goalTerrain.Effect = effect;
            goalTerrain.Transform = new Transform();
            goalTerrain.Transform.LocalScale = Vector3.One;

            // Position it 45 units forward. 
            // Since size is 40 (radius 20), Start ends at Z=20. Goal starts at Z=25 (45-20).
            // This leaves a small 5-unit gap to jump over.
            goalTerrain.Transform.LocalPosition = new Vector3(0, 0, 45);

            // --- Player Setup ---
            player = new PlayerController(voidTerrain, camera, game.Content, game.GraphicsDevice, light);
            player.Transform.LocalPosition = new Vector3(0, 2, 0); // Start position
            player.FallLimit = -25f; // Respawn if they hit the void terrain

            // --- LEVEL GENERATION: Box -> Token -> Box ---

            // 1. Starting Platform (Safe zone)
            //Wall startPlat = new Wall(game.Content, camera, game.GraphicsDevice, light);
            //startPlat.Transform.LocalPosition = new Vector3(0, 0, 0);
            //startPlat.Transform.LocalScale = new Vector3(2, 1, 2); // Make start big
            //platforms.Add(startPlat);
            //player.Walls.Add(startPlat); // Register collision

            // 5. Generate the Steps
            int steps = 5;
            float stepDistance = 8f; // Gap size

            for (int i = 1; i <= steps; i++)
            {
                // Calculate position: Move forward (Z) and slightly down or up? 
                // Let's go Forward (Z)
                Vector3 pos = new Vector3(0, 0, i * stepDistance);

                // A. Create the Box (Wall)
                Wall box = new Wall(game.Content, camera, game.GraphicsDevice, light);
                box.Transform.LocalPosition = pos;
                box.Transform.LocalScale = new Vector3(1.5f, 1f, 1.5f)/30; // Standard size
                platforms.Add(box);
                player.Walls.Add(box); // IMPORTANT: Add to player's collision list!

                // B. Create Token ON TOP of the box
                Token t = new Token(game.Content, camera, game.GraphicsDevice, light);
                t.Transform.LocalPosition = pos - new Vector3(0, 5f, 0); // Floating above box
                tokens.Add(t);
            }

            // 5. Exit Flag at the end
            exitFlag = new Flag(game.Content, camera, game.GraphicsDevice, light);
            exitFlag.Transform.LocalPosition = new Vector3(0, -10, (steps + 1) * stepDistance);
        }

        public override void Update()
        {
            if (InputManager.IsKeyReleased(Keys.Escape)) ((FinalProject)game).SwitchScene("Menu");

            player.Update();
            exitFlag.Update();

            // Update all platforms
            foreach (Wall w in platforms) w.Update();

            // Token Collision
            for (int i = tokens.Count - 1; i >= 0; i--)
            {
                tokens[i].Update();
                if (Vector3.Distance(player.Transform.LocalPosition, tokens[i].Transform.LocalPosition) < 2.0f)
                {
                    player.Tokens++;
                    tokens.RemoveAt(i);
                }
            }

            // Win Condition
            float dist = Vector3.Distance(player.Transform.LocalPosition, exitFlag.Transform.LocalPosition);
            System.Diagnostics.Debug.WriteLine($"Flag Distance: {dist}");
            if (Vector3.Distance(player.Transform.LocalPosition, exitFlag.Transform.LocalPosition) < 5.0f)
            {
                // WIN CONDITION MET!
                // We only allow exit if they have collected at least 1 token (Optional rule)   
                ((FinalProject)game).SwitchScene("Level3");
                System.Diagnostics.Debug.WriteLine("You Win!");
            }
        }

        public override void Draw()
        {
            game.GraphicsDevice.Clear(Color.Black);

            game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            // Draw Void Terrain
            effect.Parameters["View"].SetValue(camera.View);
            effect.Parameters["Projection"].SetValue(camera.Projection);
            effect.Parameters["World"].SetValue(voidTerrain.Transform.World);
            effect.Parameters["CameraPosition"].SetValue(camera.Transform.Position);
            voidTerrain.Draw();

            // Draw Platforms
            foreach (Wall w in platforms) w.Draw();

            // Draw Tokens & Flag
            foreach (Token t in tokens) t.Draw();
            exitFlag.Draw();

            // Draw UI
            spriteBatch.Begin();
            spriteBatch.DrawString(font, "LEVEL 2: Box Jump", new Vector2(10, 10), Color.Red);
            spriteBatch.DrawString(font, "Tokens: " + player.Tokens, new Vector2(10, 30), Color.White);
            Vector2 center = new Vector2(game.GraphicsDevice.Viewport.Width / 2, game.GraphicsDevice.Viewport.Height / 2);
            spriteBatch.DrawString(font, "+", center, Color.Red);
            // Show Tokens Needed
            spriteBatch.DrawString(font, $"Tokens: {player.Tokens} / 5 Needed", new Vector2(10, 30), Color.White);

            // Show Teleports (Green if available, Red if out)
            Color telColor = (player.TeleportsUsed < player.Tokens) ? Color.Green : Color.Red;
            spriteBatch.DrawString(font, $"Teleports Used: {player.TeleportsUsed}", new Vector2(10, 50), telColor);
            spriteBatch.End();
        }
    }
}