using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float torqueAmount = 20f;
    [SerializeField] float boostSpeed = 20f;
    [SerializeField] float baseSpeed = 20f;
    [SerializeField] float boostLerpSpeed = 10f;
    [SerializeField] AudioClip PickItemSound;

    Rigidbody2D rb2d;
    SurfaceEffector2D surfaceEffector2D;

    // Biến theo dõi xoay cho trick
    float previousRotation;
    float cumulativeRotation;

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        surfaceEffector2D = FindAnyObjectByType<SurfaceEffector2D>();
        previousRotation = transform.eulerAngles.z;
        cumulativeRotation = 0f;
    }

    void Update()
    {
        RotatePlayer();
        HandleBoost();
        CheckRotationTrick();
    }

    private void HandleBoost()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            // Boost liên tục: tăng dần tốc độ lên mức boost
            float targetSpeed = baseSpeed + boostSpeed;
            surfaceEffector2D.speed = Mathf.Lerp(surfaceEffector2D.speed, targetSpeed, Time.deltaTime * boostLerpSpeed);
        }
        else
        {
            // Giảm dần về tốc độ cơ bản
            surfaceEffector2D.speed = Mathf.Lerp(surfaceEffector2D.speed, baseSpeed, Time.deltaTime * boostLerpSpeed);
        }
    }

    private void RotatePlayer()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        rb2d.AddTorque(-horizontalInput * torqueAmount);
    }

    private void CheckRotationTrick()
    {
        float currentRotation = transform.eulerAngles.z;
        float deltaAngle = Mathf.DeltaAngle(previousRotation, currentRotation);

        // Nếu hướng xoay thay đổi, reset tích lũy
        if (cumulativeRotation != 0 && Mathf.Sign(deltaAngle) != Mathf.Sign(cumulativeRotation))
        {
            cumulativeRotation = 0;
        }
        cumulativeRotation += deltaAngle;
        previousRotation = currentRotation;

        // Nếu đã xoay đủ 360 độ, cộng điểm trick và phát âm thanh
        if (Mathf.Abs(cumulativeRotation) >= 360f)
        {
            // Cộng điểm trick (ví dụ: 50 điểm cho mỗi vòng xoay)
            ScoreManager.Instance.AddTrickScore(50);
            // Phát âm thanh cộng điểm (đảm bảo GameObject có AudioSource)
            GetComponent<AudioSource>().PlayOneShot(PickItemSound);

            // Reset tích lũy góc xoay
            cumulativeRotation = 0;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Item"))
        {
            ScoreManager.Instance.AddCollectibleScore(10);
            // Phát âm thanh bằng cách tạo AudioSource tạm thời
            //AudioSource.PlayClipAtPoint(GetComponent<AudioSource>().clip, transform.position, 5f);
            GetComponent<AudioSource>().PlayOneShot(PickItemSound);
            // Ẩn phần hiển thị của item
            collision.GetComponent<SpriteRenderer>().enabled = false;
            // Nếu cần, vô hiệu hóa collider để tránh các va chạm tiếp theo
            collision.GetComponent<Collider2D>().enabled = false;
        }
    }
}
