using UnityEngine;
using System.Collections;

public class EnemyGuideWatcher : MonoBehaviour {

    public EnemyController enemyObject = null;

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Platform")
        {
            enemyObject.switchDirections();
        }
            
    }
}
