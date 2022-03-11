using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySound : MonoBehaviour
{
    [SerializeField] private float volume;
    private AudioSource audioSource;

    private void Start() 
    {
        audioSource = GetComponent<AudioSource>();    
    }

    public void PlayKeyboardSound()
    {
        audioSource.volume = volume;
        audioSource.Play();
    }   
}
