using UnityEngine;

public class FocusPlayerUI : MonoBehaviour
{
    void LateUpdate()
    {
        //　カメラと同じ向きに設定
        transform.rotation = Camera.main.transform.rotation;
    }
}
