using UnityEngine;

public class door : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private BoxCollider2D collider2D;
    [SerializeField] private GameManger GM;
    [SerializeField] private LevelController LC;

    public void OnLeverToggle(bool isOpen)
    {
        if (animator != null)
        {
            animator.SetBool("Open", isOpen);
            collider2D.enabled = false;
            LC.unlocked = true;
        }
        else
        {
            gameObject.SetActive(!isOpen); // crude fallback
        }
    }
}

