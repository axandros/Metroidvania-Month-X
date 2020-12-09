using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound
{
    public string Name;
    public AudioClip Clip;
    public bool Loop;
    [Range(0, 3)]
    public float Volume = 1;
    [HideInInspector]
    public AudioSource Source;
}

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private List<Sound> _GameSounds;

    private static AudioManager _instance;
    [Range(0,9)]
    public int Volume = 5;

    private void Awake()
    {
        if(_instance != null)
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
        foreach(Sound s in _GameSounds)
        {
            if(s.Clip != null)
            {
                s.Source = gameObject.AddComponent<AudioSource>();
                s.Source.clip = s.Clip;
                s.Source.volume = s.Volume;
                s.Source.loop = s.Loop;
            }
        }
        
    }

    void UpdateVolume()
    {
        float vol = this.Volume / 9.0f;
        Debug.Log("Setting volume to: " + vol);
        AudioListener.volume = vol; 
    }

    public static void SetVolume(int i)
    {
        _instance.Volume = Mathf.Clamp(i, 0, 9);
        _instance.UpdateVolume();
    }

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
            if(i >= _instance._GameSounds.Count) { i = -1; }
        }
        return i;
    }

    public static void Play(string clipName)
    {
        int i = GetIndex(clipName);
        Play(i);
    }

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

    public static void Stop(string clipName)
    {
        int i = GetIndex(clipName);
        Stop(i);
    }

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
