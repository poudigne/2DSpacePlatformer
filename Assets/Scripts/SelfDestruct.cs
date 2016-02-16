using UnityEngine;
using System.Collections;

public class SelfDestruct : MonoBehaviour {
    public float fuseLength = 1.0f;
    private float destructTime = 0.0f;

    // Use this for initialization
    void Start () {
        destructTime = Time.time + fuseLength;
	}
	
	// Update is called once per frame
	void Update () {
        if (destructTime < Time.time)
            Destroy(gameObject);
	}
}
