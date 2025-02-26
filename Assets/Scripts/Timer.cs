using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText; // References a TMPro object to display remaining time as text
    [Header("Timer Settings")]
    public float remainingTime; // Determines how much time is remaining in the timer. Since the game level should be about 60-90 seconds, set this somewhere in that range

    private SceneController sceneController;

    // Start is called before the first frame update
    void Start()
    {
        sceneController = FindObjectOfType<SceneController>();
    }

    // Update is called once per frame
    void Update()
    {
        // If there is still time left in the Timer...
        if (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
        }
        // If the timer has run out...
        else if (remainingTime <= 0)
        {
            remainingTime = 0;
            // Transition to Win Screen
            sceneController.GameOver();
        }
        // Convert remaining time to an integer
        int seconds = Mathf.FloorToInt(remainingTime);
        // Converting Seconds from int to String to be used by TMPro
        timerText.text = seconds.ToString();
    }
}
