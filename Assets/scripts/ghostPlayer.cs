using System.Collections.Generic;
using UnityEngine;

public class GhostPlayer : MonoBehaviour
{
    [SerializeField] private GameObject standingAnimate;
    [SerializeField] private GameObject walkingAnimate;
    [SerializeField] private GameManger GM;

    private Rigidbody2D body;
    private List<Commands> replayCommands;
    private int currentIndex = 0;
    private int move = 0;
    private Vector3 zero = Vector3.zero;
    bool jumped = false;


    public void Initialize(List<Commands> commands, GameManger gameManager)
    {
        replayCommands = new List<Commands>(commands);
        GM = gameManager;
    }

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    { 

        if (currentIndex >= replayCommands.Count) return;

        float currentTime = GM.getTimerSeconds();
        Commands cmd = replayCommands[currentIndex];
        Commands nextcmd;
        

        if (cmd.time <= currentTime)
        {
            try
            {
                nextcmd = cmd.nextCommand;
            }
            catch
            {
                nextcmd = cmd;
            }

            //once hit next command it will go to next command rather than incrementing
            if (nextcmd.time <= currentTime)
            {
                if (cmd.facingRight != transform.localScale.x > 0)
                    Flip();
                transform.position = nextcmd.pos;
                currentIndex++;
                jumped = false;
                return;
            }

            if(cmd.facingRight && cmd.moving)
            {
                standingAnimate.SetActive(false);
                walkingAnimate.SetActive(true);
                move = 1;
            }

            if (!cmd.facingRight && cmd.moving)
            {
                standingAnimate.SetActive(false);
                walkingAnimate.SetActive(true);
                move = -1;
            }

            if (!cmd.moving)
            {
                standingAnimate.SetActive(true);
                walkingAnimate.SetActive(false);
                move = 0;
            }

            Vector3 targetVelocity = new Vector2(move * 10f, body.linearVelocity.y);
            body.linearVelocity = Vector3.SmoothDamp(body.linearVelocity, targetVelocity, ref zero, 0.05f);

            if (cmd.jumping && !jumped)
            {
                jumped = true;
                body.AddForce(new Vector2(0f, 400f)); // You'll need to pass in jump force
            }
            // optional: flip sprite
            if (cmd.facingRight != transform.localScale.x > 0)
                Flip();
        }
    }

    private void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}
