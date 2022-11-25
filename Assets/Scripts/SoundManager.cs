using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    [SerializeField] AudioSource EffectSource;
    [SerializeField] AudioSource MusicSource;
    float highPitched = 1.1f;
    float lowPitched = 0.9f;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    public void PlayMusic(AudioClip clip)
    {
        float randomPitch = Random.Range(lowPitched, highPitched);
        EffectSource.pitch = randomPitch;
        EffectSource.clip = clip;
        EffectSource.Play();
    }

    public void UnloadMusic(AudioClip clip)
    {
        clip.UnloadAudioData();
        EffectSource.clip = null;
    }

    public void PlayEffect(AudioClip clip)
    {
        MusicSource.clip = clip;
        MusicSource.Play();
    }
    
}
