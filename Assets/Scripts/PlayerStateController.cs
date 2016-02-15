using UnityEngine;
using System.Collections;
using System;

public class PlayerStateController : MonoBehaviour {

    public enum PlayerStates
    {
        idle,
        left,
        right,
        jump,
        landing,
        falling,
        kill,
        resurrect,
        firingWeapon,
        _enumCount
    }

    public static float[] stateDelayTimer = new float[(int)PlayerStates._enumCount];

    public delegate void playerStateHandler(PlayerStates newState);

    public static event playerStateHandler onStateChange;

    void LateUpdate()
    { 
        float horizontal = Input.GetAxis("Horizontal");
        if (horizontal != 0f)
            CallStateChange(horizontal < 0 ? PlayerStates.left : PlayerStates.right);
        else
            CallStateChange(PlayerStates.idle);

        float jump = Input.GetAxis("Jump");
        if (jump > 0.0f)
            CallStateChange(PlayerStates.jump);

        float firing = Input.GetAxis("Fire1");
        if (firing > 0.0f)
            CallStateChange(PlayerStateController.PlayerStates.firingWeapon);
    }

    void CallStateChange(PlayerStates newState)
    {
        if (onStateChange != null)
            onStateChange(newState);
    }
}
