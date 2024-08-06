using UnityEngine;

public class GunSEController : MonoBehaviour
{
    [SerializeField]
    private AudioClip   gunSound;
    [SerializeField]
    private AudioSource Source;

    public void GunSEPlay()
    {
        if (gunSound == null) { return; }
        if (Source.isPlaying) { return; }
        Source.PlayOneShot(gunSound);
    }
}
