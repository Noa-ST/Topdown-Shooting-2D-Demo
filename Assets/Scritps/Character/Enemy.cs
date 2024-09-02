using UnityEngine;

public class Enemy : Actor
{
    private Player m_player; // Tham chiếu đến đối tượng Player
    private EnemyStat m_enemyStats; // Tham chiếu đến dữ liệu thống kê của kẻ thù

    private float m_curDamage; // Sát thương hiện tại của kẻ thù
    private float m_xpBonus; // Lượng XP mà kẻ thù sẽ thưởng khi bị tiêu diệt

    // Thuộc tính để truy cập sát thương hiện tại của kẻ thù
    public float CurDamage { get => m_curDamage; private set => m_curDamage = value; }

    // Phương thức khởi tạo, được gọi khi kẻ thù được khởi tạo
    public override void Init()
    {
        m_player = GameManager.Ins.Player; // Gán đối tượng Player từ GameManager

        // Nếu không có dữ liệu thống kê hoặc không tìm thấy Player thì thoát khỏi hàm
        if (statData == null || m_player == null) return;

        //dòng mã này giúp bạn chuyển đổi dữ liệu thống kê của một đối tượng thành một đối tượng kẻ thù cụ thể và gán nó cho m_enemyStats để sử dụng các thuộc tính và phương thức đặc biệt của EnemyStat.
        m_enemyStats = (EnemyStat)statData; // Gán dữ liệu thống kê cho EnemyStat
        m_enemyStats.Load(); // Tải dữ liệu thống kê của kẻ thù từ hệ thống lưu trữ

        StatsCaculate(); // Tính toán các chỉ số của kẻ thù

        // Đăng ký sự kiện khi kẻ thù chết
        OnDead.AddListener(OnSpawnColectable); // Gọi phương thức để sinh ra vật phẩm khi kẻ thù chết
        OnDead.AddListener(OnAddXpToPlayer); // Gọi phương thức để thêm XP cho người chơi khi kẻ thù chết
    }

    // Tính toán các chỉ số của kẻ thù
    private void StatsCaculate()
    {
        var playerStats = m_player.PlayerStats; // Lấy thống kê của người chơi

        if (playerStats == null) return; // Nếu không có thống kê của người chơi thì thoát khỏi hàm

        // Tính toán lượng máu tăng thêm dựa trên cấp độ của người chơi
        float hpUpgrade = m_enemyStats.hpUp * Helper.GetUpgradeFormuala(playerStats.level + 1);
        // Tính toán lượng sát thương tăng thêm dựa trên cấp độ của người chơi
        float damageUgrade = m_enemyStats.damageUp * Helper.GetUpgradeFormuala(playerStats.level + 1);
        // Tạo giá trị ngẫu nhiên cho XP thưởng
        float randomXpBonus = Random.Range(m_enemyStats.minXpBonus, m_enemyStats.maxXpBonus);

        // Cập nhật các chỉ số hiện tại của kẻ thù
        CurHp = m_enemyStats.hp + hpUpgrade; // Cập nhật lượng máu hiện tại
        CurDamage = m_enemyStats.damage * damageUgrade; // Cập nhật sát thương hiện tại
        m_xpBonus = randomXpBonus * Helper.GetUpgradeFormuala(playerStats.level + 1); // Cập nhật lượng XP thưởng
    }

    // Phương thức được gọi khi kẻ thù chết
    protected override void Die()
    {
        base.Die(); // Gọi phương thức chết của lớp cơ sở

        m_amin.SetTrigger(AminConst.ENEMY_DEAD_PARAM); // Kích hoạt animation chết của kẻ thù
    }

    // Phương thức để sinh ra vật phẩm khi kẻ thù chết
    private void OnSpawnColectable()
    {
        CollectableManager.Ins.Spawn(transform.position); // Sinh ra vật phẩm tại vị trí của kẻ thù
    }

    // Phương thức để thêm XP cho người chơi khi kẻ thù chết
    private void OnAddXpToPlayer()
    {
        GameManager.Ins.Player.AddXp(m_xpBonus); // Thêm XP cho người chơi
    }

    // Phương thức FixedUpdate được gọi trong mỗi khung hình vật lý
    private void FixedUpdate()
    {
        Move(); // Gọi phương thức di chuyển của kẻ thù
    }

    // Phương thức di chuyển của kẻ thù
    protected override void Move()
    {
        // Nếu kẻ thù đã chết hoặc không tìm thấy người chơi thì không di chuyển
        if (IsDead || m_player == null) return;

        // Tính toán hướng di chuyển về phía người chơi
        Vector2 playerDir = m_player.transform.position - transform.position;
        playerDir.Normalize(); // Chuẩn hóa hướng di chuyển

        // Nếu kẻ thù không bị hất văng
        if (!m_isKnockback)
        {
            FlipTool(playerDir); // Xử lý việc lật hình kẻ thù nếu cần thiết
            m_rb.velocity = playerDir * m_enemyStats.moveSpeed * Time.deltaTime; // Cập nhật vận tốc di chuyển
            return;
        }

        // Nếu kẻ thù bị hất văng, di chuyển ngược lại
        m_rb.velocity = playerDir * -m_enemyStats.knockbackForce * Time.deltaTime;
    }

    // Phương thức để lật hình kẻ thù nếu cần thiết
    private void FlipTool(Vector2 playerDir)
    {
        // Nếu hướng di chuyển là về phía bên phải
        if (playerDir.x > 0)
        {
            // Nếu kẻ thù đã hướng về bên phải thì không làm gì cả
            if (transform.localScale.x > 0) return;
            // Ngược lại, lật hình kẻ thù để hướng về bên phải
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        }
        // Nếu hướng di chuyển là về phía bên trái
        else if (playerDir.x < 0)
        {
            // Nếu kẻ thù đã hướng về bên trái thì không làm gì cả
            if (transform.localScale.x < 0) return;
            // Ngược lại, lật hình kẻ thù để hướng về bên trái
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        }
    }

    // Phương thức được gọi khi đối tượng bị vô hiệu hóa
    private void OnDisable()
    {
        OnDead.RemoveListener(OnSpawnColectable); // Gỡ bỏ sự kiện sinh vật phẩm khi kẻ thù chết
        OnDead.RemoveListener(OnAddXpToPlayer); // Gỡ bỏ sự kiện thêm XP cho người chơi khi kẻ thù chết
    }
}
