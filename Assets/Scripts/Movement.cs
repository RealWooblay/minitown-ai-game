using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class RandomMovement2D : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;         // Speed of movement

    [Header("Time Intervals")]
    public float minMoveTime = 1f;       // Minimum time moving
    public float maxMoveTime = 3f;       // Maximum time moving
    public float minIdleTime = 1f;       // Minimum time idle
    public float maxIdleTime = 6f;       // Maximum time idle

    [Header("References")]
    private Rigidbody2D rb;
    private Vector2 moveDirection;
    private bool isMoving;

    [Header("Animations")]
    private Animator animator;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        // Begin the random movement routine
        StartCoroutine(MovementRoutine());
    }

    private void FixedUpdate()
    {
        // If moving, apply velocity in the chosen random direction
        // Otherwise, set velocity to zero
        rb.linearVelocity = isMoving ? moveDirection * moveSpeed : Vector2.zero;
    }

    private IEnumerator MovementRoutine()
    {
        while (true)
        {
            // Pick a random angle (0 to 360) for movement direction
            float angle = Random.Range(0f, 360f);
            moveDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            isMoving = true;
            animator.SetBool("isWalking", true);
            animator.SetFloat("InputX", moveDirection.x);
            animator.SetFloat("InputY", moveDirection.y);

            // Move for a random duration
            float moveDuration = Random.Range(minMoveTime, maxMoveTime);
            yield return new WaitForSeconds(moveDuration);

            // Idle
            isMoving = false;
            animator.SetBool("isWalking", false);
            animator.SetFloat("LastInputX", moveDirection.x);
            animator.SetFloat("LastInputY", moveDirection.y);
            float idleDuration = Random.Range(minIdleTime, maxIdleTime);
            yield return new WaitForSeconds(idleDuration);
        }
    }
}