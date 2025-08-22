using System;
using UnityEngine;

public class MoveOnSwipe : MonoBehaviour
{
    private TapSwipeDetection swipeDetection;
    private Rigidbody rb;

    // --- Jumping Variables ---
    [SerializeField] private float jumpForce = 5f;                   // Strength of the jump
    [SerializeField] private float groundCheckDistance = 0.2f;       // Distance to check for ground
    [SerializeField] private float groundCheckOffset = 0.5f;         // Offset from player's center to ray origin
    [SerializeField] private LayerMask groundLayer;                  // Layer mask to identify ground
    [SerializeField] private AudioClip jumpClip;
    private Vector3[] lanePositions;

    private int currentLane = 1; // Start in the middle lane (index 1)

    // --- Initialization ---
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        float y = transform.position.y;
        lanePositions = new Vector3[3]
        {
        new Vector3(-6f, y, 0f), // Left lane
        new Vector3(0f, y, 0f),  // Middle lane
        new Vector3(6f, y, 0f)   // Right lane
         };

        // Find the swipe detection script in the scene
        swipeDetection = FindAnyObjectByType<TapSwipeDetection>();
        if (swipeDetection == null)
        {
            Debug.LogError("TapSwipeDetection not found in scene.");
            return;
        }

        // Subscribe to swipe events
        Debug.Log("MoveOnSwipe: Subscribing to swipe events.");
        swipeDetection.OnSwipeLeft += MoveLeft;
        swipeDetection.OnSwipeRight += MoveRight;
        swipeDetection.OnSwipeUp += Jump;

        // Set initial position to middle lane
        transform.position = lanePositions[currentLane];
    }

    // --- Movement Functions ---
    private void MoveLeft()
    {
        if (currentLane > 0)
        {
            currentLane--;
            MoveToLane();
            Debug.Log("Moved Left");
        }
    }

    private void MoveRight()
    {
        if (currentLane < lanePositions.Length - 1)
        {
            currentLane++;
            MoveToLane();
            Debug.Log("Moved Right");
        }
    }

    private void MoveToLane()
    {
        transform.position = lanePositions[currentLane];
        Debug.Log($"Moved to lane {currentLane} at position {transform.position}");
    }

    // --- Jumping ---
    private void Jump()
    {
   
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        AudioManager.instance.PlaySoundFX(jumpClip, transform, 1f);
        Debug.Log("Jumped");
    }

}