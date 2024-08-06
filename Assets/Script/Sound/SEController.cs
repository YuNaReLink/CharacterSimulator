using System.Collections.Generic;
using UnityEngine;

public class SEController : MonoBehaviour
{
    private AudioSource audioSource;

    [SerializeField]
    private List<AudioClip> clips = new List<AudioClip>();
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void SEPlay(int value)
    {
        audioSource.PlayOneShot(clips[value]);
    }
}
