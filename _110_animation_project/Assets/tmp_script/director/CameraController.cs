using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private bool isMoveForward = false;
    [SerializeField] private float speed = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            isMoveForward = true;
        }
        if (isMoveForward)
        {
            Vector3 forward = transform.forward;
            forward.y = 0;
            transform.position += forward * Time.deltaTime * speed;
        }
    }
}
