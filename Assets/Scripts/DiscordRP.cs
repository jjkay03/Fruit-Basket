// Original script by sventomasek - https://github.com/sventomasek/Discord-Script-for-Unity

using Discord;
using UnityEngine;


public class NewBehaviourScript : MonoBehaviour {
    
    /* -------------------------------- Variables ------------------------------- */
    public long applicationID = 1246255524593401917;
    [Space]
    //public string details = "In a game";
    public string state = "Score: ";
    [Space]
    public string largeImage = "main";
    public string largeText = "Fruit Basket";

    private long time;

    private static bool instanceExists;
    private GameManager gameManager;
    public Discord.Discord discord;

    /* --------------------------------- Methods -------------------------------- */
    void Awake() {
        // Transition the GameObject between scenes, destroy any duplicates
        if (!instanceExists) {
            instanceExists = true;
            DontDestroyOnLoad(gameObject);
        }
        else if (FindObjectsOfType(GetType()).Length > 1) {
            Destroy(gameObject);
        }
    }

    void Start() {
        // Get game manager
        gameManager = FindObjectOfType<GameManager>();

        // Log in with the Application ID
        discord = new Discord.Discord(applicationID, (System.UInt64)Discord.CreateFlags.NoRequireDiscord);

        time = System.DateTimeOffset.Now.ToUnixTimeMilliseconds();

        UpdateStatus();
    }

    void Update() {
        // Destroy the GameObject if Discord isn't running
        try {
            discord.RunCallbacks();
        }
        catch {
            Destroy(gameObject);
        }
    }

    void LateUpdate() {
        UpdateStatus();
    }

    void UpdateStatus() {
        // Update Status every frame
        try {
            var activityManager = discord.GetActivityManager();
            var activity = new Discord.Activity {
                State = state + gameManager.score,
                Assets = {
                    LargeImage = largeImage,
                    LargeText = largeText
                },
                Timestamps = {
                    Start = time
                }
            };

            activityManager.UpdateActivity(activity, (res) => {
                if (res != Discord.Result.Ok) Debug.LogWarning("Failed connecting to Discord!");
            });
        }
        catch {
            // If updating the status fails, Destroy the GameObject
            Destroy(gameObject);
        }
    }
}
