using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Suspension : MonoBehaviour
{
    public Drivable parentDrivable;
    public LayerMask layerMask;

    public Transform wheel;

    // Update is called once per frame
    void Update()
    {

        RaycastHit hit;

        if (Physics.Raycast(wheel.position, -parentDrivable.chassisRigidbody.transform.up, out hit, 2.0f, layerMask))
        {
            transform.position = Vector3.Lerp(transform.position, hit.point, Time.deltaTime);
        }

    }
}