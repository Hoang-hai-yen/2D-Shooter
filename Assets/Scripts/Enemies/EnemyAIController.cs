using UnityEngine;
using System.Collections;

public class EnemyAIController : MonoBehaviour
{
    // 1. Định nghĩa các trạng thái của AI
    public enum AIState
    {
        Patrolling, // Tuần tra
        Chasing,    // Truy đuổi
        Guarding,
        Shooting,   // Bắn
        Returning   // Quay về
    }

    [Header("Trạng thái")]
    public AIState currentState;

    [Header("Thiết lập Tuần Tra")]
    public Transform patrolPointA;
    public Transform patrolPointB;
    public float patrolSpeed = 2f;
    private Transform currentPatrolTarget;
    private Vector3 originalPosition; // Vị trí ban đầu để quay về

    [Header("Thiết lập Chiến Đấu")]
    public float detectionRange = 10f; // Tầm nhìn
    public float shootingRange = 7f;   // Tầm bắn (phải < detectionRange)
    public float chaseSpeed = 4f;
    public LayerMask wallLayer; // Gán layer "Wall" vào đây

    [Header("Thiết lập Súng")]
    public GunData enemyGun; // Kéo GunData của Enemy vào
    public Transform gunPivot;
    public Transform firePoint;
    public SpriteRenderer enemyGunSprite; // Kéo "EnemyGunSprite" vào đây
    
    private float nextFireTime = 0f;
    [Header("Thiết lập Loot")]
    public GameObject gunPickupPrefab; // <-- THÊM DÒNG NÀY (Kéo Prefab "GunPickup" vào)
    [Header("Âm thanh (Kéo thả)")] // <-- 1. TẠO HEADER (HOẶC THÊM VÀO)
    public AudioClip deathSound;

    // --- Biến nội bộ ---
    private Transform playerTransform; // Để lưu vị trí Player
    private Rigidbody2D rb;
    private SpriteRenderer enemySpriteRenderer; // Để lật sprite thân
    private AudioSource audioSource; // <-- 1. THÊM BIẾN NÀY

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        enemySpriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        
        Health health = GetComponent<Health>();
        health.OnDeath += HandleDeath; 

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
        
        originalPosition = transform.position; // Ghi nhớ vị trí spawn

