using UnityEngine;
using UnityEngine.UI;

public class EnemyUIStatus : MonoBehaviour
{
    //HP�\���pUI
    [SerializeField]
    private GameObject HPUI;
    public GameObject GetHPUI() { return HPUI; }
    //HP�\���p�X���C�_�[
    [SerializeField]
    private Slider hpSlider;
    public Slider HPSlider { get { return hpSlider; }set { hpSlider = value; } }

    public void SetHp(float hp,float maxhp)
    {

        //HP�\���pUI�̃A�b�v�f�[�g
        UpdateHPValue(hp,maxhp);

        if (hp != maxhp)
        {
            ActiveStatusUI(true);
        }
        if (hp <= 0)
        {
            //HP�\���pUI���\���ɂ���
            ActiveStatusUI(false);
        }
    }

    public void ActiveStatusUI(bool enabled)
    {
        HPUI.SetActive(enabled);
    }

    public bool IsActiveStatusUI() { return HPUI.activeSelf; }

    public void UpdateHPValue(float hp,float maxHp)
    {
        hpSlider.value = hp / maxHp;
    }
}
