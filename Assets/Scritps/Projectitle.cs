using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectitle : MonoBehaviour
{
    [Header("Base Setting:")]
    [SerializeField] private float m_speed; // Tốc độ của viên đạn.
    private float m_damage; // Sát thương mà viên đạn gây ra.

    private float m_curSpeed; // Tốc độ hiện tại của viên đạn, được khởi tạo bằng m_speed.

    [SerializeField] private GameObject m_bodyHitPrefab; // Prefab hiển thị hiệu ứng khi đạn va chạm với kẻ thù.

    private Vector2 m_lastPosition; // Vị trí cuối cùng của viên đạn trong khung hình trước.
    private RaycastHit2D m_raycastHit; // Lưu kết quả của Raycast để kiểm tra va chạm.

    public float Damage { get => m_damage; set => m_damage = value; } // Thuộc tính để lấy hoặc đặt giá trị sát thương.

    private void Start()
    {
        m_curSpeed = m_speed; // Đặt tốc độ hiện tại của đạn bằng tốc độ gốc.
        RefreshLastPos(); // Cập nhật vị trí cuối cùng của viên đạn.
    }

    private void Update()
    {
        // Di chuyển đạn về phía trước theo hướng của nó với tốc độ m_curSpeed.
        transform.Translate(transform.right * m_curSpeed * Time.deltaTime, Space.World);

        DealDamage(); // Kiểm tra và xử lý va chạm với kẻ thù.
        RefreshLastPos(); // Cập nhật lại vị trí cuối cùng của viên đạn.
    }

    // Phương thức kiểm tra va chạm và gây sát thương nếu đạn chạm vào kẻ thù.
    private void DealDamage()
    {
        // Tính toán hướng ray từ vị trí cuối cùng đến vị trí hiện tại.
        Vector2 rayDirection = (Vector2)transform.position - m_lastPosition;

        // Thực hiện raycast từ vị trí cuối cùng đến vị trí hiện tại của viên đạn.
        // Việc sử dụng rayDirection.magnitude làm khoảng cách raycast giúp phát hiện các đối tượng mà viên đạn có thể đã đi xuyên qua trong một khung hình (do tốc độ di chuyển cao), tránh tình trạng "lọt" qua đối tượng mà không có va chạm được phát hiện.
        m_raycastHit = Physics2D.Raycast(m_lastPosition, rayDirection, rayDirection.magnitude);

        var collider = m_raycastHit.collider;

        // Nếu không có va chạm nào, thoát khỏi phương thức.
        if (!m_raycastHit || collider == null) return;

        // Nếu va chạm với kẻ thù, xử lý gây sát thương.
        if (collider.CompareTag(TagConst.ENEMY_TAG))
        {
            DealDamageToEnemy(collider); // Gây sát thương cho kẻ thù.
        }
    }

    // Phương thức gây sát thương cho kẻ thù.
    private void DealDamageToEnemy(Collider2D collider)
    {
        Actor actorComp = collider.GetComponent<Actor>(); // Lấy component Actor của kẻ thù.

        actorComp?.Takedamage(m_damage); // Gây sát thương cho kẻ thù nếu có component Actor.

        if (m_bodyHitPrefab)
        {
            // Nếu có prefab hiệu ứng, tạo hiệu ứng tại điểm va chạm.
            Instantiate(m_bodyHitPrefab, (Vector3)m_raycastHit.point, Quaternion.identity);
        }

        Destroy(gameObject); // Hủy viên đạn sau khi va chạm và gây sát thương.
    }

    // Cập nhật vị trí cuối cùng của viên đạn trong khung hình hiện tại.
    private void RefreshLastPos()
    {
        m_lastPosition = (Vector2)transform.position;
    }

    // Reset kết quả raycast khi đối tượng bị disable.
    private void OnDisable()
    {
        m_raycastHit = new RaycastHit2D();
    }
}
