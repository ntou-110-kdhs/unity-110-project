using System.Collections;
using Cinemachine;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float speed = 0;
    [SerializeField] private CinemachineDollyCart cdc;
    [SerializeField] private CinemachineVirtualCamera cvc;
    [SerializeField] private Transform test;
    [SerializeField] private GameObject switch1;
    [SerializeField] private Material red;
    private bool isAni = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 moveDirection = Vector3.zero;
       
        if (Input.GetKey(KeyCode.I))
        {
            moveDirection = Vector3.forward;
        }
        if (Input.GetKey(KeyCode.K))
        {
            moveDirection = -Vector3.forward;
        }
        if (Input.GetKey(KeyCode.L))
        {
            moveDirection = Vector3.right;
            
        }
        if (Input.GetKey(KeyCode.J))
        {
            moveDirection = -Vector3.right;
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            cdc.m_Speed = 0.8f;
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            isAni = true;
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            switch1.transform.GetComponent<MeshRenderer>().material = red;
        }
        if (isAni && test.position.y <= 7.65f)
        {
            Vector3 tmpVec = test.position;
            tmpVec.y += Time.deltaTime * 2;
            test.position = tmpVec;
        }
        moveDirection.y = 0;
        moveDirection *= speed * Time.deltaTime;
        transform.position += transform.TransformDirection(moveDirection);
       

    }
}
