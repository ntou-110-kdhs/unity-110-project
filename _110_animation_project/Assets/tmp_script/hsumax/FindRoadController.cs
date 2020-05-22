using System;
using UnityEngine;
using UnityEngine.AI;
using Cinemachine;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(CharacterController))]
public class FindRoadController : MonoBehaviour
{
    [SerializeField] private Transform target;//target to follow
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private CharacterController character;
    [SerializeField] private GameObject agentRoadSet;
    [SerializeField] private Transform[] setPoints;
    [SerializeField] private int pointSetSize = 0;
    [SerializeField] private int nextPoint = 0;
    [SerializeField] private bool isSetPoint = false;
    [SerializeField] private Vector3 tmpXYZ;
    private float tmpYoffset = 0.0f;


    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        character = GetComponent<CharacterController>();
        if(agentRoadSet == null)
        {
            agentRoadSet = GameObject.Find("agentRoadPoint");
            pointSetSize = agentRoadSet.transform.childCount;
        } 
        setPoints = new Transform[pointSetSize];
        for (int i = 0; i < pointSetSize; i++) 
        {
            if (setPoints[i] == null)
            {
                setPoints[i] = agentRoadSet.transform.GetChild(i);
            }
        }
        //GameObject.Find("agentRoadPoint").GetComponent<GameObject>();
        tmpYoffset = character.height / 2;
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            agent.SetDestination(target.position);
            //FaceTarget();
        }
        else if(pointSetSize > 0)
        {
            if (!isSetPoint)
            {
                if (tmpXYZ == null) tmpXYZ = new Vector3(setPoints[nextPoint].position.x, setPoints[nextPoint].position.y, setPoints[nextPoint].position.z);
                else
                {
                    tmpXYZ.x = setPoints[nextPoint].position.x;
                    tmpXYZ.y = setPoints[nextPoint].position.y;
                    tmpXYZ.z = setPoints[nextPoint].position.z;
                }
                isSetPoint = true;
            }
            agent.SetDestination(setPoints[nextPoint].position);
            if(transform.position.x == tmpXYZ.x && transform.position.z == tmpXYZ.z && Math.Abs(tmpXYZ.y - transform.position.y) < tmpYoffset)
            {
                Debug.Log("NPC position : " + transform.position);
                isSetPoint = false;
                nextPoint++;
                nextPoint %= pointSetSize;
            }
            //Debug.Log("setPoints : "+ setPoints[0].position);
            //if (setPoints[nextPoint] == null)
            //{
            //    Debug.Log("setPoints " + nextPoint+" is null.");
            //}
            //nextPoint++;
            //nextPoint %= pointSetSize;

        }
    }
}
