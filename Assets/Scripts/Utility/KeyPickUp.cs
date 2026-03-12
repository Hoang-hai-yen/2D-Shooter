using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    public AudioClip collectSound;

    public void Collect()
    {
        // Phát âm thanh tại vị trí chìa khóa
        if (collectSound != null)
        {
            AudioSource.PlayClipAtPoint(collectSound, transform.position);
        }
        // Tự hủy
        Destroy(gameObject);
    }
}