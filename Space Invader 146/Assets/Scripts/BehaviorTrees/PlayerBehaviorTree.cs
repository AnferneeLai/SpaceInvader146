using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerBehaviorTree : MonoBehaviour {
    public float timeElapsed = 0;
	public float moveSpeed = 3f;
    const float speed = 3f;

    public bool validTarget = false;
    public bool validPosition = false;
	private Rigidbody2D laser;
	public Rigidbody2D bullet;
    public Rigidbody2D m_rigidbody2D;

    public GameObject bonus;
    public LayerMask ignoredLayers;
    public LayerMask enemyLayer;
    public LayerMask enemyBullets;

    public ActionNode targetCheckNode;
    public ActionNode bonusCheckNode;
    public ActionNode bunkerCheckNode;
    public ActionNode bulletCheckNode;

    public ActionNode projectileCheckNode;
    public ActionNode movementNode;
    public Sequence attackSequence;
    public Selector targetSelector;
    public Selector moveSelector;

    
	void Start () {      
        m_rigidbody2D = GetComponent<Rigidbody2D>();
        bonusCheckNode = new ActionNode(BonusCheck);
        targetCheckNode = new ActionNode(TargetCheck);  
        bunkerCheckNode = new ActionNode(BunkerCheck);
        bulletCheckNode = new ActionNode(BulletCheck);

        projectileCheckNode = new ActionNode(ProjectileCheck);
        movementNode = new ActionNode(Movement);

        targetSelector = new Selector(new List<Node> {
            bonusCheckNode,
            targetCheckNode,
        });

        attackSequence = new Sequence(new List<Node> {
            bulletCheckNode,
            bunkerCheckNode,
            targetSelector,
        });

        moveSelector = new Selector(new List<Node> {
            projectileCheckNode,
            movementNode,
        });

    }

    public void Update() {
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector2.up) * 9f, Color.red);
        // Vector3 raycastOffset = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
        // RaycastHit2D hit = Physics2D.Raycast(raycastOffset, transform.TransformDirection(Vector2.up), 9f, ~ignoredLayers);
        // if(hit.collider != null) {
        //     Debug.Log(hit.collider.name);
        // }

    	if (!CounterScript.counter && !EnemyCounter.gameWin) {
            EvaluateMovement();
            EvaluateAttack();

            timeElapsed += Time.deltaTime;
            // if(timeElapsed > 1) {
            //     timeElapsed = 0;
            //     EvaluateAttack();
            // }


		} 

		else {
			m_rigidbody2D.velocity = Vector2.zero;
		}    

        
    }
	
	public void EvaluateAttack() {
        attackSequence.Evaluate();
        StartCoroutine(ExecuteAttack());
    }

    private IEnumerator ExecuteAttack() {
        yield return new WaitForSeconds(0f);

        if(attackSequence.nodeState == NodeStates.SUCCESS) {
        	float x = transform.position.x;
			float y = transform.position.y + 0.35f;
         	laser = Instantiate (bullet, new Vector2 (x, y), Quaternion.identity);
        }     

    }

    public void EvaluateMovement() {
        moveSelector.Evaluate();
        StartCoroutine(ExecuteMovement());
    }

    private IEnumerator ExecuteMovement() {
        yield return new WaitForSeconds(0f);

        if(projectileCheckNode.nodeState == NodeStates.SUCCESS) {
        	// float random = Random.Range(0, 1);
            // if(random == 0) {m_rigidbody2D.AddForce(new Vector2(-6f, 0));}
            // if(random == 1) {m_rigidbody2D.AddForce(new Vector2(6f, 0));}
            if(moveSpeed < 0) {moveSpeed = speed;}
            else if(moveSpeed > 0) {moveSpeed = -speed;}
 
        }    
        else if(movementNode.nodeState == NodeStates.SUCCESS) {
            m_rigidbody2D.velocity = new Vector2 (moveSpeed, 0);
        } 

    }

    private NodeStates BulletCheck() {
        if(GameObject.FindGameObjectWithTag("PlayerBullet")) {
            return NodeStates.FAILURE;
        } else {
            return NodeStates.SUCCESS;
        }
    }

    private NodeStates BonusCheck() {
        if(bonus.GetComponent<SpriteRenderer>().isVisible) {
            return NodeStates.SUCCESS;
        } else {
            return NodeStates.FAILURE;
        }
    }

    private NodeStates TargetCheck() {
        Vector3 raycastOffset = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);        
        RaycastHit2D hit = Physics2D.Raycast(raycastOffset, transform.TransformDirection(Vector2.up), 35f, enemyLayer);
        if(hit.collider != null) {
            validTarget = true;
            Debug.Log("under enemy");
        }
        else {
            validTarget = false;
        }
        if(validTarget) {
            return NodeStates.SUCCESS;
        } else {
            return NodeStates.FAILURE;
        }
    }

    private NodeStates BunkerCheck() {
        Vector3 raycastOffset = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);        
        RaycastHit2D hit = Physics2D.Raycast(raycastOffset, transform.TransformDirection(Vector2.up), 15f, ~ignoredLayers);
        if(hit.collider != null) {
            if(!hit.collider.CompareTag("Bunker")) {
                Debug.Log("under bunker");
                return NodeStates.SUCCESS;
            }
            return NodeStates.RUNNING;
        }
        else {
            return NodeStates.FAILURE;
        }
    }
    
    private NodeStates ProjectileCheck() {
        Vector3 raycastOffset = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);        
        RaycastHit2D hit = Physics2D.Raycast(raycastOffset, transform.TransformDirection(Vector2.up), 15f, enemyBullets);
        if(hit.collider != null) {
            Debug.Log("dodging");
            return NodeStates.SUCCESS;
        }
        else {
            return NodeStates.FAILURE;
        }
    }

    private NodeStates Movement() {
        if(transform.position.x < -9.5f) {moveSpeed = speed;}
        else if(transform.position.x > 9.5f) {moveSpeed = -speed;}
        return NodeStates.SUCCESS;
    }

}