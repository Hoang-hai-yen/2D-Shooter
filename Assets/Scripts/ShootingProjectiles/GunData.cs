using UnityEngine;

[CreateAssetMenu(fileName = "New GunData", menuName = "Game/Gun Data")]
public class GunData : ScriptableObject
{
    [Header("Thông số cơ bản")]
    public new string name; 
    public float damage;
    public float fireRate; // Tốc độ bắn (khoảng cách giữa các LẦN CLICK / LOẠT BẮN)

    [Header("Hình ảnh & Đạn")]
    public Sprite gunSprite; 
    public GameObject bulletPrefab;

    [Header("Thông số Shotgun (Bắn Chùm)")]
    [Range(1, 20)]
    public int bulletsPerShotgun = 1; // Số đạn TRONG 1 CHÙM (Shotgun)
    public float spreadAngle = 0f; // Độ tỏa của chùm

    [Header("Thông số Burst (Bắn Loạt)")]
    [Range(1, 10)]

    public int bulletsPerBurst = 1; // Số đạn TRONG 1 LOẠT BẮN (Burst)
    public float timeBetweenBurstShots = 0.1f; // Độ trễ giữa các viên đạn TRONG LOẠT
    [Header("Âm thanh")] // <-- THÊM DÒNG NÀY
    public AudioClip gunshotSound; // <-- THÊM DÒNG NÀY
}