using System;
using UnityEngine;

public class SFXAudioObserver : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    public static event Action<AudioSource> OnPlaySFX; 

    private SFXAudioObserver(){}

    private void Awake()
    {
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        OnPlaySFX?.Invoke(audioSource);
    }
}
