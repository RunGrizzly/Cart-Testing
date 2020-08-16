using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drivable : MonoBehaviour
{

    //The raycast with important hit information related to the normal of the surface.
    RaycastHit hit;

    public bool seeFloor;
    public bool magnetised;

    [Space(5)]
    [Header("Required References")]
    public GameObject forcePointFront;
    public GameObject forcePointBack;
    public GameObject gimbal;
    public GameObject dynamicUpPoint;
    public GameObject body;
    public GameObject normalMarker;
    [Space(5)]
    public NormalProbe forwardProbe;
    public NormalProbe backProbe;
    public NormalProbe leftProbe;
    public NormalProbe rightProbe;
    public List<NormalProbe> normalProbes = new List<NormalProbe>();
    [Space(5)]
    public Rigidbody parentRigidbody;
    public Rigidbody chassisRigidbody;
    public List<Rigidbody> wheels = new List<Rigidbody>();
    [Space(5)]
    public LayerMask layerMask;

    Vector3 forwardDirection;
    Vector3 reverseDirection;
    Vector3 lastNormal = Vector3.zero;
    Vector3 lastPoint = Vector3.zero;
    Vector3 attraction;
    Vector3 repulsion;
    Vector3 dynamicUpPos;
    Vector3 dynamicUpHeading;
    Vector3 hoverPos;

    [Space(5)]
    [Header("Physics Limits")]
    public float maxVelocity;
    public float maxYVelocity;
    public float yDamp;

    [Space(5)]
    [Header("Physics Parameters")]
    public float dynamicUpHeight;
    public float magnetiseDistance;
    public float orientationSpeed;
    public float magnetStrength;
    public float predictiveLift;
    public float floorCheckDist;
    public float levelSpeed;
    public float hoverHeight;
    //Distance to target height.
    float magnetSnapModifier;
    //Rotate the gimbal equal to the dive offset.
    float dive;
    float positionFactor;

    [Space(5)]
    [Header("Turning")]
    public float turnStrength;
    public float handbrakeStrength;
    float turnModifier;
    [Range(-45, 45)]
    float currTurn = 0.0f;
    [Header("Movement")]
    public float forwardAcl;
    public float backwardAcl;
    public float brakeStrength;
    float currThrust = 0.0f;
    float currBrake = 0.0f;

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

    //If we're upside down and in freefall.
    void LevelOut()
    {
        //Level self
        dynamicUpPos = Vector3.Lerp(dynamicUpPos, transform.position + Vector3.up * dynamicUpHeight, Time.deltaTime * levelSpeed);
    }
    //If we are sticking to a surface.
    void UpdatePlayerUp()
    {
        //Update the dynamic up point. Lerp from current point to new point.
        dynamicUpPoint.transform.position = dynamicUpPos;

        //Have the gimbal look at the up position.
        gimbal.transform.LookAt(dynamicUpPoint.transform, transform.up);

        //Lerp the parent player object to match the gimbal rotation.
        transform.rotation = Quaternion.Lerp(transform.rotation, gimbal.transform.rotation, Time.deltaTime * orientationSpeed);

        //Then update our heading.
        dynamicUpHeading = dynamicUpPoint.transform.position - transform.position;
    }

    IEnumerator DoHandbrake()
    {

        Debug.Log("Handbraking");

        //Initial boost.
        parentRigidbody.AddForce(chassisRigidbody.transform.forward * ((VelocityFilter.GetLocalVelocity(chassisRigidbody).z / 10 * Mathf.Abs(Input.GetAxisRaw("LeftStickHorizontal"))) * handbrakeStrength));

        while (Input.GetButton("RightBumper"))
        {
            //Modify turn based on velocity and handbrake strength.

            turnModifier = Mathf.Abs(VelocityFilter.GetLocalVelocity(chassisRigidbody).x) / 100 * handbrakeStrength;

            yield return null;
        }

        //Reset turn
        turnModifier = turnStrength;

    }

    IEnumerator DoBoost()
    {

        Debug.Log("Boosting");

        float t = 0;

        while (Input.GetButton("ControllerA") && t < 2.25f)
        {
            //Modify turn based on velocity and handbrake strength.
            currThrust *= 1.25f;
            t += Time.deltaTime;

            yield return null;
        }

    }

    void Update()
    {

        //Thrust
        currThrust = Input.GetAxis("RightTrigger") * forwardAcl;
        currBrake = Input.GetAxis("LeftTrigger") * backwardAcl;
        //Turning
        currTurn = Input.GetAxis("LeftStickHorizontal") * turnModifier;

        Debug.Log(VelocityFilter.GetLocalVelocity(parentRigidbody));

    }

    void FixedUpdate()
    {

        VelocityFilter.DampY(parentRigidbody, maxYVelocity, yDamp);

        //Lock chassis to position.
        //TODO: Add phyiscality with a configurable joint.
        chassisRigidbody.transform.position = transform.position;

        UpdatePlayerUp();

        chassisRigidbody.AddTorque(chassisRigidbody.transform.up * currTurn);

        if (Physics.Raycast(transform.position, -chassisRigidbody.transform.up, out hit, floorCheckDist, layerMask))
        {

            seeFloor = true;

            //Do this always when we are inside floor check distance.
            LevelOut();
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
                parentRigidbody.drag = 0.5f;
                parentRigidbody.AddForce(Vector3.down * 50);
                turnModifier = turnStrength;

            }

            else if (hit.distance < magnetiseDistance)
            {

                magnetised = true;

                //If we are in magentise ditance.

                // //Normal way of doing it.
                dynamicUpPos = hit.point + smoothNormal * dynamicUpHeight;

                //Normal probe has a prediction factor that lifts the predicted normal.
                //When we are driving forwards, up slopes, we need to adjust this to reflect the change in angle.
                float angleSlack = Vector3.Angle(new Vector3(0, VelocityFilter.GetLocalVelocity(chassisRigidbody).y, 0), forwardDirection);
                //Account for LOOK AT rotational offset.
                angleSlack -= 90;
                Debug.Log("Angle slack is " + angleSlack);
                //Adjust the forward prediction based on the discrepency between current forward velocity and predictied forward.
                //The higher/lower this number means we are driving into or away from the surface. Very useful.
                forwardProbe.predictionFactor = (angleSlack / 100) * predictiveLift;

                //Handbrake
                if (Input.GetButtonDown("RightBumper"))
                {

                    StartCoroutine(DoHandbrake());

                }

                if (Input.GetButtonDown("ControllerA"))
                {

                    StartCoroutine(DoBoost());

                }

                parentRigidbody.drag = 3;

                //Snap our gimbal to our hover height.
                gimbal.transform.position = hoverPos;
                //Lerp the player position to the gimbal position.
                transform.position = Vector3.Lerp(transform.position, gimbal.transform.position, Time.deltaTime * magnetStrength);
                //Adjust the acceleration direction to use the predictive forward from our forward probe.
                forwardDirection = forwardProbe.probedForwardAdj;
                reverseDirection = backProbe.probedForward;

                Vector3 normalHeading = hit.point - chassisRigidbody.transform.position;

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

            seeFloor = false;
            magnetised = false;

            //Do this outside floor check distance.

            LevelOut();
            gimbal.transform.position = transform.position;
            //VelocityFilter.LockUpwards(parentRigidbody, yDamp);
            parentRigidbody.drag = 0.5f;
            parentRigidbody.AddForce(Vector3.down * 60);
            turnModifier = turnStrength;

        }

        Debug.Log("Position factor is " + positionFactor);
        Debug.Log("Absolute PF is " + Mathf.Abs(positionFactor));
        Debug.Log("Distance from hover height is " + magnetSnapModifier);
        Debug.Log("Velocity magnitude is " + parentRigidbody.velocity.magnitude);
        Debug.Log("Normal up is " + hit.normal);
        Debug.Log("Dive angle is " + dive);
        Debug.Log("Current turn = " + currTurn);
        Debug.Log("Sideways velocity is " + Mathf.Abs(VelocityFilter.GetLocalVelocity(parentRigidbody).x));

        //Debug.DrawRay(forwardProbe.transform.position, forwardProbe.probedForward * 5, Color.white, 3.0f);
        Debug.DrawRay(transform.position, parentRigidbody.velocity / 20, Color.yellow, 3.0f);

    }

}