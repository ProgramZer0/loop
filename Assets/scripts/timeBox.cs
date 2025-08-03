using UnityEngine;

public class timeBox : MonoBehaviour
{
    [SerializeField] private GameManger GM;

    private float spawnTime = 0f;
    private Vector3 spawnPos;

    private void Start()
    {
        spawnPos = transform.position;
    }

    public void Restart()
    {
        transform.position = spawnPos;
    }
}
