using UnityEngine;
using TMPro;

public class TimerUI : MonoBehaviour
{
    public TMP_Text timerText;
    private float elapsedTime = 0f;
    private bool isRunning = false;  // Start paused

    void Update()
    {
        // Start timer on first input
        if (!isRunning && Input.anyKeyDown)
        {
            isRunning = true;
        }

        if (!isRunning) return;

        elapsedTime += Time.deltaTime;
        UpdateTimerDisplay(elapsedTime);
    }

    void UpdateTimerDisplay(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        int milliseconds = Mathf.FloorToInt((time * 1000) % 1000);
        timerText.text = string.Format("Time: {0:00}:{1:00}.{2:000}", minutes, seconds, milliseconds);
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    public void ResetTimer()
    {
        elapsedTime = 0f;
        isRunning = false;
        UpdateTimerDisplay(elapsedTime);
    }
}
