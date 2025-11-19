using CPI311.GameEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lab10
{
    public class Lab10 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // Game Objects
        TerrainRenderer terrain;
        Camera camera;
        Effect effect;

        // Lighting Variables
        Vector3 lightPosition = new Vector3(0, 10, 0); // Adjust as needed

        public Lab10()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _graphics.GraphicsProfile = GraphicsProfile.HiDef;
        }

        protected override void Initialize()
        {
            Time.Initialize();
            InputManager.Initialize();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // *** 1. Setup Camera ***
            camera = new Camera();
            camera.Transform = new Transform();
            camera.Transform.LocalPosition = new Vector3(0, 0, 0);
            camera.NearPlane = 0.1f;
            camera.FarPlane = 1000f;

            // *** 2. Initialize Terrain ***
            // Loading textures for Heightmap and Normalmap
            Texture2D heightMap = Content.Load<Texture2D>("Heightmap"); 
            Texture2D normalMap = Content.Load<Texture2D>("Normalmap"); 

            // Initialize TerrainRenderer with size (100x100) and resolution (200x200)
            terrain = new TerrainRenderer(
                heightMap,
                Vector2.One * 100,
                Vector2.One * 200,
                GraphicsDevice); // 

            terrain.NormalMap = normalMap; // 

            // Setup Terrain Transform and Scale
            terrain.Transform = new Transform();
            terrain.Transform.LocalScale *= new Vector3(1, 5, 1); // Stretch height by 5 

            // *** 3. Setup Effect ***
            effect = Content.Load<Effect>("TerrainShader"); 

           
            effect.Parameters["AmbientColor"].SetValue(new Vector3(0.1f, 0.1f, 0.1f));
            effect.Parameters["DiffuseColor"].SetValue(new Vector3(1f, 1f, 1f));
            effect.Parameters["SpecularColor"].SetValue(new Vector3(0, 0, 0.2f));
            effect.Parameters["Shininess"].SetValue(20f);
            effect.Parameters["LightPosition"].SetValue(lightPosition);
            effect.Parameters["NormalMap"].SetValue(normalMap);
        }

        protected override void Update(GameTime gameTime)
        {
            Time.Update(gameTime);
            InputManager.Update();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // *** 4. Camera Movement ***
            // Simple WASD controls to move the camera locally
            if (InputManager.IsKeyDown(Keys.W)) // Move forward
                camera.Transform.LocalPosition += camera.Transform.Forward * Time.ElapsedGameTime * 10f;
            if (InputManager.IsKeyDown(Keys.S)) // Move backward
                camera.Transform.LocalPosition += camera.Transform.Backward * Time.ElapsedGameTime * 10f;
            if (InputManager.IsKeyDown(Keys.A)) // Rotate Left
                camera.Transform.Rotate(Vector3.Up, Time.ElapsedGameTime);
            if (InputManager.IsKeyDown(Keys.D)) // Rotate Right
                camera.Transform.Rotate(Vector3.Up, -Time.ElapsedGameTime);

            // *** 5. Terrain Following (GetAltitude) ***
            // Clamp the camera height to the terrain height + offset
            Vector3 currentPos = camera.Transform.LocalPosition;
            float terrainHeight = terrain.GetAltitude(currentPos);

            // Only apply if valid height returned (-1 indicates off-grid)
            if (terrainHeight > -1000) // Simple check
            {
                camera.Transform.LocalPosition = new Vector3(
                    currentPos.X,
                    terrainHeight + 1, // +1 meter up [cite: 86-89]
                    currentPos.Z
                );
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // *** 6. Draw Terrain ***
            // Update Effect Parameters for Matrices 
            effect.Parameters["World"].SetValue(terrain.Transform.World);
            effect.Parameters["View"].SetValue(camera.View);
            effect.Parameters["Projection"].SetValue(camera.Projection);
            effect.Parameters["CameraPosition"].SetValue(camera.Transform.Position);
            effect.Parameters["LightPosition"].SetValue(lightPosition);

            // Draw for each pass 
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                terrain.Draw();
            }

            base.Draw(gameTime);
        }
    }
}