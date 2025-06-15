using System.Diagnostics;
using UnityEngine;

public class GameOverHandler : MonoBehaviour
{
    // Game Over Camera Object
    public Camera gameOverCamera;

    // Call this method to trigger Game Over
    public void GameOver()
    {
        UnityEngine.Debug.Log("Game Over!");

        if (gameOverCamera != null)
        {
            gameOverCamera.enabled = true;
            Camera.main.enabled = false;
        }

        GameObject.Find("TimeManager").GetComponent<TimerUI>().StopTimer();

        // Destroy the GameObject this script is attached to
        Destroy(gameObject);
    }

    // Example: Trigger Game Over on collision with "Enemy"
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Finish"))
        {
            GameOver();
        }
    }
}