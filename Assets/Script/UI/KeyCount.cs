using UnityEngine;
using UnityEngine.UI;

//����UI�̃J�E���g�̃e�L�X�g�̏��������Ă���N���X
public class KeyCount : MonoBehaviour
{
    private Text keyCountText = null;

    public Text GetKeyCountText() {  return keyCountText; }
    void Start()
    {
        keyCountText = GetComponent<Text>();
    }
}
