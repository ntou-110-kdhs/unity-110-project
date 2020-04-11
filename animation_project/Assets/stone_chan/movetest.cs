using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class movetest : MonoBehaviour
{
    /// <summary>
    /// 垂直輸入量
    /// </summary>
    [SerializeField]
    [Header("垂直輸入量")]
    private float input_V;
    /// <summary>
    /// 水平輸入量
    /// </summary>
    [SerializeField]
    [Header("水平輸入量")]
    private float input_H;
    /// <summary>
    /// 結果角度
    /// </summary>
    [SerializeField]
    [Header("結果角度")]
    private float angle_Sum;
    void FixedUpdate()//固定頻率重複執行
    {
        //接Input.GetAxis("Vertical")及("Horizontal")的回傳值
        input_V = Input.GetAxis("Vertical");
        input_H = Input.GetAxis("Horizontal");
        //如果按住WSAD任何一按鍵，才執行以下程式
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.S))
        {
            //三角函數計算
            angle_Sum = Mathf.Atan(input_H / input_V) / (Mathf.PI / 180);
            angle_Sum = input_V < 0 ? angle_Sum + 180 : angle_Sum;
            //如果角度是NaN，讓他變成0
            if (float.IsNaN(angle_Sum))
                angle_Sum = 0;
            //角色轉向
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, angle_Sum, transform.eulerAngles.z);
        }
    }
}