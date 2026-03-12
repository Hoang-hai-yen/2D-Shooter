using UnityEngine;

public class Bullet : MonoBehaviour
{
    // Những biến này sẽ được súng set khi bắn ra
    public Team team;
    public float damage;
    public float bulletSpeed = 20f;

    private Rigidbody2D rb;
    [Header("Hiệu ứng (Kéo Prefab vào)")]
    public GameObject hitEffectPrefab;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // Bắn viên đạn theo hướng "màu đỏ" (trục X) của nó
        rb.linearVelocity = transform.right * bulletSpeed;

        // Tự hủy sau 3 giây nếu không trúng gì
        Destroy(gameObject, 3f);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // 1. Lấy component Health từ đối tượng va chạm
        if (other.TryGetComponent<Health>(out Health health))
        {
            // 2. Kiểm tra xem có phải team địch không
            if (health.team != this.team)
            {
                // 3. Gây sát thương
                health.TakeDamage((int)damage);

                // 4. Phá hủy đạn ngay khi trúng
                Destroy(gameObject);
            }
        }

        // Nếu đạn va vào tường (Layer "Wall")
        if (other.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            SpawnHitEffect(); // Gọi hàm tạo hiệu ứng
            Destroy(gameObject);
        }
    }
    private void SpawnHitEffect()
    {
        if (hitEffectPrefab != null)
        {
            // Tạo ra Prefab hiệu ứng tại vị trí của viên đạn
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
        }
    }
}