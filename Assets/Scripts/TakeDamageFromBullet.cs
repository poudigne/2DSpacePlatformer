using UnityEngine;
using System.Collections;

public class TakeDamageFromBullet : MonoBehaviour {

    public delegate void hitByPlayerBullet();
    public event hitByPlayerBullet hitByBullet;
	
    void OnTriggerEnter2D(Collider2D collidedObject)
    {
        Debug.Log(collidedObject.tag);
        if (collidedObject.tag == "Player Bullet")
        {
            if (hitByBullet != null)
                hitByBullet();
        }
    }
}
