using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drivable : MonoBehaviour
{
    //The raycast with important hit information related to the normal of the surface.
    RaycastHit hit;
    Vector3 lastNormal = Vector3.zero;
    Vector3 lastPoint = Vector3.zero;

    public Vector3 smoothNormal;
    public float trackSmoothing;

    public GameObject gimbal;

    public GameObject body;

    public NormalProbe forwardProbe;
    public NormalProbe backProbe;
    public NormalProbe leftProbe;
    public NormalProbe rightProbe;
    public List<NormalProbe> normalProbes = new List<NormalProbe>();

    public GameObject dynamicUpPoint;
    public float dynamicUpHeight;
    public float orientationSpeed;
    Vector3 dynamicUpPos;

    public Rigidbody parentRigidbody;
    public Rigidbody chassisRigidbody;
    float deadZone = 0.1f;

    [Range(0, 20.0f)]
    public float magnetStrength;

    [Range(0, 2.0f)]
    public float magnetThreshold;

    public float floorCheckDist;

    public float rotationFix;

    GameObject orientation;
    public GameObject normalMarker;
    // GameObject normalMarkerClone;

    public Transform com;

    public float hoverForce = 1500f;
    public float gravityForce = 1000f;
    public float hoverHeight = 1.35f;
    public List<Rigidbody> wheels = new List<Rigidbody>();

    public float forwardAcl = 10000.0f;
    public float brakeStrength = 2500.0f;
    float currThrust = 0.0f;
    float currBrake = 0.0f;

    public float turnStrength = 10f;
    [Range(-45, 45)]
    float currTurn = 0.0f;

    public GameObject leftAirBrake;
    public GameObject rightAirBrake;
    //public ParticleSystem[] dustTrails = new ParticleSystem[4];

    public LayerMask layerMask;

    void Start()
    {
        // rigidbody = GetComponent<Rigidbody>();

    }

    void OnDrawGizmos()
    {
        // if (Application.isPlaying == false)
        // {
        //     dynamicUpPos = transform.position;
        //     dynamicUpPoint.transform.position = dynamicUpPos;
        //     // //Dynamic up position;
        //     dynamicUpPoint.transform.position = dynamicUpPos;
        //     //Heading to dynamic up.
        //     Vector3 dynamicUp = dynamicUpPoint.transform.position - transform.position;
        //     //Lock the gimbal to our position.
        //     gimbal.transform.position = transform.position;
        //     //Have the gimbal look at the up position.
        //     gimbal.transform.LookAt(dynamicUpPoint.transform, transform.up);
        // }

        // Gizmos.color = Color.white;
        // Gizmos.DrawWireSphere(dynamicUpPoint.transform.position, 0.5f);
        // Gizmos.color = Color.cyan;
        // Gizmos.DrawWireSphere(dynamicUpPos, 0.5f);

    }

    void Update()
    {

        // Vector3 normalsSum = normalProbes[0].normal + normalProbes[1].normal + normalProbes[2].normal + normalProbes[3].normal;

        // smoothNormal = normalsSum / normalProbes.Count;

        //Thrust
        currThrust = Input.GetAxis("RightTrigger") * forwardAcl;
        currBrake = Input.GetAxis("LeftTrigger") * brakeStrength;

        currTurn = Input.GetAxis("LeftStickHorizontal") * turnStrength * Input.GetAxis("RightTrigger");

    }

    void FixedUpdate()
    {

        chassisRigidbody.transform.position = transform.position;

        // //Dynamic up position;
        dynamicUpPoint.transform.position = dynamicUpPos;
        //Heading to dynamic up.
        Vector3 dynamicUp = dynamicUpPoint.transform.position - transform.position;
        //Lock the gimbal to our position.
        gimbal.transform.position = transform.position;
        //Have the gimbal look at the up position.
        gimbal.transform.LookAt(dynamicUpPoint.transform, transform.up);


        if (Physics.Raycast(transform.position, -chassisRigidbody.transform.up, out hit, floorCheckDist, layerMask))
        {

            orientation = new GameObject();
            orientation.name = "Orientation";
            //ChangeTest
            //Set normal marker to the transform rotation.
            //Checking to see of the 'forward' is consistent.
            // GameObject normalMarkerClone = Instantiate(normalMarker);
            // normalMarkerClone.transform.position = hit.point + hit.normal * hoverHeight;
            // normalMarkerClone.transform.rotation = Quaternion.FromToRotation(Vector3.forward, hit.normal) * Quaternion.Euler(90, 0, 0);
            // normalMarkerClone.GetComponent<Renderer>().material.color = Color.green;

            Vector3 normalHeading = hit.point - chassisRigidbody.transform.position;
            // float normalDistance = normalHeading.magnitude;
            // Vector3 normalDirection = normalHeading / normalDistance;

            // /Debug.Log("Player hit the floor");
            Debug.Log("Normal up is " + hit.normal);
            Debug.DrawLine(transform.position, hit.point, Color.green, 1.0f);

            dynamicUpPos = hit.point + hit.normal * dynamicUpHeight;

            //Make the car orient faster the further the projected up gets away.

            // Vector3 orientLag = dynamicUpPos - dynamicUpPoint.transform.position;
            // float orientV = orientLag.magnitude * orientationSpeed;

            //Lerp the parent player object to match the gimbal rotation.
            transform.rotation = Quaternion.Lerp(transform.rotation, gimbal.transform.rotation, Time.deltaTime * orientationSpeed);

            //While object is above the hover height.
            //Attract based on distance.

            //While object is below hover height.
            //Repel based on distance.

            //How to get a signed distance based on normal.
            //Get position factor from vector3 dot.

            float positionFactor = Vector3.Dot(chassisRigidbody.transform.up, hit.point + hit.normal * hoverHeight - transform.position);

            Debug.Log("Position factor is " + positionFactor);

            float attractionModifier = 0;
            float repulsionModifier = 0;

            if (positionFactor < magnetThreshold * -1)
            {
                //we are above the hover height.
                //Position factor is negative.



                attractionModifier = magnetStrength + positionFactor;
                repulsionModifier = magnetStrength + positionFactor / 2;
            }

            if (positionFactor > magnetThreshold)
            {
                //We are below the hover height
                //Position factor is positive.
                repulsionModifier = magnetStrength + positionFactor;
                attractionModifier = magnetStrength + positionFactor / 2;

                // transform.Translate(dynamicUpPos * magnetStrength * -1);

            }



            // float attraction = (1 - hit.distance / hoverHeight) * attractionModifier;
            // float repulsion = (1 - hit.distance / hoverHeight) * repulsionModifier;

            parentRigidbody.AddForce(dynamicUp * repulsionModifier);
            parentRigidbody.AddForce(dynamicUp * attractionModifier);

            // Destroy(normalMarkerClone);

            Destroy(orientation);

            // //Get averaged forward force.
            // Vector3 forwarForceAvg = chassisRigidbody.transform.forward * forwardProbe.normal

            // Forward force.
            parentRigidbody.AddForce(((chassisRigidbody.transform.forward + forwardProbe.probedForward) / 2) * currThrust);

            //transform.Translate(chassisRigidbody.transform.forward * currThrust, Space.World);

            //Turn
            //transform.Rotate(new Vector3(0, turnStrength * Input.GetAxis("LeftStickHorizontal"), 0), Space.Self);
            chassisRigidbody.AddTorque(chassisRigidbody.transform.up * turnStrength * Input.GetAxis("LeftStickHorizontal"));

            lastNormal = hit.normal;
            lastPoint = hit.point;

        }
        else
        {
            parentRigidbody.AddForce(Vector3.down * 10);
        }

    }
}