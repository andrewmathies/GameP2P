using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlatformerController : PhysicsObject {

	public float maxSpeed = 7f;
	public float jumpTakeoffSpeed = 7f;

	/* private Animator anim;

	void Awake() {
		anim = GetComponent<Animator>();
	}*/

	// Use this for initialization
	void Start () {
		
	}
	
	protected override void ComputeVelocity() {
		Vector2 move = Vector2.zero;

		move.x = Input.GetAxis("Horizontal");
		
		if (Input.GetButtonDown("Jump") && grounded) {
			velocity.y = jumpTakeoffSpeed;
		} else if (Input.GetButtonUp("Jump")) {
			if (velocity.y > 0)
				velocity.y = velocity.y * 0.5f;
		}
/* 
		bool flipSprite = (spriteRenderer.flipX ? (move.x > 0.01f) : (move.x < 0.01f));
		
		if (flipSprite) {
			spriteRenderer.flipX = ! spriteRenderer.flipX;
		}

		anim.SetBool("grounded", grounded);
		
		anim.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);
		anim.SetBool("idle", velocity.x == 0);
*/

		targetVelocity = move * maxSpeed;
	}
}
