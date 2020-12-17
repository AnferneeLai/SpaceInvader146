using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerBehaviorTreeV2 : MonoBehaviour {
    public float timeElapsed = 0;
	public float moveSpeed = 3f;
    const float multiplier = 1f;
    const float speed = 3f * multiplier;

	private Rigidbody2D laser;
	public Rigidbody2D bullet;
    public Rigidbody2D m_rigidbody2D;
    public List<GameObject> enemyColumns;
    public GameObject enemyShips;
    public GameObject target;
    //public GameObject bonus;
    
    public LayerMask ignoredLayers;
    public LayerMask enemyLayer;
    public LayerMask enemyBullets;

    public ActionNode targetFindNode;
    public ActionNode targetCheckNode;

    public ActionNode bonusCheckNode;
    public ActionNode bunkerCheckNode;
    public ActionNode bulletCheckNode;
    public ActionNode projectileCheckNode;
    public ActionNode movementNode;

    public Sequence attackSequence;
    public Sequence targetSequence;

    public Selector targetSelector;
    public Selector moveSelector;

    
	void Start () {      
        m_rigidbody2D = GetComponent<Rigidbody2D>();
        //bonusCheckNode = new ActionNode(BonusCheck);
        targetFindNode = new ActionNode(TargetFind);  
        targetCheckNode = new ActionNode(TargetCheck);  
        bunkerCheckNode = new ActionNode(BunkerCheck);
        bulletCheckNode = new ActionNode(BulletCheck);

        projectileCheckNode = new ActionNode(ProjectileCheck);
        movementNode = new ActionNode(Movement);

        targetSelector = new Selector(new List<Node> {
            //bonusCheckNode,
            targetFindNode,
            targetCheckNode,
        });

        targetSequence = new Sequence(new List<Node> {
            //bonusCheckNode,
            targetFindNode,
            targetCheckNode,
        });

        attackSequence = new Sequence(new List<Node> {
            bulletCheckNode,
            bunkerCheckNode,
            targetFindNode,
            //targetSelector,
        });

        moveSelector = new Selector(new List<Node> {
            projectileCheckNode,
            movementNode,
        });

        var columnArray = GameObject.FindGameObjectsWithTag("EnemyColumn");
        enemyColumns = new List<GameObject>(columnArray);
        target = enemyColumns[0];
    }

    public void Update() {
        
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
	
    public void Fire() {
        float x = transform.position.x;
        float y = transform.position.y + 0.35f;
        laser = Instantiate (bullet, new Vector2 (x, y), Quaternion.identity);
        timeElapsed = 0;
    }
	
	public void EvaluateAttack() {
        attackSequence.Evaluate();
        StartCoroutine(ExecuteAttack());
    }

    private IEnumerator ExecuteAttack() {
        yield return new WaitForSeconds(0f);

        if(attackSequence.nodeState == NodeStates.SUCCESS) {
            Fire();
        }
        else if(targetCheckNode.nodeState == NodeStates.RUNNING && bunkerCheckNode.nodeState == NodeStates.SUCCESS && bulletCheckNode.nodeState == NodeStates.SUCCESS) {
            Fire();
        }     

    }

    public void EvaluateMovement() {
        moveSelector.Evaluate();
        StartCoroutine(ExecuteMovement());
    }

    // public int CompareChildrenCount(GameObject x, GameObject y){
    //     if(x.transform.childCount > y.transform.childCount) {
    //         return x.transform.childCount;
    //     }
    //     else if (x.transform.childCount < y.transform.childCount) {
    //         return y.transform.childCount;
    //     }
    //     else {
    //         return x.transform.childCount;
    //     }
    // }

    public int CompareChildrenCount(GameObject x, GameObject y){
        if(x.transform.childCount > y.transform.childCount) {
            return -1;
        }
        else if (x.transform.childCount < y.transform.childCount) {
            return 1;
        }
        else {
            return 0;
        }
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
        if(GameObject.FindGameObjectWithTag("PlayerBullet") && timeElapsed < 0.5f/multiplier) {
            return NodeStates.FAILURE;
        } else {
            return NodeStates.SUCCESS;
        }
    }

    // private NodeStates BonusCheck() {
    //     if(bonus.GetComponent<SpriteRenderer>().isVisible) {
    //         return NodeStates.SUCCESS;
    //     } else {
    //         return NodeStates.FAILURE;
    //     }
    // }

    private NodeStates TargetFind() {
        var columnArray = GameObject.FindGameObjectsWithTag("EnemyColumn");
        enemyColumns = new List<GameObject>(columnArray);
        enemyColumns.Sort(CompareChildrenCount);
        target = enemyColumns[0];
        //Debug.Log(target.name);

        if(target != null) {
            return NodeStates.SUCCESS;
        }
        else {
            return NodeStates.FAILURE;
        }
    }

    private NodeStates TargetCheck() {
        Vector3 raycastOffset = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);        
        RaycastHit2D hit = Physics2D.Raycast(raycastOffset, transform.TransformDirection(Vector2.up), 15f, enemyLayer);
        if(hit.collider != null) {
            Debug.Log(hit.collider.name);
            if(hit.collider.name == target.name) {
                return NodeStates.SUCCESS;
            }
            return NodeStates.RUNNING;
        }
        else {
            return NodeStates.FAILURE;
        }
    }

    private NodeStates BunkerCheck() {
        Vector3 raycastOffset = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);        
        RaycastHit2D hit = Physics2D.Raycast(raycastOffset, transform.TransformDirection(Vector2.up), 15f, ~ignoredLayers);
        if(hit.collider != null) {
            if(!hit.collider.CompareTag("Bunker")) {
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
            return NodeStates.SUCCESS;
        }
        else {
            return NodeStates.FAILURE;
        }
    }

    private NodeStates Movement() {
        if(target != null) {
            var targetPosition = target.transform.position;
            float movementOffset = Random.Range(0.0f, 1.0f);
            // moving to the left
            if(enemyShips.GetComponent<Rigidbody2D>().velocity.x < 0) {
                // if player to the left of the target
                if(transform.position.x < targetPosition.x - movementOffset) {    
                    moveSpeed = speed;
                }
                else if(transform.position.x > targetPosition.x - movementOffset) {
                    moveSpeed = -speed;
                }    
            }
            else if(enemyShips.GetComponent<Rigidbody2D>().velocity.x > 0) {
                // if player to the left of the target
                if(transform.position.x < targetPosition.x + movementOffset) {    
                    moveSpeed = speed;
                }
                else if(transform.position.x > targetPosition.x + movementOffset) {
                    moveSpeed = -speed;
                }    
            }
        }
        
        
        return NodeStates.SUCCESS;
    }

    void OnTriggerEnter2D (Collider2D col) {
        if (col.gameObject.tag == "EnemyBullet"){
            //Destroy (col.gameObject);
            LifeManager.lives--;
        }
    }

}