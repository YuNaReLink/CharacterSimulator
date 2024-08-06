using UnityEngine;

public interface InterfaceAttackCommand
{
    float GetAttackNormalizedTime();
    void Execute();
}
