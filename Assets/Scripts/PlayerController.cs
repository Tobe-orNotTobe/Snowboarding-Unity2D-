using UnityEngine;

public class PlayerController : MonoBehaviour
{
	[Header("Điều khiển cơ bản")]
	[SerializeField] private float torqueAmount = 1f;
	[SerializeField] private float baseSpeed = 30f;
	[SerializeField] private float boostSpeed = 20f;

	[Header("Tốc độ")]
	[SerializeField] private float skiLineSpeedFactor = 0.8f;
	[SerializeField] private float groundSpeedFactor = 0.7f;

	[Header("Nhảy")]
	[SerializeField] private float jumpForce = 20f;
	[SerializeField] private float jumpCooldown = 0.5f;

	[Header("Hiệu ứng")]
	[SerializeField] private ParticleSystem snowEffect;
	[SerializeField] public AudioSource slidingAudio;
	[SerializeField] public AudioSource skiLineAudio;   

	private bool isFlipping = false; 
	private bool flipCompleted = false; 
	private float totalRotation = 0f; 
	private float lastFrameRotation = 0f; 
	private int flipPoints = 3; 

	private Rigidbody2D rb2d;
	private bool isGrounded;
	private bool canJump = true;
	private bool isOnSkiLine = false;
	private SkiLine currentSkiLine;
	private Vector2 surfaceNormal;

	void Start()
	{
		rb2d = GetComponent<Rigidbody2D>();
		InitializeAudio();
		lastFrameRotation = transform.eulerAngles.z;
	}

	void Update()
	{
		if (isOnSkiLine)
		{
			if (!skiLineAudio.isPlaying)
			{
				slidingAudio.Stop();    
				skiLineAudio.Play();    
			}
		}
		else
		{
			if (!slidingAudio.isPlaying)
			{
				skiLineAudio.Stop();   
				slidingAudio.Play();    
			}
		}
		HandleInputs();
		UpdateEffects();
		CheckFlip();

	}

	void FixedUpdate()
	{
		if (isGrounded)
		{
			ApplyMovement();
		}
	}

	private void InitializeAudio()
	{
		if (slidingAudio != null)
		{
			slidingAudio.Play();
			slidingAudio.Pause();
		}
	}

	private void HandleInputs()
	{
		if (Input.GetKey(KeyCode.LeftArrow))
		{
			rb2d.AddTorque(torqueAmount);
		}
		else if (Input.GetKey(KeyCode.RightArrow))
		{
			rb2d.AddTorque(-torqueAmount);
		}

		if (Input.GetKeyDown(KeyCode.Space) && canJump && isGrounded)
		{
			Jump();
		}
	}

	private void CheckFlip()
	{
		if (!isGrounded)
		{
			float currentRotation = transform.eulerAngles.z;

			float deltaRotation = Mathf.DeltaAngle(lastFrameRotation, currentRotation);

			totalRotation += Mathf.Abs(deltaRotation);

			if (!isFlipping && totalRotation > 45f)
			{
				isFlipping = true;
				flipCompleted = false;
			}

			if (isFlipping && !flipCompleted && totalRotation >= 340f)
			{
				CompletedFlip();
			}

			lastFrameRotation = currentRotation;
		}
		else if (isFlipping)
		{
			isFlipping = false;
			totalRotation = 0f;
		}
	}

	private void CompletedFlip()
	{
		flipCompleted = true;

		if (ScoreManager.Instance != null)
		{
			ScoreManager.Instance.AddPoints(flipPoints);

			Debug.Log("Flip completed! +" + flipPoints + " points");
		}


	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.contacts.Length == 0) return;

		Vector2 contactNormal = collision.contacts[0].normal;

		bool isFromAbove = IsCollisionFromAbove(contactNormal);

		if (collision.gameObject.CompareTag("SkiLine"))
		{
			HandleSkiLineCollision(collision, contactNormal, isFromAbove);
			if (isFromAbove && !IsBoardFlipped())
			{
				justLanded = true;
				landingTime = Time.time;
				isOnSkiLine = true;
			}
		}
		else if (IsGround(collision))
		{
			HandleGroundCollision(collision, contactNormal);
			justLanded = true;
			landingTime = Time.time;
		}

