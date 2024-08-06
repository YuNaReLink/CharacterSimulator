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
    //Rayを飛ばす方向
    private Vector3             direction;
    //Rayを飛ばす距離
    [SerializeField]
    private float               distance = 10f;

    private void Start()
    {
        enemy = GetComponentInParent<EnemyBase>();
        if(enemy == null)
        {
            Debug.LogError("enemyがアタッチされませんでした");
        }
        searchArea = GetComponent<SphereCollider>();
        if(searchArea == null )
        {
            Debug.LogError("searchAreaがアタッチされませんでした");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.tag != "Player"){return;}
        enemy.SetPlayerScript(other.gameObject);
        //Rayを飛ばす方向を計算
        Vector3 temp = other.transform.position - transform.position;
        direction = temp.normalized;
        //Rayを飛ばす
        ray = new Ray(transform.position, direction);
        Debug.DrawRay(ray.origin, ray.direction * distance, Color.red);  // Rayをシーン上に描画
        //主人公の方向
        var playerDirection = other.transform.position - transform.position;
        //敵の前方からの主人公の方向
        var angle = Vector3.Angle(transform.forward, playerDirection);
        //サーチする角度内だったら発見
        if(angle <= searchAngle)
        {
            // Rayが最初に当たった物体を調べる
            if (Physics.Raycast(ray.origin, ray.direction * distance, out hit))
            {
                if (!hit.collider.CompareTag("WallFloor")&&!hit.collider.CompareTag("Obstacle")&&
                    !hit.collider.CompareTag("FocusSight"))
                {
                    
                    //Debug.Log("主人公発見:" + angle);
                    //敵の状態をセットする
                    enemy.SetState(ActionState.Tracking,EnemyState.Tracking);
                }
                else
                {
                    //Debug.Log("プレイヤーとの間に壁がある");
                }
            }
            
        }
        else
        {
            //Debug.Log("視界外です(Slime)");
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
        //敵の状態を変更
        enemy.SetState(ActionState.Idle,EnemyState.ResetPosition);
        enemy.UnlockPlayerScript(other.gameObject);
    }

    /*
#if UNITY_EDITOR
    //　サーチする角度表示
    private void OnDrawGizmos()
    {
        Handles.color = Color.red;
        Handles.DrawSolidArc(transform.position, Vector3.up, Quaternion.Euler(0f, -searchAngle, 0f) * transform.forward, searchAngle * 2f, searchArea.radius);
    }
#endif
     */
}
