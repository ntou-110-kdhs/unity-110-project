using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//將目標物件拉至此函式的destination即可
//位置0 是主角腰部  1 是左手十字弓的中段    2是目標位置做運算而得
//運算方法大概是計算自己與目標物的距離   之後再除以速率   則可得出大概會有幾個大點(NODE)
//接著每1秒換至下一個大點  每0.1秒更新點的小間隔(大點之間有10格)
//狀態順序crossbowInhand->isCalled->inhand

public class Rope_DrawLine : MonoBehaviour
{
    public Transform destination;                             //目標位置
    public Transform crossBowOnSide=null;                   //主角側邊的十字弩
    private Transform tiedObjectStart = null;               //綁繩子的物件
    private Vector3 nextBigNode;                            //下一個大節點
    private Vector3 nextSmallNode;                          //下一個小節點
    private Vector3 SmallNode;                              //記憶小節點  用於保存小節點在每個大節點開始時的位置
    private LineRenderer lineRenderer=null;
    private bool crossbowInhand = false;                    //是否在拿出十字弓到射擊之前的動畫
    private bool inhand = false;                            //是否在轉移繩子至綁繩物體的動畫
    private bool ended = false;                             //是否已達到目標
    private bool isCalled = false;                          //是否被呼叫
    private float distance;                                 //手中的十字弩與目標距離
    private float speed = 0;                                //弩箭飛行距離
    private float lineNodes = 0;                            //起點至終點間能有多少間隔   distance/speed
    private float secondCounter = -10;                      //計算時間(秒)
    private float miliSecCounter = -10;                     //計算時間(毫秒)
    private float i = 0;                                    //改變線位置  每 1 秒更動
    private float j = 0;                                    //改變線位置  每0.1秒更動

    // Start is called before the first frame update
    void Start()
    {

        if (destination == null) Debug.Log("target not set");
        else
        {
            distance = Vector3.Distance(this.transform.position, destination.position);
            speed = 90;
            lineNodes = distance / speed;

            lineRenderer = gameObject.GetComponent<LineRenderer>();
            lineRenderer.positionCount = 2;
            lineRenderer.startWidth = 0.05f;
            nextSmallNode = this.transform.position;

            if (crossBowOnSide == null) crossBowOnSide = GameObject.Find("Crossbow_on_side").transform;
        }

    }

    // Update is called once per frame
    void LateUpdate()
    {

        if (isCalled)
        {
            if (destination == null) Debug.Log("target not set");
            else
            {
                lineRenderer.SetPosition(0, crossBowOnSide.position);
                if (Time.time - miliSecCounter >= 0.1)                              //使J與miliSecCounter  每0.1秒更動一次
                {
                    if (j >= 1f && !ended) j = 0;
                    else j += 0.1f;
                    miliSecCounter = Time.time;
                    //Debug.Log("j=" + j);
                }

                if (Time.time - secondCounter >= 1)                              //使J與miliSecCounter  每0.1秒更動一次
                {
                    if (i >= lineNodes) ended = true;
                    else
                    {
                        j = 0;
                        i += 1f;
                    }
                    secondCounter = Time.time;
                    //Debug.Log("i=" + i);
                }


                nextBigNode = Vector3.Lerp(this.transform.position, destination.position, i / lineNodes);                //設置大點位置
                SmallNode = Vector3.Lerp(this.transform.position, destination.position, (i - 1) / lineNodes);               //設置小點記憶位置
                nextSmallNode = Vector3.Lerp(SmallNode, nextBigNode, j);                                                 //設置小點位置
                lineRenderer.SetPosition(1, nextSmallNode);                                                              //設置線位置

            }
        }
        else if (crossbowInhand)
        {
            //若是當前是在拿出十字弓至射擊前
            lineRenderer.SetPosition(1, this.transform.position);
        }
        else if (inhand)
        {
            //若當前狀況是在手中  設至起始點在手上
            lineRenderer.SetPosition(0, this.transform.position);
        }
        else
        {

            //重置綁繩位置
            lineRenderer.SetPosition(0, crossBowOnSide.position);
            lineRenderer.SetPosition(1, crossBowOnSide.position);
        }

    }

    //呼叫此函式  代表可以進行射擊
    public void crossbowShootRope()
    {
        isCalled = true;
        crossbowInhand = false;
    }

    //當目前在拿十字弓動畫時PlayerController中的takingCrossBow會呼叫此函式   代表目前繩子的目標應在手中
    public void crossbowInHand()
    {
        crossbowInhand = true;
    }


    //當目前在綁繩子動畫時PlayerController中的shootAnimationEnd會呼叫此函式   將inhand設為true  代表目前繩子的起點應在手中
    public void ropeInHand()
    {
        inhand = true;
        isCalled = false;
    }

    //在玩家控制器中的tiedRopeAnimationEnd取用   用於獲得玩家的LINERENDERER ARRAY  並回傳該ARRAY
    public Vector3[] ropeTiedToObject()
    {
        Vector3[] positionsArray=new Vector3[lineRenderer.positionCount];
        lineRenderer.GetPositions(positionsArray);

        inhand = false;
        return positionsArray;
    }



}
