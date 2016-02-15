﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class PlayerStateListener : MonoBehaviour
{         
	public float playerWalkSpeed = 3f;
	public float playerJumpForceVertical = 500f;
	public float playerJumpForceHorizontal = 250f;
	public GameObject playerRespawnPoint = null;
	public GameObject bulletPrefab = null; 
	public Transform bulletSpawnTransform;

	private Animator playerAnimator = null;
	private PlayerStateController.playerStates previousState = PlayerStateController.playerStates.idle;
	private PlayerStateController.playerStates currentState = PlayerStateController.playerStates.idle;
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
		
		// Setup any specific starting values here
		PlayerStateController.stateDelayTimer[ (int)PlayerStateController.playerStates.jump] = 1.0f;
		PlayerStateController.stateDelayTimer[ (int)PlayerStateController.playerStates.firingWeapon] = 1.0f;
	}
    
    void LateUpdate()
    {
         onStateCycle();
    }
    
	public void hitDeathTrigger()
	{
		onStateChange(PlayerStateController.playerStates.kill);
	}
	
    // Every cycle of the engine, process the current state.
    void onStateCycle()
    {
		// Grab the current localScale of the object so we have 
		// access to it in the following code
		Vector3 localScale = transform.localScale;
		
		switch(currentState)
		{
			case PlayerStateController.playerStates.idle:
			break;
        
			case PlayerStateController.playerStates.left:
				transform.Translate(new Vector3((playerWalkSpeed * -1.0f) * Time.deltaTime, 0.0f, 0.0f));
			
				if(localScale.x > 0.0f)
				{
					localScale.x *= -1.0f;
					transform.localScale  = localScale;
				}
			
			break;
             
			case PlayerStateController.playerStates.right:
				transform.Translate(new Vector3(playerWalkSpeed * Time.deltaTime, 0.0f, 0.0f));
			
				if(localScale.x < 0.0f)
				{
					localScale.x *= -1.0f;
					transform.localScale = localScale;              
				}

			break;
             
			case PlayerStateController.playerStates.jump:
			break;
             
			case PlayerStateController.playerStates.landing:
			break;
             
			case PlayerStateController.playerStates.falling:
			break;              

			case PlayerStateController.playerStates.kill:
				onStateChange(PlayerStateController.playerStates.resurrect);
			break;         

			case PlayerStateController.playerStates.resurrect:
				onStateChange(PlayerStateController.playerStates.idle);
			break;
			
			case PlayerStateController.playerStates.firingWeapon:
			break;
		}
	}
    
    // onStateChange is called whenever we make a change to the player's state 
	// from anywhere within the game's code.
	public void onStateChange(PlayerStateController.playerStates newState)
	{
		// If the current state and the new state are the same, abort - no need 
		// to change to the state we're already in.
		if(newState == currentState)
			return;
		
		// Verify there are no special conditions that would cause this state to abort
		if(checkIfAbortOnStateCondition(newState))
			return;

         
		// Check if the current state is allowed to transition into this state. If it's not, abort.
		if(!checkForValidStatePair(newState))
			return;
         
		// Having reached here, we now know that this state change is allowed. 
		// So let's perform the necessary actions depending on what the new state is.
		switch(newState)
		{
			case PlayerStateController.playerStates.idle:
				playerAnimator.SetBool("Walking", false);
			break;
         
			case PlayerStateController.playerStates.left:
				playerAnimator.SetBool("Walking", true);
			break;
              
			case PlayerStateController.playerStates.right:
				playerAnimator.SetBool("Walking", true);
			break;
              
			case PlayerStateController.playerStates.jump:                   
				if(playerHasLanded)
				{
					// Use the jumpDirection variable to specify if the player should be jumping left, right or vertical
					float jumpDirection = 0.0f;
					if(currentState == PlayerStateController.playerStates.left)
						jumpDirection = -1.0f;
					else if(currentState == PlayerStateController.playerStates.right)
						jumpDirection = 1.0f;
					else
						jumpDirection = 0.0f;
					             
					// Apply the actual jump force
					rigidbody2D.AddForce(new Vector2(jumpDirection * playerJumpForceHorizontal, playerJumpForceVertical));
									
					playerHasLanded = false;
    				PlayerStateController.stateDelayTimer[ (int)PlayerStateController.playerStates.jump] = 0f;
				}
			break;

              
			case PlayerStateController.playerStates.landing:
				playerHasLanded = true;
				PlayerStateController.stateDelayTimer[(int)PlayerStateController.playerStates.jump]= Time.time + 0.1f;
			break;
              
			case PlayerStateController.playerStates.falling:
				PlayerStateController.stateDelayTimer[ (int)PlayerStateController.playerStates.jump] = 0.0f;
			break;              
              
			case PlayerStateController.playerStates.kill:
			break;         

			case PlayerStateController.playerStates.resurrect:
				transform.position = playerRespawnPoint.transform.position;
				transform.rotation = Quaternion.identity;
			break;
			
			case PlayerStateController.playerStates.firingWeapon:
				// Make the bullet object
				GameObject newBullet = (GameObject)Instantiate(bulletPrefab);
				              
				// Setup the bullet’s starting position
				newBullet.transform.position = bulletSpawnTransform.position;
				
				// Acquire the PlayerBulletController component on the new object so we can specify some data
				PlayerBulletController bullCon = newBullet.GetComponent<PlayerBulletController>();
				
				// Set the player object
				bullCon.playerObject = gameObject;
				              
				// Launch the bullet!
				bullCon.launchBullet();    
				              
				// With the bullet made, set the state of the player back to the previous state
				onStateChange(currentState);
			
				PlayerStateController.stateDelayTimer[(int)PlayerStateController.playerStates.firingWeapon] = Time.time + 0.25f;
			break;
		}
         
		// Store the current state as the previous state
		previousState = currentState;
		
		// And finally, assign the new state to the player object
		currentState = newState;
	}    
    
	// Compare the desired new state against the current, and see if we are 
	// allowed to change to the new state. This is a powerful system that ensures 
	// we only allow the actions to occur that we want to occur.
	bool checkForValidStatePair(PlayerStateController.playerStates newState)
	{
		bool returnVal = false;

		// Compare the current against the new desired state.
		switch(currentState)
		{
			case PlayerStateController.playerStates.idle:
				// Any state can take over from idle.
				returnVal = true;
			break;
         
			case PlayerStateController.playerStates.left:
				// Any state can take over from the player moving left.
				returnVal = true;
			break;
              
			case PlayerStateController.playerStates.right:         
				// Any state can take over from the player moving right.
				returnVal = true;              
			break;
              
			case PlayerStateController.playerStates.jump:
				// The only state that can take over from Jump is landing or kill.
				if(
					newState == PlayerStateController.playerStates.landing
					|| newState == PlayerStateController.playerStates.kill
					|| newState == PlayerStateController.playerStates.firingWeapon
				  )
						returnVal = true;
				  else
						returnVal = false;
			break;
              
			case PlayerStateController.playerStates.landing:
				// The only state that can take over from landing is idle, left or right movement.
				if(
					newState == PlayerStateController.playerStates.left
					|| newState == PlayerStateController.playerStates.right
					|| newState == PlayerStateController.playerStates.idle
					|| newState == PlayerStateController.playerStates.firingWeapon
				  )
					returnVal = true;
				else
					returnVal = false;
			break;              
              
			case PlayerStateController.playerStates.falling:    
				// The only states that can take over from falling are landing or kill
				if(
					newState == PlayerStateController.playerStates.landing
					|| newState == PlayerStateController.playerStates.kill
					|| newState == PlayerStateController.playerStates.firingWeapon
				  )
					returnVal = true;
				else
					returnVal = false;
				break;              
              
			case PlayerStateController.playerStates.kill:         
				// The only state that can take over from kill is resurrect
				if(newState == PlayerStateController.playerStates.resurrect)
					returnVal = true;
				else
					returnVal = false;
			break;              
              
			case PlayerStateController.playerStates. resurrect :
				// The only state that can take over from Resurrect is Idle
				if(newState == PlayerStateController.playerStates.idle)
					returnVal = true;
				else
					returnVal = false;                          
			break;
			
			case PlayerStateController.playerStates.firingWeapon:
				returnVal = true;
			break;
		}          
		return returnVal;
	}
	
	// checkIfAbortOnStateCondition allows us to do additional state verification, to see
	// if there is any reason this state should not be allowed to begin.
	bool checkIfAbortOnStateCondition(PlayerStateController.playerStates newState)
	{
		bool returnVal = false;
		
		switch(newState)
		{
			case PlayerStateController.playerStates.idle:
			break;
			
			case PlayerStateController.playerStates.left:
			break;
			
			case PlayerStateController.playerStates.right:
			break;
			
			case PlayerStateController.playerStates.jump:
				float nextAllowedJumpTime = PlayerStateController.stateDelayTimer[ (int)PlayerStateController.playerStates.jump ];
				
				if(nextAllowedJumpTime == 0.0f || nextAllowedJumpTime > Time.time)
					returnVal = true;
			break;
			
			case PlayerStateController.playerStates.landing:
			break;
			
			case PlayerStateController.playerStates.falling:
			break;
			
			case PlayerStateController.playerStates.kill:
			break;
			
			case PlayerStateController.playerStates.resurrect:
			break;
			
			case PlayerStateController.playerStates.firingWeapon:		
				if(PlayerStateController.stateDelayTimer[ (int)PlayerStateController.playerStates.firingWeapon] > Time.time)
					returnVal = true;
			
			break;
		}
		
		// Value of true means 'Abort'. Value of false means 'Continue'.
		return returnVal;
	}

}
