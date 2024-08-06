using System;

public class CountDown : InterfaceCountDown
{

    public event Action OnCompleted;

    private int count = 0;

    public void End()
    {
        count = 0;
        OnCompleted?.Invoke();
        OnCompleted = null;
    }
    //カウントが有効かどうか
    public bool IsEnabled() { return count > 0; }

    public bool MaxCount(int _count) {  return count >= _count; }

    public void StartTimer(int _count)
    {
        count = _count;
    }

    // Update is called once per frame
    public void Update()
    {
        count--;
        if (count <= 0)
        {
            End();
        }
    }
}
