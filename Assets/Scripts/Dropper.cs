using UnityEngine;

public class Dropper : MonoBehaviour {
    /* -------------------------------- Variables ------------------------------- */
    [Header("Dropper")]
    public GameObject dropperBody;
    public GameObject dropperLine;
    public float maxLeftX = -2.5f;
    public float maxRightX = 2.5f;
    public float moveSpeed = 5f; // Speed at which the dropper moves

    [Header("Keybinds")]
    public KeyCode[] leftKey = { KeyCode.LeftArrow,  KeyCode.A };
    public KeyCode[] rightKey = { KeyCode.RightArrow, KeyCode.D };
    public KeyCode[] dropKey = { KeyCode.Space, KeyCode.S };

    private GameManager gameManager;


    /* ------------------------------- Unity Func ------------------------------- */
    // Start is called before the first frame update
    void Start() {
        // Get game manager
        gameManager = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update() {
        PlayerImputMoveDropper();
        PlayerImputDrop();
        HideLine();
    }

    
    /* -------------------------------- Functions ------------------------------- */
    // Function that deals with player imputs for dropper movement
    void PlayerImputMoveDropper() {
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
        // Check for drop key presses
        foreach (KeyCode key in dropKey) {
            if (Input.GetKeyDown(key)) {
                if (!gameManager.readyToDrop) break; // End if not ready to drop
                DropFruit();
            }
        }
    }

    // Function that drops the fruit in line
    void DropFruit() {
        // End if fruit queue is somehow empty
        if (gameManager.fruitsQueue.Count <= 0) {
            Debug.LogError("Can't drop next fruit because fruit queue is empty!");
            return;   
        }

        // Take the first fruit in the queue and remove it from the queue
        GameObject fruitToDrop = gameManager.fruitsQueue[0];
        gameManager.fruitsQueue.RemoveAt(0);

        // Instantiate the fruit
        Instantiate(fruitToDrop, dropperBody.transform.position, Quaternion.identity);

        // Switch ready to drop to false
        //gameManager.readyToDrop = false;

        // Add a new fruit to the queue
        gameManager.AddRandomFruitToQueue();
    }

    // Function that hides the dropper line if not ready to drop
    void HideLine() {
        if (gameManager.readyToDrop) dropperLine.GetComponent<SpriteRenderer>().enabled = true;
        else dropperLine.GetComponent<SpriteRenderer>().enabled = false;
    }
}
