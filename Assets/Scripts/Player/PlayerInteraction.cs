using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    // Lưu trữ vật thể đang đứng gần
    private IInteractable currentInteractable;

    // Hàm này được Input System "Send Messages" tự động gọi khi nhấn "E"
    void OnInteract(InputValue value)
    {
        // Nếu nhấn nút VÀ đang đứng gần một vật thể
        if (value.isPressed && currentInteractable != null)
        {
            // Gọi hàm Interact() của vật thể đó (Rương hoặc Cửa)
            currentInteractable.Interact();
        }
    }

    // Khi Player đi vào vùng Trigger
    void OnTriggerEnter2D(Collider2D other)
    {
        // Kiểm tra xem vật đó có phải là vật "Có thể tương tác" không
        if (other.TryGetComponent<IInteractable>(out IInteractable interactable))
        {
            currentInteractable = interactable;
            // (Sau này có thể hiện chữ "Nhấn E" ở đây)
        }
    }

    // Khi Player đi ra khỏi vùng Trigger
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<IInteractable>(out IInteractable interactable))
        {
            if (interactable == currentInteractable)
            {
                currentInteractable = null;
                // (Ẩn chữ "Nhấn E" ở đây)
            }
        }
    }
}