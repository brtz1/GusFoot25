using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
    // Called when the "Start Game" button is pressed
    public void OnStartGame() {
        // Load the main game scene (make sure the scene is added in Build Settings)
        SceneManager.LoadScene("GameScene");
    }
}