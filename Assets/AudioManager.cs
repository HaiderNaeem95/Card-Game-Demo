using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource[] clips;

    public void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void playSound(int i)
    {
        clips[i].Play();
    }
}
