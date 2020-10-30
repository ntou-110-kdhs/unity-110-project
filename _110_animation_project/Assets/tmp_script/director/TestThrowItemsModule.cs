using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestThrowItemsModule : MonoBehaviour
{
    //一般變數: 開頭小寫，單字分隔開頭大寫 Ex:myFirstName

    //一般函式(function):開頭小寫，單字分隔開頭大寫 Ex:myFirstFunc();

    //常數:開頭底線+小寫，單字分隔開頭大寫 Ex:_myFirstName

    //類別:開頭大寫，單字分隔開頭大寫 Ex:class MyFirstFamily { };


    [SerializeField] private GameObject throwedItem;

    private Rigidbody throwedItemRb;

    private Camera WorldToScreenPoint = null;

    private float angleX = 0;
    private float angleY = 0;

    void Start()
    {
        throwedItemRb = throwedItem.GetComponent<Rigidbody>();
    }

    void Update()
    {
        //滑鼠水平(X軸)移動
        float mouseX = Input.GetAxis("Mouse X");
        //滑鼠垂直(Y軸)移動
        float mouseY = Input.GetAxis("Mouse Y");

        angleX += mouseX * 5;
        angleY += mouseY * 5;

        Vector3 dir = transform.forward;
        Vector3 euler = new Vector3(0, angleX, angleY);

        dir = Rotate(dir, euler);

        Ray ray = new Ray(transform.position, dir);
        Debug.DrawRay(ray.origin, ray.direction, Color.red);



        if (Input.GetKeyDown(KeyCode.T))
        {
            throwedItem.transform.position = transform.position + Vector3.up * 3;
            throwedItemRb.velocity = dir * 20;
        }
    }

    public Vector3 Rotate(Vector3 source, Vector3 rotate)
    {
        Quaternion q = Quaternion.Euler(rotate);
        return q * source;
    }
}
