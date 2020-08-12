using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalProbe : MonoBehaviour
{

    //public float probeSpread;

    public float probeDist;
    public LayerMask layerMask;

    [Range(0, 3)]
    public float markerSize;

    public Vector3 normal;
    public Vector3 smoothedNormal;
    public Vector3 point;

    public Vector3 probedForward;

    private void OnDrawGizmos()
    {

        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit, probeDist, layerMask))
        {
            normal = hit.normal;
            smoothedNormal = NormalSmoother.SmoothedNormal(hit);
            point = hit.point;

            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, hit.point);
            Gizmos.DrawSphere(hit.point, markerSize);

            Vector3 helperPosition = transform.position + -transform.up * hit.distance / 2;

            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(helperPosition, markerSize);
            Gizmos.DrawLine(helperPosition, helperPosition + Quaternion.AngleAxis(90, transform.right) * smoothedNormal);

        }
        else
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + -transform.up * probeDist);
        }

    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit, probeDist, layerMask))
        {
            normal = hit.normal;
            smoothedNormal = NormalSmoother.SmoothedNormal(hit);
            point = hit.point;
            probedForward = Quaternion.AngleAxis(90, transform.right) * smoothedNormal;

        }
    }
}