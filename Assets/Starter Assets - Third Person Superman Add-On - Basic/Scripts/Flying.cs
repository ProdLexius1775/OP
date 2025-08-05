/*using UnityEngine;
using StarterAssets;

namespace ForgeHorizon.StarterAssetsAddons.ThirdPerson
{
    public class Flying : MonoBehaviour
    {
        public bool fly;
        [Header("Component References")]
        private StarterAssetsInputs flyInput;  // StarterAssetsInputs reference
        private Camera mainCamera;  // Main camera reference
        private Animator animator;  // Animator component reference
        private CharacterController characterController;  // CharacterController component reference

        [Header("State Tracking")]
        public bool isFloating;  // Tracks if the player is floating
        public bool isFlying;  // Tracks if the player is flying
        public bool isGrounded;  // Tracks if the player is on the ground

        [Header("Movement Control")]
        private float targetSpeed;  // Speed at which the player should move
        private Vector3 smoothedMoveDirection = Vector3.zero;  // Smoothed movement direction
        private float flyX;  // Horizontal flying input
        private float flyZ;  // Vertical flying input

        [Header("Floating Bobbing Effect")]
        private float bobbingOffset;  // Offset for the bobbing effect
        private float baseFloatingY;  // The base Y position for bobbing
        private bool isBobbingInitialized = false;  // Tracks if bobbing has been initialized

        [Header("Floating Bobbing Effect")]
        [SerializeField] private float bobbingAmplitude = 0.2f;  // Amplitude of the bobbing effect
        [SerializeField] private float bobbingFrequency = 2f;  // Frequency of the bobbing effect    

        [Header("Flying Speed Variables")]
        private float currentAscendSpeed;  // Current ascending speed
        private float currentDescendSpeed;  // Current descending speed
        private float ascendVelocity;  // Smooth damping for ascending speed
        private float descendVelocity;  // Smooth damping for descending speed

        [Header("Flying Configuration")]
        [SerializeField] private float flyHorizontalSpeed = 50f;  // Horizontal flying speed
        [SerializeField] private float flyVerticalSpeed = 20f;  // Vertical flying speed
        [SerializeField] private float flySpeedFactor = 3f;  // Multiplier for flying speed when sprinting
        [SerializeField] private float flyUpwardSpeed = 100f;  // Speed when ascending
        [SerializeField] private float flyDownwardSpeed = 500f;  // Speed when descending
        [SerializeField] private float flyAnimationLerpSpeed = 5f;  // Speed at which flying animations transition
        [SerializeField] private float flyMovementLerpSpeed = 5f;  // Smooth transition speed for movement
        [SerializeField] private float smoothAscendTime = 1f;  // Time to smoothly ascend
        [SerializeField] private float smoothDescendTime = 0f;  // Time to smoothly descend

        private void Awake()
        {
            mainCamera = Camera.main;  // Get reference to the main camera
        }

        private void Start()
        {
            // Initialize references
            flyInput = GetComponent<StarterAssetsInputs>();
            animator = GetComponent<Animator>();
            characterController = GetComponent<CharacterController>();
        }

        private void Update()
        {
            Fly();  // Handle flying logic
        }

        private void Fly()
        {
            // Toggle floating state on fly input
            if (flyInput.fly)
            {
                isFloating = !isFloating;
                flyInput.fly = false;
            }

            // Update floating animation state
            animator.SetBool("Float", isFloating);

            // Disable bobbing if not floating
            if (!isFloating)
            {
                isBobbingInitialized = false;
                return;
            }

            // Initialize base Y position for bobbing if not done yet
            if (!isBobbingInitialized)
            {
                baseFloatingY = transform.position.y;
                isBobbingInitialized = true;
            }

            // Handle bobbing effect
            if (!flyInput.ascend && !flyInput.descend && smoothedMoveDirection == Vector3.zero)
            {
                bobbingOffset = Mathf.Sin(Time.time * bobbingFrequency) * bobbingAmplitude;
                Vector3 bobbingMotion = new Vector3(0f, bobbingOffset * Time.deltaTime, 0f);
                characterController.Move(bobbingMotion);
            }
            else
            {
                baseFloatingY = transform.position.y;
            }

            Vector2 input = flyInput.move;  // Get movement input for flying

            Ascend();  // Handle ascending
            Descend();  // Handle descending

            // Get the forward and right direction of the camera
            Vector3 cameraForward = mainCamera.transform.forward;
            Vector3 cameraRight = mainCamera.transform.right;

            // Ignore vertical axis for movement
            cameraForward.y = 0f;
            cameraRight.y = 0f;

            // Normalize to keep consistent direction magnitude
            cameraForward.Normalize();
            cameraRight.Normalize();

            // Get normalized movement direction
            Vector3 rawMoveDirection = (cameraForward * input.y + cameraRight * input.x).normalized;

            // Set flying state based on movement
            isFlying = rawMoveDirection != Vector3.zero;

            // Smoothen movement input for animation
            flyX = Mathf.Lerp(flyX, input.x, Time.deltaTime * flyAnimationLerpSpeed);
            flyZ = Mathf.Lerp(flyZ, input.y, Time.deltaTime * flyAnimationLerpSpeed);

            // Update flying animation states
            animator.SetFloat("FlyX", flyX);
            animator.SetFloat("FlyZ", flyZ);

            // Handle flying
            if (isFlying)
            {
                // Smoothen normal flying movement
                smoothedMoveDirection = Vector3.Lerp(smoothedMoveDirection, rawMoveDirection, Time.deltaTime * flyMovementLerpSpeed);

                // Check if sprinting is allowed
                bool canSprint = flyInput.sprint && input.y > 0;

                // Increase speed when sprinting
                if (canSprint)
                {
                    targetSpeed = flyHorizontalSpeed * flySpeedFactor;
                }
                else
                {
                    // Set speed to normal if not sprinting 
                    targetSpeed = flyHorizontalSpeed;
                }

                // Handle sprinting animation state
                animator.SetBool("Sprint", canSprint);

                // Handle final vertical flying movement
                Vector3 finalMove = smoothedMoveDirection * targetSpeed;
                if (input.y != 0)
                {
                    finalMove.y = mainCamera.transform.forward.y * flyVerticalSpeed;
                }
                else
                {
                    finalMove.y = 0f;
                }

                characterController.Move(finalMove * Time.deltaTime);  // Move the character

                // Keep horizontal camera direction only
                Vector3 cameraLook = mainCamera.transform.forward;
                cameraLook.y = 0f;
                if (cameraLook != Vector3.zero)
                {
                    // Rotate the character to face the camera
                    Quaternion targetRotation = Quaternion.LookRotation(cameraLook);

                    // Smoothen rotation
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
                }
            }
            else
            {
                smoothedMoveDirection = Vector3.zero;  // Stop movement if not flying
                animator.SetBool("Sprint", false);  // Disable sprinting animation
            }
        }

        private void Ascend()
        {
            // Set ascend speed if ascend input is triggered
            float targetAscendSpeed = 0f;
            if (flyInput.ascend) targetAscendSpeed = flyUpwardSpeed;

            // Update ascending animation state
            animator.SetBool("Ascend", flyInput.ascend);

            // Smoothen the ascend speed
            currentAscendSpeed = Mathf.SmoothDamp(currentAscendSpeed, targetAscendSpeed, ref ascendVelocity, smoothAscendTime);

            // Move upwards
            characterController.Move(Vector3.up * currentAscendSpeed * Time.deltaTime);
        }

        private void Descend()
        {
            // Set descend speed if descend input is triggered
            float targetDescendSpeed = 0f;
            if (flyInput.descend) targetDescendSpeed = -flyDownwardSpeed;

            // Smoothen the descend speed
            currentDescendSpeed = Mathf.SmoothDamp(currentDescendSpeed, targetDescendSpeed, ref descendVelocity, smoothDescendTime);

            // Move downwards
            characterController.Move(Vector3.up * currentDescendSpeed * Time.deltaTime);

            // Update descending animation state
            if (flyInput.descend && !characterController.isGrounded)
            {
                animator.SetBool("Descend", true);
            }
            else
            {
                animator.SetBool("Descend", false);
            }
        }
    }
}
*/
/*using UnityEngine;
using StarterAssets;
namespace ForgeHorizon.StarterAssetsAddons.ThirdPerson
{
    public class Flying : MonoBehaviour
    {
        public bool fly;
        [Header("Component References")]
        private StarterAssetsInputs flyInput;
        private Camera mainCamera;
        private Animator animator;
        private CharacterController characterController;
        [Header("State Tracking")]
        public bool isFloating;
        public bool isFlying;
        public bool isGrounded;
        [Header("Movement Control")]
        private float targetSpeed;
        private Vector3 smoothedMoveDirection = Vector3.zero;
        private float flyX;
        private float flyZ;
        [Header("Floating Bobbing Effect")]
        private float bobbingOffset;
        private float baseFloatingY;
        private bool isBobbingInitialized = false;
        [Header("Floating Bobbing Effect")]
        [SerializeField] private float bobbingAmplitude = 0.2f;
        [SerializeField] private float bobbingFrequency = 2f;
        [Header("Flying Speed Variables")]
        private float currentAscendSpeed;
        private float currentDescendSpeed;
        private float ascendVelocity;
        private float descendVelocity;
        [Header("Flying Configuration")]
        [SerializeField] private float flyHorizontalSpeed = 50f;
        [SerializeField] private float flyVerticalSpeed = 20f;
        [SerializeField] private float flySpeedFactor = 3f;
        [SerializeField] private float flyUpwardSpeed = 100f;
        [SerializeField] private float flyDownwardSpeed = 500f;
        [SerializeField] private float flyAnimationLerpSpeed = 5f;
        [SerializeField] private float flyMovementLerpSpeed = 5f;
        [SerializeField] private float smoothAscendTime = 1f;
        [SerializeField] private float smoothDescendTime = 0f;
        private void Awake()
        {
            mainCamera = Camera.main;
        }
        private void Start()
        {
            flyInput = GetComponent<StarterAssetsInputs>();
            animator = GetComponent<Animator>();
            characterController = GetComponent<CharacterController>();
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                isFloating = !isFloating;
            }
            Fly();
        }
        private void Fly()
        {
            animator.SetBool("Float", isFloating);
            if (!isFloating)
            {
                isBobbingInitialized = false;
                return;
            }
            if (!isBobbingInitialized)
            {
                baseFloatingY = transform.position.y;
                isBobbingInitialized = true;
            }
            if (!flyInput.ascend && !flyInput.descend && smoothedMoveDirection == Vector3.zero)
            {
                bobbingOffset = Mathf.Sin(Time.time * bobbingFrequency) * bobbingAmplitude;
                Vector3 bobbingMotion = new Vector3(0f, bobbingOffset * Time.deltaTime, 0f);
                characterController.Move(bobbingMotion);
            }
            else
            {
                baseFloatingY = transform.position.y;
            }
            Vector2 input = flyInput.move;
            Ascend();
            Descend();
            Vector3 cameraForward = mainCamera.transform.forward;
            Vector3 cameraRight = mainCamera.transform.right;
            cameraForward.y = 0f;
            cameraRight.y = 0f;
            cameraForward.Normalize();
            cameraRight.Normalize();
            Vector3 rawMoveDirection = (cameraForward * input.y + cameraRight * input.x).normalized;
            isFlying = rawMoveDirection != Vector3.zero;
            flyX = Mathf.Lerp(flyX, input.x, Time.deltaTime * flyAnimationLerpSpeed);
            flyZ = Mathf.Lerp(flyZ, input.y, Time.deltaTime * flyAnimationLerpSpeed);
            animator.SetFloat("FlyX", flyX);
            animator.SetFloat("FlyZ", flyZ);
            if (isFlying)
            {
                smoothedMoveDirection = Vector3.Lerp(smoothedMoveDirection, rawMoveDirection, Time.deltaTime * flyMovementLerpSpeed);
                bool canSprint = flyInput.sprint && input.y > 0;
                targetSpeed = canSprint ? flyHorizontalSpeed * flySpeedFactor : flyHorizontalSpeed;
                animator.SetBool("Sprint", canSprint);
                Vector3 finalMove = smoothedMoveDirection * targetSpeed;
                finalMove.y = input.y != 0 ? mainCamera.transform.forward.y * flyVerticalSpeed : 0f;
                characterController.Move(finalMove * Time.deltaTime);
                Vector3 cameraLook = mainCamera.transform.forward;
                cameraLook.y = 0f;
                if (cameraLook != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(cameraLook);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
                }
            }
            else
            {
                smoothedMoveDirection = Vector3.zero;
                animator.SetBool("Sprint", false);
            }
        }
        private void Ascend()
        {
            float targetAscendSpeed = flyInput.ascend ? flyUpwardSpeed : 0f;
            animator.SetBool("Ascend", flyInput.ascend);
            currentAscendSpeed = Mathf.SmoothDamp(currentAscendSpeed, targetAscendSpeed, ref ascendVelocity, smoothAscendTime);
            characterController.Move(Vector3.up * currentAscendSpeed * Time.deltaTime);
        }
        private void Descend()
        {
            float targetDescendSpeed = flyInput.descend ? -flyDownwardSpeed : 0f;
            currentDescendSpeed = Mathf.SmoothDamp(currentDescendSpeed, targetDescendSpeed, ref descendVelocity, smoothDescendTime);
            characterController.Move(Vector3.up * currentDescendSpeed * Time.deltaTime);
            if (flyInput.descend && !characterController.isGrounded)
            {
                animator.SetBool("Descend", true);
            }
            else
            {
                animator.SetBool("Descend", false);
            }
        }
    }
}
*/