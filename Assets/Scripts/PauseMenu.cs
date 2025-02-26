using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    private SceneController sceneController;

    // Pause Panel Game Object
    public GameObject pausePanel; // The pause menu UI asset
    public static bool isPaused = false; // Determines whether or not the game is currently paused

    [Header("Keybind Settings")]
    public KeyCode pauseKey = KeyCode.Escape; // Determines which key handles pausing (Default = Escape)

    // Start is called before the first frame update
    void Start()
    {
        // Disables pause screen
        pausePanel.SetActive(false);
        // Access Scene Controller
        sceneController = FindObjectOfType<SceneController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(pauseKey))
        {
            // If the game is currently paused...
            if (isPaused)
            {
                // Lock player's cursor to the center of the screen
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                // Resume the game
                ResumeGame();
            }
            // If the game is not currently paused...
            else
            {
                //Enable the Cursor
                sceneController.EnableCursor();
                // Pause the game
                PauseGame();
            }
        }
    }

    // Pauses the game
    public void PauseGame()
    {
        // Un-hides the pause panel
        pausePanel.SetActive(true);
        // Sets timescale to 0 to prevent gameplay
        Time.timeScale = 0f;
        // Update isPaused to show game is paused
        isPaused = true;
    }

    // Resumes the game
    public void ResumeGame()
    {
        // Hides the pause panel
        pausePanel.SetActive(false);
        // Sets timescale to return speed to normal
        Time.timeScale = 1f;
        // Update isPaused to show game is not paused
        isPaused = false;
    }
}
