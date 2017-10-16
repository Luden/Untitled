using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerInput : MonoBehaviour
{
	[SerializeField]
	private PlayerController player;

    private void Update()
    {
        player.SetHorInput(Input.GetAxisRaw("Horizontal"));
		player.SetVertInput(Input.GetAxisRaw("Vertical"));

		if (Input.GetButtonDown("Jump"))
        {
            player.SetJumpInput(true);
        }

        if (Input.GetButtonUp("Jump"))
        {
			player.SetJumpInput(false);
		}
    }
}
