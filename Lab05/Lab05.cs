using CPI311.GameEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

//LAB 5 Task, press tab key and change techniques There's like 5 shaders
namespace Lab05
{
    public class Lab05 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        public Model model;
        public Effect effect;
        public Texture2D texture;
        public Camera camera;
        public Transform cameraTransform;
        public Transform modelTransform;
        public Model torus;
        public int mode = 0;

        public Lab05()
        {
            
            _graphics = new GraphicsDeviceManager(this);
            //***********************************88
            _graphics.GraphicsProfile = GraphicsProfile.HiDef;
            //*************************************************
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            torus = Content.Load<Model>("Torus");
            effect = Content.Load<Effect>("SimpleShading");
            // Camera setup
            camera = new Camera();
            cameraTransform = new Transform();
            cameraTransform.LocalPosition = Vector3.Backward * 5f; // (0, 0, 10)
            camera.Transform = cameraTransform;
            modelTransform = new Transform();
            modelTransform.LocalPosition = Vector3.Zero;
            model = torus;
            var texture = new Texture2D(GraphicsDevice, 1, 1);
            texture.SetData(new[] { Color.White });

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            Time.Update(gameTime);
            InputManager.Update();
            if(InputManager.IsKeyPressed(Keys.Tab))
            {
                mode++;

            }   

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            Matrix view = camera.View;
            Matrix projection = camera.Projection;

            effect.CurrentTechnique = effect.Techniques[mode% effect.Techniques.Count]; //"0" is the first technique
            effect.Parameters["World"].SetValue(modelTransform.World);
            effect.Parameters["View"].SetValue(view);
            effect.Parameters["Projection"].SetValue(projection);
            effect.Parameters["LightPosition"].SetValue(Vector3.Backward * 10 +
            Vector3.Right * 5);
            effect.Parameters["CameraPosition"].SetValue(cameraTransform.Position);
            effect.Parameters["Shininess"].SetValue(20f);
            effect.Parameters["AmbientColor"].SetValue(new Vector3(0.2f, 0.2f, 0.2f));
            effect.Parameters["DiffuseColor"].SetValue(new Vector3(0.5f, 0, 0));
            effect.Parameters["SpecularColor"].SetValue(new Vector3(0, 0, 0.5f));
            effect.Parameters["DiffuseTexture"].SetValue(texture);

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                foreach (ModelMesh mesh in model.Meshes)
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {
                        GraphicsDevice.SetVertexBuffer(part.VertexBuffer);
                        GraphicsDevice.Indices = part.IndexBuffer;
                        GraphicsDevice.DrawIndexedPrimitives(
                        PrimitiveType.TriangleList,
                        part.VertexOffset,
                        0,
                        part.PrimitiveCount);
                    }
            }
            base.Draw(gameTime);
        }
    }
}
