using UnityEngine;

public class GuardianBossAI : MonoBehaviour
{
    [Header("Cài đặt Chiến đấu")]
    public GunData bulletHellData; // Dùng 1 GunData (chỉ lấy prefab đạn & damage)
    public int bulletHellAmount = 8; // Bắn 8 viên 360 độ
    public float attackCooldown = 4f; // 4s bắn 1 lần
    private float attackTimer;

    [Header("Cài đặt Gọi Lính (Minion)")]
    public GameObject enemyMinionPrefab; // Kéo prefab Enemy AI cũ vào
    public Transform[] spawnPoints; // Kéo các điểm spawn vào
    public float spawnCooldown = 10f; // 10s gọi lính 1 lần
    private float spawnTimer;

    [Header("Cài đặt Loot")]
    public GameObject keyPrefab; // Kéo prefab KeyPickup vào
    public GameObject explosionPrefab; // Kéo prefab Nổ vào
    public AudioClip deathSound;

    private Health health;
    private AudioSource audioSource;
    private Transform playerTransform; // Để bắn đạn theo hướng (nếu cần)
    private Animator animator; // Dùng cho Idle/Death
    private bool isDead = false; // Cờ báo Boss đã chết

    void Start()
    {
        health = GetComponent<Health>();
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>(); // <-- 1. LẤY ANIMATOR

        // Tìm Player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }

        // Đăng ký sự kiện chết
        health.OnDeath += HandleDeath;

        // Đặt thời gian
        attackTimer = attackCooldown;
        spawnTimer = spawnCooldown;
    }

    void OnDestroy()
    {
        if (health != null)
        {
            health.OnDeath -= HandleDeath;
        }
    }

    void Update()
    {
        // Không chạy khi game pause
        if (GameManager.isPaused) return;

        // Đếm ngược thời gian
        attackTimer -= Time.deltaTime;
        spawnTimer -= Time.deltaTime;

        // Hết giờ -> Tấn công
        if (attackTimer <= 0f)
        {
            PerformBulletHell();
            attackTimer = attackCooldown;
        }

        // Hết giờ -> Gọi lính
        if (spawnTimer <= 0f)
        {
            SpawnMinions();
            spawnTimer = spawnCooldown;
        }
    }

    /// <summary>
    /// Đòn 1: Bắn 360 độ
    /// </summary>
    void PerformBulletHell()
    {
        if (bulletHellData == null || bulletHellData.bulletPrefab == null) return;

        float angleStep = 360f / bulletHellAmount;
        float currentAngle = 0f;

        for (int i = 0; i < bulletHellAmount; i++)
        {
            // Tạo góc xoay
            Quaternion rotation = Quaternion.Euler(0, 0, currentAngle);

            // Tạo đạn
            GameObject bullet = Instantiate(bulletHellData.bulletPrefab, transform.position, rotation);
            bullet.layer = LayerMask.NameToLayer("EnemyBullet");

            // Gán thông số
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.team = Team.Enemy;
                bulletScript.damage = bulletHellData.damage;
            }

            currentAngle += angleStep;
        }

        // Phát âm thanh (nếu có)
        if (bulletHellData.gunshotSound != null)
        {
            audioSource.PlayOneShot(bulletHellData.gunshotSound);
        }
    }

    /// <summary>
    /// Đòn 2: Gọi lính
    /// </summary>
    void SpawnMinions()
    {
        if (enemyMinionPrefab == null || spawnPoints.Length == 0) return;

        foreach (Transform spawnPoint in spawnPoints)
        {
            Instantiate(enemyMinionPrefab, spawnPoint.position, spawnPoint.rotation);
        }

        // (Bạn có thể thêm âm thanh gọi lính ở đây)
    }

    /// <summary>
    /// Được gọi khi Boss chết (Health <= 0)
    /// </summary>
void HandleDeath()
    {
        if (isDead) return; // Tránh gọi 2 lần

        isDead = true; // Đánh dấu là đã chết
        
        // Tắt va chạm và vật lý
        GetComponent<Collider2D>().enabled = false;
        GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;

        // Kích hoạt animation "Die"
        animator.SetTrigger("Die");
        
        // (Chúng ta sẽ KHÔNG rơi đồ ở đây)
    }

    // --- 4. THÊM HÀM MỚI NÀY (DÙNG CHO ANIMATION EVENT) ---
    /// <summary>
    /// Hàm này được gọi bởi Animation Event ở frame cuối của anim chết
    /// </summary>
    public void OnDeathAnimationFinished()
    {
        // 1. Tạo hiệu ứng nổ
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }

        // 2. Phát âm thanh chết
        if (deathSound != null)
        {
            AudioSource.PlayClipAtPoint(deathSound, transform.position);
        }

        // 3. Rơi chìa khóa
        if (keyPrefab != null)
        {
            Instantiate(keyPrefab, transform.position, Quaternion.identity);
        }
        
        // 4. Phá hủy object Boss
        Destroy(gameObject);
    }
}