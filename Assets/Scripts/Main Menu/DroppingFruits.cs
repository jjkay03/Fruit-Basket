using UnityEngine;

public class DroppingFruits : MonoBehaviour {
    /* -------------------------------- Variables ------------------------------- */
    [Header("Spwaner Settings")]
    public float fruitsZ = 20f;
    public float dropFruitInterval = 0.4f;
    public float destroyFruitDelay = 3f;
    
    [Header("Fruits")]
    public GameObject fruitSpawner;
    public GameObject fruitsContainer;
    public GameObject fruit0, fruit1, fruit2, fruit3, fruit4, fruit5, fruit6, fruit7, fruit8, fruit9, fruit10;

    // Private
    private GameObject[] fruitsList;
    private float timeSinceLastDrop = 0f;


    /* ------------------------------- Unity Func ------------------------------- */
    // Start is called before the first frame update
    void Start() {
        // Create fruits list
        fruitsList = new GameObject[] { fruit0, fruit1, fruit2, fruit3, fruit4, fruit5, fruit6, fruit7, fruit8, fruit9, fruit10 };
    }
    
    // Update is called once per frame
    void Update() {
        // Drop fruits with intervals
        timeSinceLastDrop += Time.deltaTime;
        if (timeSinceLastDrop >= dropFruitInterval) {
            DropRandomFruit();
            timeSinceLastDrop = 0f;
        }

        // Drop fruit when pressing space
        if (Input.GetKeyDown(KeyCode.Space)) {
            DropRandomFruit();
        }
    }


    /* -------------------------------- Functions ------------------------------- */
    // Function that drops a random fruit
    void DropRandomFruit() {
        // Get the bounds of the sprite
        SpriteRenderer spriteRenderer = fruitSpawner.GetComponent<SpriteRenderer>();
        Bounds bounds = spriteRenderer.bounds;

        // Choose a random X position within the bounds
        float randomX = Random.Range(bounds.min.x, bounds.max.x);

        // Choose a random fruit from the list
        int randomIndex = Random.Range(0, fruitsList.Length);
        GameObject randomFruit = fruitsList[randomIndex];

        // Debug log to check if the selected fruit is null
        if (randomFruit == null) {
            Debug.LogError("Selected fruit is null at index: " + randomIndex);
            return;
        }

        // Spawn the fruit at the random position
        Vector3 spawnPosition = new Vector3(randomX, fruitSpawner.transform.position.y, fruitsZ);
        GameObject droppedFruit = Instantiate(randomFruit, spawnPosition, Quaternion.identity);
        droppedFruit.transform.SetParent(fruitsContainer.transform);

        // Destroy the fruit
        Destroy(droppedFruit, destroyFruitDelay);
    }
}
