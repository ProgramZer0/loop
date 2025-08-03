using UnityEngine;

public class pressureplate : MonoBehaviour
{
    [SerializeField] private Sprite offSprite;
    [SerializeField] private Sprite onSprite;
    [SerializeField] private GameManger GM;
    [SerializeField] private GameObject[] targets;

    private SpriteRenderer spriteRenderer;
    private bool isOn = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        isOn = true;
        Toggle();
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        isOn = false;
        Toggle();
    }

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
        
        UpdateSprite();
        if (targets != null)
        {
            foreach (GameObject obj in targets)
            {
                obj.GetComponent<door>().SendMessage("PlatformPush", isOn, SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    private void UpdateSprite()
    {
        spriteRenderer.sprite = isOn ? onSprite : offSprite;
    }

}
