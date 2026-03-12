using UnityEngine;
using System; // Cần thiết cho 'Action'

public class Health : MonoBehaviour
{
    public Team team;
    public int maxHealth = 100;
    public int currentHealth;
    [Tooltip("Nếu = false, object này sẽ không tự hủy (dùng cho Boss)")]
    public bool destroyOnDeath = true; // Mặc định là true

    // --- THÊM 2 DÒNG NÀY ---
    // Sự kiện này sẽ gửi (currentHealth, maxHealth)
    public event Action<int, int> OnHealthChanged;
    // ----------------------

    public event Action OnDeath;

    void Start()
    {
        currentHealth = maxHealth;
        
        // --- THÊM DÒNG NÀY ---
        // Thông báo cho UI ngay khi game bắt đầu
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        // ----------------------
    }

    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0) return;
        currentHealth -= damage;
        
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }

        // --- THÊM DÒNG NÀY ---
        // Thông báo cho UI biết máu vừa thay đổi
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        // ----------------------
    }

    private void Die()
    {
        OnDeath?.Invoke(); 

        if (team == Team.Player)
        {
            // Logic Player chết giữ nguyên
            // ...
        }
        else
        {
            // --- SỬA LOGIC CŨ ---
            // Chỉ phá hủy nếu được phép
            if (destroyOnDeath)
            {
                Destroy(gameObject);
            }
            // ---------------------
        }
    }
    public void Heal(int amount)
    {
        if (currentHealth <= 0) return; 
        currentHealth += amount;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        
        // --- THÊM DÒNG NÀY ---
        // Thông báo cho UI biết máu vừa thay đổi
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        // ----------------------
    }
}