using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    public PlayerStateController.PlayerStates currentPlayerState = PlayerStateController.PlayerStates.idle;
    public GameObject playerObject = null;
    public float cameraTrackingSpeed = 0.2f;
    private Vector3 lastTargetPosition = Vector3.zero;
    private Vector3 currTargetPosition = Vector3.zero;
    private float currLerpDistance = 0.0f;

    // Use this for initialization
    void Start () {
        Vector3 playerPos = playerObject.transform.position;
        Vector3 cameraPos = transform.position;
        Vector3 startTargPos = playerPos;

        startTargPos.z = cameraPos.z;
        lastTargetPosition = startTargPos;
        currTargetPosition = startTargPos;
        currLerpDistance = 1.0f;
    }

    void OnEnable()
    {
        PlayerStateController.onStateChange += onPlayerStateChange;
    }
    void OnDisable()
    {
        PlayerStateController.onStateChange -= onPlayerStateChange;
    }

    void onPlayerStateChange(PlayerStateController.PlayerStates newState)
    {
        currentPlayerState = newState;
    }

    void LateUpdate()
    {
        onStateCycle();

        currLerpDistance += cameraTrackingSpeed;
        transform.position = Vector3.Lerp(lastTargetPosition, currTargetPosition, currLerpDistance);
    }

    void onStateCycle()
    {
        switch (currentPlayerState)
        {
            case PlayerStateController.PlayerStates.firingWeapon:
            case PlayerStateController.PlayerStates.falling:
            case PlayerStateController.PlayerStates.jump:
            case PlayerStateController.PlayerStates.landing:
            case PlayerStateController.PlayerStates.idle:
            case PlayerStateController.PlayerStates.left:
            case PlayerStateController.PlayerStates.right:
                trackPlayer();
                break;
            
                break;
        }
    }

    void trackPlayer()
    {
        Vector3 currCamPos = transform.position;
        Vector3 currPlayerPos = playerObject.transform.position;
        if (currCamPos.x == currPlayerPos.x && currCamPos.y == currPlayerPos.y)
        {
            currLerpDistance = 1f;
            lastTargetPosition = currCamPos;
            currTargetPosition = currCamPos;
            return;
        }

        currLerpDistance = 0f;
        lastTargetPosition = currCamPos;
        currTargetPosition = currPlayerPos;
        currTargetPosition.z = currCamPos.z;
    }

    void stopTrackingPlayer()
    {
        Vector3 currCamPos = transform.position;
        currTargetPosition = currCamPos;
        lastTargetPosition = currCamPos;
        currLerpDistance = 1.0f;
    }
}
