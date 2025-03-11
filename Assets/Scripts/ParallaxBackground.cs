using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    public Transform target; // Nhân vật hoặc Camera
    public float parallaxStrength = 0.1f; // Độ mạnh hiệu ứng parallax
    public bool infiniteScrolling = true;
    public bool FreezyY;
    public bool IsDead = false;

    [Header("Death Movement Settings")]
    public float deathMoveSpeed = 50.0f; // Tốc độ di chuyển khi nhân vật chết

    private float textureUnitSizeX;
    private float textureUnitSizeY;
    private Vector3 initialOffset; // Khoảng cách ban đầu giữa target và background
    private Vector3 targetPosition; // Vị trí mục tiêu khi di chuyển khi chết

    [SerializeField]
    private GameObject DieChecker;

    void Start()
    {
        if (target == null)
        {
            Debug.LogError("Target for parallax is not assigned!", this);
            enabled = false;
            return;
        }

        // Lấy khoảng cách ban đầu giữa target và this transform
        initialOffset = transform.position - target.position;

        // Lấy kích thước sprite
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            textureUnitSizeX = spriteRenderer.bounds.size.x;
            textureUnitSizeY = spriteRenderer.bounds.size.y;
        }
    }

    void LateUpdate()
    {
        if (IsDead)
        {
            // Tính toán vị trí mục tiêu khi chết
            targetPosition = new Vector2(target.position.x, 0);
            Vector2 targetDie = new Vector2(target.position.x, target.position.y);
            // Di chuyển từ từ đến vị trí mục tiêu
            transform.position = Vector3.Lerp(
                transform.position,
                targetPosition,
                deathMoveSpeed * Time.deltaTime
            );
            DieChecker.transform.position = Vector3.Lerp(
                transform.position,
                targetDie,
                deathMoveSpeed * Time.deltaTime
            );
        }
        else if (FreezyY)
        {
            transform.position =
                new Vector3(
                    -target.position.x * parallaxStrength + target.position.x,
                    transform.position.y,
                    transform.position.z
                ) + new Vector3(initialOffset.x, 0);
        }
        else
        {
            transform.position =
                new Vector3(
                    -target.position.x * parallaxStrength + target.position.x,
                    -target.position.y * parallaxStrength + target.position.y,
                    transform.position.z
                ) + new Vector3(initialOffset.x, initialOffset.y);
        }

        // Chỉ xử lý infinite scrolling khi không ở trạng thái chết
        if (infiniteScrolling && !IsDead)
        {
            // Xử lý nền vô tận
            float distanceX = Mathf.Abs(target.position.x - transform.position.x);
            if (distanceX >= textureUnitSizeX)
                transform.position += new Vector3(
                    textureUnitSizeX * Mathf.Sign(target.position.x - transform.position.x),
                    0,
                    0
                );

            float distanceY = Mathf.Abs(target.position.y - transform.position.y);
            if (distanceY >= textureUnitSizeY)
                transform.position += new Vector3(
                    0,
                    textureUnitSizeY * Mathf.Sign(target.position.y - transform.position.y),
                    0
                );
        }
    }
}
