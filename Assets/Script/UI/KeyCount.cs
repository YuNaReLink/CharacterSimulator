using UnityEngine;
using UnityEngine.UI;

//鍵のUIのカウントのテキストの情報を持っているクラス
public class KeyCount : MonoBehaviour
{
    private Text keyCountText = null;

    public Text GetKeyCountText() {  return keyCountText; }
    void Start()
    {
        keyCountText = GetComponent<Text>();
    }
}
