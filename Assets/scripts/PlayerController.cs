using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
	[SerializeField] private float speed = 5f;
	[SerializeField] private float jumpHeght = 400f;
	[SerializeField] private Transform groundCheck;
	[SerializeField] private LayerMask groundLayer;
	[SerializeField] private GameManger GM;


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


	}


	public void Move(float move, bool crouch, bool jump)
	{
        if (move == 0)
        {
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
				currentCommands.Add(new Commands(GM.getTimerSeconds(), false, jump, isRight, transform.position));
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
					currentCommands.Add(new Commands(GM.getTimerSeconds(), false, jump, isRight, transform.position, currentCommands[currentCommands.Count - 1]));
				}
			}
			
		}
		else
        {
			bool wasMoving = false;
			bool wasJumping = false;

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
				currentCommands.Add(new Commands(GM.getTimerSeconds(), true, jump, isRight, transform.position, currentCommands[currentCommands.Count - 1]));
			}
		}

		Vector3 targetVelocity = new Vector2(move * 10f, body.linearVelocity.y);
		body.linearVelocity = Vector3.SmoothDamp(body.linearVelocity, targetVelocity, ref zero, 0.05f);

		//right
		if (move > 0 && !isRight)
		{
			currentCommands.Add(new Commands(GM.getTimerSeconds(), true, jump, isRight, transform.position, currentCommands[currentCommands.Count - 1]));
			Flip();
		}

		//left
		else if (move < 0 && isRight)
		{
			currentCommands.Add(new Commands(GM.getTimerSeconds(), true, jump, isRight, transform.position, currentCommands[currentCommands.Count - 1]));
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


	private void Flip()
	{
		// Switch the way the player is labelled as facing.
		isRight = !isRight;

		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
}