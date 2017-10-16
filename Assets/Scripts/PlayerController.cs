using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Helpers;

public class PlayerController : MonoBehaviour
{
	[SerializeField]
	Rigidbody2D _body;
	[SerializeField]
	Collider2D _collider;

	// settings
	private float _distToGround;

	// adjustments
	public float _groundMaxVelocity = 5f;
	public float _groundAcceleration = 5f;
	public float _airMaxVelocity = 5f;
	public float _airAcceleration = 2f;
	public float _jumpSpeed = 5f;
	public float _jumpTimeMax = 1f;

	// inputs
	private float _horInput;
	private float _vertInput;
	private bool _jumpInput;

	// state
	private bool _isJumpDone;
	private bool _isJumpStarted;
	private float _jumpTime;
	private bool _isGrounded;

	void Start()
	{
		_distToGround = 0f; //_collider.bounds.extents.y;
	}

	public void SetHorInput(float horInput)
	{
		_horInput = horInput;
	}

	public void SetVertInput(float vertInput)
	{
		_vertInput = vertInput;
	}

	public void SetJumpInput(bool jumpInput)
	{
		_jumpInput = jumpInput;		
	}

	private void FixedUpdate()
	{
		UpdateGrounded();
		UpdateJumping();
		UpdateMoving();
	}

	private void UpdateMoving()
	{
		var velocity = _body.velocity.x;
		var inputAcceleration = _horInput * (_isGrounded ? _groundAcceleration : _airAcceleration);

		bool isStoping = _horInput == 0;
		if (isStoping)
			inputAcceleration = Mathf.Sign(velocity) * -1f * (_isGrounded ? _groundAcceleration : _airAcceleration);

		var groundMaxVelocity = _groundMaxVelocity * Mathf.Abs(_horInput);
		var airMaxVelocity = _airMaxVelocity * Mathf.Abs(_horInput);

		var dt = Time.fixedDeltaTime;
		var newVelocity = velocity + inputAcceleration * dt;
		if (Mathf.Abs(newVelocity) > Mathf.Abs(velocity))
		{
			if (_isGrounded)
			{
				newVelocity = Mathf.Sign(newVelocity) * Mathf.Min(Mathf.Abs(newVelocity), groundMaxVelocity);
			}
			else
			{
				if (Mathf.Abs(newVelocity) > _airMaxVelocity)
				{
					newVelocity = Mathf.Sign(newVelocity) * Mathf.Max(Mathf.Abs(velocity), airMaxVelocity);
				}
			}
		}

		if ((Mathf.Sign(velocity) != Mathf.Sign(newVelocity) || velocity == 0) && isStoping)
			newVelocity = 0;

		_body.velocity = new Vector2(newVelocity, _body.velocity.y);
	}

	private void UpdateJumping()
	{
		if (_jumpInput && !_isJumpStarted && !_isJumpDone)
		{
			_isJumpStarted = true;
			if (!_isGrounded)
				_isJumpDone = true;
		}

		if (!_isJumpStarted)
			return;

		if (!_jumpInput && !_isJumpDone)
			_isJumpDone = true;

		var shouldAccelerate = _jumpTime < _jumpTimeMax && !_isJumpDone;
		if (shouldAccelerate)
		{
			_body.velocity = new Vector2(_body.velocity.x, _jumpSpeed);
		}
		else if (_body.velocity.y > 0)
		{
			_body.velocity = new Vector2(_body.velocity.x, 0);
		}

		_jumpTime += Time.fixedDeltaTime;
	}

	private void UpdateGrounded()
	{
		var hit = Physics2D.Raycast(transform.position, -Vector2.up, 0.1f, 1 << LayerMaskEx.Obstacle);
		_isGrounded = hit.collider != null;

		if (_isGrounded && !_jumpInput && _isJumpStarted)
		{
			_isJumpStarted = false;
			_isJumpDone = false;
			_jumpTime = 0;
		}
	}
}
