using UnityEngine;

public class Lever : MonoBehaviour
{
    [SerializeField] private Sprite offSprite;
    [SerializeField] private Sprite onSprite;
    [SerializeField] private GameManger GM;
    [SerializeField] private GameObject target;

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
        if (target != null)
        {

            target.SendMessage("OnLeverToggle", isOn, SendMessageOptions.DontRequireReceiver);
        }
    }

    private void UpdateSprite()
    {
        spriteRenderer.sprite = isOn ? onSprite : offSprite;
    }
}