        // --- THAY THẾ LOGIC CŨ BẰNG ĐOẠN NÀY ---
        if (patrolPointA == null || patrolPointB == null)
        {
            // Nếu không có điểm tuần tra -> Chuyển sang ĐỨNG CANH
            currentState = AIState.Guarding;
        }
        else
        {
            // Nếu có -> TUẦN TRA như cũ
            currentState = AIState.Patrolling;
            currentPatrolTarget = patrolPointB;
        }
        // ------------------------------------
    }

    void Update()
    {
        if (GameManager.isPaused)
        {
            rb.linearVelocity = Vector2.zero; // Bắt AI đứng yên
            return; // Không tính toán gì cả
        }
        // Nếu không tìm thấy Player, không làm gì cả
        if (playerTransform == null) return;

        // Đây là "State Machine" - quyết định hành động dựa trên trạng thái
        switch (currentState)
        {
            case AIState.Patrolling:
                HandlePatrol();
                break;
            case AIState.Chasing:
                HandleChase();
                break;
            case AIState.Guarding:
                HandleGuarding();
                break;
            case AIState.Shooting:
                HandleShooting();
                break;
            case AIState.Returning:
                HandleReturn();
                break;
        }

        // Hàm này đã được sửa để lật sprite (flipX) thay vì lật object (scale)
        HandleGunRotationAndFlip();
    }

    /// <summary>
    /// Hàm kiểm tra xem AI có "nhìn thấy" Player không (không bị tường che)
    /// </summary>
    bool CanSeePlayer()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer > detectionRange)
        {
            return false; // Quá xa
        }

        Vector2 directionToPlayer = (playerTransform.position - transform.position).normalized;
        
        // Chỉ kiểm tra va chạm với layer "Wall" (đã gán trên Inspector)
        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, distanceToPlayer, wallLayer);

        if (hit.collider != null)
        {
            return false; // Bị tường che
        }

        return true; 
    }

    // --- CÁC HÀM TRẠNG THÁI ---

    void HandlePatrol()
    {
        transform.position = Vector2.MoveTowards(transform.position, currentPatrolTarget.position, patrolSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, currentPatrolTarget.position) < 0.1f)
        {
            currentPatrolTarget = (currentPatrolTarget == patrolPointA) ? patrolPointB : patrolPointA;
        }

        if (CanSeePlayer())
        {
            currentState = AIState.Chasing;
        }
    }

    void HandleChase()
    {
        transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, chaseSpeed * Time.deltaTime);

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (!CanSeePlayer())
        {
            currentState = AIState.Returning;
            return;
        }

        if (distanceToPlayer <= shootingRange)
        {
            currentState = AIState.Shooting;
        }
    }

    void HandleShooting()
    {
        rb.linearVelocity = Vector2.zero;
        
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (!CanSeePlayer())
        {
            currentState = AIState.Returning;
            return;
        }

        if (distanceToPlayer > shootingRange)
        {
            currentState = AIState.Chasing;
            return;
        }

        // Bắn (có thể xử lý burst fire nếu muốn, nhưng giờ là bắn liên tục)
        if (Time.time >= nextFireTime)
        {
            if (enemyGun != null)
            {
                nextFireTime = Time.time + 1f / enemyGun.fireRate;
                Fire();
            }
        }
    }
    void HandleGuarding()
    {
        // Đứng yên
        rb.linearVelocity = Vector2.zero;

        // (Hàm HandleGunRotationAndFlip() ở Update sẽ lo việc xoay súng)

        // Nếu thấy Player -> Đuổi
        if (CanSeePlayer())
        {
            currentState = AIState.Chasing;
        }
    }
    void HandleReturn()
    {
        transform.position = Vector2.MoveTowards(transform.position, originalPosition, patrolSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, originalPosition) < 0.1f)
        {
            // --- THAY THẾ LOGIC CŨ BẰNG ĐOẠN NÀY ---
            if (patrolPointA == null || patrolPointB == null)
            {
                // Nếu không có điểm tuần tra -> Quay về ĐỨNG CANH
                currentState = AIState.Guarding;
            }
            else
            {
                // Nếu có -> Quay về TUẦN TRA
                currentState = AIState.Patrolling;
                currentPatrolTarget = patrolPointA;
            }
            // ------------------------------------
        }
        
        if(CanSeePlayer())
        {
            currentState = AIState.Chasing;
        }
    }

    // --- CÁC HÀM HỖ TRỢ ---

    void Fire()
    {
        // Copy y hệt logic Shotgun/Burst từ Player
        if (enemyGun.bulletPrefab == null) return;

        float baseAngle = firePoint.rotation.eulerAngles.z;
        float startAngle = baseAngle - enemyGun.spreadAngle / 2;
        float angleStep = 0f;
        
        if (enemyGun.bulletsPerShotgun > 1)
        {
            angleStep = enemyGun.spreadAngle / (enemyGun.bulletsPerShotgun - 1);
        }
        if (enemyGun.gunshotSound != null)
        {
            audioSource.PlayOneShot(enemyGun.gunshotSound);
        }
        for (int i = 0; i < enemyGun.bulletsPerShotgun; i++)
        {
            float currentAngle = startAngle + (angleStep * i);
            Quaternion bulletRotation = Quaternion.Euler(0, 0, currentAngle);

            GameObject bullet = Instantiate(enemyGun.bulletPrefab, firePoint.position, bulletRotation);
            
            bullet.layer = LayerMask.NameToLayer("EnemyBullet"); 

            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.team = Team.Enemy; 
                bulletScript.damage = enemyGun.damage;
            }
        }
    }

    /// <summary>
    /// (ĐÃ SỬA) Xoay súng 360 độ VÀ Lật sprite (flipX) của thân
    /// </summary>
    void HandleGunRotationAndFlip()
    {
        Vector2 directionToTarget;

        // 1. Xác định mục tiêu
        if ((currentState == AIState.Chasing || currentState == AIState.Shooting) && playerTransform != null)
        {
            directionToTarget = (playerTransform.position - transform.position).normalized;
        }
        else if (currentState == AIState.Patrolling)
        {
            directionToTarget = (currentPatrolTarget.position - transform.position).normalized;
        }
        else // Returning
        {
            directionToTarget = (originalPosition - transform.position).normalized;
        }

        // 2. Tính góc và xoay súng (360 độ)
        float angle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg;
        gunPivot.rotation = Quaternion.Euler(0, 0, angle);

        // 3. Xử lý lật sprite (Thân và Súng)
        if (angle > 90 || angle < -90)
        {
            // Súng chĩa sang trái
            gunPivot.localScale = new Vector3(1, -1, 1); // Lật sprite súng
            enemySpriteRenderer.flipX = true;            // Lật sprite thân
        }
        else
        {
            // Súng chĩa sang phải
            gunPivot.localScale = new Vector3(1, 1, 1); // Giữ nguyên sprite súng
            enemySpriteRenderer.flipX = false;          // Giữ nguyên sprite thân
        }
    }
    // ... (các hàm khác)

    /// <summary>
    /// Hàm này được gọi tự động khi sự kiện OnDeath từ script Health được kích hoạt
    /// </summary>
    private void HandleDeath()
    {
        if (deathSound != null)
        {
            // Phát âm thanh tại vị trí của Enemy
            AudioSource.PlayClipAtPoint(deathSound, transform.position);
        }
        // Nếu không set prefab rơi đồ, thì không làm gì cả
        if (gunPickupPrefab == null) return;

        // Nếu Enemy không có súng, cũng không làm gì
        if (enemyGun == null) return;

        // 1. Tạo ra vật phẩm GunPickup tại vị trí của Enemy
        GameObject pickupInstance = Instantiate(gunPickupPrefab, transform.position, Quaternion.identity);

        // 2. Lấy script GunPickup từ vật phẩm đó
        GunPickup pickupScript = pickupInstance.GetComponent<GunPickup>();
        if (pickupScript != null)
        {
            // 3. Gán GunData (súng của Enemy) vào vật phẩm
            pickupScript.Initialize(enemyGun);
        }

        // (Lưu ý: Script Health sẽ tự động Destroy(gameObject) của Enemy)
    }

    // --- THÊM HÀM NÀY ĐỂ TRÁNH LỖI MEMORY ---
    // Khi object Enemy bị hủy, chúng ta cần hủy đăng ký sự kiện
    void OnDestroy()
    {
        Health health = GetComponent<Health>();
        if (health != null)
        {
            health.OnDeath -= HandleDeath;
        }
    }
}