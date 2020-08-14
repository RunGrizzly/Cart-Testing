using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject boomStart;
    GameObject boomEnd;

    public GameObject carBody;
    public GameObject carMain;

    public float camOffsetX;
    public float camOffsetY;
    public float camOffsetZ;

    public float camAngleX;
    public float camAngleY;
    public float camAngleZ;

    // private void Start()
    // {
    //     boomEnd = new GameObject();
    //     boomEnd.transform.parent = boomStart.transform;
    //     boomEnd.transform.localPosition = new Vector3(camOffsetX, camOffsetY, camOffsetZ);
    // }

    private void Update()
    {

        // boomEnd.transform.localPosition = new Vector3(camOffsetX, camOffsetY, camOffsetZ);
        // Vector3 offsetPosition = new Vector3(boomEnd.transform.position.x, boomEnd.transform.position.y, boomEnd.transform.position.z);
        // transform.position = offsetPosition;
        // transform.eulerAngles = new Vector3(camAngle, transform.eulerAngles.y, transform.eulerAngles.z);

        boomStart.transform.position = carBody.transform.position;
        boomStart.transform.rotation = carBody.transform.rotation;
        transform.localPosition = new Vector3(camOffsetX, camOffsetY, camOffsetZ);
        boomStart.transform.eulerAngles = new Vector3(camAngleX, boomStart.transform.eulerAngles.y, camAngleZ);

        //transform.LookAt(boomStart.transform);

    }

}