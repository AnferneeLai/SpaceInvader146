using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerBehaviorTreeV4 : MonoBehaviour {
    public float timeElapsed = 0;
	public float moveSpeed = 3f;
    const float multiplier = 1f;
    const float speed = 3f * multiplier;

	private Rigidbody2D laser;
	public Rigidbody2D bullet;
    public Rigidbody2D m_rigidbody2D;
    public List<GameObject> bunkers;
    public Vector3 closestBunker;
    
    public bool moving = false;
    public bool underBunker = false;
    public LayerMask enemyBullets;

    public ActionNode bunkerMoveNode;
    public ActionNode bunkerFindNode;

    public ActionNode enemyCheckNode;
    public ActionNode bulletCheckNode;
    public ActionNode fireNode;
    public ActionNode bunkerProjectileCheckNode;

    public ActionNode projectileCheckNode;
    public ActionNode positionCheckNode;
    public ActionNode movementNode;

    public Sequence attackSequence;
    public Sequence bunkerDodgeSequence;
    public Sequence bunkerMoveSequence;
    public Sequence positionSequence;
    public Sequence fireSequence;
    public Selector defaultMoveSelector;
    public Selector moveSelector;
    
    
	void Start () {      
        m_rigidbody2D = GetComponent<Rigidbody2D>();

        positionCheckNode = new ActionNode(PositionCheck);
        enemyCheckNode = new ActionNode(EnemyCheck);
        bulletCheckNode = new ActionNode(BulletCheck);
        fireNode = new ActionNode(Fire);

        bunkerProjectileCheckNode = new ActionNode(BunkerProjectileCheck);

        bunkerFindNode = new ActionNode(BunkerFind); 
        bunkerMoveNode = new ActionNode(BunkerMove);  

        projectileCheckNode = new ActionNode(ProjectileCheck);
        movementNode = new ActionNode(Movement);

            positionSequence = new Sequence(new List<Node> {
                bunkerFindNode,
                positionCheckNode,
            });

            fireSequence = new Sequence(new List<Node> {
                enemyCheckNode,
                bulletCheckNode,
                fireNode,
            });

        attackSequence = new Sequence(new List<Node> {
            //positionSequence,
            fireSequence,
        });    

            bunkerDodgeSequence = new Sequence(new List<Node> {
                bunkerProjectileCheckNode,
                bunkerMoveNode,
            });  

            bunkerMoveSequence = new Sequence(new List<Node> {
                bunkerFindNode,
                bunkerMoveNode,
            });        

            defaultMoveSelector = new Selector(new List<Node> {
                bunkerDodgeSequence,
                movementNode,
            });

        moveSelector = new Selector(new List<Node> {
            bunkerDodgeSequence,
            bunkerMoveSequence,
            defaultMoveSelector,
        });

        bunkers = new List<GameObject>(GameObject.FindGameObjectsWithTag("CompleteBunker"));
    }

    public void Update() {
    	if (!CounterScript.counter && !EnemyCounter.gameWin) {
            if(!moving){
                moveSelector.Evaluate();
            }
            attackSequence.Evaluate();

            timeElapsed += Time.deltaTime;
		} 
		else {
			m_rigidbody2D.velocity = Vector2.zero;
		}            
    }

    public int CompareDistance(GameObject x, GameObject y){
        if(Vector2.Distance(x.transform.position, transform.position) < Vector2.Distance(y.transform.position, transform.position)) {
            return -1;
        }
        else if (Vector2.Distance(x.transform.position, transform.position) > Vector2.Distance(y.transform.position, transform.position)) {
            return 1;
        }
        else {
            return 0;
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

    private NodeStates PositionCheck() {
        if(Mathf.Abs(transform.position.x - closestBunker.x) < 0.03){
            return NodeStates.SUCCESS;
        }
        else {
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
    
    
    private NodeStates BunkerFind() {
        bunkers.Sort(CompareDistance);
        closestBunker = new Vector3(bunkers[0].transform.position.x + 0.64f, bunkers[0].transform.position.y, bunkers[0].transform.position.z);

        if(closestBunker != null) {
            return NodeStates.SUCCESS;
        }
        else {
            return NodeStates.FAILURE;
        }
    }

    private NodeStates BunkerProjectileCheck() {
        Vector3 boxCastOffset = new Vector3(transform.position.x, transform.position.y + 5.5f, transform.position.z);     
        Vector3 boxCastSize = new Vector3(0.5f, 20f, 1f);                
        RaycastHit2D hit = Physics2D.BoxCast(boxCastOffset, boxCastSize, 0f, transform.TransformDirection(Vector2.up), 15f, enemyBullets);
        if(hit.collider != null && Mathf.Abs(transform.position.x - closestBunker.x) < 0.03) {
            closestBunker = new Vector3(bunkers[1].transform.position.x + 0.64f, bunkers[1].transform.position.y, bunkers[1].transform.position.z);
            return NodeStates.SUCCESS;
        }
        else {
            return NodeStates.FAILURE;
        }
    }


    private NodeStates BunkerMove() {
        StartCoroutine("Move");
        return NodeStates.SUCCESS;

        // if(Mathf.Abs(transform.position.x - closestBunker.x) > 0.03) {
        //     if(transform.position.x < closestBunker.x) {
        //         moveSpeed = speed;
        //     }
        //     else if(transform.position.x > closestBunker.x) {
        //         moveSpeed = -speed;
        //     }

        // }
        // else {
        //     moveSpeed = 0;
        //     moving = false;
        // }
        // m_rigidbody2D.velocity = new Vector2 (moveSpeed, 0);

        // return NodeStates.SUCCESS;
    }

    IEnumerator Move() {
        moving = true;
        while(Mathf.Abs(transform.position.x - closestBunker.x) > 0.03f) {
            if(transform.position.x < closestBunker.x) {
                moveSpeed = speed;
            }
            else if(transform.position.x > closestBunker.x) {
                moveSpeed = -speed;
            }
            m_rigidbody2D.velocity = new Vector2 (moveSpeed, 0);

            yield return null;

        }
        moving = false;
    }

    private NodeStates ProjectileCheck() {
        Vector3 raycastOffset = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);        
        RaycastHit2D hit = Physics2D.Raycast(raycastOffset, transform.TransformDirection(Vector2.up), 15f, enemyBullets);
        if(hit.collider != null) {
            if(moveSpeed < 0) {moveSpeed = speed;}
            else if(moveSpeed > 0) {moveSpeed = -speed;}
            return NodeStates.SUCCESS;
        }
        else {
            return NodeStates.FAILURE;
        }
    }
    private NodeStates Movement() {
        if(transform.position.x < -9.5f) {moveSpeed = speed;}
        else if(transform.position.x > 9.5f) {moveSpeed = -speed;}
        m_rigidbody2D.velocity = new Vector2 (moveSpeed, 0);
        return NodeStates.SUCCESS;
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
        Vector3 boxCastOffset2 = new Vector3(transform.position.x, transform.position.y + 5.5f, transform.position.z);
        Vector3 boxCastOffset3 = new Vector3(transform.position.x + 0.75F, transform.position.y + 5.5f, transform.position.z);

        Vector3 boxCastSize1 = new Vector3(1.5f, 20f, 1f);                
        Vector3 boxCastSize2 = new Vector3(0.5f, 20f, 1f);                     
        Vector3 boxCastSize3 = new Vector3(1.5f, 20f, 1f);                
        // Gizmos.color = new Color(1, 0, 0, 0.5f);
        // Gizmos.DrawCube(boxCastOffset1, boxCastSize1);

        Gizmos.color = new Color(0, 1, 0, 0.5f);
        Gizmos.DrawCube(boxCastOffset2, boxCastSize2);

        // Gizmos.color = new Color(0, 0, 1, 0.5f);
        // Gizmos.DrawCube(boxCastOffset3, boxCastSize3);

    }

}