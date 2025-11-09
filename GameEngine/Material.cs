using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;


namespace CPI311.GameEngine
{
    public class Material
    {
        public Effect effect;
        public Texture2D Texture;
        public Vector3 Diffuse { get; set; }
        public Vector3 Ambient { get; set; }
        public Vector3 Specular { get; set; }
        public float Shininess { get; set; }
        public Matrix World { get; set; }
        public Camera Camera { get; set; }
        public Light Light { get; set; }
        public int Passes { get { return effect.CurrentTechnique.Passes.Count; } }
        public int CurrentTechnique { get; set; }
        // *** 1. ADD NEW PROPERTIES FOR TILING/OFFSET ***
        public Vector2 Tiling { get; set; }
        public Vector2 Offset { get; set; }
        public Material(Matrix world, Camera camera, Light light, ContentManager
        content,
        string filename, int currrentTechnique, float shininess, Texture2D
        texture)
        {
            effect = content.Load<Effect>(filename);
            World = world;
            Camera = camera;
            Light = light;
            Shininess = shininess;
            // *********************************
            CurrentTechnique = currrentTechnique;
            //************************************
            Texture = texture;
            Diffuse = Color.Gray.ToVector3();
            Ambient = Color.Gray.ToVector3();
            Specular = Color.Gray.ToVector3();
            // *** 2. INITIALIZE TILING AND OFFSET PROPERTIES ***
            Tiling = Vector2.One; // Default tiling is (1, 1)
            Offset = Vector2.Zero; // Default offset is (0, 0)
        }
        public virtual void Apply(int currentPass)
        {
            effect.CurrentTechnique = effect.Techniques[CurrentTechnique];
            effect.Parameters["World"].SetValue(World);
            effect.Parameters["View"].SetValue(Camera.View);
            effect.Parameters["Projection"].SetValue(Camera.Projection);
            effect.Parameters["CameraPosition"].SetValue(Camera.Transform.Position);
            effect.Parameters["LightPosition"].SetValue(Light.Transform.Position);
            effect.Parameters["DiffuseColor"].SetValue(Diffuse);
            effect.Parameters["AmbientColor"].SetValue(Ambient);
            effect.Parameters["SpecularColor"].SetValue(Specular);
            effect.Parameters["Shininess"].SetValue(Shininess);
            effect.Parameters["DiffuseTexture"].SetValue(Texture);
            // *** 3. PASS NEW VALUES TO THE SHADER ***
            effect.Parameters["Tiling"]?.SetValue(Tiling);
            effect.Parameters["Offset"]?.SetValue(Offset);
            effect.CurrentTechnique.Passes[currentPass].Apply();
        }
    }
}
