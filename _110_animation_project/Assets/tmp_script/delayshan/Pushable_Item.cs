using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//先幫可推動物體的TAG設置為MOVABLE
//再將RIDGBODY的ROTATION FREEZE住
//接著加上FIX JOIN 部件

public class Pushable_Item : MonoBehaviour
{
    FixedJoint fixedJoint;
    float objXVector;
    float objZVector;
    bool rightForwardHit = false;
    bool leftForwardHit = false;
    bool rightBackwardHit = false;
    bool leftBackwardHit = false;

    void Start()
    {
        objXVector = this.transform.localScale.x / 2;       //物體中心至X方向大小
        objZVector = this.transform.localScale.z / 2;       //物體中心至Z方向大小

    }



    // Update is called once per frame
    void Update()
    {
        Ray rayRightForward = new Ray(transform.position+new Vector3(objXVector, 0, objZVector), new Vector3(0, -1, 0));        //右前方的RAY 偵測是否浮空
        Ray rayLeftForward = new Ray(transform.position + new Vector3(objXVector*-1, 0, objZVector), new Vector3(0, -1, 0));    //左前方的RAY 偵測是否浮空
        Ray rayRightBackward = new Ray(transform.position + new Vector3(objXVector, 0, objZVector*-1), new Vector3(0, -1, 0));  //右後方的RAY 偵測是否浮空
        Ray rayLeftBackward = new Ray(transform.position + new Vector3(objXVector*-1, 0, objZVector*-1), new Vector3(0, -1, 0));//左後方的RAY 偵測是否浮空
        RaycastHit hit;
        fixedJoint = gameObject.GetComponent<FixedJoint>();

        Debug.DrawRay(transform.position + new Vector3(objXVector * -1, 0, objZVector * -1), new Vector3(0, -1, 0), Color.green);
        Debug.DrawRay(transform.position + new Vector3(objXVector, 0, objZVector * -1), new Vector3(0, -1, 0), Color.green);
        Debug.DrawRay(transform.position + new Vector3(objXVector * -1, 0, objZVector), new Vector3(0, -1, 0), Color.green);
        Debug.DrawRay(transform.position + new Vector3(objXVector, 0, objZVector), new Vector3(0, -1, 0), Color.green);


        //偵測右前 左前 右後 左後方是否為浮空
        if (!Physics.Raycast(rayRightForward, out hit, (this.transform.localScale.y / 2) + 0.005f))
        {
            rightForwardHit = false;
        }
        else rightForwardHit = true;

        if (!Physics.Raycast(rayLeftForward, out hit, (this.transform.localScale.y / 2) + 0.005f))
        {
            leftForwardHit = false;
        }
        else leftForwardHit = true;

        if (!Physics.Raycast(rayRightBackward, out hit, (this.transform.localScale.y / 2) + 0.005f))
        {
            rightBackwardHit = false;
        }
        else rightBackwardHit = true;

        if (!Physics.Raycast(rayLeftBackward, out hit, (this.transform.localScale.y / 2) + 0.005f))
        {
            leftBackwardHit = false;
        }
        else leftBackwardHit = true;
        //偵測右前 左前 右後 左後方是否為浮空


        if (!rightForwardHit && !leftForwardHit && !rightBackwardHit && !leftBackwardHit)   //若4角皆為浮空
        {
            //Debug.Log(hit.transform);

            if (fixedJoint != null)                //若當前有fixedJoint 且物體為浮空 則清除fixedJoint  並且清除角色的推動狀態                           
            {
                if (fixedJoint.connectedBody != null)
                {
                    fixedJoint.connectedBody.gameObject.BroadcastMessage("stopPushing");         //在playercontroller中  用於停止推動狀態
                }
                
                Destroy(fixedJoint);
            }

        }
        else
        {
            if (fixedJoint == null)                 //若當前沒有fixedJoint 且物體不為浮空 則新增fixedJoint
            {
                gameObject.AddComponent<FixedJoint>();
            }
        }

    }


}
