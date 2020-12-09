using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class delayshan_cam : MonoBehaviour
{
    [SerializeField]
    public CinemachineVirtualCamera m_camera;
    [SerializeField]
    public GameObject[] m_node;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)){
            m_camera.LookAt = m_node[0].transform;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            m_camera.LookAt = m_node[1].transform;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            m_camera.LookAt = m_node[2].transform;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            m_camera.LookAt = m_node[3].transform;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            m_camera.LookAt = null;
        }
    }
}
