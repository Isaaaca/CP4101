using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : RoomConfinedCamera
{
    private void Update()
    {
        if (player.GetVelocity().x > 3f &&
            player.IsFacingRight())
        {
            vcamFrame.m_ScreenX = 0.25f;
        }
        else if (player.GetVelocity().x < -3f &&
            !player.IsFacingRight())
        {
            vcamFrame.m_ScreenX = 0.75f;

        }
    }
}
