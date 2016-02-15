using UnityEngine;
using System.Collections;

public class PlayerColliderListener : MonoBehaviour {

    public PlayerStateListener targetStateListener = null;

    void OnTriggerEnter2D(Collider2D collidedObject)
    {
        
        switch (collidedObject.tag)
        {
            case "Platform":
                targetStateListener.onStateChange(PlayerStateController.PlayerStates.landing);
                break;
            case "DeathTrigger":
                targetStateListener.onStateChange(PlayerStateController.PlayerStates.kill);
                break;
        }
    }

    void OnTriggerExit2D(Collider2D collidedObject)
    {
        switch (collidedObject.tag)
        {
            case "Platform":
                targetStateListener.onStateChange(PlayerStateController.PlayerStates.falling);
                break;
            case "DeathTrigger":
                break;
        }
    }


}
