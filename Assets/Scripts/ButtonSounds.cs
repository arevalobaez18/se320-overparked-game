
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(AudioSource))]
public class ButtonSounds : MonoBehaviour
{
    [Header("Select Sounds")]
    [Tooltip("Array of audio clips to randomly play on button click.")]
    [SerializeField] private AudioClip[] selectClips;

    [Header("Hover Sound")]
    [Tooltip("Audio clip to play when button is hovered.")]
    [SerializeField] private AudioClip hoverClip;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    // Call this from the button's OnClick event.
    public void PlayRandomClickSound()
    {
        if (selectClips != null && selectClips.Length > 0)
        {
            int index = Random.Range(0, selectClips.Length);
            audioSource.PlayOneShot(selectClips[index]);
        }
    }

    // Call this from an EventTrigger's PointerEnter event for hover.
    public void PlayHoverSound()
    {
        if (hoverClip != null)
        {
            audioSource.PlayOneShot(hoverClip);
        }
    }
}
