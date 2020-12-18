using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour {

	public Rigidbody2D bullet;
    public BulletWatch BulletWatch;
    public int BulletLimit = 1;

	void Fire () {
            float x = transform.position.x;
            float y = transform.position.y - 0.4f;
            if (BulletWatch.EnemyBulletCount >= BulletLimit)
            {
                CancelInvoke("Fire");
            }
            else
            {
                Instantiate(bullet, new Vector2(x, y), Quaternion.identity);


                float time = Random.Range(0.1f, 0.8f);
                Invoke("ChooseWhenFire", time);
            }

        
    }
}
