using UnityEngine;

public class BulletShot : MonoBehaviour
{
    [SerializeField]
    private GameObject      bullet;
    private GameObject      pseudoBullet;
    [SerializeField]
    private float           bulletSpeed;

    private bool            rug = false;

    private TimeCountDown   timer_Explosion;
    [SerializeField]
    private float explosionTimerCount = 2.0f;

    private GunSEController gunSEController;

    private void Start()
    {
        timer_Explosion = new TimeCountDown();
        if(timer_Explosion == null)
        {
            Debug.LogError("timer_Explpsionが生成されていません");
        }
        gunSEController = GetComponent<GunSEController>();
        if(gunSEController == null)
        {
            Debug.LogError("gunSEControllerがアタッチされていません");
        }
    }

    private void FixedUpdate()
    {
        if (timer_Explosion.IsEnabled())
        {
            timer_Explosion.Update();
        }
        else
        {
            timer_Explosion.End();
        }
    }

    private void LateUpdate()
    {
        if (rug && !timer_Explosion.IsEnabled())
        {
            ROG();
        }
    }

    public void FireBullet()
    {
        if(rug != false) { return; }
        pseudoBullet = Instantiate(bullet,transform.position,Quaternion.Euler(transform.parent.eulerAngles.x, transform.parent.eulerAngles.y, 0));
        Rigidbody bulletrb = pseudoBullet.GetComponent<Rigidbody>();
        bulletrb.AddForce(transform.forward * -bulletSpeed);
        timer_Explosion.StartTimer(explosionTimerCount);
        rug = true;
        gunSEController.GunSEPlay();
    }

    private void ROG()
    {
        rug = false;
    }
}
