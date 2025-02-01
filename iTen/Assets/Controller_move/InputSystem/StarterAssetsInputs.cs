using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{

	public class StarterAssetsInputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;
		public bool crouch = false;
		public bool flash = false;

		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;

#if ENABLE_INPUT_SYSTEM
		public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			if(cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}

		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}
		public void OnCrouch(InputValue value)
		{
			if (value.isPressed)
                CrouchInput();
		}

		// 손전등 기능 추가
        public void OnFlash(InputValue value)
        {
            if (value.isPressed)
                FlashInput();
        }
#endif


        public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		} 

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}

		public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}
		
		public void CrouchInput()
		{
			crouch = crouch == true ? false : true;
		}

        public void FlashInput()
        {
            flash = flash == true ? false : true;

            if (flash)
            {
                Debug.Log("손전등 켰다.");
            }
            else
            {
                Debug.Log("손전등 껐다");
            }

            if (FlashlightManager.Instance != null)
            {
                FlashlightManager.Instance.SetFlashlightState(flash);
            }
            else
            {
                Debug.LogWarning("FlashlightManager 인스턴스가 존재하지 않습니다.");
            }
        }

        private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}
	}
	
}