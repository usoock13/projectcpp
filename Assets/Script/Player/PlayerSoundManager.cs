using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundManager : MonoBehaviour
{
    AudioSource playerAudioSource;

    public AudioClip[] basicAttackSounds;
    public AudioClip dodgeSound;
    public AudioClip dodgeAttackSound;
    public AudioClip dodgeAttackHitSound;

    void Start() {
        playerAudioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(AudioClip clip) {
        playerAudioSource.PlayOneShot(clip);
    }
}
