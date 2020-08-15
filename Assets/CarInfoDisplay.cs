using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CarInfoDisplay : MonoBehaviour
{
    Drivable target;

    public TextMeshProUGUI velocityDisplay;

    public TextMeshProUGUI localVelocityXDisp;
    public TextMeshProUGUI localVelocityYDisp;
    public TextMeshProUGUI localVelocityZDisp;

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Drivable>();
    }

    private void Update()
    {
        velocityDisplay.text = "Velocity: " + "\n" + Mathf.RoundToInt(target.parentRigidbody.velocity.magnitude).ToString();
        localVelocityXDisp.text = "X Velocity: " + "\n" + Mathf.RoundToInt(VelocityFilter.GetLocalVelocity(target.chassisRigidbody).x);
        localVelocityYDisp.text = "Y Velocity: " + "\n" + Mathf.RoundToInt(VelocityFilter.GetLocalVelocity(target.chassisRigidbody).y);
        localVelocityZDisp.text = "Z Velocity: " + "\n" + Mathf.RoundToInt(VelocityFilter.GetLocalVelocity(target.chassisRigidbody).z);

    }
}