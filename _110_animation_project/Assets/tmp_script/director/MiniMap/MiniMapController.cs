using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapController : MonoBehaviour
{
    // 自己本身的攝影機
    private Camera cam;

    // 玩家
    private Transform player;
    [SerializeField] private GameObject playerMarkPrefab;
    void Start()
    {

        cam = GetComponent<Camera>();

        player = GameObject.FindGameObjectWithTag("Player").transform;
        
        Instantiate(playerMarkPrefab, player.position, playerMarkPrefab.transform.rotation, player);


        cam.cullingMask |= (1 << 9);
    }

    void LateUpdate()
    {
        Vector3 newPosition = player.transform.position;
        newPosition.y = transform.position.y;
        transform.position = newPosition;
    }
}
