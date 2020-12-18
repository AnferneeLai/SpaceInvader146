using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterControllerScript : MonoBehaviour {

	private Rigidbody2D master;

	public float moveSpeed = 2.0f;


	void Start () {
		master = GetComponent<Rigidbody2D> ();
	}

	void Update () {
		if (!EnemyCounter.gameWin && !LifeManager.gameOver && !CounterScript.counter) {
			master.velocity = new Vector2 (moveSpeed, 0);
		}

		if (transform.position.x >= 10.5f) {
			transform.position = new Vector2 (-25.5f, 6.0f);
		}
	}

}
