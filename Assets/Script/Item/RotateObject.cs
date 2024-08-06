using UnityEngine;

public class RotateObject : MonoBehaviour
{
    [SerializeField]
    private float rotateSpeed = 1.0f;

    void Update()
    {
        if (Mathf.Approximately(Time.timeScale, 0f)) { return; }
        transform.Rotate(0, rotateSpeed, 0);
    }
}
