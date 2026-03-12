using UnityEngine;

public class DestroyAfterAnimation : MonoBehaviour
{
    // Hàm này sẽ được gọi bởi Animation Event
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}