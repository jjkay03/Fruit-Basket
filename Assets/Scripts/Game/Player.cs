using UnityEngine;

public class Player : MonoBehaviour {
    /* -------------------------------- Variables ------------------------------- */
    [Header("Textures")]
    public Sprite spritePlayerNormal;
    public Sprite spritePlayerHappy;
    public Sprite spritePlayerSad;

    [Header("Player State")]
    public PlayerState playerState = PlayerState.Normal;
    public enum PlayerState {
        Normal,
        Happy, 
        Sad
    }

    // Private
    private GameManager gameManager;


    /* ------------------------------- Unity Func ------------------------------- */
    // Start is called before the first frame update
    void Start() {
        // Get game manager
        gameManager = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update() {
        UpdatePLayer();
        UpdatePlayerState();
    }


    /* -------------------------------- Functions ------------------------------- */
    // Function that updates player based player state
    void UpdatePLayer() {
        if (playerState == PlayerState.Normal) this.GetComponent<SpriteRenderer>().sprite = spritePlayerNormal;
        else if (playerState == PlayerState.Happy) this.GetComponent<SpriteRenderer>().sprite = spritePlayerHappy;
        else if (playerState == PlayerState.Sad) this.GetComponent<SpriteRenderer>().sprite = spritePlayerSad;
    }

    // Function that changes player state based on stuff happening in the game
    void UpdatePlayerState() {
        // Happy
        if (!gameManager.readyToDrop) playerState = PlayerState.Happy;
        else playerState = PlayerState.Normal;

        // Sad
        if (gameManager.gameLost) playerState = PlayerState.Sad;
    }
}
