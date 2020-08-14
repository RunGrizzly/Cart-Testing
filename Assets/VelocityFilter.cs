using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VelocityFilter
{

    public static void DampY(Rigidbody rb, float max, float damp)
    {
        Vector3 localVelocity = rb.transform.InverseTransformDirection(rb.velocity);

        Debug.Log("Parent Rigidbody world velocity is + " + rb.velocity);
        //World velocity up/down is y.
        Debug.Log("Parent Rigidbody local velocity is " + localVelocity);
        //local velocity up/down is z.

        //If the local velocity z ( the relative Y) velocity exceeds our up down limit.
        if (Mathf.Abs(localVelocity.z) > max)
        {
            //Dampen it.
            localVelocity.z /= damp * 2;
        }

        rb.velocity = rb.transform.TransformDirection(localVelocity);

    }

    public static Vector3 GetLocalVelocity(Rigidbody rb)
    {
        return rb.transform.InverseTransformDirection(rb.velocity);
    }

    public static void LockUpwards(Rigidbody rb, float damp)
    {

        Vector3 rbVelocity = rb.velocity;

        if (rbVelocity.y > 0)
        {
            rbVelocity.y /= damp;
        }

        rb.velocity = rbVelocity;
    }
}