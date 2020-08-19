using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CarInfoDisplay : MonoBehaviour
{
    Drivable drivable;
    Cart playerCart;

    public TextMeshProUGUI velocityDisplay;

    public TextMeshProUGUI localVelocityXDisp;
    public TextMeshProUGUI localVelocityYDisp;
    public TextMeshProUGUI localVelocityZDisp;
    public TextMeshProUGUI magnetCheckDisp;
    public TextMeshProUGUI seeFloorDisp;
    public TextMeshProUGUI forwardForceDisp;
    public TextMeshProUGUI reverseForceDisp;

    public RectTransform energyBar;

    private void Start()
    {
        drivable = GameObject.FindGameObjectWithTag("Player").GetComponent<Drivable>();
        playerCart = drivable.playerCart;
        GameManager.ins.carInfoDisplay = this;
        GetComponent<Canvas>().worldCamera = GameManager.ins.uiCamera;
        GetComponent<Canvas>().planeDistance = 1;
    }

    private void Update()
    {

        energyBar.localScale = new Vector3(playerCart.currEnergy / playerCart.maxEnergy, 1, 1);

        velocityDisplay.text = "Velocity: " + "\n" + Mathf.RoundToInt(drivable.parentRigidbody.velocity.magnitude).ToString();
        localVelocityXDisp.text = "X Velocity: " + "\n" + Mathf.RoundToInt(VelocityFilter.GetLocalVelocity(drivable.chassisRigidbody).x);
        localVelocityYDisp.text = "Y Velocity: " + "\n" + Mathf.RoundToInt(VelocityFilter.GetLocalVelocity(drivable.chassisRigidbody).y);
        localVelocityZDisp.text = "Z Velocity: " + "\n" + Mathf.RoundToInt(VelocityFilter.GetLocalVelocity(drivable.chassisRigidbody).z);
        forwardForceDisp.text = "Forward: " + "\n" + Mathf.RoundToInt(drivable.forwardForce.magnitude).ToString();
        reverseForceDisp.text = "Reverse: " + "\n" + Mathf.RoundToInt(drivable.reverseForce.magnitude).ToString();

        if (drivable.seeFloor == true)
        {
            seeFloorDisp.color = Color.green;
        }
        else
        {
            seeFloorDisp.color = Color.red;
        }

        if (drivable.isMagnetised == true)
        {

            magnetCheckDisp.color = Color.green;
        }
        else
        {

            magnetCheckDisp.color = Color.red;
        }

    }
}