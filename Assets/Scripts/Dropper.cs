using UnityEngine;

public class Dropper : MonoBehaviour {
    /* -------------------------------- Variables ------------------------------- */
    [Header("Dropper")]
    public float maxLeftX;
    public float maxRightX;
    public float moveSpeed = 5f; // Speed at which the dropper moves

    [Header("Keybinds")]
    public KeyCode[] leftKey = { KeyCode.LeftArrow,  KeyCode.A };
    public KeyCode[] rightKey = { KeyCode.RightArrow, KeyCode.D };
    public KeyCode[] dropKey = { KeyCode.Space, KeyCode.S };


    /* ------------------------------- Unity Func ------------------------------- */
    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        PlayerInputMoveDropper();
    }

    
    /* -------------------------------- Functions ------------------------------- */
    // Function that deals with player inputs for dropper
    void PlayerInputMoveDropper() {
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
}
