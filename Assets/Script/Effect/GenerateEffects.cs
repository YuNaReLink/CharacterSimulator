using System.Collections.Generic;
using UnityEngine;

public class GenerateEffects : MonoBehaviour
{
    //各オブジェクトにコンポーネントして
    //セットしたエフェクトを生成するだけのクラス
    [SerializeField]
    private List<GameObject> effects;
    //エフェクトを生成する
    public void GenerateEffect(int _state, Vector3 _pos)
    {
        if (effects[_state] == null)
        {
            Debug.LogWarning("エフェクトがセットされていません");
            return;
        }
        GameObject effect = Instantiate(effects[_state]);
        //エフェクトが発生する場所を決定する
        effect.transform.position = _pos;
    }
}
