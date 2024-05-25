using UnityEngine;

public class GameManager : MonoBehaviour {
    /* -------------------------------- Variables ------------------------------- */
    [Header("Fruits")]
    public GameObject fruit0;
    public GameObject fruit1;
    
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
    public void FruitCollided(GameObject selfFruit, GameObject otherFruit, int fruitID) {
        // Print the names of the colliding objects
        Debug.Log("Fruit collided: " + selfFruit.name + " & " + otherFruit.name);

        // Destroy both fruits
        Destroy(selfFruit); Destroy(otherFruit);

        // Summon new fruit
        // TODO
    }
}
