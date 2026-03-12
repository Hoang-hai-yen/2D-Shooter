using UnityEngine;
using System; // Cần cho "Action" (Sự kiện)
public class GameManager : MonoBehaviour
{
    // Biến "static" nghĩa là mọi script khác đều có thể truy cập
    // nó mà không cần tham chiếu, chỉ cần gọi GameManager.isPaused
    public static bool isPaused = false;
    public static bool hasKey = false;

    // Sự kiện để thông báo cho UI biết khi Player nhặt được chìa khóa
    public static event Action OnKeyCollected;
    public static GunData persistentGunData; // Biến này sẽ "sống" qua các màn

    void Awake() // Dùng Awake để chạy trước các hàm Start khác
    {
        // Đảm bảo khi bắt đầu màn chơi, Player không có chìa khóa
        hasKey = false;
    }
    public static void CollectKey()
    {
        if (hasKey) return; // Đã có rồi thì thôi

        hasKey = true;
        OnKeyCollected?.Invoke(); // Phát sự kiện
        Debug.Log("ĐÃ NHẶT ĐƯỢC CHÌA KHÓA!");
    }
}