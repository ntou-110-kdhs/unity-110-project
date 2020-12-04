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

    // 黑平面
    [SerializeField] private GameObject balckPlane = null;
    void Start()
    {

        cam = GetComponent<Camera>();

        player = GameObject.FindGameObjectWithTag("Player").transform;

        Vector3 prefabPos = transform.position;

        GameObject prefab = Instantiate(playerMarkPrefab, prefabPos, playerMarkPrefab.transform.rotation, transform);

        prefabPos = prefab.transform.localPosition;
        prefabPos.z += 10;
        prefab.transform.localPosition = prefabPos;


        cam.cullingMask |= (1 << 9);

        
    }

    void LateUpdate()
    {
        Vector3 newPosition = player.transform.position;
        newPosition.y = transform.position.y;
        transform.position = newPosition;


    }

    /// <summary>
    /// 設置黑屏
    /// </summary>
    /// <param name="enable"> 是否開啟</param>
    public void setBlackPanel(bool enable)
    {
        balckPlane.SetActive(enable);
    }
}
