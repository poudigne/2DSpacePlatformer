using UnityEngine;
using System.Collections;

public class PlayerBulletController : MonoBehaviour {

    public GameObject player = null;
    public float bulletSpeed = 15.0f;

    private float selfDestructTimer = 0.0f;
    private Rigidbody2D body;

    public void launchBullet()
    {
        Debug.Log("Bullet Launched");
        float mainXScale = player.transform.localScale.x;
        Vector2 bulletForce;
        if (mainXScale < 0.0f)
            bulletForce = new Vector2(bulletSpeed * -1, 0);
        else
            bulletForce = new Vector2(bulletSpeed, 0);

        body.velocity = bulletForce;
        selfDestructTimer = Time.time + 1.0f;
    }

	// Use this for initialization
	void Awake() {
        body = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
        if (selfDestructTimer > 0.0f && selfDestructTimer < Time.time)
            Destroy(gameObject);
	}
}
