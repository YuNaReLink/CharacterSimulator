using UnityEngine;

public class BombBullet : MonoBehaviour
{
    [SerializeField]
    private float           bombCount = 3.0f;
    [SerializeField]
    private SphereCollider  bulletBody;
    [SerializeField]
    private SphereCollider  bombRange;
    private GenerateEffects generateEffects;

    private TimeCountDown   timer_BombCount;

    private bool            effctFlag = false;

    // Start is called before the first frame update
    void Start()
    {
        generateEffects = GetComponent<GenerateEffects>();
        if (generateEffects == null)
        {
            Debug.LogError("generateEffectsがアタッチされていません"); 
        }
        timer_BombCount = new TimeCountDown();
        if(timer_BombCount != null)
        {
            effctFlag = true;
            timer_BombCount.StartTimer(bombCount);
        }
        else
        {
            Debug.LogError("timer_BombCountが生成されませんでした");
        }
    }

    private void Update()
    {
        if (timer_BombCount.IsEnabled())
        {
            timer_BombCount.Update();
        }
        else
        {
            timer_BombCount.End();
        }
    }

    private void LateUpdate()
    {
        if(!timer_BombCount.IsEnabled())
        {
            ExplosionBullet();
        }
    }

    private void ExplosionBullet()
    {
        if (!bombRange.enabled)
        {
            bombRange.enabled = true;
        }
        if (effctFlag)
        {
            generateEffects.GenerateEffect(0, transform.position);
            effctFlag = false;
        }
        Destroy(gameObject,0.5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag != "Enemy") { return; }
        timer_BombCount.End();
    }

    private void OnCollisionEnter(Collision collision)
    {
        timer_BombCount.End();
    }
}
