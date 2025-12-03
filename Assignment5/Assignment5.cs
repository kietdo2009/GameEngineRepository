using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CPI311.GameEngine;

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
            // Inside Assignment4.cs -> LoadContent()

            cameraTransform = new Transform();
            // 1. Move Camera WAY up (GameConstants says 25,000, let's try 5,000 for now so you can see)
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
            terrain = new TerrainRenderer(heightMap, new Vector2(20, 20), new Vector2(20, 20));
            if (terrain.Transform == null)
            {
                terrain.Transform = new Transform();
                terrain.Transform.LocalScale = Vector3.One; // Ensure scale isn't zero
            }
            // Set the light on the terrain so we can see it
            terrain.NormalMap = normalMap;
            terrain.Effect = terrainEffect;
            // Create the player
            player = new Player(terrain, Content, camera, GraphicsDevice, light);
            // Add to the scene components so it updates automatically

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            player.Update();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            terrain.Draw();
            player.Draw();
            base.Draw(gameTime);
        }
    }
}
