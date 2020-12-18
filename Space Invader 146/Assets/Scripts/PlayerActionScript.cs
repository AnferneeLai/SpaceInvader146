using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActionScript : MonoBehaviour {
	
	private Rigidbody2D laser;
	private Rigidbody2D player;

    public Transform playerTransform;
	public int fireBullet = 100;
	public float moveSpeed = 6f;
	public Rigidbody2D bullet;
    public SpriteRenderer playerRenderer;
    public float hitTimer;

    void Start () {
		player = GetComponent<Rigidbody2D> ();
        hitTimer = 1;
	}

	void Update () {
        if(Time.timeScale == 0 && hitTimer > 0)
        {
            hitTimer -= Time.unscaledDeltaTime;
        }
        if (hitTimer <= 0)
        {

            hitTimer = 1;
            playerTransform.position = new Vector3(-8, transform.position.y, transform.position.z);
            Time.timeScale = 1;
        }

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
            Destroy(col.gameObject);
            LifeManager.lives--;
            if (LifeManager.lives != 0)
            {
                Hit();
            }

        }
	}

    public void Hit()
    {
        playerRenderer.material.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        StartCoroutine(HitFlicker());

    }
    private IEnumerator HitFlicker()
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(0.2f);
        playerRenderer.material.color = new Color(1.0f, 1.0f, 1.0f, 1f);
        StartCoroutine(HitFlicker1());



    }
    private IEnumerator HitFlicker1()
    {
        yield return new WaitForSecondsRealtime(0.2f);
        playerRenderer.material.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        StartCoroutine(HitFlicker2());

    }
    private IEnumerator HitFlicker2()
    {
        yield return new WaitForSecondsRealtime(0.2f);
        playerRenderer.material.color = new Color(1.0f, 1.0f, 1.0f, 1f);


    }

    private IEnumerator HitFlicker3()
    {
        yield return new WaitForSecondsRealtime(0.2f);
        playerRenderer.material.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        StartCoroutine(HitFlicker4());

    }
    private IEnumerator HitFlicker4()
    {
        yield return new WaitForSecondsRealtime(0.2f);
        playerRenderer.material.color = new Color(1.0f, 1.0f, 1.0f, 1f);

        

    }

}
