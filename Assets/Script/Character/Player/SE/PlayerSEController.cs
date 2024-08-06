using UnityEngine;

public class PlayerSEController : MonoBehaviour
{
    [SerializeField]
    private AudioSource         audioSource;

    [SerializeField]
    private AudioClip   damageSound;

    [SerializeField]
    private AudioClip   footstepSound;
    [SerializeField]
    private AudioClip   jumpSound;
    [SerializeField]
    private AudioClip   rollingSound;
    [SerializeField]
    private AudioClip   flipSound;
    [SerializeField]
    private AudioClip   weaponSetOutSound;
    [SerializeField]
    private AudioClip   weaponSetInSound;
    [SerializeField]
    private AudioClip   swordSound;
    [SerializeField]
    private AudioClip   lastSwordSound;
    [SerializeField]
    private AudioClip   spinSwordSound;
    [SerializeField]
    private AudioClip   shieldSound;
    [SerializeField]
    private AudioClip   defenseSound;
    [SerializeField]
    private AudioClip   rotateSound;
    private bool        rotateSEFlag = false;
    public bool         RotateSEFlag { get { return rotateSEFlag; } set {  rotateSEFlag = value; } }
    [SerializeField]
    private AudioClip   hipDropSound;
    private bool        hipDropSEFlag = false;
    public bool         HipDropSEFlag { get { return hipDropSEFlag; } set {  hipDropSEFlag = value; } }
    [SerializeField]
    private AudioClip   bombSound;
    [SerializeField]
    private AudioClip   grabSound;
    [SerializeField]
    private AudioClip   climbWallSound;
    [SerializeField]
    private AudioClip   stepJumpSound;

    public void DamageSEPlay()
    {
        audioSource.PlayOneShot(damageSound);
    }

    public void RunSEPlay()
    {
        audioSource.PlayOneShot(footstepSound);
    }

    public void JumpSEPlay()
    {
        audioSource.PlayOneShot(jumpSound);
    }
    public void RollingSEPlay()
    {
        audioSource.PlayOneShot(rollingSound);
    }
    public void FlipSEPlay()
    {
        audioSource.PlayOneShot(flipSound);
    }
    public void WeaponSetSound(bool _enabled)
    {
        if (_enabled)
        {
            audioSource.PlayOneShot(weaponSetOutSound);
        }
        else
        {
            audioSource.PlayOneShot(weaponSetInSound);
        }
    }
    public void SwordSEPlay()
    {
        audioSource.PlayOneShot(swordSound);
    }
    public void LastSwordSEPlay()
    {
        audioSource.PlayOneShot(lastSwordSound);
    }
    public void SpinSwordAttackSEPlay()
    {
        audioSource.PlayOneShot(spinSwordSound);
    }
    public void ShieldSEPlay()
    {
        audioSource.PlayOneShot(shieldSound);
    }

    public void DefenseSEPlay()
    {
        audioSource.PlayOneShot(defenseSound);
    }

    public void RotateBodySEPlay()
    {
        audioSource.PlayOneShot(rotateSound);
    }
    public void HipDropSEPlay()
    {
        audioSource.PlayOneShot(hipDropSound);
    }
    public void BombSEPlay()
    {
        audioSource.PlayOneShot(bombSound);
    }

    public void GrabSEPlay()
    {
        audioSource.PlayOneShot(grabSound);
    }

    public void ClimbWallSEPlay()
    {
        audioSource.PlayOneShot(climbWallSound);
    }

    public void StepJumpSEPlay()
    {
        audioSource.PlayOneShot(stepJumpSound);
    }
}
