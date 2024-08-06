using UnityEngine;
using UnityEngine.UI;

public class EnemyUIStatus : MonoBehaviour
{
    //HP表示用UI
    [SerializeField]
    private GameObject HPUI;
    public GameObject GetHPUI() { return HPUI; }
    //HP表示用スライダー
    [SerializeField]
    private Slider hpSlider;
    public Slider HPSlider { get { return hpSlider; }set { hpSlider = value; } }

    public void SetHp(float hp,float maxhp)
    {

        //HP表示用UIのアップデート
        UpdateHPValue(hp,maxhp);

        if (hp != maxhp)
        {
            ActiveStatusUI(true);
        }
        if (hp <= 0)
        {
            //HP表示用UIを非表示にする
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
