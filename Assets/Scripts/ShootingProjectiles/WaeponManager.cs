using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections; // Hãy chắc chắn bạn có dòng này ở đầu file
public class WeaponManager : MonoBehaviour
{
    [Header("Âm thanh (Kéo thả)")] // <-- 1. TẠO HEADER (HOẶC THÊM VÀO)
    public AudioClip gunPickupSound; // <-- 2. THÊM BIẾN NÀY

    [Header("Tham chiếu Object (Kéo từ Hierarchy)")]
    public Transform gunPivot;
    public SpriteRenderer gunSpriteRenderer;
    public Transform firePoint;

    [Header("Thông số Súng (Kéo từ Project)")]
    public GunData startingGun;
    
    private GunData currentGun;
    private Camera mainCamera;
    private float nextFireTime = 0f;
    // ... (các biến khác)
    private bool isFiringBurst = false; // Biến cờ để ngăn người chơi bắn tiếp khi súng đang burst
    private Health playerHealth; // <-- 1. THÊM BIẾN NÀY
    private AudioSource audioSource; // <-- 1. THÊM BIẾN NÀY
    void Start()
    {
        // Lấy các component (Audio, Health...)
        mainCamera = Camera.main;
        playerHealth = GetComponent<Health>();
        audioSource = GetComponent<AudioSource>();

        // KIỂM TRA SÚNG TỪ MÀN TRƯỚC
        if (GameManager.persistentGunData != null)
        {
            // Nếu có súng đã lưu -> trang bị nó
            EquipGun(GameManager.persistentGunData);
        }
        // NẾU KHÔNG CÓ (MÀN ĐẦU TIÊN) -> DÙNG SÚNG KHỞI ĐẦU
        else if (startingGun != null)
        {
            EquipGun(startingGun);
        }
    }

    public void EquipGun(GunData newGun)
    {
        currentGun = newGun;
        gunSpriteRenderer.sprite = currentGun.gunSprite;
        GameManager.persistentGunData = newGun;
    }

    void Update()
    {
        if (GameManager.isPaused) return; // Nếu đang pause, không làm gì cả
        HandleGunRotation();
    }

    // Hàm OnFire sẽ xử lý TOÀN BỘ logic bắn
    void OnFire(InputValue value)
    {
        // Chúng ta chỉ chạy code KHI NÚT VỪA ĐƯỢC NHẤN XUỐNG
        if (value.isPressed)
        {
            // Kiểm tra cooldown VÀ kiểm tra xem súng có đang burst không
            if (currentGun != null && Time.time >= nextFireTime && !isFiringBurst)
            {
                // Đặt lại thời gian hồi cho LẦN BẮN (BURST) TIẾP THEO
                nextFireTime = Time.time + 1f / currentGun.fireRate;

                // Bắt đầu Coroutine bắn loạt
                StartCoroutine(FireBurstCoroutine());
            }
        }
    }


    private void HandleGunRotation()
    {
        Vector3 mouseScreenPosition = Mouse.current.position.ReadValue();
        mouseScreenPosition.z = 10f;
        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(mouseScreenPosition);
        Vector2 directionToMouse = new Vector2(
            mouseWorldPosition.x - gunPivot.position.x,
            mouseWorldPosition.y - gunPivot.position.y
        );
        float angle = Mathf.Atan2(directionToMouse.y, directionToMouse.x) * Mathf.Rad2Deg;
        gunPivot.rotation = Quaternion.Euler(0, 0, angle);
        if (angle > 90 || angle < -90)
        {
            gunPivot.localScale = new Vector3(1, -1, 1);
        }
        else
        {
            gunPivot.localScale = new Vector3(1, 1, 1);
        }
    }
    private void Fire() // Hàm này giờ sẽ bắn ra 1 chùm (shotgun)
    {
        if (currentGun.bulletPrefab == null) return;

        float baseAngle = firePoint.rotation.eulerAngles.z;
        float startAngle = baseAngle - currentGun.spreadAngle / 2;
        
        float angleStep = 0f;
        // ĐỔI TÊN BIẾN Ở DÒNG NÀY
        if (currentGun.bulletsPerShotgun > 1)
        {
            // VÀ DÒNG NÀY
            angleStep = currentGun.spreadAngle / (currentGun.bulletsPerShotgun - 1);
        }
        if (currentGun.gunshotSound != null)
        {
            audioSource.PlayOneShot(currentGun.gunshotSound);
        }

        // VÀ DÒNG NÀY
        for (int i = 0; i < currentGun.bulletsPerShotgun; i++)
        {
            float currentAngle = startAngle + (angleStep * i);
            Quaternion bulletRotation = Quaternion.Euler(0, 0, currentAngle);

            GameObject bullet = Instantiate(currentGun.bulletPrefab, firePoint.position, bulletRotation);
            bullet.layer = LayerMask.NameToLayer("PlayerBullet");

            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.team = Team.Player;
                bulletScript.damage = currentGun.damage;
            }
        }
    }

    private IEnumerator FireBurstCoroutine()
    {
        isFiringBurst = true; // Đánh dấu là súng ĐANG BẮN, không cho bắn tiếp

        // Lặp lại N lần (số đạn trong 1 loạt)
        for (int i = 0; i < currentGun.bulletsPerBurst; i++)
        {
            // Gọi hàm Fire() (hàm này sẽ bắn ra 1 viên, hoặc 1 chùm shotgun)
            Fire();

            // Chờ một khoảng thời gian ngắn trước khi bắn viên tiếp theo
            yield return new WaitForSeconds(currentGun.timeBetweenBurstShots);
        }

        isFiringBurst = false; // Bắn xong, cho phép bắn tiếp
    }
    // ... (các hàm khác)

    /// <summary>
    /// Hàm này tự động được Unity gọi khi Collider của Player
    /// va chạm với một Collider "Is Trigger" khác.
    /// </summary>
    void OnTriggerEnter2D(Collider2D other)
    {
        // 1. Thử kiểm tra Súng
        if (other.TryGetComponent<GunPickup>(out GunPickup gunPickup))
        {
            EquipGun(gunPickup.gunData);
            Destroy(other.gameObject);

            if (gunPickupSound != null)
            {
                audioSource.PlayOneShot(gunPickupSound);
            }
            return; // Đã nhặt, thoát ra
        }

        // 2. Thử kiểm tra Máu
        if (other.TryGetComponent<HealthPotion>(out HealthPotion potion))
        {
            playerHealth.Heal(potion.healthToRestore);
            potion.Collect();
            return; // Đã nhặt, thoát ra
        }

        // 3. THÊM LOGIC KIỂM TRA CHÌA KHÓA
        if (other.TryGetComponent<KeyPickup>(out KeyPickup key))
        {
            // Báo cho GameManager biết đã có chìa
            GameManager.CollectKey();

            // Bảo chìa khóa tự hủy và phát âm thanh
            key.Collect();
            return; // Đã nhặt, thoát ra
        }
    }
}
