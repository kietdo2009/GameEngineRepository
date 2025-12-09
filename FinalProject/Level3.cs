using CPI311.GameEngine;
using FinalProject;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace FinalProject
{
    public class Level3 : GameScene
    {
        Camera camera;
        Light light;
        PlayerController player;
        List<Token> tokens;
        List<Wall> platforms;
        SpriteFont font;
        Effect effect;
        Flag exitFlag;
        TerrainRenderer ground;
        List<TowerVisual> decorWalls;

        // Settings
        const int WIN_TOKENS = 6;
        const float FLOOR_HEIGHT = 12f;
        const int FLOORS = 6;

        // --- NEW: COOLDOWN TIMER ---
        float moveTimer = 0f;
        const float MOVE_DELAY = 0.5f; // Wait 0.5 seconds between teleports

        public Level3(Game game) : base(game)
        {
            tokens = new List<Token>();
            platforms = new List<Wall>();
            decorWalls = new List<TowerVisual>(); 
        }

        public override void LoadContent()
        {
            font = game.Content.Load<SpriteFont>("Font");

            camera = new Camera();
            camera.Transform = new Transform();
            camera.Transform.LocalPosition = new Vector3(0, 5, 40);
            camera.Transform.Rotate(Vector3.Right, -0.1f);
            camera.FarPlane = 2000f;

            light = new Light();
            light.Transform = new Transform();
            light.Transform.LocalPosition = new Vector3(0, 50, 0);

            Texture2D texture = game.Content.Load<Texture2D>("Square");
            effect = game.Content.Load<Effect>("TerrainShader");

            effect.Parameters["AmbientColor"].SetValue(light.Ambient.ToVector3());
            effect.Parameters["DiffuseColor"].SetValue(light.Diffuse.ToVector3());
            effect.Parameters["SpecularColor"].SetValue(light.Specular.ToVector3());
            effect.Parameters["Shininess"].SetValue(32f);
            effect.Parameters["DiffuseTexture"].SetValue(texture);

            Texture2D heightMap = game.Content.Load<Texture2D>("Square");
            ground = new TerrainRenderer(heightMap, new Vector2(100, 100), new Vector2(100, 100));
            ground.NormalMap = texture;
            ground.Effect = effect;
            ground.Transform = new Transform();
            ground.Transform.LocalPosition = Vector3.Zero;

            player = new PlayerController(ground, camera, game.Content, game.GraphicsDevice, light);
            player.Transform.LocalPosition = new Vector3(0, 2, 20);
            player.FallLimit = -10f;


            // --- 4. DECORATIVE TOWER WALLS (Chaos Edition) ---
            float distance = 50f;
            float scale = 0.5f;

            // Wall 1 (North) - Planks
            TowerVisual wall1 = new TowerVisual(game.Content, camera, game.GraphicsDevice, light, "planks");
            wall1.Transform.LocalPosition = new Vector3(0, -10, distance);
            wall1.Transform.LocalScale = new Vector3(scale, scale, scale);
            decorWalls.Add(wall1);

            // Wall 2 (South) - Roof
            TowerVisual wall2 = new TowerVisual(game.Content, camera, game.GraphicsDevice, light, "roof");
            wall2.Transform.LocalPosition = new Vector3(0, -10, -distance);
            wall2.Transform.LocalScale = new Vector3(scale, scale, scale);
            wall2.Transform.Rotate(Vector3.Up, MathHelper.Pi);
            decorWalls.Add(wall2);

            // Wall 3 (East) - Details (Door)
            TowerVisual wall3 = new TowerVisual(game.Content, camera, game.GraphicsDevice, light, "details");
            wall3.Transform.LocalPosition = new Vector3(distance, -10, 0);
            wall3.Transform.LocalScale = new Vector3(scale, scale, scale);
            wall3.Transform.Rotate(Vector3.Up, -MathHelper.PiOver2);
            decorWalls.Add(wall3);

            // Wall 4 (West) - Water (Weird but cool)
            TowerVisual wall4 = new TowerVisual(game.Content, camera, game.GraphicsDevice, light, "water");
            wall4.Transform.LocalPosition = new Vector3(-distance, -10, 0);
            wall4.Transform.LocalScale = new Vector3(scale, scale, scale);
            wall4.Transform.Rotate(Vector3.Up, MathHelper.PiOver2);
            decorWalls.Add(wall4);

            SetupTower();
            SpawnTokens();

            exitFlag = new Flag(game.Content, camera, game.GraphicsDevice, light);
            exitFlag.Transform.LocalPosition = new Vector3(0, (FLOORS * FLOOR_HEIGHT) - 2, 0);
        }

        private void SetupTower()
        {
            platforms.Clear();
            player.Walls.Clear();

            for (int i = 1; i <= FLOORS; i++)
            {
                Wall floor = new Wall(game.Content, camera, game.GraphicsDevice, light);
                float xPos = (i % 2 == 0) ? 12 : -12;

                floor.Transform.LocalPosition = new Vector3(xPos, i * FLOOR_HEIGHT, 0);
                floor.Transform.LocalScale = new Vector3(.25f, 4f, 6f) / 30;

                platforms.Add(floor);
                player.Walls.Add(floor);
            }
        }

        private void SpawnTokens()
        {
            tokens.Clear();
            player.Tokens = 0;

            for (int i = 1; i <= FLOORS; i++)
            {
                Token t = new Token(game.Content, camera, game.GraphicsDevice, light);
                float xPos = (i % 2 == 0) ? 10 : -10;
                t.Transform.LocalPosition = new Vector3(xPos, (i * FLOOR_HEIGHT) + 2, 0);
                tokens.Add(t);
            }
        }

        private void ResetLevel()
        {
            player.Transform.LocalPosition = new Vector3(0, 2, 20);
            player.Rigidbody.Velocity = Vector3.Zero;
            player.Tokens = 0;
            player.TeleportsUsed = 0;
            moveTimer = 0f; // Reset timer
            SpawnTokens();
        }

        public override void Update()
        {
            if (InputManager.IsKeyReleased(Keys.Escape)) ((FinalProject)game).SwitchScene("Menu");

            // --- 1. DECREASE TIMER ---
            if (moveTimer > 0)
            {
                moveTimer -= Time.ElapsedGameTime;
            }

            // --- 2. CHECK INPUT + TIMER ---
            // Only allow teleport if timer is finished (<= 0)
            if (InputManager.IsMousePressed(0) && moveTimer <= 0)
            {
                // Reset the timer immediately so you can't click again for 0.5 seconds
                moveTimer = MOVE_DELAY;
                SoundManager.Play("teleport");
                if (tokens.Count > 0)
                {
                    Token nextTarget = tokens[0];

                    // Teleport Logic
                    player.Transform.LocalPosition = nextTarget.Transform.LocalPosition;
                    player.Rigidbody.Velocity = Vector3.Zero;
                    player.TeleportsUsed++;
                }
                else
                {
                    // Teleport to Flag
                    player.Transform.LocalPosition = exitFlag.Transform.LocalPosition;
                    player.Rigidbody.Velocity = Vector3.Zero;
                }
            }

            if (player.Transform.LocalPosition.Y < -10f) ResetLevel();

            player.Update();
            exitFlag.Update();
            foreach (Wall w in platforms) w.Update();

            // Token Collision
            for (int i = tokens.Count - 1; i >= 0; i--)
            {
                tokens[i].Update();
                if (Vector3.Distance(player.Transform.LocalPosition, tokens[i].Transform.LocalPosition) < 3.0f)
                {
                    player.Tokens++;
                    tokens.RemoveAt(i);
                }
            }

            foreach (var wall in decorWalls) wall.Update();

            // Win Condition
            if (Vector3.Distance(player.Transform.LocalPosition, exitFlag.Transform.LocalPosition) < 3.0f)
            {
                if (player.Tokens >= WIN_TOKENS)
                {
                    System.Diagnostics.Debug.WriteLine("YOU WIN THE GAME!");
                    ((FinalProject)game).SwitchScene("Credits");
                }
            }
        }

        public override void Draw()
        {
            game.GraphicsDevice.Clear(Color.Black);
            game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            effect.Parameters["View"].SetValue(camera.View);
            effect.Parameters["Projection"].SetValue(camera.Projection);
            effect.Parameters["World"].SetValue(ground.Transform.World);
            effect.Parameters["CameraPosition"].SetValue(camera.Transform.Position);
            ground.Draw();

            foreach (Wall w in platforms) w.Draw();
            foreach (Token t in tokens) t.Draw();
            exitFlag.Draw();

            foreach (var wall in decorWalls) wall.Draw();

            spriteBatch.Begin();
            spriteBatch.DrawString(font, "LEVEL 3: The Sinister Tower", new Vector2(10, 10), Color.Red);
            spriteBatch.DrawString(font, $"Tokens: {player.Tokens}", new Vector2(10, 30), Color.White);

            // Show Cooldown status
            // Show Teleports (Green if available, Red if out)
            Color telColor = (player.TeleportsUsed < player.Tokens) ? Color.Green : Color.Red;
            spriteBatch.DrawString(font, $"Teleports Used: {player.TeleportsUsed}", new Vector2(10, 50), telColor);
            spriteBatch.End();

            spriteBatch.Begin();
            Vector2 center = new Vector2(game.GraphicsDevice.Viewport.Width / 2, game.GraphicsDevice.Viewport.Height / 2);
            spriteBatch.DrawString(font, "+", center, Color.Red);
            spriteBatch.End();
        }
    }
}