using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour
{

    public Transform jointA;
    public Transform jointB;

    public GameObject axle;

    public Drivable parentDrivable;
    public LayerMask layerMask;

    public Transform wheel;

    private void OnDrawGizmos()
    {
        axle.transform.position = jointA.position;

        RaycastHit hit;

        if (Physics.Raycast(wheel.position, -parentDrivable.chassisRigidbody.transform.up, out hit, 2.0f, layerMask))
        {
            transform.position = Vector3.Lerp(transform.position, hit.point, Time.deltaTime);
        }

        Vector3 axleOrientation = jointA.position - jointB.position;

        axle.transform.LookAt(jointB);
    }

    // Update is called once per frame
    void Update()
    {

        axle.transform.position = jointA.position;

        RaycastHit hit;

        if (Physics.Raycast(wheel.position, -parentDrivable.chassisRigidbody.transform.up, out hit, 2.0f, layerMask))
        {
            transform.position = Vector3.Lerp(transform.position, hit.point, Time.deltaTime);
        }

        Vector3 axleOrientation = jointA.position - jointB.position;

        axle.transform.LookAt(jointB);

    }
}