using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class PlayerStateListener : MonoBehaviour {

    public float playerWalkSpeed = 3f;
    public float playerJumpForceVertical = 500f;
    public float playerJumpForceHorizontal = 200f;
    public GameObject playerRespawnPoint = null;
    public GameObject bulletPrefab = null;
    public Transform bulletSpawnTransform;

    private Animator playerAnimator = null;
    private PlayerStateController.PlayerStates previousState = PlayerStateController.PlayerStates.idle;
    private PlayerStateController.PlayerStates currentState = PlayerStateController.PlayerStates.idle;
    private bool playerHasLanded = true;

    private Rigidbody2D body;

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
        body = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>();
        PlayerStateController.stateDelayTimer[(int)PlayerStateController.PlayerStates.jump] = 1.0f;
        PlayerStateController.stateDelayTimer[(int)PlayerStateController.PlayerStates.firingWeapon] = 1.0f;
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
            case PlayerStateController.PlayerStates.jump:
                break;
            case PlayerStateController.PlayerStates.landing:
                break;
            case PlayerStateController.PlayerStates.falling:
                break;
            case PlayerStateController.PlayerStates.kill:
                onStateChange(PlayerStateController.PlayerStates.resurrect);
                break;
            case PlayerStateController.PlayerStates.resurrect:
                onStateChange(PlayerStateController.PlayerStates.idle);
                break;
            case PlayerStateController.PlayerStates.firingWeapon:
                break;
        }
    }

    public void onStateChange(PlayerStateController.PlayerStates newState) 
    {
        
        if (newState == currentState)
            return;
        if (!checkForValidStatePair(newState))
            return;
        if (checkIfAbortOnStateCondition(newState))
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
            case PlayerStateController.PlayerStates.jump:
                if (playerHasLanded)
                {
                    float jumpDirection = 0.0f;
                    if (currentState == PlayerStateController.PlayerStates.left)
                        jumpDirection = -1.0f;
                    else if (currentState == PlayerStateController.PlayerStates.right)
                        jumpDirection = 1.0f;
                    else
                        jumpDirection = 0.0f;

                    body.AddForce(new Vector2(jumpDirection * playerJumpForceHorizontal, playerJumpForceVertical));

                    playerHasLanded = false;
                    PlayerStateController.stateDelayTimer[(int)PlayerStateController.PlayerStates.jump] = 0f;
                }
                break;

            case PlayerStateController.PlayerStates.landing:
                playerHasLanded = true;
                PlayerStateController.stateDelayTimer[(int)PlayerStateController.PlayerStates.jump] = Time.time + 0.1f;
                break;
            case PlayerStateController.PlayerStates.falling:
                PlayerStateController.stateDelayTimer[(int)PlayerStateController.PlayerStates.jump] = 0f;
                break;
            case PlayerStateController.PlayerStates.kill:
                break;
            case PlayerStateController.PlayerStates.resurrect:
                transform.position = playerRespawnPoint.transform.position;
                transform.rotation = Quaternion.identity;
                body.velocity = Vector2.zero;
                break;
            case PlayerStateController.PlayerStates.firingWeapon:
                GameObject newBullet = (GameObject)Instantiate(bulletPrefab);
                newBullet.transform.position = bulletSpawnTransform.position;
                PlayerBulletController bullCon = newBullet.GetComponent<PlayerBulletController>();
                bullCon.player = gameObject;
                bullCon.launchBullet();
                onStateChange(currentState);
                PlayerStateController.stateDelayTimer[(int)PlayerStateController.PlayerStates.firingWeapon] = Time.time + 0.25f;
                break;
        }

        previousState = currentState;
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
            case PlayerStateController.PlayerStates.jump:
                if (newState == PlayerStateController.PlayerStates.landing
                    || newState == PlayerStateController.PlayerStates.kill
                    || newState == PlayerStateController.PlayerStates.firingWeapon)
                    returnVal = true;
                else
                    returnVal = false;
                break;
            case PlayerStateController.PlayerStates.landing:
                if (newState == PlayerStateController.PlayerStates.left
                    || newState == PlayerStateController.PlayerStates.right
                    || newState == PlayerStateController.PlayerStates.idle
                    || newState == PlayerStateController.PlayerStates.firingWeapon)
                    returnVal = true;
                else
                    returnVal = false;
                break;
            case PlayerStateController.PlayerStates.kill:
                if (newState == PlayerStateController.PlayerStates.resurrect)
                    returnVal = true;
                else
                    returnVal = false;
                break;
            case PlayerStateController.PlayerStates.resurrect:
                if (newState == PlayerStateController.PlayerStates.idle)
                    returnVal = true;
                else
                    returnVal = false;
                break;
            case PlayerStateController.PlayerStates.falling:
                returnVal = true;
                break;
            case PlayerStateController.PlayerStates.firingWeapon:
                returnVal = true;
                break;
        }
        return returnVal;
    }

    bool checkIfAbortOnStateCondition(PlayerStateController.PlayerStates newState)
    {
        bool returnVal = false;
        switch (newState)
        {
            case PlayerStateController.PlayerStates.idle:
                break;
            case PlayerStateController.PlayerStates.left:
                break;
            case PlayerStateController.PlayerStates.right:
                break;

            case PlayerStateController.PlayerStates.jump:
                float nextAllowedJumpTime = PlayerStateController.stateDelayTimer[(int)PlayerStateController.PlayerStates.jump];
                if (nextAllowedJumpTime == 0.0f || nextAllowedJumpTime > Time.time)
                    returnVal = true;
                break;
            case PlayerStateController.PlayerStates.landing:
                break;
            case PlayerStateController.PlayerStates.falling:
                break;
            case PlayerStateController.PlayerStates.kill:
                break;
            case PlayerStateController.PlayerStates.resurrect:
                break;
            case PlayerStateController.PlayerStates.firingWeapon:
                if (PlayerStateController.stateDelayTimer[(int)PlayerStateController.PlayerStates.firingWeapon] > Time.time)
                    returnVal = true;
                break;
        }
        return returnVal;
    }

    public void hitDeathTrigger()
    {
        onStateChange(PlayerStateController.PlayerStates.kill);
    }
}
