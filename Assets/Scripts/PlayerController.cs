using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Helpers;

public class PlayerController : MonoBehaviour
{
	[SerializeField]
	Transform _leftLeg;
	[SerializeField]
	Transform _rightLeg;

	// settings
	private float _distToGround;

	// adjustments
	public float _groundMaxVelocity = 5f;
	public float _groundAcceleration = 5f;
	public float _airMaxVelocity = 5f;
	public float _airAcceleration = 2f;

	public float _jumpSpeedMax = 5f;
	public float _jumpGravity = 5f;
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
	private Vector2 _velocity;

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
		UpdatePosition();
	}

	private void UpdatePosition()
	{
		var oldPos = transform.position;
		var vel = _velocity;
		var dt = Time.fixedDeltaTime;

		var deltaX = vel.x * dt;
		var deltaY = vel.y * dt;

		if (deltaY < 0)
			deltaY = -HitGround(-deltaY);
		transform.position = new Vector2(oldPos.x + deltaX, oldPos.y + deltaY);
	}

	private void UpdateMoving()
	{
		var velocity = _velocity.x;
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

		_velocity = new Vector2(newVelocity, _velocity.y);
	}

	private void UpdateJumping()
	{
		if (_jumpInput && !_isJumpStarted && !_isJumpDone)
		{
			_isJumpStarted = true;
			if (!_isGrounded)
				_isJumpDone = true;
		}

		var dt = Time.fixedDeltaTime;
		if (!_isJumpStarted)
		{
			var speed = Mathf.Max(_velocity.y - _jumpGravity * dt * dt, -_jumpSpeedMax);
			if (_isGrounded)
				speed = 0;
			_velocity = new Vector2(_velocity.x, speed);
			return;
		}

		if (!_jumpInput && !_isJumpDone)
			_isJumpDone = true;

		var shouldAccelerate = _jumpTime < _jumpTimeMax && !_isJumpDone;
		if (shouldAccelerate)
		{
			_velocity = new Vector2(_velocity.x, _jumpSpeedMax);
		}
		else
		{
			var speed = Mathf.Max(_velocity.y - _jumpGravity * dt * dt, -_jumpSpeedMax);
			if (_isGrounded)
				speed = 0;
			_velocity = new Vector2(_velocity.x, speed);
		}

		_jumpTime += Time.fixedDeltaTime;
	}

	private float HitGround(float offset)
	{
		var hitLeft = Physics2D.Raycast(_leftLeg.position, -Vector2.up, offset, 1 << LayerMaskEx.Obstacle);
		var hitRight = Physics2D.Raycast(_rightLeg.position, -Vector2.up, offset, 1 << LayerMaskEx.Obstacle);

		var newOffset = offset;
		if (hitLeft.collider != null)
			newOffset = Mathf.Min(newOffset, _leftLeg.position.y - hitLeft.point.y);
		if (hitRight.collider != null)
			newOffset = Mathf.Min(newOffset, _rightLeg.position.y - hitRight.point.y);

		return newOffset;
	}

	private void UpdateGrounded()
	{
		float hitPos;
		_isGrounded = HitGround(0.001f) < 0.001f;

		if (_isGrounded && !_jumpInput && _isJumpStarted)
		{
			_isJumpStarted = false;
			_isJumpDone = false;
			_jumpTime = 0;
		}
	}
}
