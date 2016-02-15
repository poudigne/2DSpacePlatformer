﻿using UnityEngine;
using System.Collections;

public class PlayerStateController : MonoBehaviour 
{
	public enum playerStates
	{
		idle = 0,
		left,
		right,
		jump,
		landing,
		falling,
		kill,
		resurrect
	}
		
	public delegate void playerStateHandler(PlayerStateController.playerStates newState);
	public static event playerStateHandler onStateChange;
	
	void LateUpdate () 
	{
		// Detect the current input of the Horizontal axis, then broadcast a state update for the player as appropriate
		float horizontal = Input.GetAxis("Horizontal");
		if(horizontal != 0.0f)
		{
			if(horizontal < 0.0f)
			{
				if(onStateChange != null)
					onStateChange(PlayerStateController.playerStates.left);
			}
			else
			{
				if(onStateChange != null)
					onStateChange(PlayerStateController.playerStates.right);
			}
		}
		else
		{
			if(onStateChange != null)
				onStateChange(PlayerStateController.playerStates.idle);
		}
	}
}