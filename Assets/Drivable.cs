using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drivable : MonoBehaviour
{
    //The raycast with important hit information related to the normal of the surface.
    RaycastHit hit;
    public LayerMask layerMask;

    public List<Rigidbody> wheels = new List<Rigidbody>();
    public List<NormalProbe> normalProbes = new List<NormalProbe>();

    public GameObject forcePointFront;
    public GameObject forcePointBack;
    public GameObject gimbal;
    public GameObject dynamicUpPoint;
    public GameObject body;
    public GameObject normalMarker;

    Vector3 forwardDirection;
    Vector3 reverseDirection;
    Vector3 lastNormal = Vector3.zero;
    Vector3 lastPoint = Vector3.zero;
    Vector3 attraction;
    Vector3 repulsion;
    Vector3 dynamicUpPos;
    Vector3 hoverPos;

    public NormalProbe forwardProbe;
    public NormalProbe backProbe;
    public NormalProbe leftProbe;
    public NormalProbe rightProbe;

    public Rigidbody parentRigidbody;
    public Rigidbody chassisRigidbody;

    public float forwardAcl;
    public float backwardAcl;
    public float maxVelocity;
    public float maxYVelocity;
    public float yDamp;
    public float dynamicUpHeight;
    public float orientationSpeed;

    public float magnetStrength;
    public float predictiveLift;
    public float magnetiseDistance;
    [Range(0, 2.0f)]
    public float magnetThreshold;
    public float floorCheckDist;
    public float rotationFix;
    public float hoverHeight = 1.35f;
    public float brakeStrength = 2500.0f;
    public float turnStrength = 10f;
    [Range(-45, 45)]
    float currThrust = 0.0f;
    float currBrake = 0.0f;
    float attractionModifier;
    float repulsionModifier;
    float currTurn = 0.0f;
    //Rotate the gimbal equal to the dive offset.
    float dive;
    float positionFactor;
    //Distance to target height.
    float magnetSnapModifier;

    void Start()
    {
        dynamicUpPos = transform.position + chassisRigidbody.transform.up * dynamicUpHeight;
        dynamicUpPoint.transform.position = dynamicUpPos;

        //Heading to dynamic up.
        Vector3 dynamicUp = dynamicUpPoint.transform.position - transform.position;

        //Lock the gimbal to our position.
        gimbal.transform.position = transform.position;

    }

    Vector3 GetAverageNormal(Vector3 hitNormal)
    {
        Vector3 avgNormal = Vector3.zero;

        foreach (NormalProbe normalProbe in normalProbes)
        {
            avgNormal += normalProbe.smoothedNormal;
        }

        avgNormal += hitNormal;
        avgNormal = avgNormal / normalProbes.Count;
        return avgNormal;

    }

    void LevelOut()
    {
        //Level self
        dynamicUpPos = Vector3.Lerp(dynamicUpPos, transform.position + Vector3.up * dynamicUpHeight, Time.deltaTime * orientationSpeed / 4);
    }

    void OnDrawGizmos()
    {
        if (Application.isPlaying == false)
        {

            dynamicUpPos = transform.position + chassisRigidbody.transform.up * dynamicUpHeight;
            dynamicUpPoint.transform.position = dynamicUpPos;

            //Heading to dynamic up.
            Vector3 dynamicUp = dynamicUpPoint.transform.position - transform.position;

            //Lock the gimbal to our position.
            gimbal.transform.position = transform.position;
            // //Have the gimbal look at the up position.
            // gimbal.transform.LookAt(dynamicUpPoint.transform, transform.up);
        }

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(dynamicUpPoint.transform.position, 0.5f);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(dynamicUpPos, 0.5f);

        // Gizmos.DrawRay(transform.position, accelDirection,);

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position + -chassisRigidbody.transform.up * floorCheckDist, 0.5f);
        Gizmos.color = Color.green;
        //Gizmos.DrawSphere(transform.position + -chassisRigidbody.transform.up * magnetiseDistance, 0.5f);

        Gizmos.color = Color.magenta;
        Gizmos.DrawRay(transform.position, attraction);
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, repulsion);

        Gizmos.color = Color.blue;
        Gizmos.DrawCube(hoverPos, new Vector3(1f, 1f, 1f));

    }

    void Update()
    {

        //Thrust
        currThrust = Input.GetAxis("RightTrigger") * forwardAcl;
        currBrake = Input.GetAxis("LeftTrigger") * backwardAcl;

        currTurn = Input.GetAxis("LeftStickHorizontal") * turnStrength * Input.GetAxis("RightTrigger");

        Debug.Log(VelocityFilter.GetLocalVelocity(parentRigidbody));

    }

    void FixedUpdate()
    {

        VelocityFilter.DampY(parentRigidbody, maxYVelocity, yDamp);
        //Lock chassis to position.

        //TODO: Add phyiscality with a configurable joint.
        chassisRigidbody.transform.position = transform.position;

        //Update the dynamic up point.
        dynamicUpPoint.transform.position = dynamicUpPos;
        //Then update our heading.
        Vector3 dynamicUp = dynamicUpPoint.transform.position - transform.position;

        //Have the gimbal look at the up position.
        gimbal.transform.LookAt(dynamicUpPoint.transform, transform.up);
        //Lerp the parent player object to match the gimbal rotation.
        transform.rotation = Quaternion.Lerp(transform.rotation, gimbal.transform.rotation, Time.deltaTime * orientationSpeed);

        chassisRigidbody.AddTorque(chassisRigidbody.transform.up * turnStrength * Input.GetAxis("LeftStickHorizontal"));

        if (Physics.Raycast(transform.position, -chassisRigidbody.transform.up, out hit, floorCheckDist, layerMask))
        {

            //Do this always when we are inside floor check distance.

            float orientationSpeedAdj = orientationSpeed * (1 / hit.distance);
            float magnetStrengthAdj = magnetStrength * (1 / hit.distance);

            Vector3 smoothNormal = NormalSmoother.SmoothedNormal(hit);
            Debug.DrawRay(hit.point, smoothNormal * 5.0f, Color.white, 2.0f);

            hoverPos = hit.point + smoothNormal * hoverHeight;

            if (hit.distance > magnetiseDistance)
            {
                //If we haven't reached magnetise distance.
                LevelOut();
                gimbal.transform.position = transform.position;
                parentRigidbody.drag = 0;
                parentRigidbody.AddForce(Vector3.down * 50);

            }

            else if (hit.distance < magnetiseDistance)
            {

                //If we are in magentise ditance.

                //Adjust the probes predictive forward based on current speed.
                foreach (NormalProbe normalProbe in normalProbes)
                {
                    //Assign predictive lift as a percentage of velocity.
                    normalProbe.predictionFactor = (parentRigidbody.velocity.magnitude / 100) * predictiveLift;
                }

                parentRigidbody.drag = 3;

                //Snap our gimbal to our hover height.
                gimbal.transform.position = hoverPos;
                //Lerp the player position to the gimbal position.
                transform.position = hoverPos;

                //Adjust the acceleration direction to use the predictive forward from our forward probe.
                forwardDirection = forwardProbe.probedForward;
                reverseDirection = backProbe.probedForward;

                Vector3 normalHeading = hit.point - chassisRigidbody.transform.position;

                // //Normal way of doing it.
                dynamicUpPos = hit.point + smoothNormal * dynamicUpHeight;
                // //Unsmoothed(road mesh only works with this, apparently)
                // dynamicUpPos = hit.point + hit.normal * dynamicUpHeight;

                //Rotate the gimbal equal to the dive offset.
                dive = Vector3.Angle(forwardProbe.probedForward, chassisRigidbody.transform.forward);

                positionFactor = Vector3.Dot(chassisRigidbody.transform.up, hoverPos - transform.position);

                //Distance to target height.
                magnetSnapModifier = Vector3.Distance(transform.position, hoverPos);

                // attraction = (dynamicUp.normalized * repulsionModifier);
                // repulsion = (dynamicUp.normalized * attractionModifier);

                // parentRigidbody.AddForce(attraction);
                // parentRigidbody.AddForce(repulsion);

                // Forward force.
                parentRigidbody.AddForceAtPosition(forwardDirection * currThrust, forcePointFront.transform.position);
                // Backward force.
                parentRigidbody.AddForceAtPosition(reverseDirection * currBrake, forcePointBack.transform.position);

                lastNormal = hit.normal;
                lastPoint = hit.point;

            }

        }
        //If we are outside floor check distance.
        else
        {

            //Do this outside floor check distance.

            LevelOut();
            gimbal.transform.position = transform.position;
            VelocityFilter.LockUpwards(parentRigidbody, yDamp);
            parentRigidbody.drag = 0f;
            parentRigidbody.AddForce(Vector3.down * 70);

        }

        Debug.Log("Position factor is " + positionFactor);
        Debug.Log("Absolute PF is " + Mathf.Abs(positionFactor));
        Debug.Log("Distance from hover height is " + magnetSnapModifier);
        Debug.Log("Attract force is " + attractionModifier);
        Debug.Log("Repel force " + repulsionModifier);
        Debug.Log("Velocity magnitude is " + parentRigidbody.velocity.magnitude);
        Debug.Log("Normal up is " + hit.normal);
        Debug.Log("Dive angle is " + dive);

        Debug.DrawRay(forwardProbe.transform.position, forwardProbe.probedForward * 5, Color.white, 3.0f);
        Debug.DrawRay(transform.position, parentRigidbody.velocity / 20, Color.yellow, 3.0f);

    }

}