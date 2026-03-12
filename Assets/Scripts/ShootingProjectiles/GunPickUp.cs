using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class GunPickup : MonoBehaviour
{
    // Script này chỉ chứa data của súng nó đang giữ
    public GunData gunData;

    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Chúng ta tạo một hàm riêng để gán data
    // Điều này đảm bảo sprite được cập nhật ngay khi vật phẩm được tạo ra
    public void Initialize(GunData data)
    {
        gunData = data;

        // Tự động gán sprite của vật phẩm
        // bằng sprite của súng (lấy từ GunData)
        if (gunData != null)
        {
            spriteRenderer.sprite = gunData.gunSprite;
        }
    }
}