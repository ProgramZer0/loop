using UnityEngine;

public class GameManger : MonoBehaviour
{
    bool timerOn = false;
    float timer = 0;

    private void FixedUpdate()
    {
        timer = +Time.deltaTime;
    }

    public void startTimer()
    {
        timerOn = true;
    }

    public void stopTimer()
    {
        timerOn = false;
    }

    public void resetTimer()
    {
        timer = 0;
    }

    public float getTimerSeconds()
    {
        return timer;
    }
}
