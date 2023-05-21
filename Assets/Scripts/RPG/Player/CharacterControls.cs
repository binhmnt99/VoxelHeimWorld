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
        [SerializeField]
        private float slopeIncreaseMultiplier;

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
        private Vector3 rayOffset = new Vector3(0, 1.25f, 0);
        [SerializeField]
        private float rayHeight = 2.5f;
        [SerializeField]
        private LayerMask groundLayer;
        [SerializeField]
        private bool grounded;

        [Header("Slope Handling")]
        [SerializeField]
        private float maxSlopeAngle;
        [SerializeField]
        private RaycastHit slopeHit;
        [SerializeField]
        private bool exitingSlope;

        public Transform orientation;

        public Vector2 character2DInput { get; private set; }
        private Vector3 characterDirection;

        private Rigidbody rb;

        public CharacterInputControl characterInput { get; private set; }

        private bool freeze;
        private bool wallrunning;
        private bool walking = true;
        private bool combat = false;

        public MovementState state;

        public enum MovementState
        {
            freeze,
            wallrunning,
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

        private void GroundCheck()
        {
            grounded = Physics.Raycast(transform.position + rayOffset, -transform.up, rayHeight, groundLayer);
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
                animator.SetFloat("speed", context.ReadValue<Vector2>().magnitude);
            }
            if (context.performed)
            {
                if (walking)
                {
                    character2DInput = context.ReadValue<Vector2>();
                }
            }
            if (context.canceled)
            {
                character2DInput = Vector2.zero;
                animator.SetFloat("speed", 0);
            }
        }

        public bool OnSlope()
        {
            if (Physics.Raycast(transform.position + rayOffset, -transform.up, rayHeight))
            {
                float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
                return angle < maxSlopeAngle && angle != 0;
            }

            return false;
        }

        public Vector3 GetSlopeMoveDirection(Vector3 direction)
        {
            return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
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

                if (OnSlope())
                {
                    float slopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
                    float slopeAngleIncrease = 1 + (slopeAngle / 90f);

                    time += Time.deltaTime * speedIncreaseMultiplier * slopeIncreaseMultiplier * slopeAngleIncrease;
                }
                else
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
            if (OnSlope() && !exitingSlope)
            {
                rb.AddForce(GetSlopeMoveDirection(characterDirection) * movementSpeed * 20f, ForceMode.Force);

                if (rb.velocity.y > 0)
                    rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }

            // on ground
            else if (grounded)
                rb.AddForce(characterDirection.normalized * movementSpeed * 10f, ForceMode.Force);

            // in air
            else if (!grounded)
                rb.AddForce(characterDirection.normalized * movementSpeed * 10f * airMultiplier, ForceMode.Force);

            // turn gravity off while on slope
            if (!wallrunning) rb.useGravity = !OnSlope();

        }

        private void SpeedControl()
        {
            // limiting speed on slope
            if (OnSlope() && !exitingSlope)
            {
                if (rb.velocity.magnitude > movementSpeed)
                    rb.velocity = rb.velocity.normalized * movementSpeed;
            }

            // limiting speed on ground or in air
            else
            {
                Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

                // limit velocity if needed
                if (flatVel.magnitude > movementSpeed)
                {
                    Vector3 limitedVel = flatVel.normalized * movementSpeed;
                    rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
                }
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

            exitingSlope = false;
        }

        private void Jumping()
        {
            exitingSlope = true;

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

            // if (weapon.GetComponentInChildren<WeaponDamageDealer>().IsDealDamage())
            // {
            //     walking = false;
            // }
            // else
            // {
            //     walking = true;
            // }
            GroundCheck();
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
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position + rayOffset, transform.position + rayOffset - transform.up * rayHeight);
        }
    }

}
