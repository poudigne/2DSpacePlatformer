using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class PlayerStateListener : MonoBehaviour
{         
	public float playerWalkSpeed = 3f;
	
	private Animator playerAnimator = null;
	private PlayerStateController.playerStates currentState = PlayerStateController.playerStates.idle;
    
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
			break;         

			case PlayerStateController.playerStates.resurrect:
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
			break;
              
			case PlayerStateController.playerStates.landing:
			break;
              
			case PlayerStateController.playerStates.falling:
			break;              
              
			case PlayerStateController.playerStates.kill:
			break;         

			case PlayerStateController.playerStates.resurrect:
			break;                   
		}
         
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
		}          
		return returnVal;
	}
}
