namespace HyperCasual.Core
{
    /// <summary>
    /// Encapsulates audio settings parameters
    /// </summary>
    public class AudioSettings
    {
        public bool EnableMusic;
        public bool EnableSfx;
        public float MasterVolume;

        public AudioSettings()
        {
            EnableMusic = true;
            EnableSfx = true;
            MasterVolume = 0.25f;
        }
    }
}