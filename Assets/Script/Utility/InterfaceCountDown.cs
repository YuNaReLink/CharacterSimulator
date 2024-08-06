using System;

public interface InterfaceCountDown
{
    void End();
    bool IsEnabled();
    void Update();

    event Action OnCompleted;
}