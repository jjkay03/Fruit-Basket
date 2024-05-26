using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
    /* -------------------------------- Variables ------------------------------- */
    [Header("Fruits")]
    public GameObject fruitsContainer;
    public GameObject fruit0;
    public GameObject fruit1;
    public GameObject fruit2;
    public GameObject fruit3;
    public GameObject fruit4;
    public GameObject fruit5;
    public GameObject fruit6;
    public GameObject fruit7;
    public GameObject fruit8;
    public GameObject fruit9;
    public GameObject fruit10;
    public GameObject[] fruitsOrder;
    public GameObject[] fruitsDroppable;
    public List<GameObject> fruitsQueue;
    public int[] fruitsPoints = { 0, 1, 3, 6, 10, 15, 21, 28, 36, 45, 55};
    public int[] fruitsMurgeBonusPoints = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11};

    [Header("Particles")]
    public float particlesZ = -2;
    public ParticleSystem particleFruitCollision;

    [Header("Others")]
    public int score = 0;
    public bool readyToDrop = true;


    /* ------------------------------- Unity Func ------------------------------- */
    // Start is called before the first frame update
    void Start() {
        // Create fruit order list
        fruitsOrder = new GameObject[] { fruit0, fruit1, fruit2, fruit3, fruit4, fruit5, fruit6, fruit7, fruit8, fruit9, fruit10 };
        // Create droppable druits list
        fruitsDroppable = new GameObject[] { fruit0, fruit1, fruit2, fruit3 };
        // Create starting fruit queue
        fruitsQueue = new List<GameObject> { fruit0, fruit0 };
    }

    // Update is called once per frame
    void Update() {
        
    }


    /* -------------------------------- Functions ------------------------------- */
    // Function handles two similar fruits colliding
    public void SameFruitCollided(GameObject selfFruit, GameObject otherFruit, int fruitID) {
        // Play animation
        StartCoroutine(SameFruitCollidedAnimation(selfFruit, otherFruit, fruitID));

        // Add points to score
        CalculatePoint(fruitID, true);
    }

    // Coroutine to animate the fruits on collision
    private IEnumerator SameFruitCollidedAnimation(GameObject selfFruit, GameObject otherFruit, int fruitID) {
        float scaleIncrement = 0.05f;

        // Change fruit scale
        selfFruit.transform.localScale = selfFruit.transform.localScale + new Vector3(scaleIncrement, scaleIncrement, scaleIncrement);
        otherFruit.transform.localScale = otherFruit.transform.localScale + new Vector3(scaleIncrement, scaleIncrement, scaleIncrement);
        
        // Get SpriteRenderer components and change colors to white
        // TODO: MAKE THE FRUIT FLASH WHITE

        // Spwan particles
        Vector2 midpoint = (selfFruit.transform.position + otherFruit.transform.position) / 2f; // Position between fruits
        StartCoroutine(SpawnParticleFruitCollision(midpoint));

        // Wait for a short duration
        yield return new WaitForSeconds(0.05f);

        // Destroy both fruits
        Destroy(selfFruit);
        Destroy(otherFruit);

        // Murge two fruits
        MurgeFruits(selfFruit, otherFruit, fruitID);
    }

    // Function that summon a new fruit by murging two of them
    void MurgeFruits(GameObject selfFruit, GameObject otherFruit, int fruitID) {
        fruitID += 1;

        // Check if fruitID is within valid range
        if (!(fruitID >= 0 && fruitID < fruitsOrder.Length)) return;

        // Get the next fruit prefab from the array
        GameObject nextFruitPrefab = fruitsOrder[fruitID];
        
        // Calculate midpoint position between selfFruit and otherFruit
        Vector2 midpoint = (selfFruit.transform.position + otherFruit.transform.position) / 2f;

        // Spawn the murgedFruit at the midpoint position
        GameObject murgedFruit = Instantiate(nextFruitPrefab, midpoint, Quaternion.identity);
        murgedFruit.transform.SetParent(fruitsContainer.transform); 
    }

    // Coroutine that summons and gets rid of particleFruitCollision
    private IEnumerator SpawnParticleFruitCollision(Vector2 position) {
        Vector3 spawnPosition = new Vector3(position.x, position.y, particlesZ);
        ParticleSystem particleInstance = Instantiate(particleFruitCollision, spawnPosition, Quaternion.identity);
        yield return new WaitForSeconds(particleInstance.main.duration); // Wait for the duration of the particle system        
        Destroy(particleInstance.gameObject); // Destroy the particle system
    }

    // Function that adds a random fruit to the queue
    public void AddRandomFruitToQueue() {
        int randomIndex = Random.Range(0, fruitsDroppable.Length);
        GameObject randomFruit = fruitsDroppable[randomIndex];
        fruitsQueue.Add(randomFruit);
    }

    // Function that calculate the point gained from fruits
    private void CalculatePoint(int fruitID, bool murge=false) {
        if (murge) {score += fruitsMurgeBonusPoints[fruitID];}
        score += fruitsPoints[fruitID];
    }
}
