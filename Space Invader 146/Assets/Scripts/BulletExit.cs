using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletExit : MonoBehaviour
{
    public float SelfDestructTimer;

    void Start()
    {
        SelfDestructTimer = 3;
    }

    void Update()
    {
        if (SelfDestructTimer > 0)
        {
            SelfDestructTimer -= Time.deltaTime;
        }

        if (SelfDestructTimer <= 0)
        {
            Destroy(gameObject);
        }
    }
    void OnBecameInvisible()
    {
        Destroy(gameObject);

    }
}
