using System;
using UnityEngine;

//UnityのTime.deltaTimeを使ったカウントダウンのクラス
//CountDownとは違いフレームではなくタイマー方式でカウントする
public class TimeCountDown : InterfaceCountDown
{

    public event Action OnCompleted;

    private float count = 0;

    public void End()
    {
        count = 0;
        OnCompleted?.Invoke();
        OnCompleted = null;
    }
    //カウントが有効かどうか
    public bool IsEnabled() { return count > 0; }

    public bool MaxCount(float _count) { return count >= _count; }

    public void StartTimer(float _count)
    {
        count = _count;
    }

    // Update is called once per frame
    public void Update()
    {
        count -= Time.deltaTime;
        if (count <= 0)
        {
            End();
        }
    }
}
