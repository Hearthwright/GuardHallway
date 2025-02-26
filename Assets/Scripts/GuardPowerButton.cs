using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardPowerButton : MonoBehaviour
{
    public GameObject guard; // The guard object
    public GameObject button; // The button object that the player can press
    public float freezeDuration = 3f; // Duration in seconds to freeze the guard
    private bool isGuardFrozen = false;
    private UnityEngine.AI.NavMeshAgent guardNavMeshAgent; // The NavMeshAgent controlling the guard's movement

    // Start is called before the first frame update
    void Start()
    {
        guardNavMeshAgent = guard.GetComponent<UnityEngine.AI.NavMeshAgent>();
    }

    public void PressButton()
    {
        if (!isGuardFrozen)
        {
            StartCoroutine(FreezeGuard());
        }
    }

    System.Collections.IEnumerator FreezeGuard()
    {
        // Set isGuardFrozen flag to true
        isGuardFrozen = true;
        // Stop the guard from moving
        guardNavMeshAgent.isStopped = true; 
        Debug.Log("Guard stopped for " + freezeDuration + " seconds.");
        
        yield return new WaitForSeconds(freezeDuration);
        // Resume the guard's movement
        guardNavMeshAgent.isStopped = false; 
        Debug.Log("Guard resumed movement.");
        // Set isGuardFrozen flag to false
        isGuardFrozen = false;
    }
}
