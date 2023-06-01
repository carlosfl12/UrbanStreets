using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip[] audios;
    public int currentAudio = 0;
    public AudioSource audioSource;
    public float timeToChangeAudio = 2f;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (!audioSource.isPlaying) {
            timeToChangeAudio += 1 * Time.deltaTime;
            SelectNextAudio();
        }
    }

    public void SelectNextAudio() {
        audioSource.Stop();
        currentAudio = currentAudio + 1 % audios.Length;
        audioSource.clip = audios[currentAudio];
        if (timeToChangeAudio > 3) {
            audioSource.Play();
            timeToChangeAudio = 0f;
        }
    }
}
