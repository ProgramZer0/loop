using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpHeight = 400f;
    [SerializeField] private float interactRange = 2f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask interactLayer;
    [SerializeField] private GameManger GM;
    [SerializeField] private GameObject standingAnimate;
    [SerializeField] private GameObject walkingAnimate;
    [SerializeField] private GameObject ghostPrefab;

    private Rigidbody2D body;
    private bool inAir = false;
    private bool jumping = false;
    private bool interact = false;
    private bool timeFutureTravel = false;
    private bool timePastTravel = false;
    private bool quickReset = false;
    private bool isRight = true;
    private float inputH = 0f;
    private Vector3 zero = Vector3.zero;

    private List<Commands> currentCommands;

    // Track previous state for optimized recording
    private bool lastMoving = false;
    private bool lastJumping = false;
    private bool lastFacingRight = true;
    private bool cannotMove = true;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        currentCommands = new List<Commands>();

        currentCommands.Add(new Commands(GM.getTimerSeconds(), false, false, isRight, false, transform.position, null));
    }

    private void Update()
    {
        if (currentCommands.Count == 0)
        {
            currentCommands.Add(new Commands(GM.getTimerSeconds(), false, false, isRight, false, transform.position, null));
        }

        inputH = Input.GetAxis("Horizontal") * speed;

        if (Input.GetKeyDown(KeyCode.Mouse0)) interact = true;
        if (Input.GetKeyDown(KeyCode.Space)) jumping = true;
        if (Input.GetKeyDown(KeyCode.E)) timeFutureTravel = true;
        if (Input.GetKeyDown(KeyCode.R)) timePastTravel = true;
        if (Input.GetKeyDown(KeyCode.T)) quickReset = true;
    }

    private void FixedUpdate()
    {
        inAir = !Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);

        float moveAmount = inputH * Time.fixedDeltaTime;
        bool moving = inputH != 0f;
        if (cannotMove) return;

        if (interact)
        {
            TryInteract();
        }

        Move(moveAmount, false, jumping);

        if (timeFutureTravel)
        {
            TravelFuture();
            timeFutureTravel = false;
        }

        if (timePastTravel)
        {
            TravelPast();
            timePastTravel = false;
        }

        if (quickReset)
        {
            Reset();
            quickReset = false;
        }
    }

    internal void lockMovement(bool val)
    {
        cannotMove = val;
    }

    private void TryInteract()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, interactRange, interactLayer);
        if (hit != null)
        {
            Lever lever = hit.GetComponent<Lever>();
            if (lever != null)
            {
                Debug.Log("toggling");
                lever.Toggle();
            }
        }
    }

    private void Move(float move, bool crouch, bool jump)
    {
        bool moving = move != 0f;

        // Animations
        standingAnimate.SetActive(!moving);
        walkingAnimate.SetActive(moving);

        TryRecordCommand(moving, jump);

        Vector3 targetVelocity = new Vector2(move * 10f, body.linearVelocity.y);
        body.linearVelocity = Vector3.SmoothDamp(body.linearVelocity, targetVelocity, ref zero, 0.05f);

        // Flip character
        if (move > 0 && !isRight)
        {
            Flip();
        }
        else if (move < 0 && isRight)
        {
            Flip();
        }

        if (!inAir && jump)
        {
            Debug.Log("jumping");
            inAir = true;
            body.AddForce(new Vector2(0f, jumpHeight));
            jumping = false;
        }
    }

    private void TryRecordCommand(bool moving, bool jump)
    {
        bool hasChanged = moving != lastMoving || jump != lastJumping || isRight != lastFacingRight || interact;

        if (hasChanged)
        {
            Commands lastCmd = currentCommands.Count > 0 ? currentCommands[currentCommands.Count - 1] : null;
            Commands newCmd = new Commands(GM.getTimerSeconds(), moving, jump, isRight, interact, transform.position, lastCmd);
            if (lastCmd != null) lastCmd.setNext(newCmd);
            currentCommands.Add(newCmd);
        }

        lastMoving = moving;
        lastJumping = jump;
        lastFacingRight = isRight;
        interact = false;
    }

    private void Flip()
    {
        isRight = !isRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void TravelPast()
    {
        //Debug.Log("travling");

        if (currentCommands.Count == 0) return;

        TryRecordCommand(false, false);

        GameObject ghost = Instantiate(ghostPrefab, currentCommands[0].pos, Quaternion.identity);
        ghost.GetComponent<GhostPlayer>().Initialize(currentCommands, GM, GM.getTimerSeconds() - 5f);

        

        if (GM.getTimerSeconds() >= 5f)
        {
            GM.setTimer(GM.getTimerSeconds() - 5f);
            findandDeleteCmdsPast(GM.getTimerSeconds());
        }
        else
        {
            currentCommands.Clear();
            GM.resetTimer();
        }
    }

    private void findandDeleteCmdsPast(float time)
    {
        currentCommands.RemoveAll(cmd => cmd.time < time);
        if (currentCommands.Count > 0)
            currentCommands[0].lastCommand = null;
    }

    private void TravelFuture()
    {
        GM.setTimer(GM.getTimerSeconds() + 5f);
    }

    private void Reset()
    {
        foreach (GameObject ghostn in GameObject.FindGameObjectsWithTag("Ghost"))
        {
            Destroy(ghostn);
        }

        GM.resetTimer();
        currentCommands.Clear();
    }
}