using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using TMPro;


public class GameManager : MonoBehaviour {
    /* -------------------------------- Variables ------------------------------- */
    [Header("Fruits")]
    public GameObject fruitsContainer;
    public GameObject fruit0, fruit1, fruit2, fruit3, fruit4, fruit5, fruit6, fruit7, fruit8, fruit9, fruit10;
    public GameObject[] fruitsOrder;
    public List<GameObject> fruitsQueue;
    public List<(GameObject fruit, float weight)> fruitsDroppable = new List<(GameObject, float)>();
    public int[] fruitsPoints = { 0, 1, 3, 6, 10, 15, 21, 28, 36, 45, 55, 66};
    public int[] fruitsMergeBonusPoints = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12};
    public float fruitsZ = -1;

    [Header("Particles")]
    public float particlesZ = -2;
    public GameObject particlesContainer;
    public ParticleSystem particleFruitCollision;

    [Header("Audio")]
    public float volumeSFX = 1; // Between 0->1
    public GameObject audioContainer;
    public GameObject audioSource;
    public AudioClip SFX_fruitMurge;
    public AudioClip SFX_fruit10Murge;

    [Header("Score")]
    public int score = 0;
    public TextMeshProUGUI textScore;

    [Header("Next Fruit")]
    public GameObject nextFruit;
    public GameObject nextFruitDisplay;

    [Header("Others")]
    public bool readyToDrop = true;
    public bool gameLost = false;
    public bool murgedFruitKeepMovement = true;
    public float fruitDestroyScaleIncrement = 0.05f;

    // Private
    private bool hasGameLostBeenCalled = false;


    /* ------------------------------- Unity Func ------------------------------- */
    // Start is called before the first frame update
    void Start() {
        // Create fruit order list
        fruitsOrder = new GameObject[] { fruit0, fruit1, fruit2, fruit3, fruit4, fruit5, fruit6, fruit7, fruit8, fruit9, fruit10 };
        // Create starting fruit queue
        fruitsQueue = new List<GameObject> { fruit0, fruit0 };
        // Initialize the droppable fruits with their weights
        fruitsDroppable.Add((fruit0, 0.25f));
        fruitsDroppable.Add((fruit1, 0.30f));
        fruitsDroppable.Add((fruit2, 0.30f));
        fruitsDroppable.Add((fruit3, 0.15f));

        // Place first next fruit
        Invoke("UpdateNextFruitDisplay", 0.1f);
    }

    // Update is called once per frame
    void Update() {
        // Update score on screen
        textScore.text = score.ToString();

        // End the game gameLost switch to true
        if (gameLost && !hasGameLostBeenCalled) GameLost();

        // Update next fruit if it changes
        //if (nextFruit != fruitsQueue[1]) UpdateNextFruitDisplay();
    }


    /* -------------------------------- Functions ------------------------------- */
    // Coroutine that summons and gets rid of particles
    public IEnumerator SpawnParticle(ParticleSystem particleSystem, Vector2 position) {
        Vector3 spawnPosition = new Vector3(position.x, position.y, particlesZ);
        ParticleSystem particleInstance = Instantiate(particleSystem, spawnPosition, Quaternion.identity);
        particleInstance.transform.SetParent(particlesContainer.transform); // Set particlesContainer as parent
        yield return new WaitForSeconds(particleInstance.main.duration); // Wait for the duration of the particle system        
        Destroy(particleInstance.gameObject); // Destroy the particle system
    }
    
    // Function handles two similar fruits colliding
    public void SameFruitCollided(GameObject selfFruit, GameObject otherFruit, int fruitID) {
        // Play animation
        StartCoroutine(SameFruitCollidedAnimation(selfFruit, otherFruit, fruitID));

        // Add points to score
        CalculatePoint(fruitID, true);
    }

    // Coroutine to animate the fruits on collision
    IEnumerator SameFruitCollidedAnimation(GameObject selfFruit, GameObject otherFruit, int fruitID) {
        float fruitDestroyScaleIncrement = 0.05f;

        // Change fruit scale
        selfFruit.transform.localScale = selfFruit.transform.localScale + new Vector3(fruitDestroyScaleIncrement, fruitDestroyScaleIncrement, fruitDestroyScaleIncrement);
        otherFruit.transform.localScale = otherFruit.transform.localScale + new Vector3(fruitDestroyScaleIncrement, fruitDestroyScaleIncrement, fruitDestroyScaleIncrement);

        // Spwan particles
        Vector2 midpoint = (selfFruit.transform.position + otherFruit.transform.position) / 2f; // Position between fruits
        StartCoroutine(SpawnParticle(particleFruitCollision, midpoint));

        // Play SFX
        PlaySFX(SFX_fruitMurge, true, 0.8f, 1.2f);

        // Wait for a short duration
        yield return new WaitForSeconds(0.05f);

        // Destroy both fruits
        Destroy(selfFruit);
        Destroy(otherFruit);

        // Merge two fruits
        MergeFruits(selfFruit, otherFruit, fruitID);
    }

    // Function that summon a new fruit by murging two of them
    void MergeFruits(GameObject selfFruit, GameObject otherFruit, int fruitID) {
        // Get next fruit if fruitID <= 9
        if (fruitID <= 9) {
            // Get next fruit ID
            int nextFruitID = fruitID + 1;

            // Check if fruitID is within valid range
            if (!(nextFruitID >= 0 && nextFruitID < fruitsOrder.Length)) return;

            // Get the next fruit prefab from the array
            GameObject nextFruitPrefab = fruitsOrder[nextFruitID];
            
            // Calculate midpoint position between selfFruit and otherFruit
            Vector2 midpoint = (selfFruit.transform.position + otherFruit.transform.position) / 2f;

            // Calculate average velocity
            Rigidbody2D selfRb = selfFruit.GetComponent<Rigidbody2D>();
            Rigidbody2D otherRb = otherFruit.GetComponent<Rigidbody2D>();
            Vector2 averageVelocity = (selfRb.linearVelocity + otherRb.linearVelocity) / 2f;
            float averageAngularVelocity = (selfRb.angularVelocity + otherRb.angularVelocity) / 2f;    
            
            // Spawn the mergedFruit at the midpoint position
            GameObject mergedFruit = Instantiate(nextFruitPrefab, new Vector3(midpoint.x, midpoint.y, fruitsZ), Quaternion.identity);
            mergedFruit.transform.SetParent(fruitsContainer.transform);
            
            // Apply the average velocity and angular velocity
            if (murgedFruitKeepMovement) {
                Rigidbody2D mergedRb = mergedFruit.GetComponent<Rigidbody2D>();
                if (mergedRb != null) {
                    mergedRb.linearVelocity = averageVelocity;
                    mergedRb.angularVelocity = averageAngularVelocity;
                }
            }
        }
        // Clear board if fruitID = 10 (water melon)
        else if (fruitID == 10) {
            // Play fruit10 merge sound
            PlaySFX(SFX_fruit10Murge);
            
            // Clear board
            StartCoroutine(ClearBoard());
        }
    }

    // Function that adds a random fruit to the queue with weighted probabilities
    public void AddRandomFruitToQueue() {
        // Calculate totalWeight
        float totalWeight = 0f;
        foreach (var fruit in fruitsDroppable) totalWeight += fruit.weight;

        // Generate a random number between 0 and totalWeight
        float randomValue = Random.Range(0f, totalWeight);

        // Determine which fruit to select based on the random value
        float cumulativeWeight = 0f;
        for (int i = 0; i < fruitsDroppable.Count; i++) {
            cumulativeWeight += fruitsDroppable[i].weight;
            if (randomValue <= cumulativeWeight) {
                fruitsQueue.Add(fruitsDroppable[i].fruit);
                break;
            }
        }
    }

    // Function that displays next fruit
    public void UpdateNextFruitDisplay() {
        // Save 2nd fruit in queue in nextFruit var
        nextFruit = fruitsQueue[1];

        // Remove any child of nextFruitDisplay
        for (int i = nextFruitDisplay.transform.childCount - 1; i >= 0; i--) {
            GameObject.Destroy(nextFruitDisplay.transform.GetChild(i).gameObject);
        }

        // Instantiate next fruit as a child of nextFruitDisplay
        GameObject nextFruitObject = Instantiate(nextFruit, nextFruitDisplay.transform.position, Quaternion.identity);
        nextFruitObject.transform.SetParent(nextFruitDisplay.transform);
        nextFruitObject.GetComponent<Rigidbody2D>().simulated = false; // Turn off simulation on rb2d
    }

    // Function that calculates the points gained from fruits
    void CalculatePoint(int fruitID, bool merge = false) {
        // Validate fruitID
        if (fruitID < 0 || fruitID >= fruitsPoints.Length) {
            Debug.LogError($"Invalid fruitID: {fruitID}. It should be within the range 0 to {fruitsPoints.Length - 1}.");
            return;
        }
        
        // Add points
        if (merge && fruitID < fruitsMergeBonusPoints.Length) {
            score += fruitsMergeBonusPoints[fruitID];
        }
        score += fruitsPoints[fruitID];
    }

    // Coroutine to destroy all fruits
    IEnumerator ClearBoard(float destroyDelay = 0.05f) {
        Transform containerTransform = fruitsContainer.transform;

        // Iterate through each child object backwards
        while (containerTransform.childCount > 0) {
            // Get the child object at the last index
            GameObject fruitObject = containerTransform.GetChild(containerTransform.childCount - 1).gameObject;
            
            // Get the fruit ID from the FruitScript attached to the fruitObject
            Fruit fruitScript = fruitObject.GetComponent<Fruit>();
            int fruitID = fruitScript.fruitID;

            // Animation
            fruitObject.transform.localScale = fruitObject.transform.localScale + new Vector3(fruitDestroyScaleIncrement, fruitDestroyScaleIncrement, fruitDestroyScaleIncrement);
            StartCoroutine(SpawnParticle(particleFruitCollision, fruitObject.transform.position));

            // Play SFX
            PlaySFX(SFX_fruitMurge, true, 0.8f, 1.2f);

            // Wait for a little before going to next fruit and destroying
            yield return new WaitForSeconds(destroyDelay);

            // Calculate point from fruit and destroy it
            CalculatePoint(fruitID);
            Destroy(fruitObject);
        }
    }

    // Function that plays the SFX
    void PlaySFX(AudioClip audioClip, bool pitchRange=false, float pitch1=0f, float pitch2=0f ) {
        // Create audio source object as child of audioContainer
        GameObject localAudioSource = Instantiate(audioSource);
        localAudioSource.transform.SetParent(audioContainer.transform); // Set audioContainer as parent
        AudioSource localAudioSourceComponent = localAudioSource.GetComponent<AudioSource>(); // Get component

        // Change pitch if pitchRange true
        if (pitchRange) localAudioSourceComponent.pitch = Random.Range(pitch1, pitch2);

        // Play sound
        localAudioSourceComponent.volume = volumeSFX;
        localAudioSourceComponent.clip = audioClip;
        localAudioSourceComponent.Play();

        // Destroy object
        Destroy(localAudioSource, localAudioSourceComponent.clip.length);
    }

    // Function that ends the game when its lost
    void GameLost() {
        hasGameLostBeenCalled = true;
        readyToDrop = false;
        StartCoroutine(ClearBoard()); // Clear the board
    }

    // Function to restrat the game
    public void Restart() {
        Debug.Log("RESTART GAME");

        // Reload scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
