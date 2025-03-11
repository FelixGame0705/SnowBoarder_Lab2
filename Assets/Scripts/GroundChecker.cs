using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    [Header("Ground Detection Settings")]
    [SerializeField]
    private string groundTag = "Ground";

    [SerializeField]
    public bool debugMode = false;

    // Events
    public delegate void GroundStateChanged();
    public event GroundStateChanged OnGroundStateChanged;
    public delegate void LandingChanged();
    public event LandingChanged OnLandingChanged;
    public delegate void FlyingChanged();
    public event FlyingChanged OnFlyingChanged;

    // State
    [SerializeField]
    private bool m_IsOnGround = false;
    private bool m_WasOnGround = false;

    // Public accessors
    public bool IsGrounded => m_IsOnGround;
    public bool JustLanded => !m_WasOnGround && m_IsOnGround;
    public bool JustLeftGround => m_WasOnGround && !m_IsOnGround;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        //CheckCollision(collider);
        if (collider.CompareTag(groundTag))
            OnLandingChanged?.Invoke();
        m_IsOnGround = true;
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        //CheckCollision(collider);
        if (collider.CompareTag(groundTag))
            OnFlyingChanged?.Invoke();
        m_IsOnGround = false;
    }
}
