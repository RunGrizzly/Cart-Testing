using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CartCharacterController : MonoBehaviour
{

    void Update()
    {
        CharacterController controller = GetComponent<CharacterController>();

        // Rotate around y - axis
        transform.Rotate(0, Input.GetAxis("LeftStickHorizontal") * rotateSpeed, 0);

        // Move forward / backward
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        float curSpeed = speed * Input.GetAxis("RightTrigger");
        controller.SimpleMove(forward * curSpeed);
    }

    private void FixedUpdate()
    {
        // //Dynamic up position;
        dynamicUpPoint.transform.position = dynamicUpPos;
        //Heading to dynamic up.
        Vector3 dynamicUp = dynamicUpPoint.transform.position - transform.position;
        //Lock the gimbal to our position.
        gimbal.transform.position = transform.position;
        //Have the gimbal look at the up position.
        gimbal.transform.LookAt(dynamicUpPoint.transform, transform.up);
    }
}