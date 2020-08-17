using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Cinemachine.Utility;
using UnityEngine;

public class CameraMotionControl : MonoBehaviour
{

    public CinemachineVirtualCamera cmCam;

    private void Start()
    {
        cmCam = GetComponentInChildren<CinemachineVirtualCamera>();
    }

    // if (target.isMagnetised == false)
    // {
    //     cinemachineFollow.GetCinemachineComponent<CinemachineTransposer>().m_RollDamping = 5.0f;
    // }
    // if (target.isMagnetised == true)
    // {
    //     cinemachineFollow.GetCinemachineComponent<CinemachineTransposer>().m_RollDamping = 0.2f;
    // }

    public void FreezePitch()
    {
        // public CinemachineVirtualCamera cinemachineFollow;
        // public CinemachineBrain cinemachineBrain;
        // public Drivable target;
    }

    public void FreezeRoll(bool state)
    {

        if (state == true)
        {
            cmCam.GetCinemachineComponent<CinemachineTransposer>().m_RollDamping = 20f;
        }
        else
        {
            cmCam.GetCinemachineComponent<CinemachineTransposer>().m_RollDamping = 0.2f;
        }

    }

    public void FreezeYaw()
    {

    }
}