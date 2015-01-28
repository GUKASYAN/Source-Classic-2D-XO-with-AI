using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    public List<AudioClip> Sounds;
    public static SoundManager Instance;

    private void Start()
    {
        Instance = this;
    }
    public void PlayChecked()
    {
        AudioSource.PlayClipAtPoint(Sounds[0], Vector3.zero);
    }
      public void PlayFalse()
    {
        AudioSource.PlayClipAtPoint(Sounds[1],Vector3.zero);
    }
    public void SoundTumbler()
    {
        audio.mute = !audio.mute;
    }
}