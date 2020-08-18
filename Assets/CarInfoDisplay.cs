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
    public TextMeshProUGUI magnetCheckDisp;
    public TextMeshProUGUI seeFloorDisp;
    public TextMeshProUGUI forwardForceDisp;
    public TextMeshProUGUI reverseForceDisp;

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Drivable>();
        GameManager.ins.carInfoDisplay = this;
        GetComponent<Canvas>().worldCamera = GameManager.ins.uiCamera;
        GetComponent<Canvas>().planeDistance = 1;
    }

    private void Update()
    {
        velocityDisplay.text = "Velocity: " + "\n" + Mathf.RoundToInt(target.parentRigidbody.velocity.magnitude).ToString();
        localVelocityXDisp.text = "X Velocity: " + "\n" + Mathf.RoundToInt(VelocityFilter.GetLocalVelocity(target.chassisRigidbody).x);
        localVelocityYDisp.text = "Y Velocity: " + "\n" + Mathf.RoundToInt(VelocityFilter.GetLocalVelocity(target.chassisRigidbody).y);
        localVelocityZDisp.text = "Z Velocity: " + "\n" + Mathf.RoundToInt(VelocityFilter.GetLocalVelocity(target.chassisRigidbody).z);
        forwardForceDisp.text = "Forward: " + "\n" + Mathf.RoundToInt(target.forwardForce.magnitude).ToString();
        reverseForceDisp.text = "Reverse: " + "\n" + Mathf.RoundToInt(target.reverseForce.magnitude).ToString();

        if (target.seeFloor == true)
        {
            seeFloorDisp.color = Color.green;
        }
        else
        {
            seeFloorDisp.color = Color.red;
        }

        if (target.isMagnetised == true)
        {

            magnetCheckDisp.color = Color.green;
        }
        else
        {

            magnetCheckDisp.color = Color.red;
        }

    }
}