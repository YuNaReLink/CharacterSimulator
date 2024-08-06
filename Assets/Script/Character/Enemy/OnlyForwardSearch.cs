using UnityEditor;
using UnityEngine;
using static CharacterManager;

public class OnlyForwardSearch : MonoBehaviour
{
    private EnemyBase           enemy;
    private SphereCollider      searchArea;
    [SerializeField]
    private float               searchAngle = 130f;

    private Ray                 ray;
    private RaycastHit          hit;
    //Ray���΂�����
    private Vector3             direction;
    //Ray���΂�����
    [SerializeField]
    private float               distance = 10f;

    private void Start()
    {
        enemy = GetComponentInParent<EnemyBase>();
        if(enemy == null)
        {
            Debug.LogError("enemy���A�^�b�`����܂���ł���");
        }
        searchArea = GetComponent<SphereCollider>();
        if(searchArea == null )
        {
            Debug.LogError("searchArea���A�^�b�`����܂���ł���");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.tag != "Player"){return;}
        enemy.SetPlayerScript(other.gameObject);
        //Ray���΂��������v�Z
        Vector3 temp = other.transform.position - transform.position;
        direction = temp.normalized;
        //Ray���΂�
        ray = new Ray(transform.position, direction);
        Debug.DrawRay(ray.origin, ray.direction * distance, Color.red);  // Ray���V�[����ɕ`��
        //��l���̕���
        var playerDirection = other.transform.position - transform.position;
        //�G�̑O������̎�l���̕���
        var angle = Vector3.Angle(transform.forward, playerDirection);
        //�T�[�`����p�x���������甭��
        if(angle <= searchAngle)
        {
            // Ray���ŏ��ɓ����������̂𒲂ׂ�
            if (Physics.Raycast(ray.origin, ray.direction * distance, out hit))
            {
                if (!hit.collider.CompareTag("WallFloor")&&!hit.collider.CompareTag("Obstacle")&&
                    !hit.collider.CompareTag("FocusSight"))
                {
                    
                    //Debug.Log("��l������:" + angle);
                    //�G�̏�Ԃ��Z�b�g����
                    enemy.SetState(ActionState.Tracking,EnemyState.Tracking);
                }
                else
                {
                    //Debug.Log("�v���C���[�Ƃ̊Ԃɕǂ�����");
                }
            }
            
        }
        else
        {
            //Debug.Log("���E�O�ł�(Slime)");
        }
    }

    private bool SearchEnabled(RaycastHit hit)
    {
        switch(hit.collider.tag)
        {
            case "WallFloor":
            case "Obstacle":
            case "FocusSight":
                return false;
        }
        return true;
    }

    private void OnTriggerExit(Collider other)
    {
        if( other.tag != "Player") {  return; }
        //�G�̏�Ԃ�ύX
        enemy.SetState(ActionState.Idle,EnemyState.ResetPosition);
        enemy.UnlockPlayerScript(other.gameObject);
    }

    /*
#if UNITY_EDITOR
    //�@�T�[�`����p�x�\��
    private void OnDrawGizmos()
    {
        Handles.color = Color.red;
        Handles.DrawSolidArc(transform.position, Vector3.up, Quaternion.Euler(0f, -searchAngle, 0f) * transform.forward, searchAngle * 2f, searchArea.radius);
    }
#endif
     */
}
