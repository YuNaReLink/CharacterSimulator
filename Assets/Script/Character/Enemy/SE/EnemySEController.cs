using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySEController : MonoBehaviour
{
    [SerializeField]
    AudioSource audioSource;

    public enum EnemySETag
    {
        Null = -1,
        Attack,
        Damage,
        Die,
        DataEnd
    }

    [SerializeField]
    private List<AudioClip> seClips = new List<AudioClip>();
    public List<AudioClip> SEClips() {  return seClips; }

    public void NoMotionByPlaySE(EnemySETag tag)
    {
        audioSource.PlayOneShot(seClips[(int)tag]);
    }

    public void AttackSEPlay()
    {
        audioSource.PlayOneShot(seClips[(int)EnemySETag.Attack]);
    }
}
