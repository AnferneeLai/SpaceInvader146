using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerBehaviorTreeV3 : MonoBehaviour {
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
    public GameObject bonus;
    
    public LayerMask ignoredLayers;
    public LayerMask enemyLayer;
    public LayerMask enemyBullets;

    public ActionNode bonusCheckNode;
    public ActionNode targetCheckNode;
    public ActionNode targetFindNode;

    public ActionNode enemyCheckNode;
    public ActionNode bunkerCheckNode;
    public ActionNode bulletCheckNode;
    public ActionNode fireNode;

    public ActionNode leftProjectileCheckNode;
    public ActionNode aboveProjectileCheckNode;
    public ActionNode rightProjectileCheckNode;
    public ActionNode movementNode;

    public Sequence attackSequence;
    public Selector targetSelector;
    public Sequence targetCheckSequence;
    public Sequence fireSequence;
    public Selector projectileCheckSelector;
    public Selector moveSelector;

    
	void Start () {      
        m_rigidbody2D = GetComponent<Rigidbody2D>();

        bonusCheckNode = new ActionNode(BonusCheck);
        enemyCheckNode = new ActionNode(EnemyCheck);
        targetFindNode = new ActionNode(TargetFind); 

        targetCheckNode = new ActionNode(TargetCheck);  
        bunkerCheckNode = new ActionNode(BunkerCheck);
        bulletCheckNode = new ActionNode(BulletCheck);
        fireNode = new ActionNode(Fire);

        leftProjectileCheckNode = new ActionNode(LeftProjectileCheck);
        //aboveProjectileCheckNode = new ActionNode(AboveProjectileCheck);
        rightProjectileCheckNode = new ActionNode(RightProjectileCheck);

        movementNode = new ActionNode(Movement);

            fireSequence = new Sequence(new List<Node> {
                targetCheckNode,
                bunkerCheckNode,
                bulletCheckNode,
                fireNode,
            });

                targetCheckSequence = new Sequence(new List<Node> {
                    enemyCheckNode,
                    targetFindNode,
                }); 

            targetSelector = new Selector(new List<Node> {
                bonusCheckNode,
                targetCheckSequence,
            }); 

        attackSequence = new Sequence(new List<Node> {
            targetSelector,
            fireSequence,
        });    

            projectileCheckSelector = new Selector(new List<Node> {
                leftProjectileCheckNode,
                //aboveProjectileCheckNode,
                rightProjectileCheckNode,
            });        

        moveSelector = new Selector(new List<Node> {
            projectileCheckSelector,
            movementNode,
        });

        var columnArray = GameObject.FindGameObjectsWithTag("EnemyColumn");
        enemyColumns = new List<GameObject>(columnArray);
        target = enemyColumns[0];
    }

    public void Update() {
        //Debug.DrawRay(new Vector3(transform.position.x - 1f, transform.position.y, transform.position.z), transform.TransformDirection(Vector2.up) * 9f, Color.red);
        //Debug.DrawRay(transform.position, transform.TransformDirection(Vector2.up) * 9f, Color.red);
        //Debug.DrawRay(new Vector3(transform.position.x + 1f, transform.position.y, transform.position.z), transform.TransformDirection(Vector2.up) * 9f, Color.red);
        Vector3 boxCastOffset = new Vector3(transform.position.x, transform.position.y + 5.5f, transform.position.z);     
        Vector3 boxCastSize = new Vector3(6f, 10f, 1f); 
    	if (!CounterScript.counter && !EnemyCounter.gameWin) {
            moveSelector.Evaluate();
            attackSequence.Evaluate();

            timeElapsed += Time.deltaTime;
		} 
		else {
			m_rigidbody2D.velocity = Vector2.zero;
		}            
    }

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

    private NodeStates BonusCheck() {
        if(bonus.GetComponent<SpriteRenderer>().isVisible) {
            target = bonus;
            return NodeStates.SUCCESS;
        } else {
            return NodeStates.FAILURE;
        }
    }

    private NodeStates EnemyCheck() {
        if(GameObject.FindGameObjectWithTag("EnemyColumn") != null) {
            return NodeStates.SUCCESS;
        }
        else {
            return NodeStates.FAILURE;
        }
    }
    
    private NodeStates TargetFind() {
        var columnArray = GameObject.FindGameObjectsWithTag("EnemyColumn");
        enemyColumns = new List<GameObject>(columnArray);
        enemyColumns.Sort(CompareChildrenCount);
        target = enemyColumns[0];

        if(target != null) {
            return NodeStates.SUCCESS;
        }
        else {
            return NodeStates.FAILURE;
        }
    }

    private NodeStates TargetCheck() {
        if(target != null) {
            return NodeStates.SUCCESS;
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
        }
        return NodeStates.FAILURE;
        
    }
    
    private NodeStates BulletCheck() {
        if(GameObject.FindGameObjectWithTag("PlayerBullet") && timeElapsed < 0.5f/multiplier) {
            return NodeStates.FAILURE;
        } else {
            return NodeStates.SUCCESS;
        }
    }

    private NodeStates Fire() {
        float x = transform.position.x;
        float y = transform.position.y + 0.35f;
        laser = Instantiate (bullet, new Vector2 (x, y), Quaternion.identity);
        timeElapsed = 0;
        return NodeStates.SUCCESS;
    }
    
    private NodeStates LeftProjectileCheck() {
        Vector3 boxCastOffset = new Vector3(transform.position.x - .75f, transform.position.y + 5.5f, transform.position.z);     
        Vector3 boxCastSize = new Vector3(1.5f, 20f, 1f);                
        RaycastHit2D hit = Physics2D.BoxCast(boxCastOffset, boxCastSize, 0f, transform.TransformDirection(Vector2.up), 15f, enemyBullets);
        if(hit.collider != null) {
            if(hit.collider.transform.position.y < -1f) {
                moveSpeed = 0;
                return NodeStates.SUCCESS;
            }
            //Debug.Log("left projectile spotted");
            moveSpeed = speed;
            return NodeStates.SUCCESS;
        }
        else {
            return NodeStates.FAILURE;
        }
    }

    private NodeStates RightProjectileCheck() {
        Vector3 boxCastOffset = new Vector3(transform.position.x + .75f, transform.position.y + 5.5f, transform.position.z);     
        Vector3 boxCastSize = new Vector3(1.5f, 20f, 1f);                
        RaycastHit2D hit = Physics2D.BoxCast(boxCastOffset, boxCastSize, 0f, transform.TransformDirection(Vector2.up), 15f, enemyBullets);
        if(hit.collider != null) {
            if(hit.collider.transform.position.y < -1f) {
                moveSpeed = 0;
                return NodeStates.SUCCESS;
            }            
            //Debug.Log("Projectile spotted to right");
            moveSpeed = -speed;
            return NodeStates.SUCCESS;
        }
        else {
            return NodeStates.FAILURE;
        }
    }

    
    // private NodeStates LeftProjectileCheck() {
    //     Vector3 boxCastOffset = new Vector3(transform.position.x - .5f, transform.position.y + 5.5f, transform.position.z);     
    //     Vector3 boxCastSize = new Vector3(.5f, 20f, 1f);                
    //     RaycastHit2D hit = Physics2D.BoxCast(boxCastOffset, boxCastSize, 0f, transform.TransformDirection(Vector2.up), 15f, enemyBullets);
    //     if(hit.collider != null) {
    //         if(hit.collider.transform.position.y < -1f) {
    //             moveSpeed = 0;
    //             return NodeStates.SUCCESS;
    //         }
    //         //Debug.Log("left projectile spotted");
    //         moveSpeed = speed;
    //         return NodeStates.SUCCESS;
    //     }
    //     else {
    //         return NodeStates.FAILURE;
    //     }
    // }

    // private NodeStates AboveProjectileCheck() {
    //     //Vector3 raycastOffset = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);        
    //     //RaycastHit2D hit = Physics2D.Raycast(raycastOffset, transform.TransformDirection(Vector2.up), 15f, enemyBullets);
    //     Vector3 boxCastOffset = new Vector3(transform.position.x, transform.position.y + 5.5f, transform.position.z);     
    //     Vector3 boxCastSize = new Vector3(.5f, 20f, 1f);                
    //     RaycastHit2D hit = Physics2D.BoxCast(boxCastOffset, boxCastSize, 0f, transform.TransformDirection(Vector2.up), 15f, enemyBullets);
    //     if(hit.collider != null) {
    //         //Debug.Log("Projectile spotted above");
    //         var rand = Random.Range(0, 1);
    //         moveSpeed = rand == 0 ? speed : -speed;
    //         return NodeStates.SUCCESS;
    //     }
    //     else {
    //         return NodeStates.FAILURE;
    //     }
    // }

    // private NodeStates RightProjectileCheck() {
    //     Vector3 boxCastOffset = new Vector3(transform.position.x + .5f, transform.position.y + 5.5f, transform.position.z);     
    //     Vector3 boxCastSize = new Vector3(.5f, 20f, 1f);                
    //     RaycastHit2D hit = Physics2D.BoxCast(boxCastOffset, boxCastSize, 0f, transform.TransformDirection(Vector2.up), 15f, enemyBullets);
    //     if(hit.collider != null) {
    //         if(hit.collider.transform.position.y < -1f) {
    //             moveSpeed = 0;
    //             return NodeStates.SUCCESS;
    //         }            
    //         //Debug.Log("Projectile spotted to right");
    //         moveSpeed = -speed;
    //         return NodeStates.SUCCESS;
    //     }
    //     else {
    //         return NodeStates.FAILURE;
    //     }
    // }

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
            m_rigidbody2D.velocity = new Vector2 (moveSpeed, 0);
            return NodeStates.SUCCESS;
        }
        else{
            m_rigidbody2D.velocity = new Vector2 (0, 0);    
            return NodeStates.FAILURE;
        }
    }

    void OnTriggerEnter2D (Collider2D col) {
        if (col.gameObject.tag == "EnemyBullet"){
            //Destroy (col.gameObject);
            LifeManager.lives--;
        }
    }

    void OnDrawGizmosSelected()
    {
        // Draw a semitransparent blue cube at the transforms position
        Vector3 boxCastOffset1 = new Vector3(transform.position.x - 0.75F, transform.position.y + 5.5f, transform.position.z);
        //Vector3 boxCastOffset2 = new Vector3(transform.position.x, transform.position.y + 5.5f, transform.position.z);
        Vector3 boxCastOffset3 = new Vector3(transform.position.x + 0.75F, transform.position.y + 5.5f, transform.position.z);

        Vector3 boxCastSize1 = new Vector3(1.5f, 20f, 1f);                
        //Vector3 boxCastSize2 = new Vector3(0.5f, 20f, 1f);                     
        Vector3 boxCastSize3 = new Vector3(1.5f, 20f, 1f);                
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawCube(boxCastOffset1, boxCastSize1);

        //Gizmos.color = new Color(0, 1, 0, 0.5f);
        //Gizmos.DrawCube(boxCastOffset2, boxCastSize2);

        Gizmos.color = new Color(0, 0, 1, 0.5f);
        Gizmos.DrawCube(boxCastOffset3, boxCastSize3);

    }

}