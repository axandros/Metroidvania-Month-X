using System.Collections.Generic;
using UnityEngine;

namespace AxTools
{
    /// <summary>
    /// A Serialized sound class to add sounds to the Sound Manager.
    /// </summary>
    [System.Serializable]
    public class Sound
    {

        [Tooltip("The name of the sound clip for management.")]
        public string Name;

        [Tooltip("The audio clip asset to be played. ")]
        public AudioClip Clip;

        [Tooltip("Should the clip loop automatically?")]
        public bool Loop;
        
        [Tooltip("What is the volume of the clip?")]
        [Range(0, 1)]
        public float Volume = 1;


        ///<summary>The AudioSource that the Audio Manager generates.</summary>
        [HideInInspector]
        public AudioSource Source;
    }

    /// <summary>
    /// A Singleton class to manage audio across all scenes.
    /// Currently, additions made in one scene that are not included in the current active sudio manager will be lost,
    /// so all new sounds should be added to the prefab.
    /// </summary>
    [DisallowMultipleComponent]
    public class AudioManager : MonoBehaviour
    {
        [SerializeField, Tooltip("The list of sounds for the game.")]
        private List<Sound> _GameSounds;

        /// <summary>
        /// Singleton instance variable.
        /// </summary>
        private static AudioManager _instance;

        [Range(0, 1), Tooltip("The overall volume for all sounds.")]
        public float Volume = 1;

        // Unity Awake method.  Called when activated.
        private void Awake()
        {
            // Singleton handling.
            if (_instance != null)
            {
                _instance.UpdateVolume();
                Destroy(this);
                return;
            }
            else
            {
                _instance = this;
                _instance.UpdateVolume();
                DontDestroyOnLoad(this);
            }

            // Create Audio Source componenets.
            foreach (Sound s in _GameSounds)
            {
                if (s.Clip != null)
                {
                    s.Source = gameObject.AddComponent<AudioSource>();
                    s.Source.clip = s.Clip;
                    s.Source.volume = s.Volume;
                    s.Source.loop = s.Loop;
                }
            }

        }

        /// <summary> Function to change global volume. </summary>
        /// <remarks>Currently just updates the Audio Listener.</remarks>
        void UpdateVolume()
        {
            //Debug.Log("Setting volume to: " + _instance.Volume);
            AudioListener.volume = _instance.Volume;
        }

        /// <summary>
        /// Update the global volume.
        /// </summary>
        /// <param name="i">A value between 0 and 1. 0 is silent, 1 is full volume.</param>
        public static void SetVolume(float i)
        {
            _instance.Volume = Mathf.Clamp(i, 0, 1);
            _instance.UpdateVolume();
        }

        /// <summary>Get the index ID of the sound clip with the name. </summary>
        /// <remarks>Used to more quickly fetch the sound.</remarks>
        /// <param name="clipName">The string name of the clip to play.</param>
        /// <returns>Sound Index ID</returns>
        public static int GetIndex(string clipName)
        {
            int i = -2;
            if (_instance)
            {
                i = 0;
                for (i = 0; i < _instance._GameSounds.Count; i++)
                {
                    if (_instance._GameSounds[i].Name == clipName)
                    {
                        break;
                    }
                }
                if (i >= _instance._GameSounds.Count) { i = -1; }
            }
            return i;
        }

        /// <summary>Play a sound with the given name. 
        /// Does nothing if the sound does not exist. </summary>
        /// <param name="clipName">The string name of the clip to play.</param>
        public static void Play(string clipName)
        {
            int i = GetIndex(clipName);
            Play(i);
        }

        /// <summary>
        /// Play the sound at the given index.
        /// Does nothing if out of range.
        /// </summary>
        /// <param name="index">Sound Index ID</param>
        public static void Play(int index)
        {
            if (_instance)
            {
                if (index < _instance._GameSounds.Count
                    && index >= 0
                    && _instance._GameSounds[index] != null
                    && _instance._GameSounds[index].Source != null)
                {
                    _instance._GameSounds[index].Source.Play();
                }
            }
        }

        /// <summary>
        /// Stop playing the sound with the given name.
        /// </summary>
        /// <param name="clipName">The string name of the clip to stop.</param>
        public static void Stop(string clipName)
        {
            int i = GetIndex(clipName);
            Stop(i);
        }

        /// <summary>
        /// Stop playing the sound at the given sound ID.
        /// </summary>
        /// <param name="index">The sound Index ID.</param>
        public static void Stop(int index)
        {

            if (_instance)
            {
                if (index < _instance._GameSounds.Count
                    && index >= 0
                    && _instance._GameSounds[index] != null
                    && _instance._GameSounds[index].Source != null)
                {
                    _instance._GameSounds[index].Source.Stop();
                }
            }
        }
    }
}