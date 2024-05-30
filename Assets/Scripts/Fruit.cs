using UnityEngine;

public class Fruit : MonoBehaviour {
    /* -------------------------------- Variables ------------------------------- */
    [Header("Fruit")]
    public int fruitID;
    public bool collidedWithSameFruit = false;

    // Private
    private GameManager gameManager;


    /* ------------------------------- Unity Func ------------------------------- */
    // Start is called before the first frame update
    void Start() {
        // Get game manager
        gameManager = FindObjectOfType<GameManager>();
    }

    // Deal with Fruits colliding, both are needed because OnCollisionStay2D doesn't trigger when fruits collide really fast
    void OnCollisionEnter2D(Collision2D collision) { FruitCollision(collision); } // Triggers when collides
    void OnCollisionStay2D(Collision2D collision) { FruitCollision(collision); } // Triggers when collides and stay


    /* -------------------------------- Functions ------------------------------- */
    // Function that deals with fruit colliding with another fruit
    void FruitCollision(Collision2D collision) {
        Fruit otherFruit = collision.gameObject.GetComponent<Fruit>();
        
        // Check ending scenarios
        if (collidedWithSameFruit) return; // End if has already collided with same fruit
        if (!collision.gameObject.CompareTag("Fruit")) return; // End if collision is not a fruit
        if (otherFruit == null) return; // End if collided fruit doesn't exist
        if (otherFruit.collidedWithSameFruit) return; // End if collided with fruit already collided with same fruit
        if (otherFruit.fruitID != fruitID) return; // End if both collided fruit are not the same (not same fruitID)

        // Mark both fruits as collided
        collidedWithSameFruit = true;
        otherFruit.collidedWithSameFruit = true;

        // Call the GameManager to handle the collision
        gameManager.SameFruitCollided(gameObject, collision.gameObject, fruitID);
    }
}
