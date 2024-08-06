using System.Collections.Generic;
using UnityEngine;

public class GenerateEffects : MonoBehaviour
{
    //�e�I�u�W�F�N�g�ɃR���|�[�l���g����
    //�Z�b�g�����G�t�F�N�g�𐶐����邾���̃N���X
    [SerializeField]
    private List<GameObject> effects;
    //�G�t�F�N�g�𐶐�����
    public void GenerateEffect(int _state, Vector3 _pos)
    {
        if (effects[_state] == null)
        {
            Debug.LogWarning("�G�t�F�N�g���Z�b�g����Ă��܂���");
            return;
        }
        GameObject effect = Instantiate(effects[_state]);
        //�G�t�F�N�g����������ꏊ�����肷��
        effect.transform.position = _pos;
    }
}
