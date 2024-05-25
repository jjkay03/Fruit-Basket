using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
    /* -------------------------------- Variables ------------------------------- */
    [Header("Fruits")]
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
    public int[] fruitsPoints = { 2, 4, 6, 8, 10, 12, 14, 16, 20, 22};

    [Header("Particles")]
    public float particlesZ = -1;
    public ParticleSystem particleFruitCollision;


    /* ------------------------------- Unity Func ------------------------------- */
    // Start is called before the first frame update
    void Start() {
        fruitsOrder = new GameObject[] { fruit0, fruit1, fruit2, fruit3 };
    }

    // Update is called once per frame
    void Update() {
        
    }


    /* -------------------------------- Functions ------------------------------- */
    // Function handles two similar fruits colliding
    public void SameFruitCollided(GameObject selfFruit, GameObject otherFruit, int fruitID) {
        // Print the names of the colliding objects
        Debug.Log("Fruit collided: " + selfFruit.name + " & " + otherFruit.name);

        // Play animation
        StartCoroutine(SameFruitCollidedAnimation(selfFruit, otherFruit, fruitID));
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

        // Summon new fruit
        SummonNewFruit(selfFruit, otherFruit, fruitID);
    }

    // Function that summon a new fruit
    void SummonNewFruit(GameObject selfFruit, GameObject otherFruit, int fruitID) {
        fruitID += 1;

        // Check if fruitID is within valid range
        if (!(fruitID >= 0 && fruitID < fruitsOrder.Length)) return;

        // Get the next fruit prefab from the array
        GameObject nextFruitPrefab = fruitsOrder[fruitID];
        
        // Calculate midpoint position between selfFruit and otherFruit
        Vector2 midpoint = (selfFruit.transform.position + otherFruit.transform.position) / 2f;

        // Spawn the nextFruit at the midpoint position
        GameObject nextFruit = Instantiate(nextFruitPrefab, midpoint, Quaternion.identity);

        // Log the name of the next fruit spawned
        Debug.Log("Next fruit spawned: " + nextFruit.name);  
    }

    // Coroutine that summons and gets rid of particleFruitCollision
    private IEnumerator SpawnParticleFruitCollision(Vector2 position) {
        Vector3 spawnPosition = new Vector3(position.x, position.y, particlesZ);
        ParticleSystem particleInstance = Instantiate(particleFruitCollision, spawnPosition, Quaternion.identity);
        yield return new WaitForSeconds(particleInstance.main.duration); // Wait for the duration of the particle system        
        Destroy(particleInstance.gameObject); // Destroy the particle system
    }

}
