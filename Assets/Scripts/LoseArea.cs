using System.Collections.Generic;
using UnityEngine;

public class LoseArea : MonoBehaviour {
    /* -------------------------------- Variables ------------------------------- */
    public float loseTime = 2f;
    
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

    // Update is called once per frame
    void Update() {
        
    }

    // Trigers when object collides with an otherone and stay
    void OnCollisionStay2D(Collision2D collision) { 
        float currentTime = losingFruits.GetValueOrDefault(collision.gameObject, 0f);
        losingFruits[collision.gameObject] = currentTime + Time.deltaTime;

        if (currentTime + Time.deltaTime >= loseTime ) {
            gameManager.gameLost = true;
        }

        //Debug.Log("DELTA TIME: " + Time.deltaTime + "\nCURRENT TIME: " + currentTime, collision.gameObject);
    }

    void OnCollisionExit2D(Collision2D collision) {
        losingFruits.Remove(collision.gameObject);
    }


    /* -------------------------------- Functions ------------------------------- */

}
