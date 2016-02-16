using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    public float walkingSpeed = 0.45f;
    public TakeDamageFromBullet bulletColliderListener = null;
    public ParticleSystem deathFX001 = null;
    public ParticleSystem deathFX002 = null;
    public ParticleSystem deathFX003 = null;

    private bool walkingLeft = true;

    void OnEnable()
    {
        Debug.Log("OnEnable");
        bulletColliderListener.hitByBullet += hitByBullet;
    }
    void OnDisable()
    {
        bulletColliderListener.hitByBullet -= hitByBullet;
    }

    void Start()
    {
        walkingLeft = (Random.Range(0, 2) == 1);
        updateVisualWalkOrientation();
    }
    
    void Update()
    {
        if (walkingLeft)
        {
            transform.Translate(new Vector3(walkingSpeed * Time.deltaTime, 0.0f, 0.0f));
        }
        else
        {
            transform.Translate(new Vector3((walkingSpeed * -1.0f)* Time.deltaTime, 0.0f, 0.0f));
        }
    }

	public void switchDirections()
    {
        walkingLeft = !walkingLeft;

        updateVisualWalkOrientation();
    }

    void updateVisualWalkOrientation()
    {
        Vector3 localScale = transform.localScale;
        if (walkingLeft)
        {
            if (localScale.x > 0.0f)
            {
                localScale.x = localScale.x * -1.0f;
                transform.localScale = localScale;
            }
        }
        else
        {
            if (localScale.x < 0.0f)
            {
                localScale.x = localScale.x * 1.0f;
                transform.localScale = localScale;
            }
        }
    }

    void hitByBullet()
    {
        var Particle_001 = Instantiate(deathFX001);
        Particle_001.Play();

        var Particle_002 = Instantiate(deathFX002);
        Particle_002.Play();

        var Particle_003 = Instantiate(deathFX003);
        Particle_003.Play();

        Vector3 enemyPos = transform.position;
        Vector3 particlePosition = new Vector3(enemyPos.x, enemyPos.y, enemyPos.z + 1.0f);
        Particle_001.transform.position = particlePosition;
        Particle_002.transform.position = particlePosition;
        Particle_003.transform.position = particlePosition;

        Destroy(gameObject, 0.1f);
    }
}
