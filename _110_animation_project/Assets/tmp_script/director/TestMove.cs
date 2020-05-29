using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMove : MonoBehaviour
{
    [SerializeField] private bool moveRight;
    [SerializeField] private bool moveUp;
    [SerializeField] private bool moveDown;
    [SerializeField] private bool moveLeft;
    [SerializeField] private bool moveForward;
    [SerializeField] private bool moveBack;
    [SerializeField] private float speed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (moveRight)
        {
            transform.position += transform.right * Time.deltaTime * speed;
        }
        else if (moveUp)
        {
            transform.position += transform.up * Time.deltaTime * speed;
        }
        else if (moveLeft)
        {
            transform.position -= transform.right * Time.deltaTime * speed;
        }
        else if (moveDown)
        {
            transform.position -= transform.up * Time.deltaTime * speed;
        }
        else if (moveForward)
        {
            transform.position += transform.forward * Time.deltaTime * speed;
        }
        else if (moveBack)
        {
            transform.position -= transform.forward * Time.deltaTime * speed;
        }

    }
}
