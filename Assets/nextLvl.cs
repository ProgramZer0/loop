using UnityEngine;

public class nextLvl : MonoBehaviour
{
    [SerializeField] private LevelController LC;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        LC.NextArea();
    }
}
