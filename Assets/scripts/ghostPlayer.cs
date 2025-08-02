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
    private bool jumped = false;

    public void Initialize(List<Commands> commands, GameManger gameManager)
    {
        replayCommands = new List<Commands>(commands);
        GM = gameManager;
    }

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        body.isKinematic = true; // Prevent physics interference
    }

    private void FixedUpdate()
    {
        if (replayCommands == null || currentIndex >= replayCommands.Count - 1) return;

        float currentTime = GM.getTimerSeconds();
        Commands cmd = replayCommands[currentIndex];
        Commands nextCmd = cmd.nextCommand;

        if (nextCmd == null)
        {
            transform.position = cmd.pos;
            walkingAnimate.SetActive(false);
            standingAnimate.SetActive(true);
            return;
        }

        if (currentTime >= nextCmd.time)
        {
            currentIndex++;
            jumped = false;
            return;
        }

        // Interpolate position based on time between current and next command
        float t = Mathf.InverseLerp(cmd.time, nextCmd.time, currentTime);
        transform.position = Vector2.Lerp(cmd.pos, nextCmd.pos, t);

        // Handle animations
        if (cmd.moving)
        {
            walkingAnimate.SetActive(true);
            standingAnimate.SetActive(false);
        }
        else
        {
            walkingAnimate.SetActive(false);
            standingAnimate.SetActive(true);
        }

        // Handle jump once
        if (cmd.jumping && !jumped)
        {
            jumped = true;
            body.isKinematic = false;
            body.linearVelocity = new Vector2(body.linearVelocity.x, 0f); // Reset Y velocity
            body.AddForce(new Vector2(0f, 400f)); // Use your player�s actual jump force
        }

        // Handle sprite flipping
        bool ghostIsFacingRight = transform.localScale.x > 0;
        if (ghostIsFacingRight != cmd.facingRight)
            Flip();
    }

    private void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }


    [ContextMenu("Debug: Print Commands")]
    public void DebugCommands()
    {
        if (replayCommands == null || replayCommands.Count == 0)
        {
            Debug.Log("GhostPlayer has no commands.");
            return;
        }

        Debug.Log($"GhostPlayer has {replayCommands.Count} commands:");

        for (int i = 0; i < replayCommands.Count; i++)
        {
            Commands cmd = replayCommands[i];
            string msg = $"[{i}] Time: {cmd.time:F2}, Pos: {cmd.pos}, " +
                         $"Moving: {cmd.moving}, Jumping: {cmd.jumping}, " +
                         $"FacingRight: {cmd.facingRight}";
            Debug.Log(msg);
        }
    }
}
/*
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
*/