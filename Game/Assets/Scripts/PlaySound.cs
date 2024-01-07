using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlaySound : MonoBehaviour
{
    public AudioSource soundPlayer;

    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void playThisSoundEffect(AudioClip audio)
    {
        soundPlayer.clip = audio;
        soundPlayer.Play();
    }
}
