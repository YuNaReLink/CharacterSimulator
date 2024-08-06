using UnityEngine;

public class TargetLock_ON_UI : MonoBehaviour
{
    public void ActiveLock_ON_UI(GameUIController uIController)
    {
        Camera camera = Camera.main;
        PlayerController playerController = uIController.GetPlayerController();
        if(camera == null) { return; }
        if(playerController == null) { return; }
        if (playerController.GetFocusObject().FocusFlag)
        {
            if (!playerController.GetStateInput().IsMouseMiddle()) {
                uIController.LockONUIObject.gameObject.SetActive(false);
                return; 
            }
            uIController.LockONUIObject.gameObject.SetActive(true);
            // 3Dオブジェクトのワールド座標をスクリーン座標に変換
            Vector3 screenPosition = camera.WorldToScreenPoint(playerController.GetFocusObject().GetFocusObjectPosition());

            // CanvasのRectTransformを取得
            Canvas canvas = uIController.LockONUIObject.canvas;
            RectTransform canvasRect = canvas.GetComponent<RectTransform>();

            // スクリーン座標をキャンバスのローカル座標に変換
            Vector2 uiPosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPosition,null, out uiPosition);

            // UIオブジェクトの位置を設定

            uIController.LockONUIObject.rectTransform.localPosition = Vector2.Lerp(uIController.LockONUIObject.rectTransform.localPosition,uiPosition,Time.deltaTime * 100.0f);
        }
        else
        {
            uIController.LockONUIObject.gameObject.SetActive(false);
        }
    }
}
