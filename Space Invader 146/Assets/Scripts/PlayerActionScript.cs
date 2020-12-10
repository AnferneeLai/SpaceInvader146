using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActionScript : MonoBehaviour {
	
	private Rigidbody2D laser;
	private Rigidbody2D player;

	public int fireBullet = 100;
	public float moveSpeed = 6f;
	public Rigidbody2D bullet;
    public BulletWatch BulletWatch;

	void Start () {
		player = GetComponent<Rigidbody2D> ();
	}

	void Update () {
		if (!CounterScript.counter && !EnemyCounter.gameWin) {
			if (transform.position.x > -9.925f && Input.GetKey (KeyCode.A)) {
				player.velocity = new Vector2 (-moveSpeed, 0);
			} 

			else if (transform.position.x < 9.925f && Input.GetKey (KeyCode.D)) {
				player.velocity = new Vector2 (moveSpeed, 0);
			}	

			else
				player.velocity = Vector2.zero;

			if (fireBullet > 0 && Input.GetKeyDown (KeyCode.Space)) {
				fireBullet--;
				float x = transform.position.x;
				float y = transform.position.y + 0.35f;
				laser = Instantiate (bullet, new Vector2 (x, y), Quaternion.identity);
			}

			if (laser == null){
				fireBullet = 1;
			}
		} 

		else {
			player.velocity = Vector2.zero;
		}
	}

	void OnTriggerEnter2D (Collider2D col) {
		if (col.gameObject.tag == "EnemyBullet"){
			Destroy (col.gameObject);
            BulletWatch.EnemyBulletCount -= 1;
            LifeManager.lives--;

		}
	}

}