		if (isFlipping)
		{
			isFlipping = false;
			totalRotation = 0f;
		}
	}

	void OnCollisionExit2D(Collision2D collision)
	{
		if (collision.gameObject.CompareTag("SkiLine"))
		{
			ExitSkiLine();
		}
		else if (IsGround(collision))
		{
			ExitGround();
		}
	}

	private bool IsCollisionFromAbove(Vector2 contactNormal)
	{
		return Vector2.Dot(contactNormal, Vector2.up) > 0.5f;
	}

	private bool IsGround(Collision2D collision)
	{
		return collision.gameObject.CompareTag("Ground");
	}

	private void HandleSkiLineCollision(Collision2D collision, Vector2 contactNormal, bool isFromAbove)
	{
		
		if (isFromAbove && !IsBoardFlipped())
		{
			EnterSkiLine(collision, contactNormal);
		}
		else
		{
			IgnoreCollision(collision);
		}
	}

	private void HandleGroundCollision(Collision2D collision, Vector2 contactNormal)
	{
		isGrounded = true;

		if (!isOnSkiLine)
		{
			surfaceNormal = contactNormal;
		}
	}

	private void EnterSkiLine(Collision2D collision, Vector2 contactNormal)
	{
		isGrounded = true;
		isOnSkiLine = true;
		surfaceNormal = contactNormal;
		currentSkiLine = collision.gameObject.GetComponent<SkiLine>();
	}

	private void ExitSkiLine()
	{
		isOnSkiLine = false;
		currentSkiLine = null;

		if (!isGrounded)
		{
			justLanded = false; 
		}
	}

	private void ExitGround()
	{
		if (!isOnSkiLine)
		{
			isGrounded = false;
			justLanded = false; 
		}
	}

	private void IgnoreCollision(Collision2D collision)
	{
		Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>(), true);

		Invoke(nameof(RestoreCollision), 0.5f);
	}

	private void RestoreCollision()
	{
		Collider2D[] colliders = FindObjectsOfType<Collider2D>();
		Collider2D playerCollider = GetComponent<Collider2D>();

		foreach (Collider2D collider in colliders)
		{
			if (collider.CompareTag("SkiLine"))
			{
				Physics2D.IgnoreCollision(collider, playerCollider, false);
			}
		}
	}

	private bool IsBoardFlipped()
	{
		float zRotation = transform.eulerAngles.z;

		if (zRotation < 0)
			zRotation += 360f;
		else if (zRotation > 360f)
			zRotation -= 360f;


		return (zRotation > 90f && zRotation < 270f);
	}

	private bool justLanded = false; 
	private float landingTime = 0f; 
	private float boostDuration = 0.5f; 

	private void ApplyMovement()
	{
		Vector2 moveDirection = GetMoveDirection();
		float speedMultiplier = GetSpeedMultiplier();
		float maxSpeed = isOnSkiLine ? 35f : 50f;

		rb2d.AddForce(moveDirection * baseSpeed * speedMultiplier, ForceMode2D.Force);

		if (justLanded && Time.time - landingTime < boostDuration &&
			rb2d.linearVelocity.magnitude < baseSpeed * 0.8f)
		{
			BoostSpeed(moveDirection);
		}

		LimitSpeed(maxSpeed);
	}

	private Vector2 GetMoveDirection()
	{
		return new Vector2(surfaceNormal.y, -surfaceNormal.x).normalized;
	}

	private float GetSpeedMultiplier()
	{
		float slopeAngle = Vector2.Angle(surfaceNormal, Vector2.up);
		float baseMultiplier = 1f + (slopeAngle / 180f);
		float surfaceFactor = isOnSkiLine ? skiLineSpeedFactor : groundSpeedFactor;

		if (isOnSkiLine && currentSkiLine != null)
		{
			surfaceFactor *= currentSkiLine.GetFrictionAt(transform.position);
		}

		return baseMultiplier * surfaceFactor;
	}

	private void BoostSpeed(Vector2 direction)
	{
		float boostAmount = isOnSkiLine ? boostSpeed * 1.2f : boostSpeed;
		rb2d.AddForce(direction * boostAmount, ForceMode2D.Impulse);
	}

	private void LimitSpeed(float maxSpeed)
	{
		if (rb2d.linearVelocity.magnitude > maxSpeed)
		{
			rb2d.linearVelocity = rb2d.linearVelocity.normalized * maxSpeed;
		}
	}

	private void Jump()
	{
		if (isGrounded)
		{
			rb2d.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
			canJump = false;
			Invoke(nameof(ResetJump), jumpCooldown);

			isFlipping = false;
			totalRotation = 0f;
			lastFrameRotation = transform.eulerAngles.z;
		}
	
	}

	private void ResetJump()
	{
		canJump = true;
	}

	private void UpdateEffects()
	{
		if (snowEffect != null)
		{
			if (isGrounded)
			{
				PlaySnowEffect();
			}
			else
			{
				StopSnowEffect();
			}
		}

		if (slidingAudio != null)
		{
			if (isGrounded)
			{
				ResumeslidingAudio();
			}
			else
			{
				PauseslidingAudio();
			}
		}
	}

	private void PlaySnowEffect()
	{
		if (!snowEffect.isPlaying) snowEffect.Play();
	}

	private void StopSnowEffect()
	{
		if (snowEffect.isPlaying) snowEffect.Stop();
	}

	private void ResumeslidingAudio()
	{
		if (!slidingAudio.isPlaying) slidingAudio.UnPause();
		UpdateslidingAudio();
	}

	private void PauseslidingAudio()
	{
		if (slidingAudio.isPlaying) slidingAudio.Pause();
	}

	private void UpdateslidingAudio()
	{
		float speedRatio = rb2d.linearVelocity.magnitude / baseSpeed;
		slidingAudio.pitch = isOnSkiLine
			? Mathf.Lerp(1.0f, 1.5f, speedRatio)
			: Mathf.Lerp(0.8f, 1.2f, speedRatio);

		float slopeAngle = Vector2.Angle(surfaceNormal, Vector2.up);
		slidingAudio.volume = Mathf.Lerp(0.2f, 1.0f, Mathf.Clamp01(slopeAngle / 45f));
	}
}