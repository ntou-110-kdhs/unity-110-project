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
        if (canAssassinate && Input.GetMouseButtonDown(0))
        {
            assassinReady();
        }
    }

    void FixedUpdate()
    {
        canAssassinate = false;
        assassinTarget = null;
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
            Vector3 offset = -assassinTarget.forward;

            transform.position = assassinTarget.position + offset;
            transform.forward = assassinTarget.forward;
            isAssassinReady = true;            
        }
    }


    public void assassinFinish()
    {
        isAssassinReady = false;

        assassinTarget.GetComponentInChildren<HealthSystem>().isDamaged(100);

    }
}
