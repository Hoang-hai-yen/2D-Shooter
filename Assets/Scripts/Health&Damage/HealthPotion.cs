using UnityEngine;

public class HealthPotion : MonoBehaviour
{
    public int healthToRestore = 25; // Lượng máu sẽ hồi

    [Header("Âm thanh (Kéo thả)")]
    public AudioClip collectSound; // Âm thanh "ực ực" hoặc "ting!"

    // Hàm này được gọi bởi WeaponManager khi nhặt
    public void Collect()
    {
        // 1. Phát âm thanh (nếu có)
        if (collectSound != null)
        {
            // Tùy chọn này sẽ tạo một AudioSource tạm thời tại vị trí
            // của bình thuốc, phát tiếng, và tự hủy sau khi phát xong.
            // Đây là cách tốt nhất để đảm bảo âm thanh không bị ngắt.
            AudioSource.PlayClipAtPoint(collectSound, transform.position);
        }
        
        // 2. Phá hủy bình thuốc
        Destroy(gameObject);
    }
}