using UnityEngine;

public class Lever : MonoBehaviour
{
    [SerializeField] private Sprite offSprite;
    [SerializeField] private Sprite onSprite;
    [SerializeField] private GameManger GM;
    [SerializeField] private GameObject[] targets;

    private SpriteRenderer spriteRenderer;
    private bool isOn = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateSprite();
    }

    public void Restart()
    {
        isOn = false;
    }
        
    public void Toggle()
    {
        isOn = !isOn;
        UpdateSprite();
        if (targets != null)
        {
            foreach (GameObject obj in targets)
            {
                obj.GetComponent<door>().SendMessage("OnLeverToggle", isOn, SendMessageOptions.DontRequireReceiver);
            }
            
        }
    }

    private void UpdateSprite()
    {
        spriteRenderer.sprite = isOn ? onSprite : offSprite;
    }
}
