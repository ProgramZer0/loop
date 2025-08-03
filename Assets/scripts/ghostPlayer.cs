using System.Collections.Generic;
using UnityEngine;

public class GhostPlayer : MonoBehaviour
{
    [SerializeField] private GameObject standingAnimate;
    [SerializeField] private GameObject walkingAnimate;
    [SerializeField] private GameManger GM;
    [SerializeField] private GameObject ghostTotal;

    private Rigidbody2D body;
    private List<Commands> replayCommands;
    private float startTimeOffset = 0f;
    private Vector3 zero = Vector3.zero;
    private bool jumped = false;

    public void Initialize(List<Commands> commands, GameManger gameManager, float spawnTime)
    {
        replayCommands = new List<Commands>(commands);
        GM = gameManager;
        startTimeOffset = spawnTime;
    }

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        //if spawn time is bigger than the current time, then this object should disapear.
        if (startTimeOffset > GM.getTimerSeconds())
        {
            ghostTotal.SetActive(false);
            return;
        }
        else
            ghostTotal.SetActive(true);


        if (replayCommands.Count == 0) return;

        Commands cmd = null;
        Commands next = null;
        for (int i = 0; i < replayCommands.Count - 1; i++)
        {
            if (replayCommands[i].time <= GM.getTimerSeconds() && replayCommands[i + 1].time > GM.getTimerSeconds())
            {
                cmd = replayCommands[i];
                next = replayCommands[i + 1];
                break;
            }
        }

        if (cmd == null)
        {
            if (GM.getTimerSeconds() >= replayCommands[replayCommands.Count - 1].time)
            {
                cmd = replayCommands[replayCommands.Count - 1];
                next = cmd;
            }
            else
            {
                standingAnimate.SetActive(true);
                walkingAnimate.SetActive(false);
                return;
            }
        }

        if (next != null && next != cmd)
        {
            float t = Mathf.InverseLerp(cmd.time, next.time, GM.getTimerSeconds());
            transform.position = Vector2.Lerp(cmd.pos, next.pos, t);
        }
        else
        {
            standingAnimate.SetActive(true);
            walkingAnimate.SetActive(false);
            transform.position = cmd.pos;
        }

        if (cmd.facingRight && cmd.moving)
        {
            standingAnimate.SetActive(false);
            walkingAnimate.SetActive(true);
        }
        else if (!cmd.facingRight && cmd.moving)
        {
            standingAnimate.SetActive(false);
            walkingAnimate.SetActive(true);
        }
        else
        {
            standingAnimate.SetActive(true);
            walkingAnimate.SetActive(false);
        }

        int move = 0;
        if (cmd.moving)
            move = cmd.facingRight ? 1 : -1;
        if (cmd.jumping && !jumped)
        {
            jumped = true;
            body.AddForce(new Vector2(0f, 400f));
        }

        if (cmd.facingRight != transform.localScale.x > 0)
            Flip();

        if (!cmd.moving) return;
        Vector3 targetVelocity = new Vector2(move * 10f, body.linearVelocity.y);
        body.linearVelocity = Vector3.SmoothDamp(body.linearVelocity, targetVelocity, ref zero, 0.05f);

        
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