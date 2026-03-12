using UnityEngine;
using UnityEngine.InputSystem; // 1. Phải thêm thư viện này

public class Controller : MonoBehaviour // Đảm bảo tên class này khớp với tên file
{
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 moveDirection; // Biến này sẽ được cập nhật bởi Input System
    private Animator animator;

    [HideInInspector]
    public bool isFacingRight = true;
    [Header("Âm thanh (Kéo thả)")]
    public AudioSource footstepAudioSource; // <-- 1. Kéo AudioSource THỨ HAI vào đây
    public AudioClip footstepSound;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // 2. Hàm này sẽ được "Send Messages" (từ Bước 3) tự động gọi
    // Tên hàm OnMove phải khớp với tên Action "Move" (từ Bước 2)
    void OnMove(InputValue value)
    {
        // 3. Lấy giá trị Vector2 từ input
        moveDirection = value.Get<Vector2>();
    }

    void Update()
    {
        if (GameManager.isPaused) return; // Không đọc input khi pause
        // 4. Update không còn đọc Input, chỉ xử lý logic hướng nhìn
        UpdateFacingDirection();
        UpdateAnimator();
    }

    void FixedUpdate()
    {
        if (GameManager.isPaused)
        {
            rb.linearVelocity = Vector2.zero; // Dừng Player ngay lập tức
            return;
        }
        // 5. Phần vật lý giữ nguyên
        rb.linearVelocity = moveDirection * moveSpeed;
    }

    private void UpdateFacingDirection()
    {
        if (moveDirection.x > 0.01f)
        {
            isFacingRight = true;
        }
        else if (moveDirection.x < -0.01f)
        {
            isFacingRight = false;
        }
    }

private void UpdateAnimator()
    {
        bool isMoving = moveDirection.magnitude > 0.01f;
        animator.SetBool("isMoving", isMoving);
        animator.SetBool("isFacingRight", isFacingRight);

        // --- 3. THÊM LOGIC ÂM THANH NÀY ---
        if (footstepSound != null && footstepAudioSource != null)
        {
            if (isMoving && !footstepAudioSource.isPlaying)
            {
                // Nếu đang đi VÀ chưa phát tiếng -> Bắt đầu phát (lặp lại)
                footstepAudioSource.clip = footstepSound;
                footstepAudioSource.loop = true;
                footstepAudioSource.Play();
            }
            else if (!isMoving && footstepAudioSource.isPlaying)
            {
                // Nếu đứng yên VÀ đang phát tiếng -> Dừng
                footstepAudioSource.Stop();
            }
        }
        // ---------------------------------
    }
}