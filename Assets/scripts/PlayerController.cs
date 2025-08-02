using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpHeight = 400f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
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

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        currentCommands = new List<Commands>();

        currentCommands.Add(new Commands(GM.getTimerSeconds(), false, false, isRight, transform.position, null));
    }

    private void Update()
    {
        if (currentCommands.Count == 0)
        {
            currentCommands.Add(new Commands(GM.getTimerSeconds(), false, false, isRight, transform.position, null));
        }

        inputH = Input.GetAxis("Horizontal") * speed;

        interact = Input.GetKeyDown(KeyCode.Mouse0);
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
        bool hasChanged = moving != lastMoving || jump != lastJumping || isRight != lastFacingRight;

        if (hasChanged)
        {
            Commands lastCmd = currentCommands.Count > 0 ? currentCommands[currentCommands.Count - 1] : null;
            Commands newCmd = new Commands(GM.getTimerSeconds(), moving, jump, isRight, transform.position, lastCmd);
            if (lastCmd != null) lastCmd.setNext(newCmd);
            currentCommands.Add(newCmd);
        }

        lastMoving = moving;
        lastJumping = jump;
        lastFacingRight = isRight;
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
        Debug.Log("travling");

        if (currentCommands.Count == 0) return;

        GameObject ghost = Instantiate(ghostPrefab, currentCommands[0].pos, Quaternion.identity);
        ghost.GetComponent<GhostPlayer>().Initialize(currentCommands, GM);

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

/*
public class PlayerController : MonoBehaviour
{
	[SerializeField] private float speed = 5f;
	[SerializeField] private float jumpHeght = 400f;
	[SerializeField] private Transform groundCheck;
	[SerializeField] private LayerMask groundLayer;
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

    private void Update()
    {
		inputH = Input.GetAxis("Horizontal") * speed;

		if (Input.GetKeyDown(KeyCode.Mouse0))
			interact = true;
		else
			interact = false;

		if (Input.GetKeyDown(KeyCode.Space))
			jumping = true;
		if (Input.GetKeyDown(KeyCode.E))
			timeFutureTravel = true;
		if (Input.GetKeyDown(KeyCode.R))
			timePastTravel = true;
		if (Input.GetKeyDown(KeyCode.T))
			quickReset = true;

	}

	private void Awake()
	{
		currentCommands = new List<Commands>(); 
		body = GetComponent<Rigidbody2D>();
	}

	private void FixedUpdate()
	{
		inAir = !Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);

		Move(inputH * Time.fixedDeltaTime, false, jumping);

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

    private void Reset()
    {
		GM.resetTimer();
	}

	private void TravelPast()
    {
		if (currentCommands.Count == 0)
			return;

		GameObject ghost = Instantiate(ghostPrefab, currentCommands[0].pos, Quaternion.identity);
		ghost.GetComponent<GhostPlayer>().Initialize(currentCommands, GM);

		if (GM.getTimerSeconds() >= 5f)
		{
			GM.setTimer(GM.getTimerSeconds() - 5f);
			findandDeleteCmdsPast(GM.getTimerSeconds() - 5f);
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
		{
			currentCommands[0].lastCommand = null;
		}
	}

    private void TravelFuture()
    {
		GM.setTimer(GM.getTimerSeconds() + 5f);
    }

    public void Move(float move, bool crouch, bool jump)
	{
        if (move == 0)
        {
			standingAnimate.SetActive(true);
			walkingAnimate.SetActive(false);
			int count;

            try
            {
				count = currentCommands.Count;
			}
            catch
            {
				count = 0;

			}

			//brand new person
			if (count == 0)
            {
				AddCommand(false, jump);
			}
			//not moving but seeing if its needed
			else
            {
				bool wasMoving = false;
				try
				{
					wasMoving = currentCommands[currentCommands.Count - 1].lastCommand.moving | currentCommands[currentCommands.Count - 1].lastCommand.jumping;

				}
				catch
				{
					wasMoving = false;
				}

				if (wasMoving)
				{
					AddCommand(false, jump);
				}
			}
			
		}
		else
        {
			standingAnimate.SetActive(false);
			walkingAnimate.SetActive(true);

			bool wasMoving = false;

			try
			{
				wasMoving = currentCommands[currentCommands.Count - 1].lastCommand.moving ;
			}
			catch
			{
				wasMoving = false;
			}

			if (!wasMoving | jump)
            {
				AddCommand(true, jump);
			}
		}

		Vector3 targetVelocity = new Vector2(move * 10f, body.linearVelocity.y);
		body.linearVelocity = Vector3.SmoothDamp(body.linearVelocity, targetVelocity, ref zero, 0.05f);

		//right
		if (move > 0 && !isRight)
		{
			AddCommand(true, jump);
			Flip();
		}

		//left
		else if (move < 0 && isRight)
		{
			AddCommand(true, jump);
			Flip();
		}

		if (!inAir && jump)
		{
			Debug.Log("jumping");
			inAir = true;
			body.AddForce(new Vector2(0f, jumpHeght));
			jumping = false;
		}
	}
	private void AddCommand(bool moving, bool jumping)
    {
		Commands last = currentCommands.Count > 0 ? currentCommands[currentCommands.Count - 1] : null;
		Commands newCmd = new Commands(GM.getTimerSeconds(), moving, jumping, isRight, transform.position, last);
		if (last != null) last.setNext(newCmd);
		currentCommands.Add(newCmd);
	}
	
	private void Flip()
	{
		// Switch the way the player is labelled as facing.
		isRight = !isRight;

		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
}*/