using UnityEngine;

public class LifeGauge : MonoBehaviour
{
    //ライフゲージプレハブ
    [SerializeField]
    private GameObject lifeObject;
    [SerializeField]
    private GameObject lifeframe;
    //有効なライフ
    private int enabledLife;

    //最大ライフ
    private int maxLife;

    //ライフゲージを全削除&HP分作成
    public void SetLifeGauge(float life)
    {
        maxLife = (int)life-1;
        //体力を一旦全削除
        for(int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
        //現在の体力数分のライフゲージを作成
        for(int i = 0; i < life; i++)
        {
            if(lifeObject != null)
            {
                Instantiate<GameObject>(lifeObject, transform);
            }
            else
            {
                Instantiate<GameObject>(lifeframe, transform);
            }
        }
    }


    //ダメージ分だけ削除(damageがハートを減らす数値)
    public void SetLifeGauge2(float damage)
    {
        if (lifeObject == null) { return; }
        //ハートのスケール値を計測する変数
        if(maxLife < 0) { return; }
        Vector3 lastHeartScale = transform.GetChild(maxLife).localScale;
        //damageから小数点以下を分解した数値
        float fraction = damage - Mathf.Floor(damage);
        //小数がある場合
        if(fraction != 0)
        {
            transform.GetChild(maxLife).localScale -= new Vector3(fraction, fraction, fraction);
            if (transform.GetChild(maxLife).localScale.x <= 0)
            {
                maxLife -= 1;
            }
            damage -= fraction;
            while(damage > 0)
            {
                float halfdamage = damage * transform.GetChild(maxLife).localScale.x;
                transform.GetChild(maxLife).localScale -= new Vector3(halfdamage, halfdamage, halfdamage);
                if(transform.GetChild(maxLife).localScale.x <= 0)
                {
                    maxLife -= 1;
                }
                damage -= halfdamage;
                if(maxLife < 0)
                {
                    return;
                }
            }
        }
        else
        {
            while (damage > 0)
            {
                float onedamage = damage;
                if(damage > 1)
                {
                    onedamage /= damage;
                }
                float halfdamage = onedamage * transform.GetChild(maxLife).localScale.x;
                transform.GetChild(maxLife).localScale -= new Vector3(halfdamage, halfdamage, halfdamage);
                if (transform.GetChild(maxLife).localScale.x <= 0)
                {
                    maxLife -= 1;
                }
                damage -= halfdamage;
                if (maxLife < 0)
                {
                    return;
                }
            }
        }
    }
}
