using System.Collections.Generic;
using UnityEngine;

public class WarningArea : MonoBehaviour {
    /* -------------------------------- Variables ------------------------------- */
    public GameObject loseLine;
    public float warningTime = 1f;
    
    private Dictionary<GameObject, float> warningFruitsTimes;
    private List<GameObject> warningFruits;


    /* ------------------------------- Unity Func ------------------------------- */
    // Start is called before the first frame update
    void Start() {
        // Initialize dic and list
        warningFruitsTimes = new Dictionary<GameObject, float>();
        warningFruits = new List<GameObject>();

        // Make sure lose line is hidden when game start
        loseLine.GetComponent<SpriteRenderer>().enabled = false;
    }

    // Update is called once per frame
    void Update() {
        // Check if there are any warning fruits in the list
        if (warningFruits.Count > 0) loseLine.GetComponent<SpriteRenderer>().enabled = true;
        else loseLine.GetComponent<SpriteRenderer>().enabled = false;
    }

    // Triggers when object stays in the warning area
    void OnTriggerStay2D(Collider2D collision) {
        // Check if collision object has the "Fruit" tag
        if (!collision.CompareTag("Fruit")) return;

        // Add fruit in warning area to warning fruits dic and update the time its been in
        float currentTime = warningFruitsTimes.GetValueOrDefault(collision.gameObject, 0f);
        warningFruitsTimes[collision.gameObject] = currentTime + Time.deltaTime;

        // Check if fruit has been in warning area for too long
        if (currentTime + Time.deltaTime >= warningTime) {
            // Add the fruit to the warningFruits list if it's not already there
            if (!warningFruits.Contains(collision.gameObject)) {
                warningFruits.Add(collision.gameObject);
            }
        }
    }

    // Triggers when object exits the warning area
    void OnTriggerExit2D(Collider2D collision) {
        // Remove fruit from dic and list
        warningFruitsTimes.Remove(collision.gameObject);
        warningFruits.Remove(collision.gameObject);
    }
}
