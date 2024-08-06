using UnityEngine;

public class PropellerSEController : MonoBehaviour
{
    [SerializeField]
    private AudioClip   propellerSound;
    [SerializeField]
    private AudioSource propellerSource;

    public void PropellerSEPlay()
    {
        if(propellerSound == null) { return; }
        if (propellerSource.isPlaying) { return; }
        propellerSource.PlayOneShot(propellerSound);
    }
    public void PropellerSEStop()
    {
        if(propellerSound == null) { return; }
        propellerSource.Stop();
    }
}
