using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class PlayerStateListener : MonoBehaviour {

    public float playerWalkSpeed = 3f;
    public float playerJumpForceVertical = 500f;
    public float playerJumpForceHorizontal = 250f;
    public GameObject playerRespawnPoint = null;
    public GameObject bulletPrefab = null;
    public Transform bulletSpawnTransform;

    private Animator playerAnimator = null;
    private PlayerStateController.PlayerStates previousState = PlayerStateController.PlayerStates.idle;
    private PlayerStateController.PlayerStates currentState = PlayerStateController.PlayerStates.idle;
    private bool playerHasLanded = true;

    void OnEnable()
    {
        PlayerStateController.onStateChange += onStateChange;
    }

    void OnDisable()
    {
        PlayerStateController.onStateChange -= onStateChange;
    }


    void Start()
    {
        playerAnimator = GetComponent<Animator>();
    }

    void LateUpdate()
    {
        onStateCycle();
    }

    void onStateCycle()
    {
        Vector3 localScale = transform.localScale;

        switch (currentState)
        {
            case PlayerStateController.PlayerStates.idle:
                break;
            case PlayerStateController.PlayerStates.left:
                transform.Translate(new Vector3((playerWalkSpeed * -1.0f) * Time.deltaTime, 0.0f, 0.0f));
                if (localScale.x > 0.0f)
                {
                    localScale.x *= -1.0f;
                    transform.localScale = localScale;
                }

                break;
            case PlayerStateController.PlayerStates.right:
                transform.Translate(new Vector3(playerWalkSpeed * Time.deltaTime, 0.0f, 0.0f));
                if (localScale.x < 0.0f)
                {
                    localScale.x *= -1.0f;
                    transform.localScale = localScale;
                }
                break;
        }
    }

    public void onStateChange(PlayerStateController.PlayerStates newState) 
    {
        if (newState == currentState)
            return;

        if (!checkForValidStatePair(newState))
            return;

        switch (newState)
        {
            case PlayerStateController.PlayerStates.idle:
                playerAnimator.SetBool("Walking", false);
                break;
            case PlayerStateController.PlayerStates.left:
            case PlayerStateController.PlayerStates.right:
                playerAnimator.SetBool("Walking", true);
                break;
        }

        currentState = newState;
    }

    bool checkForValidStatePair(PlayerStateController.PlayerStates newState)
    {
        bool returnVal = false;
        switch (currentState)
        {
            case PlayerStateController.PlayerStates.idle:
                returnVal = true;
                break;
            case PlayerStateController.PlayerStates.left:
                returnVal = true;
                break;
            case PlayerStateController.PlayerStates.right:
                returnVal = true;
                break;
        }
        return returnVal;
    }
}
