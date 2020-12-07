using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomConfinedCamera : MonoBehaviour
{
    protected Cinemachine.CinemachineConfiner confiner;
    protected Cinemachine.CinemachineVirtualCamera vcam;
    protected Cinemachine.CinemachineFramingTransposer vcamFrame;
    protected Transform followTarget;
    [SerializeField] protected PlayerController player = null;


    private void Awake()
    {
        Room.OnEnterRoom += ChangeRoom;
    }
    private void OnDestroy()
    {
        Room.OnEnterRoom -= ChangeRoom;

    }

    // Start is called before the first frame update
    void Start()
    {
        confiner = GetComponent<Cinemachine.CinemachineConfiner>();
        vcam = GetComponent<Cinemachine.CinemachineVirtualCamera>();
        vcamFrame = vcam.GetCinemachineComponent<Cinemachine.CinemachineFramingTransposer>();
        followTarget = player.transform;
    }

    void ChangeRoom(Room room)
    {
        confiner.InvalidatePathCache();
        confiner.m_BoundingShape2D = room.GetCollider2D();
    }
}
