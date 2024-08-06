using UnityEngine;

public class LifeGauge : MonoBehaviour
{
    //���C�t�Q�[�W�v���n�u
    [SerializeField]
    private GameObject lifeObject;
    [SerializeField]
    private GameObject lifeframe;
    //�L���ȃ��C�t
    private int enabledLife;

    //�ő僉�C�t
    private int maxLife;

    //���C�t�Q�[�W��S�폜&HP���쐬
    public void SetLifeGauge(float life)
    {
        maxLife = (int)life-1;
        //�̗͂���U�S�폜
        for(int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
        //���݂̗̑͐����̃��C�t�Q�[�W���쐬
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


    //�_���[�W�������폜(damage���n�[�g�����炷���l)
    public void SetLifeGauge2(float damage)
    {
        if (lifeObject == null) { return; }
        //�n�[�g�̃X�P�[���l���v������ϐ�
        if(maxLife < 0) { return; }
        Vector3 lastHeartScale = transform.GetChild(maxLife).localScale;
        //damage���珬���_�ȉ��𕪉��������l
        float fraction = damage - Mathf.Floor(damage);
        //����������ꍇ
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
