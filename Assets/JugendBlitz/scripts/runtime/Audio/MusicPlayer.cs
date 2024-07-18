using System.Collections.Generic;
using UnityEngine;

namespace JugendBlitz.scripts.runtime.Audio
{
    public class MusicPlayer : MonoBehaviour
    {
        [SerializeField] private AudioClip gameMusicClip;
        [SerializeField] private AudioClip mainMenuMusicClip;
        [SerializeField] private AudioClip winSoundClip;

        private static Dictionary<MusicType, AudioSource> _audioSourcesDict;

        private void Awake()
        {
            _audioSourcesDict = new Dictionary<MusicType, AudioSource>
            {
                { MusicType.GameMusic, CreateAudioSource(gameMusicClip, false, false) },
                { MusicType.MainMenuMusic, CreateAudioSource(mainMenuMusicClip, false, true) },
                { MusicType.WinSound, CreateAudioSource(winSoundClip, false, false) },
            };
        }

        public static void StartMusic(MusicType type) => _audioSourcesDict[type].Play();

        public static void StopMusic(MusicType type) => _audioSourcesDict[type].Stop();
        
        public static void StopAllAudio()
        {
            foreach (var audioSource in _audioSourcesDict.Values) audioSource.Stop();
        }

        private AudioSource CreateAudioSource(AudioClip clip, bool playOnAwake, bool loop)
        {
            var audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = clip;
            audioSource.playOnAwake = playOnAwake;
            audioSource.loop = loop;

            return audioSource;
        }
    }
}