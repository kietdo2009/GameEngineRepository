using CPI311.GameEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Assignment5
{
    public class Assignment5 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        TerrainRenderer terrain;
        Camera camera;
        Transform cameraTransform;
        Transform lightTransform;
        Light light;
        Player player;
        Effect effect;
        Agent agent;
        // Change single agent to a list
        List<Agent> agents;
        SpriteFont font; // Needed for the UI 
        int score = 0;   // To track caught aliens

        public Assignment5()
        {
            _graphics = new GraphicsDeviceManager(this);
            // Ensure the HiDef profile is requested before the GraphicsDevice is created
            _graphics.GraphicsProfile = GraphicsProfile.HiDef;
            _graphics.ApplyChanges();

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            Time.Initialize();
            InputManager.Initialize();  
            ScreenManager.Initialize(_graphics);    
            base.Initialize();

        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            cameraTransform = new Transform();
        
            cameraTransform.LocalPosition = new Vector3(0, 30, 0);

            // 2. Rotate Camera to look DOWN (90 degrees on the X axis)
            cameraTransform.Rotation = Quaternion.CreateFromAxisAngle(Vector3.Right, -MathHelper.PiOver2);

            camera = new Camera();
            camera.Transform = cameraTransform;
            camera.FarPlane = 30000f;
            // Setup Light
            lightTransform = new Transform();
            lightTransform.LocalPosition = Vector3.Backward * 20 + Vector3.Up * 20 + Vector3.Right * 20;
            light = new Light();
            light.Transform = lightTransform;
            // Load texture assets (Make sure these names match your Content content!)
            Texture2D heightMap = Content.Load<Texture2D>("mazeH"); // Your 20x20 maze image
            Texture2D normalMap = Content.Load<Texture2D>("mazeN"); // Your normal map
            Effect terrainEffect = Content.Load<Effect>("TerrainShader"); // From Lab 10

            // assign to the field so subsequent code uses the same instance
            effect = terrainEffect;

            // now safe
            effect.Parameters["AmbientColor"].SetValue(light.Ambient.ToVector3());
            effect.Parameters["DiffuseColor"].SetValue(light.Diffuse.ToVector3());
            effect.Parameters["SpecularColor"].SetValue(light.Specular.ToVector3());
            effect.Parameters["Shininess"].SetValue(32f);

            effect.Parameters["DiffuseTexture"].SetValue(heightMap);
            // set it on the terrain
            terrain = new TerrainRenderer(heightMap, new Vector2(40, 40), new Vector2(20, 20));
            if (terrain.Transform == null)
            {
                terrain.Transform = new Transform();
                terrain.Transform.LocalScale = Vector3.One; // Ensure scale isn't zero
            }
            // Set the light on the terrain so we can see it
            terrain.NormalMap = normalMap;
            terrain.Effect = effect;
            
            // Create the player
            player = new Player(terrain, Content, camera, GraphicsDevice, light);
            agent = new Agent(terrain, Content, camera, GraphicsDevice, light);
            // Add to the scene components so it updates automatically

            // Load a font (make sure you create a SpriteFont in the Content pipeline)
            font = Content.Load<SpriteFont>("Font");

            agents = new List<Agent>();
            for (int i = 0; i < 3; i++)
            {
                Agent a = new Agent(terrain, Content, camera, GraphicsDevice, light);
                // You might want to offset their start positions slightly
                agents.Add(a);
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            Time.Update(gameTime);
            InputManager.Update();

            // TODO: Add your update logic here
            if (Vector3.Distance(player.Transform.LocalPosition, agent.Transform.LocalPosition) < 1f)
            {
                agent.path = null;
            }

            // Update the loop to handle multiple agents
            foreach (var agent in agents)
            {
                agent.Update();

                // Check collision with Player
                if (Vector3.Distance(player.Transform.LocalPosition, agent.Transform.LocalPosition) < 1.5f) // Increased radius slightly
                {
                   
                    agent.RandomPathFinding(); // Or whatever method resets their position
                    score++; // Increment score
                }
            }
            player.Update();
            agent.Update();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here'
            // ... existing 3D drawing code ...
            effect.Parameters["View"].SetValue(camera.View);
            effect.Parameters["Projection"].SetValue(camera.Projection);
            effect.Parameters["World"].SetValue(terrain.Transform.World);

            // Optional: If your shader uses camera position for specular lighting (shininess)
            effect.Parameters["CameraPosition"].SetValue(camera.Transform.Position);

            foreach (var agent in agents)
                agent.Draw();

            _spriteBatch.Begin();
            _spriteBatch.DrawString(font, $"Time: {Time.TotalGameTime.TotalSeconds:0.00}", new Vector2(10, 10), Color.White);
            _spriteBatch.DrawString(font, "Aliens Caught: " + score, new Vector2(10, 30), Color.White);
            _spriteBatch.End();

            // Reset render states after SpriteBatch (Critical for 3D)
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.BlendState = BlendState.Opaque;
            terrain.Draw();
            player.Draw();
            agent.Draw();
            base.Draw(gameTime);
        }
    }
}
