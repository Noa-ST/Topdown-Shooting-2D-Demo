using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Weapon : MonoBehaviour
{
    [Header("Common:")]
    public WeaponStats statData; // Dữ liệu thống kê của vũ khí, được lưu trữ trong một ScriptableObject.

    [SerializeField] private Transform m_shootingPoint; // Điểm bắn, nơi viên đạn sẽ xuất hiện khi bắn.

    [SerializeField] private GameObject m_bulletPb; // Prefab của viên đạn.
    [SerializeField] private GameObject m_muzzleFlashPb; // Prefab của hiệu ứng lóe sáng khi bắn.

    private float m_curFR; // Tốc độ bắn hiện tại (fire rate).
    private int m_curBullets; // Số lượng đạn hiện tại trong băng.
    private float m_curReloadTime; // Thời gian nạp đạn hiện tại.

    private bool m_isShooted; // Cờ cho biết liệu đã bắn trong khung hình này hay chưa.
    private bool m_isReloading; // Cờ cho biết liệu vũ khí đang trong quá trình nạp đạn hay không.

    [Header("Event:")]
    public UnityEvent OnShoot; // Sự kiện khi bắn.
    public UnityEvent OnReload; // Sự kiện khi bắt đầu nạp đạn.
    public UnityEvent OnReloadDone; // Sự kiện khi hoàn tất nạp đạn.

    private void Start()
    {
        LoadStat(); // Tải các thông số của vũ khí khi bắt đầu.
    }

    private void LoadStat()
    {
        if (statData == null) return;

        statData.Load(); // Tải dữ liệu từ `statData`.
        m_curFR = statData.firerate; // Khởi tạo tốc độ bắn hiện tại bằng giá trị từ `statData`.
        m_curReloadTime = statData.reloadTime; // Khởi tạo thời gian nạp đạn hiện tại.
        m_curBullets = statData.bullets; // Khởi tạo số đạn hiện tại từ `statData`.
    }

    private void Update()
    {
        ReduceFirerate(); // Giảm thời gian chờ giữa các lần bắn.
        ReduceReloadTime(); // Giảm thời gian chờ nạp đạn.
    }

    private void ReduceReloadTime()
    {
        if (!m_isReloading) return; // Nếu không đang nạp đạn, thoát khỏi phương thức.
        m_curReloadTime -= Time.deltaTime; // Giảm thời gian nạp đạn mỗi khung hình.

        if (m_curReloadTime > 0) return; // Nếu chưa hết thời gian nạp đạn, tiếp tục chờ.
        LoadStat(); // Tải lại thông số sau khi nạp đạn.
        m_isReloading = false;
        OnReloadDone?.Invoke(); // Gọi sự kiện hoàn tất nạp đạn.
    }

    private void ReduceFirerate()
    {
        if (!m_isShooted) return; // Nếu không đã bắn, thoát khỏi phương thức.
        m_curFR -= Time.deltaTime; // Giảm thời gian chờ giữa các lần bắn.

        if (m_curFR > 0) return; // Nếu chưa thể bắn lại, tiếp tục chờ.
        m_curFR = statData.firerate; // Đặt lại thời gian bắn theo giá trị từ `statData`.
        m_isShooted = false; // Cho phép bắn tiếp.
    }

    public void Shoot(Vector3 targetDirection)
    {
        if (m_isShooted || m_shootingPoint == null || m_curBullets <= 0) return; // Kiểm tra các điều kiện để bắn.

        if (m_muzzleFlashPb)
        {
            var muzzleFlashClone = Instantiate(m_muzzleFlashPb, m_shootingPoint.position, transform.rotation); // Tạo hiệu ứng lóe sáng tại điểm bắn.
            muzzleFlashClone.transform.SetParent(m_shootingPoint);

        }

        if (m_bulletPb)
        {
            var bulletClone = Instantiate(m_bulletPb, m_shootingPoint.position, transform.rotation); // Tạo ra viên đạn.
            var projectileComp = bulletClone.GetComponent<Projectitle>(); // Lấy component Projectitle từ viên đạn.
 
            if (projectileComp != null)
            {
                projectileComp.Damage = statData.damage; // Gán giá trị sát thương cho viên đạn.
            }
        }

        m_curBullets--; // Giảm số đạn hiện tại.
        m_isShooted = true; // Đặt cờ cho biết đã bắn.
        if (m_curBullets <= 0)
        {
            Reload(); // Nếu hết đạn, bắt đầu nạp đạn.
        }

        OnShoot?.Invoke(); // Gọi sự kiện bắn.
    }

    public void Reload()
    {
        m_isReloading = true; // Đặt cờ cho biết đang nạp đạn.
        OnReload.Invoke(); // Gọi sự kiện bắt đầu nạp đạn.
    }
}
