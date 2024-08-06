using UnityEngine;

public class ClearFlag
{
    private static bool clearFlag = false;

    public static bool IsClearFlag() {  return clearFlag; }
    public static bool SetClearFlag(bool _enabled) { return clearFlag = _enabled; }
}
