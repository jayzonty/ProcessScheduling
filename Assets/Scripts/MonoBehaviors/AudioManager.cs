using UnityEngine;

namespace ProcessScheduling
{
    /// <summary>
    /// Script for handling audio-related functions
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        /// <summary>
        /// Default audio clip to be played in a level
        /// </summary>
        public AudioClip defaultLevelAudioClip;

        /// <summary>
        /// Reference to the AudioSource component
        /// </summary>
        private AudioSource audioSource;

        /// <summary>
        /// Play the provided audio clip
        /// </summary>
        /// <param name="audioClip">Audio clip to play</param>
        public void PlayAudioClip(AudioClip audioClip, bool isLooping = false)
        {
            if (audioClip == null)
            {
                return;
            }

            if (audioSource != null)
            {
                audioSource.loop = isLooping;
                audioSource.clip = audioClip;
                audioSource.Play();
            }
        }

        /// <summary>
        /// Stops the audio clip that is currently playing
        /// </summary>
        public void StopCurrentAudioClip()
        {
            if (audioSource != null)
            {
                audioSource.Stop();
            }
        }

        /// <summary>
        /// Unity callback function called when the script
        /// is created.
        /// </summary>
        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        /// <summary>
        /// Unity callback function called before the first
        /// Update() call.
        /// </summary>
        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
