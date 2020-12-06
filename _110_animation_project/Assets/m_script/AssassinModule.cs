using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssassinModule : MonoBehaviour
{
    [SerializeField] private bool canAssassinate = false;
    public bool CanAssassinate { get { return canAssassinate; } set { canAssassinate = value; } }


    private Transform assassinTarget = null;

    private bool isAssassinReady = false;
    public bool IsAssassinReady { get { return isAssassinReady; } set { isAssassinReady = value; } }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {   
        /*
        if (canAssassinate && Input.GetMouseButtonDown(0))
        {
            assassinReady();
        }*/
    }

    void FixedUpdate()
    {
        canAssassinate = false;

        if (!isAssassinReady)
        {
            assassinTarget = null;
        }
        
    }


    private void OnTriggerStay(Collider other)
    {
        if (other.transform.tag == "Enemy")
        {
            Quaternion rot = Quaternion.FromToRotation(transform.forward, other.transform.forward);
            float angle = rot.eulerAngles.y;
            //Debug.Log(rot.eulerAngles.y);
            if (Mathf.Abs(180.0f - angle) >= 120) // 300~80度
            {
                canAssassinate = true;
                assassinTarget = other.transform;
            }
        }                
    }

    /// <summary>
    /// 暗殺準備動作完成(瞬移到對方背後
    /// </summary>
    public void assassinReady()
    {
        if(assassinTarget != null)
        {
            Vector3 offset = -assassinTarget.forward*2;

            transform.position = assassinTarget.position + offset;
            transform.forward = assassinTarget.forward;
            assassinTarget.GetComponent<FindRoadController>().Assassinated();
            assassinTarget.GetComponent<EnemyAnimateController>().knightAssassinated();
            isAssassinReady = true;            
        }
    }


    /// <summary>
    /// 暗殺完成
    /// 把敵人的血扣光 死亡
    /// </summary>
    public void assassinFinish()
    {
        isAssassinReady = false;

        assassinTarget.GetComponentInChildren<HealthSystem>().isDamaged(100);

    }
}
