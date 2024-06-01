using UnityEngine;

public class Dropper : MonoBehaviour {
    /* -------------------------------- Variables ------------------------------- */
    [Header("Dropper")]
    public GameObject dropperBody;
    public GameObject dropperLine;
    public float maxLeftX = -3f;
    public float maxRightX = 3f;
    public float moveSpeed = 5f; // Speed at which the dropper moves
    public float fruitZ = -1f;
    
    [Header("Dropped Fruit")]
    public GameObject fruitsContainer;
    public GameObject dropFruit;
    public bool dropFruitCollided = true;

    [Header("Particles")]
    public ParticleSystem particleFallingFruit;

    [Header("Keybinds")]
    public KeyCode[] leftKey = { KeyCode.LeftArrow,  KeyCode.A };
    public KeyCode[] rightKey = { KeyCode.RightArrow, KeyCode.D };
    public KeyCode[] dropKey = { KeyCode.Space,KeyCode.DownArrow, KeyCode.S };

    private GameManager gameManager;


    /* ------------------------------- Unity Func ------------------------------- */
    // Start is called before the first frame update
    void Start() {
        // Get game manager
        gameManager = FindObjectOfType<GameManager>();
        
        // Place first drop fruit (0.5s after game start, for queue to generate)
        Invoke("PlaceDropFruit", 0.1f);
    }

    // Update is called once per frame
    void Update() {
        // Functions that contently run
        PlayerImputMoveDropper();
        PlayerImputDrop();
        HideLine();

        // Check for drop fruit colliding
        if (CheckDroppedFruitCollision()) {
            dropFruit = null; // Reset dropFruit
            PlaceDropFruit(); // Place new fruit to drop
            gameManager.UpdateNextFruitDisplay(); // Update next fruit display
            gameManager.readyToDrop = true;
        }
    }

    
    /* -------------------------------- Functions ------------------------------- */
    // Function that deals with player imputs for dropper movement
    void PlayerImputMoveDropper() {
        // Stop taking imput if game lost or paused
        if (gameManager.gameLost || PauseMenu.GAME_PAUSED) return;
        
        // Get droper position
        Vector3 position = transform.position;

        // Check for left key presses
        foreach (KeyCode key in leftKey) {
            if (Input.GetKey(key)) {
                position.x -= moveSpeed * Time.deltaTime;
                break; // Move left if any left key is pressed
            }
        }

        // Check for right key presses
        foreach (KeyCode key in rightKey) {
            if (Input.GetKey(key)) {
                position.x += moveSpeed * Time.deltaTime;
                break; // Move right if any right key is pressed
            }
        }

        // Clamp the position to the specified boundaries
        position.x = Mathf.Clamp(position.x, maxLeftX, maxRightX);

        // Apply the new position to the dropper object itself
        transform.position = position;
    }

    // Function that handles player imput for dropping
    void PlayerImputDrop() {
        // Stop taking imput if game lost or paused
        if (gameManager.gameLost || PauseMenu.GAME_PAUSED) return;

        // Check for drop key presses
        foreach (KeyCode key in dropKey) {
            if (Input.GetKeyDown(key)) {
                if (!gameManager.readyToDrop) break; // End if not ready to drop
                DropFruit();
            }
        }
    }

    // Place the drop fruit on the droper
    void PlaceDropFruit() {
        // Take the first fruit in the queue
        GameObject firstFruitInQueue = gameManager.fruitsQueue[0];

        // Instantiate the fruit as a child of dropperBody
        Vector3 spawnPosition = new Vector3(dropperBody.transform.position.x, dropperBody.transform.position.y, fruitZ);
        dropFruit = Instantiate(firstFruitInQueue, spawnPosition, Quaternion.identity);
        dropFruit.transform.SetParent(dropperBody.transform);

        // Get the Rigidbody2D component and turn off simulation
        dropFruit.GetComponent<Rigidbody2D>().simulated = false;
    }

    // Function that drops the fruit in line
    void DropFruit() {
        // End if fruit queue is somehow empty
        if (gameManager.fruitsQueue.Count <= 0) {
            Debug.LogError("Can't drop next fruit because fruit queue is empty!");
            return;   
        }

        // Reset dropFruitCollided
        dropFruitCollided = false;

        // Drop fruit (Change parrent and enable simulated on rb2d)
        dropFruit.transform.SetParent(fruitsContainer.transform);
        dropFruit.GetComponent<Rigidbody2D>().simulated = true;

        // Switch ready to drop to false
        gameManager.readyToDrop = false;

        // Add a new fruit to the queue and a new one
        gameManager.fruitsQueue.RemoveAt(0);
        gameManager.AddRandomFruitToQueue();
    }

    // Function that checks if the dropped fruit collided
    bool CheckDroppedFruitCollision() {
        if (dropFruit == null || dropFruitCollided) return false; // End if dropFruit null or already collided

        // Get fruit collider
        Collider2D fruitCollider = dropFruit.GetComponent<Collider2D>();
        if (fruitCollider == null) return false; // End if no collider found

        // Check for collisions
        Collider2D[] colliders = Physics2D.OverlapBoxAll(fruitCollider.bounds.center, fruitCollider.bounds.size, 0f);
        foreach (Collider2D collider in colliders) {
            if (collider.gameObject == dropFruit) continue;

            // Check for specific tags
            if (collider.CompareTag("Floor") || collider.CompareTag("Fruit")) {
                // Fruit collided with floor or another fruit
                dropFruitCollided = true;
                return true;
            }
        }
        return false;
    }

    // Function that hides the dropper line if not ready to drop
    void HideLine() {
        if (gameManager.readyToDrop) dropperLine.GetComponent<SpriteRenderer>().enabled = true;
        else dropperLine.GetComponent<SpriteRenderer>().enabled = false;
    }
}
