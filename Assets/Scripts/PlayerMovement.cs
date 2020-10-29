using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private const float MINIMUM_DISTANCE_MAGNITUDE = 0.1f;
    [Header("dev controls")]
    public bool resetTarget;

    private GamepadInputManager inputManager;
    private AIController aIController;
    public PathFinding currentPathFinding;
    public Vector3? currentPathStep = null;
    public bool didStartPathFinding = false;
    public bool togglePathFindingMode = false;
    [Header("other")]
    public float speed = 6.0f;

    public float rotationSpeed = 5f;
    public float jumpSpeed = 8.0f;
    public float gravity = -9.8f;
    public float jumpHeight = 3f;

    public Transform groundCheck;
    public float groundDistance = .4f;
    public LayerMask groundMask;

    private Vector3 velocity;
    private CharacterController controller;
    private SelectionController selectionController;
    Transform LookPos;
    private bool isGrounded;
    Vector3? moveTarget;
    Vector3? reachTarget;
    float distanceToTarget;


    // unitychan
    private Animator anim;
    private AnimatorStateInfo currentBaseState;

    private float animSpeed = 1.5f;
    private float animMoveSpeed = 1.0f;
    static int idleState = Animator.StringToHash("Base Layer.Idle");
    static int locoState = Animator.StringToHash("Base Layer.Locomotion");
    static int jumpState = Animator.StringToHash("Base Layer.Jump");
    static int restState = Animator.StringToHash("Base Layer.Rest");
    static int castSpellState = Animator.StringToHash("Base Layer.CastSpell");

    void Start()
    {
        inputManager = TagResolver.i.inputManager;
        controller = GetComponent<CharacterController>();
        aIController = GetComponent<AIController>();
        LookPos = transform.Find("LookPos");
        selectionController = TagResolver.i.selectionController;
        // unitychan
        anim = GetComponent<Animator>();
        anim.speed = animSpeed;
    }

    public void MoveToPoint(Vector3 destination, float distance = 0)
    {
        moveTarget = destination;
        distanceToTarget = distance;
    }

    public void ReachToPoint(Vector3 destination, PathFinding pathFinding)
    {
        reachTarget = destination;
        currentPathFinding = pathFinding;
    }

    void CastSpell(bool flag)
    {
        anim.SetBool("CastSpell", flag);
    }

    private void StopMovement()
    {
        moveTarget = null;
        reachTarget = null;
        currentPathStep = null;
        togglePathFindingMode = false;
        anim.SetFloat("Speed", 0);
        SwitchAIState(AIStateMode.Idle);
    }
    int STUCK_CONFIRM_NUMBER = 100;
    Vector3? prePosition;
    public int stuckValue = 0;
    void TrackStuckPossibility()
    {
        if (moveTarget == null) return;
        if (prePosition != null)
        {
            if (transform.position.x == prePosition.Value.x && transform.position.z == prePosition.Value.z) stuckValue++;
            else stuckValue = 0;
        }

        if (stuckValue >= STUCK_CONFIRM_NUMBER)
        {
            stuckValue = 0;
            Debug.Log("CONFIRM STUCK!");
            if (reachTarget != null)
            {
                reachTarget = null;
                currentPathStep = null;
                TogglePathFindingModeAndRecalculate();
            }
            else
            {
                reachTarget = moveTarget;
            }
        }
        prePosition = transform.position;
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (resetTarget)
        {
            resetTarget = false;
            StopMovement();
        }
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        if (CameraSettings.isThirdPerson && currentPathStep != null)
        {
            currentPathFinding.DrawPath();
            Vector3 pos = transform.position;
            var dest = currentPathStep.Value;
            var offset = dest - pos;//Utils.CentrifyPosition(pos);
            offset = new Vector3(offset.x, 0, offset.z);
            Debug.DrawLine(pos, dest, Color.blue);
            if (offset.magnitude >= MINIMUM_DISTANCE_MAGNITUDE)
            {
                Quaternion lookRotation = Quaternion.LookRotation(offset);
                transform.rotation = lookRotation;
                offset = offset.normalized * speed;
                anim.SetFloat("Speed", animMoveSpeed);
                controller.Move(offset * Time.deltaTime);
                if ((CheckForJumpPossibility() == true && isGrounded)) // check if close enough and jump
                {
                    Jump();
                }
            }
            else
            {
                for (var i = 0; i < currentPathFinding.pathNodes.Count; i++)
                    if (currentPathFinding.pathNodes[i].pos.Equals(currentPathStep.Value))
                    {
                        if (i == currentPathFinding.pathNodes.Count - 1)
                        {
                            didStartPathFinding = false;
                            StopMovement();
                            break;
                        }
                        else
                        {
                            currentPathStep = currentPathFinding.pathNodes[i + 1].pos;
                            break;
                        }
                    }

            }
            TrackStuckPossibility();
        }
        else if (CameraSettings.isThirdPerson && reachTarget != null)
        {
            Vector3 pos = transform.position;
            var dest = reachTarget.Value;
            var offset = dest - pos;
            Debug.DrawLine(pos, dest, Color.green);
            if (offset.magnitude >= MINIMUM_DISTANCE_MAGNITUDE)
            {
                if (!didStartPathFinding)
                {
                    didStartPathFinding = true;
                    Vector3 startPos = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);

                    if (togglePathFindingMode) currentPathFinding.ToggleMode();
                    if (currentPathFinding.isClosed)
                    {
                        currentPathStep = currentPathFinding.pathNodes[0].pos;
                    }
                    else
                    {
                        StopMovement();
                    };
                }
            }
            else
            {
                StopMovement();
            }
            TrackStuckPossibility();
        }
        else if (CameraSettings.isThirdPerson && moveTarget != null)
        {
            Vector3 targetDir = (moveTarget.Value - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(targetDir.x, 0, targetDir.z));
            transform.rotation = lookRotation;
            var offset = moveTarget.Value - transform.position;
            var magnitude = Mathf.Max(distanceToTarget, MINIMUM_DISTANCE_MAGNITUDE);
            Debug.DrawLine(transform.position, moveTarget.Value, Color.blue);

            if (offset.magnitude > magnitude)
            {
                offset = offset.normalized * speed;
                anim.SetFloat("Speed", animMoveSpeed);
                controller.Move(offset * Time.deltaTime);

                if (CheckForJumpPossibility() == true && isGrounded) // check if close enough and jump
                {
                    Jump();
                }
                else if (CheckForJumpPossibility() == false)
                {
                    reachTarget = moveTarget;
                }
                else
                {
                    StopMovement();
                }
            }
            else
            {
                StopMovement();
            }
            TrackStuckPossibility();
        }
        else if (
            CameraSettings.isThirdPerson &&
            (Math.Abs(inputManager.MovementValue.x) > 0 || Math.Abs(inputManager.MovementValue.y) > 0) &&
            aIController.isIdleing
        )
        {
            if (moveTarget != null) moveTarget = null;
            Vector3 targetDir = new Vector3(inputManager.MovementValue.x, 0f, inputManager.MovementValue.y).normalized;
            targetDir = CameraSettings.CurrentCamera.transform.TransformDirection(targetDir);
            targetDir.y = 0.0f;
            if (targetDir.magnitude < 1)
            {
                targetDir *= 1 / targetDir.magnitude;
            }
            Quaternion lookRotation = Quaternion.LookRotation(targetDir, Vector3.up);
            transform.rotation = lookRotation;
            Vector3 offset = targetDir * speed;
            anim.SetFloat("Speed", animMoveSpeed);
            controller.Move(offset * Time.deltaTime);
            if (CheckForJumpPossibility() == true && isGrounded) // check if close enough and jump
            {
                Jump();
            }
        }
        else if (CameraSettings.isFirstPerson)
        {
            float horizontal = inputManager.MovementValue.x;
            float vertical = inputManager.MovementValue.y;
            if (horizontal > 0 || vertical > 0)
            {
                if (currentBaseState.fullPathHash == castSpellState && selectionController.isDestroyingBlock()) anim.SetBool("CastSpell", false);
            }
            Vector3 moveDirection = transform.right * horizontal + transform.forward * vertical;
            Vector3 offset = moveDirection * speed;
            anim.SetFloat("Speed", Mathf.Max(vertical, horizontal) * speed);
            controller.Move(offset * Time.deltaTime);
            if (inputManager.JumpValue && isGrounded)
            {
                Jump();
            }
        }
        else
        {
            anim.SetFloat("Speed", 0);
        }
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void TogglePathFindingModeAndRecalculate()
    {
        didStartPathFinding = false;
        togglePathFindingMode = !togglePathFindingMode;
    }

    void Jump()
    {
        velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
    }

    bool? CheckForJumpPossibility()
    {
        Quaternion startingAngle = Quaternion.AngleAxis(0, transform.forward);
        Quaternion stepAngle = Quaternion.AngleAxis(5, transform.right);

        float aggroRadius = 1.2f;
        RaycastHit hit;
        var direction = startingAngle * transform.forward;
        var pos = LookPos.position;
        for (var i = 0; i < 20; i++)
        {
            if (Physics.Raycast(pos, direction, out hit, aggroRadius, groundMask))
            {
                Debug.DrawRay(pos, direction * hit.distance, Color.yellow);
                if (i < 6)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                Debug.DrawRay(pos, direction * aggroRadius, Color.white);
            }
            direction = stepAngle * direction;
        }
        return null;
    }
    private void SwitchAIState(AIStateMode newAIState)
    {
        aIController.aiState.mode = newAIState;
    }
}
