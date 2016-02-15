using UnityEngine;
using System.Collections;

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
        resurrect
    }

    public delegate void playerStateHandler(PlayerStates newState);

    public static event playerStateHandler onStateChange;

    void LateUpdate()
    { 
        float horizontal = Input.GetAxis("Horizontal");
        if (horizontal != 0f)
        {
            CallStateChange(horizontal < 0 ? PlayerStates.left : PlayerStates.right);
        }
        else
        {
            CallStateChange(PlayerStates.idle);
        }
    }

    void CallStateChange(PlayerStates newState)
    {
        if (onStateChange != null)
        {
            onStateChange(newState);
        }
    }
}
