using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage_script : MonoBehaviour
{
    public bool onAttack = false;
    public float damage_status = 1;
    public float damage_dealt = 25;
    HealthSystem npc_HS;
    EnemyAnimateController npc_EnemyAC;
    public void Attacking()             //在CM中的call_attacking
    {
        onAttack = true;
    }
    public void Stop_Attacking()
    {
        onAttack = false; ;
    }

    private int getangle(Transform target)          //判斷是否在指定角度內 是則傳1  否則傳0   目前都是以武器往上3階層
    {
        Transform this_obj = transform.parent.parent.parent;
        Vector3 targetDir = this_obj.position - target.position;
        targetDir.y = 0;
        float angle = Vector3.Angle(target.forward, targetDir);
       // Debug.Log(angle);
        if (angle <= 80.0f)
        {
            return 1;
        }
        else return 0;
    }


    private void OnTriggerEnter(Collider other)
    {
        
        if (other.tag == "Enemy")
        {
            
            npc_HS = other.GetComponentInChildren<HealthSystem>();
            npc_EnemyAC = other.GetComponentInChildren<EnemyAnimateController>();
            if (!npc_HS.Ishitted && onAttack == true && npc_EnemyAC.Isblocking == true && getangle(other.transform) == 1)       //阻擋後對方的ishitted轉為TRUE 且判定對方角度  以及當前是否為防禦中
            {
                Debug.Log("BLOCKED");
                npc_HS.Ishitted = true;
                npc_HS.got_blocked();                // got_blocked  在HC裡

            }
            else if (!npc_HS.Ishitted && onAttack == true)
            {
                //Debug.Log("before hit" + other_HC.ishitted);
                npc_HS.Ishitted = true;
                //Debug.Log("after hit" );
                npc_HS.isDamaged(20);
            }
        }

    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Enemy")
        {

            npc_HS = other.GetComponentInChildren<HealthSystem>();
            npc_EnemyAC = other.GetComponentInChildren<EnemyAnimateController>();
            if (!npc_HS.Ishitted && onAttack == true && npc_EnemyAC.Isblocking == true && getangle(other.transform) == 1)       //阻擋後對方的ishitted轉為TRUE 且判定對方角度  以及當前是否為防禦中
            {
                Debug.Log("BLOCKED");
                npc_HS.Ishitted = true;
                npc_HS.got_blocked();                // got_blocked  在HC裡

            }
            else if (!npc_HS.Ishitted && onAttack == true)
            {
                //Debug.Log("before hit" + other_HC.ishitted);
                npc_HS.Ishitted = true;
                //Debug.Log("after hit" );
                npc_HS.isDamaged(20);
            }
        }
    }

}

