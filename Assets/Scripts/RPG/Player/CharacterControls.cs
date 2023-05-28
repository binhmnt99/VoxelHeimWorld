using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Voxel
{
    public class CharacterControls : MonoBehaviour
    {
        private Animator animator;

        private float movementSpeed;
        private float desiredMovementSpeed;
        private float lastMoveSpeed;

        private ThirdPersonCamera thirdcamera;
        //private GameObject crosshair;
        public GameObject weapon;

        [Header("Movement")]
        [SerializeField]
        private float walkSpeed = 5;
        [SerializeField]
        private float airMinSpeed;
        [SerializeField]
        private float speedIncreaseMultiplier;

        [Header("Jumping")]
        [SerializeField]
        private float jumpForce;
        [SerializeField]
        private float jumpCooldown;
        [SerializeField]
        private float airMultiplier;
        [SerializeField]
        private bool readyToJump;
        private bool isJumpKeyPress;

        [Header("Ground Check")]
        [SerializeField]
        private bool grounded;
        private CharacterFootDetected characterFootDetected;
        private bool detectWall;
        private CharacterWallDetected characterWallDetected;

        public Transform orientation;

        public Vector2 character2DInput { get; private set; }
        private Vector3 characterDirection;

        private Rigidbody rb;

        public CharacterInputControl characterInput { get; private set; }

        private bool freeze;
        private bool walking = true;
        private bool combat = false;

        public MovementState state;

        public enum MovementState
        {
            freeze,
            walking,
            air
        }

        private void Awake()
        {
            thirdcamera = Camera.main.GetComponent<ThirdPersonCamera>();
            // crosshair = GameObject.Find("PlayerCanvas").transform.GetChild(1).gameObject;
            animator = GetComponentInChildren<Animator>();
            characterInput = new CharacterInputControl();
            rb = GetComponent<Rigidbody>();
            rb.freezeRotation = true;
            readyToJump = true;
            characterFootDetected = GetComponentInChildren<CharacterFootDetected>();
            characterWallDetected = GetComponentInChildren<CharacterWallDetected>();
            characterInput.Player.Enable();
            MoveInput();
            JumpInput();
            CombatModeInput();
            AttackInput();
            PrimarySkillInput();
            SecondarySkillInput();
        }

        private void SecondarySkillInput()
        {
            characterInput.Player.SecondarySkill.started += SecondarySkillInput;
            characterInput.Player.SecondarySkill.performed += SecondarySkillInput;
            characterInput.Player.SecondarySkill.canceled += SecondarySkillInput;
        }

        private void SecondarySkillInput(InputAction.CallbackContext context)
        {
            if (combat)
            {
                if (context.started)
                {
                    animator.SetTrigger("primaryskill");
                }
                if (context.performed)
                {

                }
                if (context.canceled)
                {

                }
            }
        }

        private void PrimarySkillInput()
        {
            characterInput.Player.PrimarySkill.started += PrimarySkillInput;
            characterInput.Player.PrimarySkill.performed += PrimarySkillInput;
            characterInput.Player.PrimarySkill.canceled += PrimarySkillInput;
        }

        private void PrimarySkillInput(InputAction.CallbackContext context)
        {
            if (combat)
            {
                if (context.started)
                {
                    animator.SetTrigger("secondaryskill");
                }
                if (context.performed)
                {

                }
                if (context.canceled)
                {

                }
            }

        }

        private void MoveInput()
        {
            characterInput.Player.Move.started += Move;
            characterInput.Player.Move.performed += Move;
            characterInput.Player.Move.canceled += Move;
        }

        private void Move(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                walking = true;
            }
            if (context.performed)
            {
                character2DInput = context.ReadValue<Vector2>();
                if (grounded)
                {
                    animator.SetFloat("speed", character2DInput.magnitude);
                }

            }
            if (context.canceled)
            {
                character2DInput = Vector2.zero;
                animator.SetFloat("speed", 0);
                walking = false;
            }
        }

        private IEnumerator SmoothlyLerpMoveSpeed()
        {
            // smoothly lerp movementSpeed to desired value
            float time = 0;
            float difference = Mathf.Abs(desiredMovementSpeed - movementSpeed);
            float startValue = movementSpeed;

            while (time < difference)
            {
                movementSpeed = Mathf.Lerp(startValue, desiredMovementSpeed, time / difference);
                time += Time.deltaTime * speedIncreaseMultiplier;

                yield return null;
            }

            movementSpeed = desiredMovementSpeed;
        }

        bool keepMomentum;
        private void StateHandler()
        {
            if (freeze)
            {
                state = MovementState.freeze;
                rb.velocity = Vector3.zero;
                desiredMovementSpeed = 0f;
            }
            else if (grounded)
            {
                state = MovementState.walking;
                desiredMovementSpeed = walkSpeed;
            }

            // Mode - Air
            else
            {
                state = MovementState.air;
                if (movementSpeed < airMinSpeed)
                    desiredMovementSpeed = airMinSpeed;
            }

            bool desiredMoveSpeedHasChanged = desiredMovementSpeed != lastMoveSpeed;

            if (desiredMoveSpeedHasChanged)
            {
                if (keepMomentum)
                {
                    StopAllCoroutines();
                    StartCoroutine(SmoothlyLerpMoveSpeed());
                }
                else
                {
                    movementSpeed = desiredMovementSpeed;
                }
            }

            lastMoveSpeed = desiredMovementSpeed;

            // deactivate keepMomentum
            if (Mathf.Abs(desiredMovementSpeed - movementSpeed) < 0.1f) keepMomentum = false;
        }

        private void MovePlayer()
        {
            characterDirection = orientation.forward * character2DInput.y + orientation.right * character2DInput.x;
            if (grounded)
            {
                rb.AddForce(characterDirection.normalized * movementSpeed * 10f, ForceMode.Force);
            }
            // in air
            else if (!grounded)
            {
                rb.AddForce(characterDirection.normalized + (-transform.up * 0.1f) * movementSpeed * 10f * airMultiplier, ForceMode.Force);
            }

            if (characterWallDetected.isWall && walking)
            {
                //rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

                rb.AddForce(transform.up * 1.2f, ForceMode.Impulse);
            }

        }

        private void SpeedControl()
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            // limit velocity if needed
            if (flatVel.magnitude > movementSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * movementSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }


        private void JumpInput()
        {
            characterInput.Player.Jump.started += Jump;
            characterInput.Player.Jump.performed += Jump;
            characterInput.Player.Jump.canceled += Jump;
        }

        private void Jump(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                isJumpKeyPress = true;

                animator.SetTrigger("jump");
            }
            if (context.performed)
            {
            }
            if (context.canceled)
            {
                isJumpKeyPress = false;
            }
        }

        private void JumpCheck()
        {
            if (isJumpKeyPress && readyToJump && grounded)
            {
                readyToJump = false;

                Jumping();

                Invoke(nameof(ResetJump), jumpCooldown);
            }
        }

        private void ResetJump()
        {
            readyToJump = true;
        }

        private void Jumping()
        {
            // reset y velocity
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }

        private void CombatModeInput()
        {
            characterInput.Player.DrawWeapon.started += CombatModeInput;
            characterInput.Player.DrawWeapon.performed += CombatModeInput;
            characterInput.Player.DrawWeapon.canceled += CombatModeInput;
        }

        private void CombatModeInput(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                combat = !combat;
                if (combat)
                {
                    thirdcamera.SwitchCameraStyle(ThirdPersonCamera.CameraStyle.Combat);
                    weapon.SetActive(true);
                    //crosshair.SetActive(true);
                }
                else
                {
                    thirdcamera.SwitchCameraStyle(ThirdPersonCamera.CameraStyle.Basic);
                    weapon.SetActive(false);
                    //crosshair.SetActive(false);
                }
            }
            if (context.performed)
            {

            }
            if (context.canceled)
            {

            }
        }

        private void AttackInput()
        {
            characterInput.Player.Attack.started += AttackInput;
            characterInput.Player.Attack.performed += AttackInput;
            characterInput.Player.Attack.canceled += AttackInput;
        }

        private void AttackInput(InputAction.CallbackContext context)
        {
            if (combat)
            {
                if (context.started)
                {
                    animator.SetTrigger("attack");
                }
                if (context.performed)
                {

                }
                if (context.canceled)
                {

                }
            }

        }

        private void Update()
        {

            grounded = characterFootDetected.isGrounded;

            JumpCheck();
            SpeedControl();
            StateHandler();
        }

        private void FixedUpdate()
        {
            MovePlayer();
        }

        private void OnDrawGizmos()
        {

        }
    }

}
