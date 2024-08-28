using System.Collections;
using System.Drawing;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    [SerializeField] private MovementData movementData;

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private Transform groundCheckPosition;

    [SerializeField] private Transform wallCheckPosition;

    private CapsuleCollider2D playerCol;
    private Rigidbody2D playerRB;

    public bool isMoving = false;
    public bool isGrounded = true;
    public bool lockMomentum = false;

    public bool canGrapple = false;
    public bool isGrappling = false;

    private Vector2 direction;
    private float moveSpeed;
    private float moveSpeedModifier = 1;

    private float fallingDuration;

    public int jumpCount;
    public int maxJumpCount;

    public int wallJumpCount;
    public int maxWallJumps;

    private Coroutine jumpRoutine;
    private Coroutine burstDragRoutine;
    private Coroutine dashRoutine;
    private Coroutine rollRoutine;
    private Coroutine plungeRoutine;

    public void InitializeMovementController()
    {
        moveSpeed = movementData.walkSpeed;
        playerCol = GetComponent<CapsuleCollider2D>();
        playerRB = GetComponent<Rigidbody2D>();
    }

    public void HandleMovment(float horizontal)
    {
        isMoving = horizontal != 0;

        // Update player facing dir
        if (!lockMomentum)
        {
            if (horizontal > 0)
                transform.localScale = new Vector3(1, 1, 1);
            else if (horizontal < 0)
                transform.localScale = new Vector3(-1, 1, 1);
        }

        if (isMoving)
        {
            direction = Camera.main.transform.forward.normalized;

            Vector3 sideDirection = Vector3.ProjectOnPlane(Camera.main.transform.right * horizontal, Vector3.up);
            direction = sideDirection.normalized;
        }
        else if (!isMoving && isGrounded)
        {
            if (burstDragRoutine != null)
                StartCoroutine(BurstDrag());
            moveSpeed = movementData.walkSpeed;
        }

        if (!isGrounded)
            fallingDuration += Time.deltaTime;

        if (playerRB.velocity.y < 0 && wallJumpCount <= 0)
            lockMomentum = false;

        SpeedControl();

        Debug.DrawRay(groundCheckPosition.position, Vector3.down * movementData.plungeThreshold, UnityEngine.Color.red);
    }

    public void HandleGrappling(float vertical, float posX)
    {
        if (plungeRoutine != null)
            return;

        if (!isGrappling)
        {
            if (!canGrapple || vertical == 0)
                return;
        }

        transform.position = new Vector3(Mathf.Lerp(transform.position.x, posX, Time.deltaTime * 10f),
            transform.position.y,
            transform.position.z);

        isGrappling = true;
        playerRB.gravityScale = 0;
        playerRB.velocity = Vector2.zero;
        playerCol.isTrigger = true;

        transform.position = new Vector3(transform.position.x, transform.position.y + Time.deltaTime * movementData.grappleSpeed * vertical, transform.position.z);
    }

    public void StopGrappling()
    {
        playerRB.gravityScale = 2;
        isGrappling = false;
        playerCol.isTrigger = false;
    }

    public void HandleJump(float horizontal)
    {
        if (plungeRoutine != null)
            return;

        if (isGrappling)
        {
            StopGrappling();

            Vector2 dir = Vector2.up;

            if (transform.localScale.x > 0)
                dir = new Vector2(0.2f, 0.5f);
            else
                dir = new Vector2(-0.2f, 0.5f);

            playerRB.AddForce(dir * movementData.baseJumpForce, ForceMode2D.Impulse);
            return;
        }

        Collider2D col = Physics2D.OverlapCircle(wallCheckPosition.position, 0.2f, ~playerLayer);
        if (col != null && col.isTrigger)
            col = null;

        if (col == null || wallJumpCount <= 0)
        {
            // Normal Jump
            if (fallingDuration > movementData.cyoteTime && jumpCount == maxJumpCount)
                return;

            if (jumpCount <= 0 && !isGrounded)
                return;

            if (jumpRoutine == null)
                jumpRoutine = StartCoroutine(JumpRoutine());
        }
        else if (wallJumpCount > 0)
        {
            // Wall Jump
            playerRB.velocity = new Vector2(playerRB.velocity.x, 0);

            Vector2 dir = Vector2.up;
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);

            if (col.transform.position.x < transform.position.x)
                dir = new Vector2(movementData.wallJumpForceX, movementData.wallJumpForceY);
            else
                dir = new Vector2(-movementData.wallJumpForceX, movementData.wallJumpForceY);

            playerRB.AddForce(dir * movementData.baseJumpForce, ForceMode2D.Impulse);

            wallJumpCount--;
            lockMomentum = true;
        }
    }

    private IEnumerator JumpRoutine()
    {
        // Normal Jump
        playerRB.velocity = new Vector2(playerRB.velocity.x, 0);
        playerRB.AddForce(transform.up * movementData.baseJumpForce, ForceMode2D.Impulse);
        jumpCount--;

        yield return new WaitForSeconds(0.1f);

        jumpRoutine = null;
    }

    public void HandleDash(float direction)
    {
        if (dashRoutine == null)
            dashRoutine = StartCoroutine(DashRoutine(direction));
    }

    private IEnumerator DashRoutine(float direction)
    {
        lockMomentum = false;
        float timer = movementData.dashDuration;

        while (timer > 0)
        {
            timer -= Time.deltaTime;

            if (direction == 0)
            {
                if (transform.localScale.x > 0)
                    direction = 1;
                else
                    direction = -1;
            }

            playerRB.velocity = new Vector2(movementData.dashSpeed * direction, 0);
            yield return null;
        }

        yield return new WaitForSeconds(movementData.dashCooldown);

        dashRoutine = null;
    }

    public void HandleRoll()
    {
        if (rollRoutine == null)
            rollRoutine = StartCoroutine(RollRoutine());
    }

    private IEnumerator RollRoutine()
    {
        Vector2 originalSize = playerCol.size;
        Vector2 originalOffset = playerCol.offset;

        float change = movementData.rollColliderSize - playerCol.size.y;

        playerCol.size = new Vector2(playerCol.size.x, movementData.rollColliderSize);
        playerCol.offset = new Vector2(playerCol.offset.x, -(Mathf.Abs(change) / 2));

        yield return new WaitForSeconds(movementData.rollFrames);

        playerCol.size = originalSize;
        playerCol.offset = originalOffset;

        yield return new WaitForSeconds(1f);

        rollRoutine = null;
    }

    public bool HandlePlunge()
    {
        RaycastHit2D groundHit = Physics2D.Raycast(groundCheckPosition.position, Vector3.down, movementData.plungeThreshold, groundLayer);
        if (groundHit)
            return false;

        StopGrappling();
        plungeRoutine = StartCoroutine(PlungeRoutine());

        return true;
    }

    private IEnumerator PlungeRoutine()
    {
        playerRB.velocity = Vector2.zero;
        playerRB.gravityScale = 0;

        yield return new WaitForSeconds(movementData.plungeDelay);

        playerRB.gravityScale = 2;
        playerRB.AddForce(Vector2.down * movementData.plungeForce, ForceMode2D.Impulse);
    }

    public void MovePlayer()
    {
        if (!isMoving || lockMomentum || isGrappling || plungeRoutine != null)
            return;

        Vector3 force;

        // Adjust drag & force
        if (isGrounded)
            force = direction * (moveSpeed + playerRB.velocity.magnitude) * 5f;
        else if (!isGrounded)
            force = direction * moveSpeed * 10f * movementData.airMultiplier * moveSpeedModifier;
        else
            force = Vector3.zero;

        // Move player
        playerRB.AddForce(force * moveSpeedModifier, ForceMode2D.Force);
    }

    public void CheckGroundCollision()
    {
        RaycastHit2D groundHit = Physics2D.Raycast(groundCheckPosition.position, Vector3.down, 100, groundLayer);
        if (!groundHit)
            return;

        float dist = Vector3.Distance(groundCheckPosition.position, groundHit.point);
        if (dist <= movementData.minGroundDist)
        {
            isGrounded = true;
            playerRB.drag = movementData.groundDrag;
            fallingDuration = 0;

            lockMomentum = false;
            wallJumpCount = maxWallJumps;
            if (jumpRoutine == null)
                jumpCount = maxJumpCount;

            plungeRoutine = null;

            if (isGrappling)
                StopGrappling();
        }
        else if (dist > movementData.minGroundDist)
        {
            isGrounded = false;
            playerRB.drag = movementData.airDrag;
        }
    }

    public void StopPlayer()
    {
        isMoving = false;
        playerRB.velocity = Vector3.zero;
    }

    public void SetMoveSpeedModifier(float newModifier)
    {
        moveSpeedModifier = newModifier;
    }

    private void SpeedControl()
    {
        Vector2 currentVel = new Vector2(playerRB.velocity.x, 0);

        if (currentVel.magnitude > moveSpeed)
        {
            Vector3 limitVel = currentVel.normalized * moveSpeed;
            playerRB.velocity = new Vector2(limitVel.x, playerRB.velocity.y);
        }
    }

    private IEnumerator BurstDrag()
    {
        playerRB.drag = 15;
        yield return new WaitForSeconds(0.3f);
        playerRB.drag = movementData.groundDrag;
        burstDragRoutine = null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(wallCheckPosition.position, 0.2f);
    }
}