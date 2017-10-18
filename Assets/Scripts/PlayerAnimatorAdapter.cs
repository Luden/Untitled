using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorAdapter : MonoBehaviour
{
	[SerializeField]
	Rigidbody2D _body;
	[SerializeField]
	Animator _animator;
	[SerializeField]
	PlayerController _player;

	int _idleId = Animator.StringToHash("Idle");
	int _speedYId = Animator.StringToHash("SpeedY");
	int _groundedId = Animator.StringToHash("Grounded");

	void FixedUpdate()
	{
		var speed = _body.velocity;

		_animator.SetBool(_idleId, speed == Vector2.zero);
		_animator.SetBool(_groundedId, _player.IsGrounded);
		_animator.SetFloat(_speedYId, speed.y);
	}
}
