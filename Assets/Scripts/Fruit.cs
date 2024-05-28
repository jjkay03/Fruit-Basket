using UnityEngine;

public class Fruit : MonoBehaviour {
    /* -------------------------------- Variables ------------------------------- */
    public int fruitID;
    public bool collidedWithSameFruit = false;
    
    private GameManager gameManager;


    /* ------------------------------- Unity Func ------------------------------- */
    // Start is called before the first frame update
    void Start() {
        // Get game manager
        gameManager = FindObjectOfType<GameManager>();
    }

    // Trigers when object collides with an otherone and stay
    void OnCollisionStay2D(Collision2D collision) {
        Fruit otherFruit = collision.gameObject.GetComponent<Fruit>();
        
        // Check ending scenarios
        if (collidedWithSameFruit) return; // End if has already collided with same fruit
        if (!collision.gameObject.CompareTag("Fruit")) return; // End if colision is not a fruit
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
