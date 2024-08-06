using System.Collections.Generic;
using UnityEngine;

public class SoundData : MonoBehaviour
{
    public enum BGMTag
    {
        Null = -1,
        TitleBGM,
        GameBGM01,
        GameBGM02,
        GameBGM03,
        GameCler01,
        Battle,
        DataEnd
    }

    public enum SETag
    {
        Null = -1,
        GameCler02,
        GameCler03,
        GameOver01,
        GameOver02,
        GameOver03,
        DataEnd
    }

    [SerializeField]
    private List<AudioClip> bgmClips = new List<AudioClip>();

    public List<AudioClip> GetBGMClips() {  return bgmClips; }

    [SerializeField]
    private List<AudioClip> seClips = new List<AudioClip>();

    public List<AudioClip> GetSEClips() { return seClips; }
}
