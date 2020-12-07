using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CamController: MonoBehaviour
{
    public static CamController Instance { get; private set; }

    [SerializeField] private Cinemachine.CinemachineVirtualCamera followCam = null;
    [SerializeField] private Cinemachine.CinemachineVirtualCamera centeredCam = null;
    [SerializeField] private Cinemachine.CinemachineVirtualCamera jumpCam = null;
    [SerializeField] private float defaultShakeIntensity = 2.5f;
    private float shakeTimer=0;

    private void Awake()
    {
        CamController.Instance = this;
    }

    public void CameraShake(float duration)
    {
        GetCurrentVcam().GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = defaultShakeIntensity;
        shakeTimer = shakeTimer > 0 ? shakeTimer + duration: duration;
    }

    public void CameraShake(float duration, float intensity)
    {
        float amplitude = GetCurrentVcam().GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain;
        GetCurrentVcam().GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = amplitude> intensity? amplitude: intensity;
        shakeTimer = shakeTimer> duration? shakeTimer: duration;
    }

    private void Update()
    {
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;
            if(shakeTimer<=0)
                GetCurrentVcam().GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0;
        }
    }

    public enum CameraMode
    {
        Follow,
        Center,
        Jump
    }

    private CameraMode camMode = CameraMode.Follow;

    public void SwitchCamera(CameraMode mode)
    {
        if(camMode != mode)
        {
            if (mode == CameraMode.Follow)
            {
                followCam.enabled = true;
                centeredCam.enabled = false;
                jumpCam.enabled = false;
                camMode = CameraMode.Follow;
            }
            else if (mode == CameraMode.Center)
            {
                followCam.enabled = false;
                centeredCam.enabled = true;
                jumpCam.enabled = false;
                camMode = CameraMode.Center;
            }
            else if (mode == CameraMode.Jump)
            {
                followCam.enabled = false;
                centeredCam.enabled = false;
                jumpCam.enabled = true;
                camMode = CameraMode.Jump;
            }
        }
    }

    public void JumpToTarget(Transform target)
    {
        SwitchCamera(CameraMode.Jump);
        ChangeFollowTarget(target);
    }
    public void PanToTarget(Transform target)
    {
        SwitchCamera(CameraMode.Center);
        ChangeFollowTarget(target);
    }

    public void ChangeFollowTarget(Transform target)
    {
        GetCurrentVcam().Follow = (target);
    }

    public CameraMode CurrentCameraMode()
    {
        return camMode;
    }

    private CinemachineVirtualCamera GetCurrentVcam()
    {
        if (camMode == CameraMode.Follow)
            return followCam;
        else if (camMode == CameraMode.Center)
            return centeredCam;
        else if (camMode == CameraMode.Jump)
            return jumpCam;
        else
            return null;
    }
}
