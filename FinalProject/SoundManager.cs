using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;

namespace FinalProject
{
    public static class SoundManager
    {
        private static Dictionary<string, SoundEffect> sounds = new Dictionary<string, SoundEffect>();

        public static void Load(ContentManager content)
        {
            // Make sure these match your file names in MGCB!
            try
            {
                sounds["teleport"] = content.Load<SoundEffect>("laserSmall_001");
            }
            catch
            {
                System.Diagnostics.Debug.WriteLine("Audio files missing! skipping...");
            }
        }

        public static void Play(string name)
        {
            if (sounds.ContainsKey(name))
            {
                // CreateInstance allows changing pitch/volume if needed later
                SoundEffectInstance instance = sounds[name].CreateInstance();
                instance.Volume = 0.5f; // 50% Volume so it doesn't blast ears
                instance.Play();
            }
        }
    }
}