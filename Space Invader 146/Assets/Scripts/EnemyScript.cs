using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour {

	public Rigidbody2D bullet;
    public BulletWatch BulletWatch;
    public int BulletLimit = 10;

    public void Fire () {
            float x = transform.position.x;
            float y = transform.position.y - 0.4f;
            Instantiate(bullet, new Vector2(x, y), Quaternion.identity);
    }
}
