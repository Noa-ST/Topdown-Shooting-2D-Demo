using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Player : Actor
{
    // Cấu hình các tham số cho Player
    [Header("Player Setting:")]
    [SerializeField] private float moveSpeed; // Tốc độ di chuyển của nhân vật
    [SerializeField] private float m_accelerationSpeed; // Tốc độ tăng tốc của nhân vật
    [SerializeField] private float m_maxMousePosDistance; // Khoảng cách tối đa mà nhân vật có thể di chuyển theo con trỏ chuột
    [SerializeField] private Vector2 m_velocityLimit; // Giới hạn vận tốc di chuyển của nhân vật

    [SerializeField] private float m_enemyDectionRadius; // Bán kính phát hiện kẻ thù
    [SerializeField] private LayerMask m_enemyDetectionLayer; // Lớp đối tượng để phát hiện kẻ thù

    private float m_curSpeed; // Tốc độ hiện tại của nhân vật
    private Actor m_enemyTargeted; // Mục tiêu là kẻ thù mà nhân vật nhắm đến
    private Vector2 m_enemyTargetedDir; // Hướng đến kẻ thù
    private PlayerStats m_playerStats; // Thống kê của người chơi (PlayerStats kế thừa từ ActionStats)

    [Header("Player Event:")]
    public UnityEvent OnAddXp; // Sự kiện khi nhân vật nhận được XP
    public UnityEvent OnLevelUp; // Sự kiện khi nhân vật lên cấp
    public UnityEvent OnLostLife; // Sự kiện khi nhân vật mất máu

    // Thuộc tính để truy cập PlayerStats
    public PlayerStats PlayerStats { get => m_playerStats; set => m_playerStats = value; }

    // Phương thức khởi tạo, gọi khi nhân vật được khởi tạo
    public override void Init()
    {
        LoadStats(); // Tải thống kê của nhân vật từ dữ liệu
    }

    // Tải thống kê của nhân vật từ dữ liệu
    private void LoadStats()
    {
        if (statData == null) return; // Nếu không có dữ liệu thống kê thì không làm gì cả

        m_playerStats = (PlayerStats)statData; // Gán dữ liệu thống kê cho PlayerStats
        m_playerStats.Load(); // Tải dữ liệu thống kê của người chơi từ hệ thống lưu trữ
        CurHp = m_playerStats.hp; // Gán số máu hiện tại cho nhân vật
    }

    // Phương thức Update được gọi mỗi khung hình
    private void Update()
    {
        Move(); // Gọi phương thức di chuyển của nhân vật
    }

    private void FixedUpdate()
    {
        // Gọi hàm DetectEnemy trong FixedUpdate để phát hiện kẻ thù trong mỗi khung hình vật lý
        DetectEnemy();
    }

    private void DetectEnemy()
    {
        // Dùng Physics2D.OverlapCircleAll để lấy tất cả các collider2D nằm trong phạm vi phát hiện của vòng tròn
        // với bán kính m_enemyDectionRadius và thuộc lớp m_enemyDetectionLayer
        var enemyFindeds = Physics2D.OverlapCircleAll(transform.position, m_enemyDectionRadius, m_enemyDetectionLayer);

        // Tìm kẻ thù gần nhất từ danh sách các kẻ thù phát hiện được
        var finalEnemy = FindNearestEnemy(enemyFindeds);

        // Nếu không tìm thấy kẻ thù gần nhất, thoát khỏi hàm
        if (finalEnemy == null) return;

        // Gán kẻ thù gần nhất cho biến m_enemyTargeted
        m_enemyTargeted = finalEnemy;

        // Gọi hàm WeaponHandle để xử lý vũ khí
        WeaponHandle();
    }

    private void WeaponHandle()
    {
        // Nếu không có mục tiêu kẻ thù hoặc không có vũ khí, thoát khỏi hàm
        if (m_enemyTargeted == null || weapon == null) return;

        // Tính toán hướng đến kẻ thù và chuẩn hóa hướng
        m_enemyTargetedDir = m_enemyTargeted.transform.position - weapon.transform.position;
        m_enemyTargetedDir.Normalize();

        // Tính toán góc quay của vũ khí để nhắm vào kẻ thù
        float angle = Mathf.Atan2(m_enemyTargetedDir.y, m_enemyTargetedDir.x) * Mathf.Rad2Deg;
        weapon.transform.rotation = Quaternion.Euler(0f, 0f, angle);

        // Nếu nhân vật đang bị hất văng, không xử lý bắn
        if (m_isKnockback) return;

        // Xử lý bắn về phía kẻ thù
        weapon.Shoot(m_enemyTargetedDir);
    }

    private Actor FindNearestEnemy(Collider2D[] enemyFindeds)
    {
        float minDistance = 0; // Khoảng cách nhỏ nhất, khởi tạo bằng 0
        Actor finalEnemy = null; // Kẻ thù gần nhất, khởi tạo bằng null

        // Nếu danh sách các kẻ thù phát hiện được là null hoặc không có phần tử nào, trả về null
        if (enemyFindeds == null || enemyFindeds.Length <= 0) return null;

        // Lặp qua tất cả các kẻ thù phát hiện được
        for (int i = 0; i < enemyFindeds.Length; i++)
        {
            var enemyFinded = enemyFindeds[i];
            // Nếu collider của kẻ thù là null, bỏ qua
            if (enemyFinded == null) return null;

            // Nếu chưa có kẻ thù nào được chọn, khởi tạo minDistance với khoảng cách từ kẻ thù hiện tại
            if (finalEnemy == null)
            {
                minDistance = Vector2.Distance(transform.position, enemyFinded.transform.position);
            }
            else
            {
                // Tính khoảng cách giữa vị trí của kẻ thù hiện tại và vị trí của đối tượng
                float distancetemp = Vector2.Distance(transform.position, enemyFinded.transform.position);
                // Nếu khoảng cách hiện tại lớn hơn minDistance, bỏ qua kẻ thù này
                if (distancetemp > minDistance) continue;
                // Cập nhật minDistance với khoảng cách mới nhỏ hơn
                minDistance = distancetemp;
            }
            // Cập nhật kẻ thù gần nhất với kẻ thù hiện tại
            finalEnemy = enemyFinded.GetComponent<Actor>();
        }
        // Trả về kẻ thù gần nhất
        return finalEnemy;
    }

    // Di chuyển của nhân vật
    protected override void Move()
    {
        // Nếu nhân vật đã chết thì không làm gì cả
        if (IsDead) return;

        // Lấy vị trí chuột trong thế giới
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // Tính hướng di chuyển từ vị trí hiện tại đến chuột
        Vector2 movingDir = mousePos - (Vector2)transform.position;
        movingDir.Normalize(); // Chuẩn hóa hướng di chuyển

        // Nếu nhân vật không bị hất văng
        if (!m_isKnockback)
        {
            // Nếu chuột trái được nhấn
            if (Input.GetMouseButton(0))
            {
                Run(mousePos, movingDir); // Gọi phương thức chạy
            }
            else
            {
                BackToIdle(); // Quay lại trạng thái đứng yên
            }
            return;
        }

        // Nếu nhân vật bị hất văng, cập nhật vận tốc dựa trên lực hất văng
        m_rb.velocity = m_enemyTargetedDir * -statData.knockbackForce * Time.deltaTime;
        m_amin.SetBool(AminConst.PLAYER_RUN_PARAM, false); // Thiết lập thông số cho animation chạy
    }

    // Trạng thái đứng yên
    private void BackToIdle()
    {
        // Giảm tốc độ dần
        m_curSpeed -= m_accelerationSpeed * Time.deltaTime;
        m_curSpeed = Mathf.Clamp(m_curSpeed, 0, m_curSpeed); // Giới hạn tốc độ trong phạm vi từ 0 đến tốc độ hiện tại

        m_rb.velocity = Vector2.zero; // Dừng di chuyển

        m_amin.SetBool(AminConst.PLAYER_RUN_PARAM, false); // Thiết lập thông số cho animation đứng yên
    }

    // Phương thức chạy của nhân vật
    private void Run(Vector2 mousePos, Vector2 movingDir)
    {
        // Tăng tốc độ dần
        m_curSpeed += m_accelerationSpeed * Time.deltaTime;
        m_curSpeed = Mathf.Clamp(m_curSpeed, 0, m_playerStats.moveSpeed); // Giới hạn tốc độ tối đa theo thông số của nhân vật
        float delta = m_curSpeed * Time.deltaTime; // Tính khoảng cách di chuyển trong mỗi frame
        float distanceToMousePos = Vector2.Distance(transform.position, mousePos); // Tính khoảng cách đến vị trí chuột
        distanceToMousePos = Mathf.Clamp(distanceToMousePos, 0, m_maxMousePosDistance / 3); // Giới hạn khoảng cách di chuyển dựa trên vị trí chuột
        delta *= distanceToMousePos; // Điều chỉnh khoảng cách di chuyển dựa trên khoảng cách đến chuột

        m_rb.velocity = movingDir * delta; // Cập nhật vận tốc di chuyển
        float velocityLimitX = Mathf.Clamp(m_rb.velocity.x, -m_velocityLimit.x, m_velocityLimit.y); // Giới hạn vận tốc theo trục X
        float velocityLimitY = Mathf.Clamp(m_rb.velocity.y, -m_velocityLimit.y, m_velocityLimit.y); // Giới hạn vận tốc theo trục Y
        m_rb.velocity = new Vector2(velocityLimitX, velocityLimitY); // Cập nhật vận tốc sau khi giới hạn

        m_amin.SetBool(AminConst.PLAYER_RUN_PARAM, true); // Thiết lập thông số cho animation chạy
    }

    // Thêm XP cho nhân vật
    public void AddXp(float xpBonus)
    {
        // Nếu không có dữ liệu thống kê của nhân vật thì không làm gì cả
        if (m_playerStats == null) return;

        m_playerStats.xp += xpBonus; // Thêm XP vào thống kê của nhân vật
        m_playerStats.Upgrade(OnUpgradeStats); // Nâng cấp thông số của nhân vật

        OnAddXp?.Invoke(); // Gọi sự kiện khi nhân vật nhận XP

        m_playerStats.Save(); // Lưu dữ liệu thống kê của nhân vật
    }

    // Xử lý khi nhân vật lên cấp
    private void OnUpgradeStats()
    {
        OnLevelUp?.Invoke(); // Gọi sự kiện khi nhân vật lên cấp
    }

    // Nhận sát thương
    public override void Takedamage(float damage)
    {
        // Nếu nhân vật đang miễn nhiễm, không nhận sát thương
        if (m_isInvincible) return;
        CurHp -= damage; // Giảm số máu hiện tại
        CurHp = Mathf.Clamp(CurHp, 0, PlayerStats.hp); // Giới hạn số máu trong phạm vi từ 0 đến máu tối đa
        Knockback(); // Thực hiện hất văng

        OnTakeDamage?.Invoke(); // Gọi sự kiện khi nhân vật bị tấn công

        // Nếu máu hiện tại <= 0
        if (CurHp > 0) return;

        // Kiểm tra kết thúc trò chơi
        GameManager.Ins.GameoverChecking(OnLostLifeDelegate, OnDeadDelegate);
    }

    // Xử lý khi nhân vật mất mạng
    private void OnLostLifeDelegate()
    {
        CurHp = m_playerStats.hp; // Khôi phục máu nhân vật

        // Dừng các coroutine liên quan đến trạng thái hất văng và miễn nhiễm
        if (m_stopKnockbackCo != null)
        {
            StopCoroutine(m_stopKnockbackCo);
        }

        if (m_invincibleCo != null)
        {
            StopCoroutine(m_invincibleCo);
        }

        Invincible(3.5f); // Thiết lập trạng thái miễn nhiễm

        OnLostLife?.Invoke(); // Gọi sự kiện khi nhân vật mất mạng
    }

    // Xử lý khi nhân vật chết
    private void OnDeadDelegate()
    {
        CurHp = 0; // Đặt máu bằng 0
        Die(); // Thực hiện hành động khi nhân vật chết
    }

    // Xử lý va chạm với các đối tượng khác
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Nếu va chạm với kẻ thù
        if (collision.gameObject.CompareTag(TagConst.ENEMY_TAG))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                Takedamage(enemy.CurDamage); // Nhận sát thương từ kẻ thù
            }
        }
        // Nếu va chạm với vật phẩm
        else if (collision.gameObject.CompareTag(TagConst.COLLECTABLE_TAG))
        {
            Collectable collectable = collision.gameObject.GetComponent<Collectable>();
            collectable?.Trigger(); // Kích hoạt hành động của vật phẩm
            Destroy(collectable.gameObject); // Xóa vật phẩm khỏi thế giới
        }
    }

    // Vẽ gizmos để hiển thị bán kính phát hiện kẻ thù trong Editor
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color32(133, 250, 47, 50); // Màu sắc của gizmos
        Gizmos.DrawSphere(transform.position, m_enemyDectionRadius); // Vẽ hình cầu để hiển thị phạm vi phát hiện
    }
}
