using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class enemyDetect : MonoBehaviour
{
    [Range(0, 180)]
    public float Alertangle;
    [Range(0, 10)]
    public float AlertRadius;

    public float yAxis = 1.0f;

    public bool Alertistrue = false;

    public GameObject target;
    /*將偵測點設為(this.transform.position.x,
                    yAxis,
                    this.transform.position.z)*/
    Vector3 Position;
    //// Start is called before the first frame update
    //void Start()
    //{
        
    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    Alert();
    //}

    private void OnDrawGizmos()
    {
        Position = new Vector3(gameObject.transform.position.x, yAxis, gameObject.transform.position.z);
        Color color = Handles.color;
        Handles.color = Color.blue;
        Vector3 StartLine = Quaternion.Euler(0, -Alertangle, 0) * this.transform.forward;
        Handles.DrawSolidArc(Position, this.transform.up, StartLine, Alertangle, AlertRadius);
        //Handles.color = color;
        Handles.color = Color.red;
        StartLine = Quaternion.Euler(0, Alertangle, 0) * this.transform.forward;
        Handles.DrawSolidArc(Position, this.transform.up, StartLine, -Alertangle, AlertRadius);
    }

    public Interactable Alert()
    {
        Interactable ret = null;
        float distance = Vector3.Distance(Position, target.transform.position);
        Vector3 vector3 = target.transform.position - Position;
        float angle = Vector3.Angle(vector3, this.transform.forward);
        if (distance <= AlertRadius && angle <= Alertangle)
        {
            print("Alertistrue is True.");
            Alertistrue = true;
            ret = target.GetComponent<Interactable>();
        }
        else
        {
            print("Alertistrue is False.");
            Alertistrue = false;
            ret = null;
        }
        //print("Quaternion.Euler(0, -Alertangle, 0) * this.transform.forward:"+ Quaternion.Euler(0, -Alertangle, 0) * this.transform.forward);
        //print("Angle:"+(int)(angle));
        //print("sphere vector:"+ vector3);
        //print("forward:" + this.transform.forward);
        //print("Dictance:"+ (int)(distance));
        return ret;
    }
}
