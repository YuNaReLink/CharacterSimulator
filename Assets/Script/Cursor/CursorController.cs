using UnityEngine;

public class CursorController
{
    public static void SetCursorLookMode(CursorLockMode mode)
    {
        Cursor.lockState = mode;
    }
    public static void SetCursorState(bool enabled)
    {
        Cursor.visible = enabled;
    }
}
