using System.Collections.Generic;
using UnityEngine;

public class LoseArea : MonoBehaviour {
    /* -------------------------------- Variables ------------------------------- */
    public float loseTime = 3f;
    
    private Dictionary<GameObject, float> losingFruits;
    private GameManager gameManager;

    
    /* ------------------------------- Unity Func ------------------------------- */
    // Start is called before the first frame update
    void Start() {
        // Get game manager
        gameManager = FindObjectOfType<GameManager>();

        // Create dictionary
        losingFruits = new Dictionary<GameObject, float>();
    }

    // Triggers when object stay lose area
    void OnTriggerStay2D(Collider2D collision) {
        // Check if collision object has the "Fruit" tag
        if (!collision.CompareTag("Fruit")) return;

        // Add fruit in losing area to losing fruits dic and update the time its been in
        float currentTime = losingFruits.GetValueOrDefault(collision.gameObject, 0f);
        losingFruits[collision.gameObject] = currentTime + Time.deltaTime;

        // Check if fruit has been in losing area for too long
        if (currentTime + Time.deltaTime >= loseTime ) {
            // If fruit has been in for too long tell game manager that game has been lost
            if (gameManager.gameLost != true) {
                gameManager.gameLost = true;
                Debug.Log("Game ending fruit stayed in lose area for too long!", collision.gameObject);
            }      
        }
    }

    // Triggers when object exit the area
    void OnTriggerExit2D(Collider2D collision) {
        losingFruits.Remove(collision.gameObject);
    }
}
