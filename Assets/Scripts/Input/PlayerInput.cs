using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName ="Player Input")]
public class PlayerInput : ScriptableObject, InputAcitons.IGamePlayActions
{
	public event UnityAction<Vector2> onMove =delegate{ };
	public event UnityAction onStopMove = delegate{ };
	public event UnityAction onFire = delegate { };
	public event UnityAction onStopFire = delegate { };


	InputAcitons inputActions;

	private void OnEnable()
	{
		inputActions = new InputAcitons();
		inputActions.GamePlay.SetCallbacks(this);
		
	}

	private void OnDisable()
	{
		DisableAllInput();
	}

	public void DisableAllInput() {
		inputActions.GamePlay.Disable();
	}
	public void EnableGameplayInput() {
		inputActions.GamePlay.Enable();

		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
	}
	public void OnMove(InputAction.CallbackContext context)
	{
		if (context.phase==InputActionPhase.Performed)
		{
			onMove.Invoke(context.ReadValue<Vector2>());
		}

		if (context.phase == InputActionPhase.Canceled)
		{
			onStopMove.Invoke();
		}
	}

	public void OnFire(InputAction.CallbackContext context)
	{
		if (context.phase == InputActionPhase.Performed)
		{
			onFire.Invoke();
		}
		if (context.phase == InputActionPhase.Canceled)
		{
			onStopFire.Invoke();
		}
	}
}
