using System.Collections.Generic;
using UnityEngine;

public class timeBox : MonoBehaviour
{
    [SerializeField] private GameManger GM;

    private float spawnTime = 0f;
    private Vector2 spawnPos;

    private void FixedUpdate()
    {
        if(GM.getTimerSeconds()  <= 0.1f)
        {
            Restart();
        }
    }

    private void Start()
    {
        spawnPos = transform.position;
    }

    public void Restart()
    {
        transform.position = spawnPos;
    }
}
