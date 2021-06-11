using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _buttonClickClip;
    [SerializeField] private List<AudioNameWithClip> _audioClips;

    private Dictionary<string, AudioClip> _audioClipsDictionary;


    private void Start()
    {
        if (PlayerPrefs.HasKey("Volume"))
            AudioListener.volume = PlayerPrefs.GetFloat("Volume"); //Load volume setting from PlayerPrefs

        //Transfer clips from list to dictionary for faster search. We cant serialize dictionary
        _audioClipsDictionary = new Dictionary<string, AudioClip>();
        for (int i = 0; i < _audioClips.Count; i++)
        {
            _audioClipsDictionary.Add(_audioClips[i].Name, _audioClips[i].Clip);
        }
    }

    public void OnVolumeChanged(EventMessage floatMessage)
    {
        float newVolumeValue = ((FloatMessage)floatMessage).FloatValue;
        PlayerPrefs.SetFloat("Volume", newVolumeValue);
        AudioListener.volume = PlayerPrefs.GetFloat("Volume");
    }

    public void PlayButtonClickSound()
    {
        if (_buttonClickClip != null)
        { 
            _audioSource.clip = _buttonClickClip;
            _audioSource.Play();
        }
    }

    public void OnRequestSound(EventMessage stringMessage)
    {
        string audioClipName = ((StringMessage)stringMessage).StringValue;
        if (_audioClipsDictionary.ContainsKey(audioClipName))
        {
            if (_audioClipsDictionary[audioClipName] != null)
            {
                _audioSource.clip = _audioClipsDictionary[audioClipName];
                _audioSource.Play();
            }
        }
        
    }


    [Serializable]
    private struct AudioNameWithClip
    {
        public string Name;
        public AudioClip Clip;
    }
}

