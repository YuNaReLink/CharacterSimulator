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
            // 3D�I�u�W�F�N�g�̃��[���h���W���X�N���[�����W�ɕϊ�
            Vector3 screenPosition = camera.WorldToScreenPoint(playerController.GetFocusObject().GetFocusObjectPosition());

            // Canvas��RectTransform���擾
            Canvas canvas = uIController.LockONUIObject.canvas;
            RectTransform canvasRect = canvas.GetComponent<RectTransform>();

            // �X�N���[�����W���L�����o�X�̃��[�J�����W�ɕϊ�
            Vector2 uiPosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPosition,null, out uiPosition);

            // UI�I�u�W�F�N�g�̈ʒu��ݒ�

            uIController.LockONUIObject.rectTransform.localPosition = Vector2.Lerp(uIController.LockONUIObject.rectTransform.localPosition,uiPosition,Time.deltaTime * 100.0f);
        }
        else
        {
            uIController.LockONUIObject.gameObject.SetActive(false);
        }
    }
}
