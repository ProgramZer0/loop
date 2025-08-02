using UnityEngine;
using TMPro;

public class GameManger : MonoBehaviour
{
    [SerializeField] private GameObject timerText;
    bool timerOn = true;
    float timer = 0;

    private void FixedUpdate()
    {
        if(timerOn)
            timer += Time.deltaTime;
        timerText.GetComponent<TextMeshProUGUI>().text = timer.ToString();

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

    public void setTimer(float time)
    {
        timer = time;
    }

    public float getTimerSeconds()
    {
        return timer;
    }
}
