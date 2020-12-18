using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviorTreeV5 : MonoBehaviour {

	[SerializeField]
	public static Rigidbody2D collection;
	private BoxCollider2D box;
	private bool start = true;
    public List<GameObject> enemyColumns;
    public List<GameObject> activeEnemyColumns;
    public string playerLocation;

	public bool[] dec;
	public bool[] rows;
	public static float moveSpeed;
    public float timeElapsed = 0;
    public float pauseTime = 0;
	private float multiplier = 1f;
    public LayerMask player;
    public bool advanced = false;

    public ActionNode leftPlayerCheckNode;
    public ActionNode setLeftColumnsActiveNode;
    public ActionNode middlePlayerCheckNode;
    public ActionNode setMiddleColumnsActiveNode;
    public ActionNode rightPlayerCheckNode;
    public ActionNode setRightColumnsActiveNode;
    public ActionNode setDefaultColumnsActiveNode;
    public ActionNode fireFromActiveColumnsNode;
    public ActionNode checkPlayerAliveNode;

    public ActionNode playerLifeCheckNode;
    public ActionNode alienCountCheckNode;
    public ActionNode increaseSpeedNode;
    public ActionNode pauseCheckNode;
    public ActionNode pauseAliensNode;
    public ActionNode columnCheckNode;
    public ActionNode advanceNode;


    public Sequence attackSequence;
    public Selector locationSelector;
    public Sequence playerLeftSequence;
    public Sequence playerMiddleSequence;
    public Sequence playerRightSequence;
    public Sequence speedUpSequence;
    public Sequence pauseSequence;
    public Sequence advanceSequence;

	void Start () {
		box = GetComponent<BoxCollider2D> ();
		collection = GetComponent<Rigidbody2D> ();

        leftPlayerCheckNode = new ActionNode(LeftCheck);
        setLeftColumnsActiveNode = new ActionNode(SetLeft);
        middlePlayerCheckNode = new ActionNode(MiddleCheck); 
        setMiddleColumnsActiveNode = new ActionNode(SetMiddle);
        rightPlayerCheckNode = new ActionNode(RightCheck); 
        setRightColumnsActiveNode = new ActionNode(SetRight);

        setDefaultColumnsActiveNode = new ActionNode(SetDefault);
        fireFromActiveColumnsNode = new ActionNode(Fire);
        checkPlayerAliveNode = new ActionNode(CheckPlayer);

        playerLifeCheckNode = new ActionNode(LifeCheck);
        alienCountCheckNode = new ActionNode(AlienCheck);
        increaseSpeedNode = new ActionNode(IncreaseCheck);
        pauseCheckNode = new ActionNode(PauseCheck);
        pauseAliensNode = new ActionNode(PauseAliens);
        columnCheckNode = new ActionNode(ColumnCheck);
        advanceNode = new ActionNode(Advance);

                playerLeftSequence = new Sequence(new List<Node> {
                    leftPlayerCheckNode,
                    setLeftColumnsActiveNode,
                });

                playerMiddleSequence = new Sequence(new List<Node> {
                    middlePlayerCheckNode,
                    setMiddleColumnsActiveNode,
                });

                playerRightSequence = new Sequence(new List<Node> {
                    rightPlayerCheckNode,
                    setRightColumnsActiveNode,
                }); 

            locationSelector = new Selector(new List<Node> {
                playerLeftSequence,
                playerMiddleSequence,
                playerRightSequence,
                setDefaultColumnsActiveNode,
            }); 

        attackSequence = new Sequence(new List<Node> {
            locationSelector,
            checkPlayerAliveNode,
            fireFromActiveColumnsNode,
        });

        speedUpSequence = new Sequence(new List<Node> {
            playerLifeCheckNode,
            alienCountCheckNode,
            increaseSpeedNode,
        });

        pauseSequence = new Sequence(new List<Node> {
            pauseCheckNode,
            pauseAliensNode,
        });    

        advanceSequence = new Sequence(new List<Node> {
            columnCheckNode,
            advanceNode,
        });   

        var columnArray = GameObject.FindGameObjectsWithTag("EnemyColumn");
        enemyColumns = new List<GameObject>(columnArray);

		dec = new bool[10];
		for (int i = 0; i < 10; i++)
			dec [i] = false;

		rows = new bool[4];
		for (int i = 0; i < 4; i++)
			rows [i] = false;

	}

	void Update () {
		moveSpeed = multiplier * (Mathf.Pow ((Mathf.Sqrt (56 - EnemyCounter.count) / (Mathf.Sqrt (Mathf.Pow (56, 2) - Mathf.Pow (EnemyCounter.count, 2)))) * 10, 3) - 0.25f);
        //Debug.Log(moveSpeed);
        timeElapsed += Time.deltaTime;

		if (!CounterScript.counter) {
			if (start) {
				moveRight ();
				start = false;
			}
            if (LifeManager.gameOver) {
                collection.velocity = Vector2.zero;
            }
            attackSequence.Evaluate();
            speedUpSequence.Evaluate();
            pauseSequence.Evaluate();
            advanceSequence.Evaluate();
            
            updateHitBox();	
		} 
        else {
			collection.velocity = Vector2.zero;
        }
	}

    private NodeStates LeftCheck() {
        Vector3 boxCastOffset = new Vector3(transform.position.x - 6.5f, transform.position.y - 5.5f, transform.position.z);     
        Vector3 boxCastSize = new Vector3(6.5f, 3f, 1f);                
        RaycastHit2D hit = Physics2D.BoxCast(boxCastOffset, boxCastSize, 0f, transform.TransformDirection(Vector2.up), 25f, player);
        if(hit.collider != null) {
            playerLocation = "left";
            return NodeStates.SUCCESS;
        }
        else {
            return NodeStates.FAILURE;
        }
    }

    private NodeStates SetLeft() {
        activeEnemyColumns.Clear();
        for(int i = 0; i < enemyColumns.Count/3; i++) {
            activeEnemyColumns.Insert(i, enemyColumns[i]);
        }
        return NodeStates.SUCCESS;
    }

    private NodeStates MiddleCheck() {
        Vector3 boxCastOffset = new Vector3(transform.position.x, transform.position.y - 5.5f, transform.position.z);     
        Vector3 boxCastSize = new Vector3(6.5f, 3f, 1f);                
        RaycastHit2D hit = Physics2D.BoxCast(boxCastOffset, boxCastSize, 0f, transform.TransformDirection(Vector2.up), 25f, player);
        if(hit.collider != null) {
            playerLocation = "middle";
            return NodeStates.SUCCESS;
        }
        else {
            return NodeStates.FAILURE;
        }
    }

    private NodeStates SetMiddle() {
        activeEnemyColumns.Clear();
        float startingIndex = enemyColumns.Count / 3;
        int count = (int)startingIndex;
        int count2 = 0;
        for(int i = (int)startingIndex; i < enemyColumns.Count * 2 / 3; i++) {
            activeEnemyColumns.Insert(count2, enemyColumns[i]);
            count++;
            count2++;
        }
        return NodeStates.SUCCESS;
    }

    private NodeStates RightCheck() {
        Vector3 boxCastOffset = new Vector3(transform.position.x + 6.5f, transform.position.y - 5.5f, transform.position.z);     
        Vector3 boxCastSize = new Vector3(6.5f, 3f, 1f);                
        RaycastHit2D hit = Physics2D.BoxCast(boxCastOffset, boxCastSize, 0f, transform.TransformDirection(Vector2.up), 25f, player);
        if(hit.collider != null) {
            playerLocation = "right";
            return NodeStates.SUCCESS;
        }
        else {
            return NodeStates.FAILURE;
        }
    }

    private NodeStates SetRight() {
        activeEnemyColumns.Clear();
        float startingIndex = enemyColumns.Count * 2 / 3;
        int count = (int)startingIndex;
        int count2 = 0;
        for(int i = (int)startingIndex; i < enemyColumns.Count; i++) {
            activeEnemyColumns.Insert(count2, enemyColumns[count]);
            count++;
            count2++;
        }
        return NodeStates.SUCCESS;
    }

    private NodeStates SetDefault() {
        activeEnemyColumns.Clear();
        for(int i = 0; i < enemyColumns.Count; i++) {
            activeEnemyColumns.Insert(i, enemyColumns[i]);
        }
        return NodeStates.SUCCESS;
    }


    private NodeStates CheckPlayer() {
        if(LifeManager.gameOver) {
            return NodeStates.FAILURE;
        }
        else {
            return NodeStates.SUCCESS;
        }
    }
    private NodeStates Fire() {
        Shuffle(activeEnemyColumns);
        foreach(GameObject col in activeEnemyColumns) {
            if(timeElapsed > 1 && col != null) {
                timeElapsed = 0;
                col.transform.GetChild(0).GetComponent<EnemyScript>().Fire();
            }
        }

        return NodeStates.SUCCESS;
    }

    private NodeStates LifeCheck() {
        if(LifeManager.lives > 1) {
            return NodeStates.SUCCESS;
        }
        else{
            return NodeStates.FAILURE;
        }
    }

    private NodeStates AlienCheck() {
        if(EnemyCounter.count < 28) {
            return NodeStates.SUCCESS;
        }
        else{
            return NodeStates.FAILURE;
        }
    }    
    
    private NodeStates IncreaseCheck() {
        if(moveSpeed < 1.2) {
            multiplier = 1.5f;
            //Debug.Log("SPEED UP");
            return NodeStates.SUCCESS;
        }
        else{
            return NodeStates.FAILURE;
        }
    } 

    private NodeStates PauseCheck() {
        var rand = Random.Range(0, 1500);
        if(rand == 69) {
            //Debug.Log("pausing");
            return NodeStates.SUCCESS;
        }
        else{
            return NodeStates.FAILURE;
        }
    }    
    
    private NodeStates PauseAliens() {
        if(collection.velocity.x != 0) {
            collection.velocity = new Vector2(0, 0);
            StartCoroutine("PauseTimer");
            return NodeStates.SUCCESS;
        }
        else{
            collection.velocity = new Vector2(moveSpeed, 0);
            return NodeStates.FAILURE;
        }
    } 

    IEnumerator PauseTimer() {
        yield return new WaitForSeconds(1);
        collection.velocity = new Vector2(moveSpeed, 0);
    }

    private NodeStates ColumnCheck() {
        if(transform.childCount < 9 && !advanced)
        {
            advanced = true;
            return NodeStates.SUCCESS;
        }
        else {
            return NodeStates.FAILURE;
        }
    }

    private NodeStates Advance() {
        moveDown();
        moveDown();

        return NodeStates.SUCCESS;
    }
    void OnDrawGizmosSelected()
    {
        // Draw a semitransparent blue cube at the transforms position
        Vector3 boxCastOffset1 = new Vector3(transform.position.x - 6.5F, transform.position.y - 5.5f, transform.position.z);
        Vector3 boxCastOffset2 = new Vector3(transform.position.x, transform.position.y - 5.5f, transform.position.z);
        Vector3 boxCastOffset3 = new Vector3(transform.position.x + 6.5F, transform.position.y - 5.5f, transform.position.z);

        Vector3 boxCastSize1 = new Vector3(6.5f, 3f, 1f);                
        Vector3 boxCastSize2 = new Vector3(6.5f, 3f, 1f);                     
        Vector3 boxCastSize3 = new Vector3(6.5f, 3f, 1f);                
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawCube(boxCastOffset1, boxCastSize1);

        Gizmos.color = new Color(0, 1, 0, 0.5f);
        Gizmos.DrawCube(boxCastOffset2, boxCastSize2);

        Gizmos.color = new Color(0, 0, 1, 0.5f);
        Gizmos.DrawCube(boxCastOffset3, boxCastSize3);

    }

    public static void Shuffle(List<GameObject> ts)
    {
        var count = ts.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            var r = UnityEngine.Random.Range(i, count);
            var tmp = ts[i];
            ts[i] = ts[r];
            ts[r] = tmp;
        }
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

	void raiseBoxOffset (int index) {
		rows [index] = true;
		box.offset = new Vector2(box.offset.x, box.offset.y + 0.55f);
		box.size = new Vector2(box.offset.x, box.size.y - 0.55f);
	}

	void OnTriggerEnter2D (Collider2D col) {
		if (col.gameObject.CompareTag("SideCollider")) {
			if (collection.velocity.x > 0) {
				moveLeft ();
			} else {
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

    void updateHitBox() {
        if (!GameObject.Find ("Alien1")) {
            if (!rows [0])
                raiseBoxOffset(0);
            if (!GameObject.Find ("Alien2")) {
                if (!rows [1])
                    raiseBoxOffset(1);
                if (!GameObject.Find ("Alien3")) {
                    if (!rows [2])
                        raiseBoxOffset(2);
                    if (!GameObject.Find ("Alien4")) {
                        if (!rows [3])
                            raiseBoxOffset(3);
                    }
                }
            }
        }

        if (!transform.Find ("EnemyColumn1")) {
            if (!dec [9])
                incrementBoxOffset (9);
            if (!transform.Find ("EnemyColumn2")) {
                if (!dec [8])
                    incrementBoxOffset (8);
                if (!transform.Find ("EnemyColumn3")) {
                    if (!dec [7])
                        incrementBoxOffset (7);
                    if (!transform.Find ("EnemyColumn4")) {
                        if (!dec [6])
                            incrementBoxOffset (6);
                        if (!transform.Find ("EnemyColumn5")) {
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

    }
}
