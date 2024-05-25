using Unity.VisualScripting;
using UnityEngine;

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
    
    private GameObject[] fruitsOrder;


    /* ------------------------------- Unity Func ------------------------------- */
    // Start is called before the first frame update
    void Start() {
        fruitsOrder = new GameObject[] { fruit0, fruit1 };
    }

    // Update is called once per frame
    void Update() {
        
    }


    /* -------------------------------- Functions ------------------------------- */
    // Function handles two similar fruits colliding
    public void SameFruitCollided(GameObject selfFruit, GameObject otherFruit, int fruitID) {
        // Print the names of the colliding objects
        Debug.Log("Fruit collided: " + selfFruit.name + " & " + otherFruit.name);

        // Destroy both fruits
        Destroy(selfFruit); Destroy(otherFruit);

        // Summon new fruit
        SummonNewFruit(selfFruit, otherFruit, fruitID);
        
    }

    // Function that summon a new fruit
    void SummonNewFruit(GameObject selfFruit, GameObject otherFruit, int fruitID) {
        fruitID += 1;

        // Check if fruitID is within valid range
        if (!(fruitID >= 0 && fruitID < fruitsOrder.Length)) {
            Debug.LogWarning("Invalid fruitID for fruitsOrder array!");
            return; // End the function
        }

        // Get the next fruit prefab from the array
        GameObject nextFruitPrefab = fruitsOrder[fruitID];
        
        // Calculate midpoint position between selfFruit and otherFruit
        Vector2 midpoint = (selfFruit.transform.position + otherFruit.transform.position) / 2f;

        // Spawn the nextFruit at the midpoint position
        GameObject nextFruit = Instantiate(nextFruitPrefab, midpoint, Quaternion.identity);

        // Log the name of the next fruit spawned
        Debug.Log("Next fruit spawned: " + nextFruit.name);  
    }
}
