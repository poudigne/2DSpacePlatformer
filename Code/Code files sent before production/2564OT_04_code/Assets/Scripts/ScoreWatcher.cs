using UnityEngine;
using System.Collections;
public class ScoreWatcher : MonoBehaviour
{
	public int currScore = 0;
	private TextMesh scoreMesh = null;
	
	void Start()
	{
		scoreMesh = gameObject.GetComponent<TextMesh>();
		scoreMesh.text = "0";
	}
	
	void OnEnable()
	{
		EnemyControllerScript.enemyDied += addScore;
	}
	
	void OnDisable()
	{
		EnemyControllerScript.enemyDied -= addScore;
	}
	
	void addScore(int scoreToAdd)
	{
		currScore += scoreToAdd;
		scoreMesh.text = currScore.ToString();
	}
}