using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage_script : MonoBehaviour
{
    public bool onAttack = false;
    public float damage_status = 1;
    public float damage_dealt = 25;
    HeathControl other_HC;
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
        
        if (other.tag == "enimy")
        {
            
            if (other.tag == "enimy")
            {
                other_HC = other.GetComponentInChildren<HeathControl>();
            }

            if (!other_HC.ishitted && onAttack == true && other.tag.Equals("enimy"))
            {
                //Debug.Log("before hit" + other_HC.ishitted);
                other_HC.ishitted = true;
                //Debug.Log("after hit" + other_HC.ishitted);
                other.BroadcastMessage("got_hit");
            }
        }

    }

}

