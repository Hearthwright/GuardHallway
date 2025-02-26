using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    // Unlocks and renables the cursor for the purpose of menu navigation
    public void EnableCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Loads the Main Menu
    public void MainMenu ()
    {
        EnableCursor();
        SceneManager.LoadScene("Main Menu");
    }

    // Loads the Game Level Scene
    public void Play()
    {
        SceneManager.LoadScene("Game Level");
    }

    // Loads the Game Level Scene
    public void TestLevel()
    {
        SceneManager.LoadScene("Test Level");
    }

    // Loads the Tutorial Scene
    public void Tutorial()
    {
        EnableCursor();
        SceneManager.LoadScene("Tutorial");
    }

    // Loads the Win Screen
    public void Win()
    {
        EnableCursor();
        SceneManager.LoadScene("Win Screen");
    }

    // Loads the Game Over Screen
    public void GameOver()
    {
        EnableCursor();
        SceneManager.LoadScene("Lose Screen");
    }

    // Closes out of the Game
    // Only works in built project, not Unity Editor
    public void Quit()
    {
        Application.Quit();
    }
}
