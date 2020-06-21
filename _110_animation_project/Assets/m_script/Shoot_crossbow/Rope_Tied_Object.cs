using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 使用前  確保玩家的TAG為PLAYER  且此物件的tag為Rope_Tied_Object  且有將material附上物件的linerenderer
 並有正確的配置繩索的PREFAB

 將此函式放置在綁繩索的物體上  此函式將判斷玩家與此物體的距離 
 若小於rangeToShoot的距離  isPlayerInRange為TRUE
 若不夠近  isPlayerInRange為false

 isPlayerInRange會被Rope_Tied_Manager_Script使用  Rope_Tied_Manager_Script會抓取所有TAG為Rope_Tied_Object的物件  並且取用此函式

     */



public class Rope_Tied_Object : MonoBehaviour
{
    public Transform Rope_tied_objcet;              //為物體的第一個子物件   綁繩子的點
    public Transform player;
    public bool isPlayerInRange = false;            //玩家是否在物體範圍內
    [SerializeField]
    private float rangeToShoot = 2.5f;
    [SerializeField]
    private GameObject ropePrefabe=null;
    private GameObject [] playerarray;
    private LineRenderer tiedObejectLineRenderer=null;
    // Start is called before the first frame update
    void Start()
    {
        //獲取TAG為PLAYER的物件
        if (player == null) playerarray = GameObject.FindGameObjectsWithTag("Player");
        if (playerarray != null) { 
            player = playerarray[0].transform;
        }
        //獲取Rope_tied_objcet (綁繩子的點)
        if (null == this.transform.GetChild(0)) Debug.Log("Tied Object does not have a Tied point");
        else Rope_tied_objcet = this.transform.GetChild(0).transform;

        if (tiedObejectLineRenderer == null)
        {
            tiedObejectLineRenderer = this.GetComponentInChildren<LineRenderer>();
            tiedObejectLineRenderer.positionCount = 2;
            tiedObejectLineRenderer.startWidth = 0.05f;
            tiedObejectLineRenderer.SetPosition(0, Rope_tied_objcet.transform.position);
            tiedObejectLineRenderer.SetPosition(1, Rope_tied_objcet.transform.position);
        }
        //確認是否有指派繩索物件
        if (ropePrefabe == null) Debug.Log("rope prefab has not been assigned");
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null) Debug.Log("player not found");
        else
        {
            //計算物體與玩家間的距離  若小於rangeToShoot  則呼叫playerCanShoot() 更改狀態
            float distance = Vector3.Distance(player.transform.position, this.transform.position);
            //Debug.Log("distance with player==" + distance);
            if (distance < rangeToShoot) isPlayerInRange=true;
            else isPlayerInRange=false;
        }
    }


    //獲得玩家的LINERENDERER的所有結點  並且將第0個設為自己的位置  在玩家控制器的tiedRopeAnimationEnd取用
    public void getLineRendererPoints(Vector3[] array)
    {

        if (tiedObejectLineRenderer != null)
        {
            tiedObejectLineRenderer.SetPositions(array);
        }
        tiedObejectLineRenderer.SetPosition(0, Rope_tied_objcet.position);

        //生成繩索物件
        float dist = Vector3.Distance(Rope_tied_objcet.position, array[1]);
        Vector3 newPosition= Vector3.Lerp(Rope_tied_objcet.position, array[1],0.5f);
        Vector3 rotationnVector = new Vector3(array[1].x - Rope_tied_objcet.position.x, array[1].y - Rope_tied_objcet.position.y, array[1].z - Rope_tied_objcet.position.z);
        Quaternion newRotation = Quaternion.LookRotation(rotationnVector);
        GameObject ropeObject= Instantiate(ropePrefabe, newPosition, newRotation);
        ropeObject.transform.localScale =new Vector3(ropeObject.transform.localScale.x, dist/2, ropeObject.transform.localScale.z);
        if (ropeObject != null) ropeObject.transform.rotation = Quaternion.LookRotation(ropeObject.transform.up);
    }

}
