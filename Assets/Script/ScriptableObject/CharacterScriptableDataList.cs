using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CharacterManager;

[CreateAssetMenu]
public class CharacterScriptableDataList : ScriptableObject
{

    [SerializeField]
    private List<CharacterScriptableObject> characterDatas = new List<CharacterScriptableObject>();
    public CharacterScriptableObject SetData(DataTag tag)
    {
        int num = (int)tag;
        return characterDatas[num];
    }
}
