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

    /* Touch input variables */
    private Vector2 previousTouchPosition;
    private bool isTouching = false;
    private bool isSwiping = false;
    private float touchStartTime;
    private float swipeThreshold = 50f;
    private float tapThreshold = 0.2f;

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
        // Functions that constantly run
        PlayerInputMoveDropper();
        PlayerInputDrop();
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
    // Function that deals with player inputs for dropper movement
    void PlayerInputMoveDropper() {
        // Stop taking input if game lost or paused
        if (gameManager.gameLost || PauseMenu.GAME_PAUSED) return;
        
        // Get dropper position
        Vector3 position = transform.position;

        // Check for left and right key presses (for keyboard)
        foreach (KeyCode key in leftKey) {
            if (Input.GetKey(key)) {
                position.x -= moveSpeed * Time.deltaTime;
                break; // Move left if any left key is pressed
            }
        }

        foreach (KeyCode key in rightKey) {
            if (Input.GetKey(key)) {
                position.x += moveSpeed * Time.deltaTime;
                break; // Move right if any right key is pressed
            }
        }

        // Check for touch input (for mobile)
        if (Input.touchCount > 0) {
            Touch touch = Input.GetTouch(0);
            Vector2 touchPosition = touch.position;

            switch (touch.phase) {
                case TouchPhase.Began:
                    previousTouchPosition = touchPosition;
                    touchStartTime = Time.time;
                    isTouching = true;
                    isSwiping = false;
                    break;

                case TouchPhase.Moved:
                    if (isTouching) {
                        Vector2 touchDelta = touchPosition - previousTouchPosition;
                        if (Mathf.Abs(touchDelta.x) > swipeThreshold) {
                            isSwiping = true;
                        }
                        position.x += touchDelta.x * moveSpeed * Time.deltaTime * 0.1f; // Adjusting scale for smoother movement
                        previousTouchPosition = touchPosition; // Update previous touch position
                    }
                    break;

                case TouchPhase.Ended:
                    isTouching = false;
                    float touchDuration = Time.time - touchStartTime;
                    if (!isSwiping && touchDuration < tapThreshold) {
                        DropFruit(); // Consider it a tap and drop the fruit
                    }
                    break;
            }
        }

        // Clamp the position to the specified boundaries
        position.x = Mathf.Clamp(position.x, maxLeftX, maxRightX);

        // Apply the new position to the dropper object itself
        transform.position = position;
    }

    // Function that handles player input for dropping
    void PlayerInputDrop() {
        // Stop taking input if game lost or paused
        if (gameManager.gameLost || PauseMenu.GAME_PAUSED) return;

        // Check for drop key presses (for keyboard)
        foreach (KeyCode key in dropKey) {
            if (Input.GetKeyDown(key)) {
                if (!gameManager.readyToDrop) break; // End if not ready to drop
                DropFruit();
            }
        }
    }

    // Place the drop fruit on the dropper
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

        // Drop fruit (Change parent and enable simulated on rb2d)
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
