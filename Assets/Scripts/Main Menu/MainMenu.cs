using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
    /* -------------------------------- Variables ------------------------------- */
    public bool inCredits = false;
    public GameObject panelMenu;
    public GameObject panelCredits;
    

    /* ------------------------------- Button Func ------------------------------ */
    // Button : Play
    public void Play() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    // Button : Credits
    public void Credits() {
        // Open credits
        if (!inCredits) {
            inCredits = true;
            panelMenu.SetActive(false);
            panelCredits.SetActive(true);
        }
        // Close credits
        else {
            inCredits = false;
            panelMenu.SetActive(true);
            panelCredits.SetActive(false);
        }
    }

    // Button : Exit
    public void QuitGame() {
        Application.Quit(); // Close game
    }
}
