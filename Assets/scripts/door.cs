using UnityEngine;

public class door : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private BoxCollider2D collider2D;
    [SerializeField] private GameManger GM;
    [SerializeField] private LevelController LC;
    [SerializeField] private bool openStart = false;

    private void Start()
    {
        if (openStart)
        {
            PlatformPush(true);
        }
    }

    public void OnLeverToggle(bool isOpen)
    {
        if (openStart) isOpen = !isOpen;
        if (animator != null)
        {
            animator.SetBool("Open", isOpen);
            if (isOpen)
            {
                collider2D.enabled = false;
                LC.unlocked = true;
            }
            else
            {
                collider2D.enabled = true;
                LC.unlocked = false;
            }
            
        }
        else
        {
            gameObject.SetActive(!isOpen); // crude fallback
        }
    }

    public void PlatformPush(bool isOpen)
    {
        if (openStart) isOpen = !isOpen;

        if (animator != null)
        {
            animator.SetBool("Open", isOpen);

            if (isOpen)
            {
                collider2D.enabled = false;
                LC.unlocked = true;
            }
            else
            {
                collider2D.enabled = true;
                LC.unlocked = false;
            }
        }
        else
        {
            gameObject.SetActive(!isOpen); 
        }
    }
}

