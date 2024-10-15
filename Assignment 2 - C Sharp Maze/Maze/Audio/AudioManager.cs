using Maze.Gamestate;
using Microsoft.Xna.Framework.Audio;

namespace Maze.Audio
{
    // Manages audio assets and their state.
    public class AudioManager
    {
        // Music.
        public SoundEffect backgroundMusic { get; private set; }
        public SoundEffectInstance backgroundMusicInstance { get; private set; }

        public void ConfigureAudio()
        {
            // Music setup
            backgroundMusicInstance.IsLooped = true;
            backgroundMusicInstance.Volume = 0.25f;
            backgroundMusicInstance.Play();
        }

        // Load music.
        public void LoadAudio()
        {
            backgroundMusic = ContentManagerHandle.Content.Load<SoundEffect>("This-is-Life");
            backgroundMusicInstance = backgroundMusic.CreateInstance();
        }
    }
}
