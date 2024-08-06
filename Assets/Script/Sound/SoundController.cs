using UnityEngine;

public class SoundController : MonoBehaviour
{

    [SerializeField]
    private AudioSource bgmAudioSource;

    private AudioClip saveBGMClip;

    private bool changeBGM = false;

    [SerializeField]
    private AudioSource seAudioSource;

    [SerializeField]
    private float bgmVolumData = 0.2f;
    public float GetBGMVolumData() {  return bgmVolumData; }

    public float BGMVolume
    {
        get
        {
            return bgmAudioSource.volume;
        }
        set
        {
            bgmAudioSource.volume = Mathf.Clamp01(value);
        }
    }

    public float SeVolume
    {
        get
        {
            return seAudioSource.volume;
        }
        set
        {
            seAudioSource.volume = Mathf.Clamp01(value);
        }
    }

    void Start()
    {
        GameObject soundManagerObject = CheckOtherSoundManager();
        bool checkResult = soundManagerObject != null && soundManagerObject != gameObject;
        if (checkResult)
        {
            Destroy(gameObject);
        }

        changeBGM = false;

        DontDestroyOnLoad(gameObject);
    }

    private GameObject CheckOtherSoundManager()
    {
        return GameObject.FindGameObjectWithTag("SoundManager");
    }

    public void StopBGM()
    {
        if (bgmAudioSource == null) { return; }
        bgmAudioSource.Stop();
        bgmAudioSource.clip = null;
    }

    public void StopSE()
    {
        if (seAudioSource == null) { return; }
        seAudioSource.Stop();
        seAudioSource.clip = null;
    }

    public void PlayBgm(AudioClip clip)
    {
        if(bgmAudioSource.clip == clip) { return; }
        if (changeBGM) { return; }
        if(bgmAudioSource.clip == null)
        {
            bgmAudioSource.clip = clip;
            if (clip == null){return;}
            bgmAudioSource.Play();
        }
        else
        {
            saveBGMClip = clip;
            changeBGM = true;
        }
    }

    public void PlaySe(AudioClip clip)
    {
        if (clip == null)
        {
            return;
        }

        seAudioSource.PlayOneShot(clip);
    }

    public void ChangeBGMUpdtate()
    {
        if (changeBGM)
        {
            BGMVolume -= 0.01f;
            if(BGMVolume <= 0 )
            {
                BGMVolume = 0;
                changeBGM = false;
                bgmAudioSource.clip = saveBGMClip;
                bgmAudioSource.Play();
            }
        }
        else if(BGMVolume <= bgmVolumData)
        {
            BGMVolume += 0.01f;
            if( BGMVolume >= bgmVolumData)
            {
                BGMVolume= bgmVolumData;
            }
        }
    }
}
