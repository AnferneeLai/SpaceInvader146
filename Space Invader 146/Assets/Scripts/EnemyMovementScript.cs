using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovementScript : MonoBehaviour {

	[SerializeField]
	public static Rigidbody2D collection;
	private BoxCollider2D box;
	private bool right = true;
	private bool start = true;

	public bool[] dec;
	public static float moveSpeed;
    public static double moveSpeedDeathMod;
    public static double moveSpeedColumnMod;
    public int enemy1;
    public int enemy2;
    public int enemy3;
    public int enemy4;
    public int enemy5;
    public int enemyCount;
    public Vector2 tempVelocity;
    public double bufferTimer;

    void Start () {
        bufferTimer = 0;
        moveSpeedDeathMod = 1;
        moveSpeedColumnMod = 0;
        box = GetComponent<BoxCollider2D> ();
		collection = GetComponent<Rigidbody2D> ();
		moveRight ();

		dec = new bool[10];
		for (int i = 0; i < 10; i++)
			dec [i] = false;
	}

	void Update () {

        enemy1 = GameObject.FindGameObjectsWithTag("Enemy1").Length;
        enemy2 = GameObject.FindGameObjectsWithTag("Enemy2").Length;
        enemy3 = GameObject.FindGameObjectsWithTag("Enemy3").Length;
        enemy4 = GameObject.FindGameObjectsWithTag("Enemy4").Length;
        enemy5 = GameObject.FindGameObjectsWithTag("Enemy5").Length;
        enemyCount = enemy1 + enemy2 + enemy3 + enemy4 + enemy5;

        if (enemyCount > 6)
        {
            moveSpeedDeathMod = 0.5;
        }

        if (enemyCount == 6)
        {
            moveSpeedDeathMod = 1;
        }
        if (enemyCount == 5)
        {
            moveSpeedDeathMod = 1.5;
        }
        if (enemyCount == 4)
        {
            moveSpeedDeathMod = 2;
        }
        if (enemyCount == 3)
        {
            moveSpeedDeathMod = 2.5;
        }
        if (enemyCount == 2)
        {
            moveSpeedDeathMod = 3;
        }
        if (enemyCount == 1)
        {
            moveSpeedDeathMod = 5;
        }
        if (PlayerBulletScript.buffer)
        {
            tempVelocity = collection.velocity;
            collection.velocity = Vector2.zero;
            bufferTimer = 0.25;
            PlayerBulletScript.buffer = false;
        }

        if (bufferTimer > 0)
        {
            bufferTimer -= Time.unscaledDeltaTime;
        }
        if (bufferTimer < 0)
        {

            bufferTimer = 0;
            collection.velocity = tempVelocity;
        }

        moveSpeed = ((float)moveSpeedDeathMod + (float)moveSpeedColumnMod) * (Mathf.Pow((Mathf.Sqrt(56 - EnemyCounter.count) / (Mathf.Sqrt(Mathf.Pow(56, 2) - Mathf.Pow(EnemyCounter.count, 2)))) * 10, 3) - 0.25f);
        
        if (!CounterScript.counter) {
			if (start) {
				moveRight ();
				start = false;
			}

			if (LifeManager.gameOver)
				collection.velocity = Vector2.zero;

			if (!transform.Find ("EnemyColumn1")) {
				if (!dec [9])
					incrementBoxOffset (9);
				if (!transform.Find ("EnemyColumn4")) {
					if (!dec [8])
						incrementBoxOffset (8);
					if (!transform.Find ("EnemyColumn3")) {
						if (!dec [7])
							incrementBoxOffset (7);
						if (!transform.Find ("EnemyColumn2")) {
							if (!dec [6])
								incrementBoxOffset (6);
							if (!transform.Find ("EnemyColumn1")) {
								if (!dec [5])
									incrementBoxOffset (5);
							}
						}
					}
				}
			}

			if (!transform.Find ("EnemyColumn11")) {
				if (!dec [0])
					decrementBoxOffset (0);
				if (!transform.Find ("EnemyColumn10")) {
					if (!dec [1])
						decrementBoxOffset (1);
					if (!transform.Find ("EnemyColumn9")) {
						if (!dec [2])
							decrementBoxOffset (2);
						if (!transform.Find ("EnemyColumn8")) {
							if (!dec [3])
								decrementBoxOffset (3);
							if (!transform.Find ("EnemyColumn7")) {
								if (!dec [4])
									decrementBoxOffset (4);
							}
						}
					}
				}
			}
		} else
			collection.velocity = Vector2.zero;
	}

	void decrementBoxOffset (int index) {
		dec [index] = true;

		box.offset = new Vector2 (box.offset.x - 0.5f, box.offset.y);
		box.size = new Vector2 (box.size.x - 1f, box.offset.y);
	}

	void incrementBoxOffset (int index) {
		dec [index] = true;

		box.offset = new Vector2 (box.offset.x + 0.5f, box.offset.y);
		box.size = new Vector2 (box.size.x - 1f, box.offset.y);
	}

	void OnTriggerEnter2D (Collider2D col) {

		if (col.gameObject.CompareTag("SideCollider")) {
			Debug.Log("bouncing back");

            moveSpeedColumnMod += 0.05;

            if (right) {
				right = false;
				moveLeft ();
			} else {
				right = true;
				moveRight ();
			}

			moveDown ();
		} else if (col.gameObject.tag == "EndGame") {
			LifeManager.lives = 0;
			LifeManager.gameOver = true;
		}
	}

	static void moveRight () {
		collection.velocity = new Vector2 (moveSpeed, 0);
	}

	void moveLeft () {
		collection.velocity = new Vector2(-moveSpeed, 0);
	}

	void moveDown () {
		float x = transform.position.x;
		float y = transform.position.y - 0.2f;
		transform.position = new Vector2 (x, y);
	}


}
