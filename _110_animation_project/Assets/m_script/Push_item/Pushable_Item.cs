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
    float objYVector;
    [SerializeField]
    bool rightForwardHit = false;
    [SerializeField]
    bool leftForwardHit = false;
    [SerializeField]
    bool rightBackwardHit = false;
    [SerializeField]
    bool leftBackwardHit = false;
    [SerializeField]
    bool rightHit = false;
    [SerializeField]
    bool leftHit = false;
    [SerializeField]
    bool backwardHit = false;
    [SerializeField]
    bool forwardHit = false;

    void Start()
    {
        if (this.GetComponent<Renderer>() != null)
        {
            objXVector=this.GetComponent<Renderer>().bounds.size.x / 2;  //物體中心至X方向大小
            objZVector = this.GetComponent<Renderer>().bounds.size.z / 2;//物體中心至Z方向大小
            objYVector=this.GetComponent<Renderer>().bounds.size.y / 2;//物體中心至Y方向大小
        }
        else if(this.GetComponent<Collider>() != null)
        {
            objXVector = this.GetComponent<Collider>().bounds.size.x / 2;  //物體中心至X方向大小
            objZVector = this.GetComponent<Collider>().bounds.size.z / 2;//物體中心至Z方向大小
            objYVector = this.GetComponent<Collider>().bounds.size.y / 2;//物體中心至Y方向大小
        }
        else
        {
            objXVector = this.transform.localScale.x / 2;       //物體中心至X方向大小
            objZVector = this.transform.localScale.z / 2;       //物體中心至Z方向大小
            objYVector = this.transform.localScale.y / 2;       //物體中心至Y方向大小
        }


    }



    // Update is called once per frame
    void Update()
    {
        Ray downWard = new Ray(transform.position + new Vector3(0, 0, 0), new Vector3(0, -1, 0));        //正下方的RAY  調整物體角度




        Ray rayRightForward = new Ray(transform.position+new Vector3(objXVector, 0, objZVector), new Vector3(0, -1, 0));        //右前方的RAY 偵測是否浮空
        Ray rayLeftForward = new Ray(transform.position + new Vector3(objXVector*-1, 0, objZVector), new Vector3(0, -1, 0));    //左前方的RAY 偵測是否浮空
        Ray rayRightBackward = new Ray(transform.position + new Vector3(objXVector, 0, objZVector*-1), new Vector3(0, -1, 0));  //右後方的RAY 偵測是否浮空
        Ray rayLeftBackward = new Ray(transform.position + new Vector3(objXVector*-1, 0, objZVector*-1), new Vector3(0, -1, 0));//左後方的RAY 偵測是否浮空

        Ray rayForward = new Ray(transform.position + new Vector3(0, 0, objZVector), new Vector3(0, -1, 0));     //前方的RAY 偵測是否浮空
        Ray rayBackward = new Ray(transform.position + new Vector3(0, 0, objZVector*-1), new Vector3(0, -1, 0));  //後方的RAY 偵測是否浮空
        Ray rayLeft = new Ray(transform.position + new Vector3(objXVector * -1, 0, 0), new Vector3(0, -1, 0));   //左方的RAY 偵測是否浮空
        Ray rayRight = new Ray(transform.position + new Vector3(objXVector, 0, 0), new Vector3(0, -1, 0));       //右方的RAY 偵測是否浮空
        RaycastHit hit;
        fixedJoint = gameObject.GetComponent<FixedJoint>();

        Debug.DrawRay(transform.position + new Vector3(objXVector * -1, 0, objZVector * -1), new Vector3(0, -1, 0), Color.green);
        Debug.DrawRay(transform.position + new Vector3(objXVector, 0, objZVector * -1), new Vector3(0, -1, 0), Color.green);
        Debug.DrawRay(transform.position + new Vector3(objXVector * -1, 0, objZVector), new Vector3(0, -1, 0), Color.green);
        Debug.DrawRay(transform.position + new Vector3(objXVector, 0, objZVector), new Vector3(0, -1, 0), Color.green);

        
        //調整物體角度

        if (Physics.Raycast(downWard, out hit, (this.transform.localScale.y / 2) + 1f))
        {/*
            Vector3 tempVector = new Vector3(hit.point.x, this.transform.position.y, hit.point.z) + new Vector3((float)1.2 * hit.normal.x, 0, (float)1.2 * hit.normal.z);
            this.transform.position = tempVector;*/
            this.transform.rotation = Quaternion.LookRotation(new Vector3(0 , 0, hit.normal.z ));
            
        }
        
        


        //偵測右前 左前 右後 左後方是否為浮空
        if (!Physics.Raycast(rayRightForward, out hit, (objYVector) + 0.1f))
        {
            rightForwardHit = false;
        }
        else rightForwardHit = true;

        if (!Physics.Raycast(rayLeftForward, out hit, (objYVector) + 0.1f))
        {
            leftForwardHit = false;
        }
        else leftForwardHit = true;

        if (!Physics.Raycast(rayRightBackward, out hit, (objYVector) + 0.1f))
        {
            rightBackwardHit = false;
        }
        else rightBackwardHit = true;

        if (!Physics.Raycast(rayLeftBackward, out hit, (objYVector) + 0.1f))
        {
            leftBackwardHit = false;
        }
        else leftBackwardHit = true;
        //偵測右前 左前 右後 左後方是否為浮空

        //偵測前 後 左 右是否為浮空
        if(!Physics.Raycast(rayForward, out hit, (objYVector) + 0.1f))
        {
            forwardHit = false;
        }
        else forwardHit = true;

        if (!Physics.Raycast(rayBackward, out hit, (objYVector) + 0.1f))
        {
            backwardHit = false;
        }
        else backwardHit = true;

        if (!Physics.Raycast(rayLeft, out hit, (objYVector) + 0.1f))
        {
            leftHit = false;
        }
        else leftHit = true;

        if (!Physics.Raycast(rayRight, out hit, (objYVector) + 0.1f))
        {
            rightHit = false;
        }
        else rightHit = true;
        //偵測前 後 左 右是否為浮空

        if (!rightForwardHit && !leftForwardHit && !rightBackwardHit && !leftBackwardHit && !forwardHit && !backwardHit && !leftHit && !rightHit)   //若8角皆為浮空
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
