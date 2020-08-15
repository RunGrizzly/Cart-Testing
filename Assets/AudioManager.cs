using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    public AudioSource musicSource;

    public List<AudioClip> musicClips = new List<AudioClip>();

    // Start is called before the first frame update
    void Start()
    {

        musicSource.clip = musicClips[0];
        musicSource.Play();
        musicSource.loop = true;
    }

    // Update is called once per frame
    void Update()
    {

    }
}