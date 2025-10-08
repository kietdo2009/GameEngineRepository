using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

// ***ADDED*** Using statement for custom engine
using CPI311.GameEngine;

namespace Lab03
{
    public class Lab03 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // ***ADDED*** LAB 3 variables
        public Model model;
        public SpriteFont font;

        // ***ADDED*** Matrices
        public Matrix world;
        public Matrix view;
        public Matrix projection;

        // ***ADDED*** Positions
        public Vector3 CameraPos = new Vector3(0, 0, 5);
        public Vector3 ModelPos = new Vector3(0, 0, 0);

        // ***ADDED*** Transform parameters
        private float yaw = 0, pitch = 0, roll = 0;
        private float scale = 1.0f;

        // ***ADDED*** Matrix order toggle
        private bool useSRT = true; // true = Scale*Rotation*Translation, false = T*R*S

        // ***ADDED*** Projection toggle + params
        private bool usePerspective = true;
        private float left = -2, right = 2, bottom = -2, top = 2, near = 0.1f, far = 100f;

        public Lab03()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // ***ADDED*** Initialize custom engine classes
            InputManager.Initialize();
            Time.Initialize();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // ***ADDED*** Load 3D model and font
            model = Content.Load<Model>("Torus");
            font = Content.Load<SpriteFont>("Font");

            // ***ADDED*** Lighting setup
            foreach (ModelMesh mesh in model.Meshes)
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;
                }
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // ***ADDED*** Update custom engine classes
            InputManager.Update();
            Time.Update(gameTime);

            // ***ADDED*** (NOTE: This line has an error, needs fix)
            float speed = 5 * Time.ElapsedGameTime;

            // ***ADDED*** Camera movement (WASD)
            if (InputManager.IsKeyDown(Keys.W)) CameraPos += Vector3.Up * speed;
            if (InputManager.IsKeyDown(Keys.S)) CameraPos += Vector3.Down * speed;
            if (InputManager.IsKeyDown(Keys.D)) CameraPos += Vector3.Right * speed;
            if (InputManager.IsKeyDown(Keys.A)) CameraPos += Vector3.Left * speed;

            // ***ADDED*** Model translation (Arrow keys)
            if (InputManager.IsKeyDown(Keys.Up)) ModelPos += Vector3.Up * speed;
            if (InputManager.IsKeyDown(Keys.Down)) ModelPos += Vector3.Down * speed;
            if (InputManager.IsKeyDown(Keys.Right)) ModelPos += Vector3.Right * speed;
            if (InputManager.IsKeyDown(Keys.Left)) ModelPos += Vector3.Left * speed;

            // ***ADDED*** Rotation controls
            if (InputManager.IsKeyDown(Keys.Insert)) yaw += speed;
            if (InputManager.IsKeyDown(Keys.Delete)) yaw -= speed;
            if (InputManager.IsKeyDown(Keys.Home)) pitch += speed;
            if (InputManager.IsKeyDown(Keys.End)) pitch -= speed;
            if (InputManager.IsKeyDown(Keys.PageUp)) roll += speed;
            if (InputManager.IsKeyDown(Keys.PageDown)) roll -= speed;

            // ***ADDED*** Scale controls (Shift + Up/Down)
            if (InputManager.IsKeyDown(Keys.LeftShift) || InputManager.IsKeyDown(Keys.RightShift))
            {
                if (InputManager.IsKeyDown(Keys.Up)) scale += 0.01f;
                if (InputManager.IsKeyDown(Keys.Down)) scale = MathHelper.Max(0.1f, scale - 0.01f);
            }

            // ***ADDED*** Toggle world matrix order (Space)
            if (InputManager.IsKeyPressed(Keys.Space))
                useSRT = !useSRT;

            // ***ADDED*** Toggle projection type (Tab)
            if (InputManager.IsKeyPressed(Keys.Tab))
                usePerspective = !usePerspective;

            // ***ADDED*** Projection parameter controls
            if (InputManager.IsKeyDown(Keys.LeftShift) || InputManager.IsKeyDown(Keys.RightShift))
            {
                if (InputManager.IsKeyDown(Keys.W)) top += 0.05f;
                if (InputManager.IsKeyDown(Keys.S)) bottom -= 0.05f;
                if (InputManager.IsKeyDown(Keys.A)) left -= 0.05f;
                if (InputManager.IsKeyDown(Keys.D)) right += 0.05f;
            }
            if (InputManager.IsKeyDown(Keys.LeftControl) || InputManager.IsKeyDown(Keys.RightControl))
            {
                if (InputManager.IsKeyDown(Keys.W)) top += 0.05f; bottom -= 0.05f;
                if (InputManager.IsKeyDown(Keys.S)) top -= 0.05f; bottom += 0.05f;
                if (InputManager.IsKeyDown(Keys.A)) left -= 0.05f; right += 0.05f;
                if (InputManager.IsKeyDown(Keys.D)) left += 0.05f; right -= 0.05f;
            }

            // ***ADDED*** Build world matrix
            Matrix S = Matrix.CreateScale(scale);
            Matrix R = Matrix.CreateFromYawPitchRoll(yaw, pitch, roll);
            Matrix T = Matrix.CreateTranslation(ModelPos);

            world = useSRT ? (S * R * T) : (T * R * S);

            // ***ADDED*** Build view matrix
            view = Matrix.CreateLookAt(CameraPos, Vector3.Zero, Vector3.Up);

            // ***ADDED*** Build projection matrix
            if (usePerspective)
                projection = Matrix.CreatePerspectiveOffCenter(left, right, bottom, top, near, far);
            else
                projection = Matrix.CreateOrthographicOffCenter(left, right, bottom, top, near, far);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // ***ADDED*** Draw the 3D model
            model.Draw(world, view, projection);

            // ***ADDED*** Draw the UI text
            _spriteBatch.Begin();
            _spriteBatch.DrawString(font, $"Camera Pos: {CameraPos}", new Vector2(10, 10), Color.Black);
            _spriteBatch.DrawString(font, $"Model Pos: {ModelPos}", new Vector2(10, 30), Color.Black);
            _spriteBatch.DrawString(font, $"Yaw:{yaw:0.00} Pitch:{pitch:0.00} Roll:{roll:0.00}", new Vector2(10, 50), Color.Black);
            _spriteBatch.DrawString(font, $"Scale:{scale:0.00}", new Vector2(10, 70), Color.Black);
            _spriteBatch.DrawString(font, $"Matrix Order: {(useSRT ? "S*R*T" : "T*R*S")}", new Vector2(10, 90), Color.Black);
            _spriteBatch.DrawString(font, $"Projection: {(usePerspective ? "Perspective" : "Orthographic")}", new Vector2(10, 110), Color.Black);

            // ***ADDED*** Instructions
            _spriteBatch.DrawString(font, "Arrow = Move Model | WASD = Move Camera", new Vector2(10, 150), Color.Black);
            _spriteBatch.DrawString(font, "Insert/Delete = Yaw | Home/End = Pitch | PgUp/PgDn = Roll", new Vector2(10, 170), Color.Black);
            _spriteBatch.DrawString(font, "Shift+Up/Down = Scale | Space = Toggle Matrix Order", new Vector2(10, 190), Color.Black);
            _spriteBatch.DrawString(font, "Tab = Toggle Projection | Shift+WASD = Center | Ctrl+WASD = Size", new Vector2(10, 210), Color.Black);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}