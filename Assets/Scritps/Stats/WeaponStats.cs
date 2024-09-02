using System;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86.Avx;

// Tạo một menu tùy chỉnh trong Unity Editor để tạo đối tượng ScriptableObject cho các thông số của vũ khí
[CreateAssetMenu(fileName = "Weapon Stats", menuName = "Nora/TDS/Create Weapon Stats")]
public class WeaponStats : Stats
{
    [Header("Base Stats")]
    public int bullets; // Số lượng đạn hiện tại của vũ khí
    public float firerate; // Tốc độ bắn của vũ khí (số phát bắn mỗi giây)
    public float reloadTime; // Thời gian nạp đạn của vũ khí
    public float damage; // Lượng sát thương mà vũ khí gây ra

    [Header("Upgrade")]
    public int level; // Cấp độ nâng cấp hiện tại của vũ khí
    public int maxLevel; // Cấp độ tối đa mà vũ khí có thể đạt được
    public int bulletUp; // Số lượng đạn được tăng thêm mỗi khi nâng cấp
    public float firerateUp; // Tăng tốc độ bắn mỗi khi nâng cấp
    public float reloadUp; // Giảm thời gian nạp đạn mỗi khi nâng cấp
    public float damageUp; // Tăng lượng sát thương mỗi khi nâng cấp
    public int upgradePrice; // Giá nâng cấp của vũ khí
    public int upgradePriceUp; // Giá trị tăng thêm vào giá nâng cấp mỗi khi vũ khí lên cấp

    [Header("Limit:")]
    public float minFirerate = 0.1f; // Tốc độ bắn tối thiểu của vũ khí
    public float minReloadTime = 0.1f; // Thời gian nạp đạn tối thiểu của vũ khí

    //Các thuộc tính này được sử dụng để cung cấp thông tin về các giá trị tăng thêm của vũ khí khi nâng cấp.Chúng giúp dễ dàng truy cập các thông tin quan trọng về nâng cấp mà không cần phải tính toán lại mỗi lần cần sử dụng.
    // Thông tin về lượng đạn được tăng thêm sau mỗi lần nâng cấp
    public int BulletUpInfo { get => bulletUp * (level + 1); }

    // Thông tin về tốc độ bắn được tăng thêm sau mỗi lần nâng cấp
    public float FirearateUpInfo { get => firerateUp * Helper.GetUpgradeFormuala(level + 1); }

    // Thông tin về thời gian nạp đạn được giảm sau mỗi lần nâng cấp
    public float ReloadTimeUpInfo { get => reloadTime * Helper.GetUpgradeFormuala(level + 1); }

    // Thông tin về lượng sát thương được tăng thêm sau mỗi lần nâng cấp
    public float DamageUpInfo { get => damageUp * Helper.GetUpgradeFormuala(level + 1); }

    // Kiểm tra xem vũ khí đã đạt cấp độ tối đa hay chưa
    public override bool IsMaxLevel()
    {
        return level >= maxLevel; // Trả về true nếu cấp độ hiện tại >= cấp độ tối đa
    }

    // Phương thức Load: Tải dữ liệu vũ khí từ PlayerPrefs
    public override void Load()
    {
        // Nếu dữ liệu vũ khí không rỗng, tải dữ liệu vào đối tượng này
        if (!string.IsNullOrEmpty(Pref.playerWeaponData))
        {
            JsonUtility.FromJsonOverwrite(Pref.playerWeaponData, this); // Tải dữ liệu từ JSON vào đối tượng
        }
    }

    // Phương thức Save: Lưu dữ liệu vũ khí vào PlayerPrefs
    public override void Save()
    {
        Pref.playerWeaponData = JsonUtility.ToJson(this); // Chuyển đổi đối tượng này thành JSON và lưu lại
    }

    // Phương thức Upgrade: Xử lý việc nâng cấp cấp độ của vũ khí
    public override void Upgrade(Action OnSuccess = null, Action OnFalled = null)
    {
        // Kiểm tra nếu người chơi đủ tiền và vũ khí chưa đạt cấp độ tối đa
        if (Pref.IsEnoughCoins(upgradePrice) && !IsMaxLevel())
        {
            // Trừ đi số tiền tương ứng với giá nâng cấp
            Pref.coins -= upgradePrice;

            // Tăng cấp độ của vũ khí
            level++;

            // Tăng số lượng đạn của vũ khí
            bullets += bulletUp * level;

            // Tăng tốc độ bắn của vũ khí và đảm bảo nó không vượt quá giới hạn tối thiểu
            firerate -= firerateUp * Helper.GetUpgradeFormuala(level);
            firerate = Mathf.Clamp(firerate, minFirerate, firerate);

            // Giảm thời gian nạp đạn của vũ khí và đảm bảo nó không vượt quá giới hạn tối thiểu
            reloadTime -= reloadTime * Helper.GetUpgradeFormuala(level);
            reloadTime = Mathf.Clamp(reloadTime, minReloadTime, reloadTime);

            // Tăng lượng sát thương của vũ khí
            damage += damageUp * Helper.GetUpgradeFormuala(level);

            // Tăng giá nâng cấp cho lần tiếp theo
            upgradePrice += upgradePriceUp * level;

            // Lưu lại trạng thái mới sau khi nâng cấp
            Save();

            // Gọi callback nếu việc nâng cấp thành công
            OnSuccess?.Invoke();

            return;
        }

        // Nếu không đủ tiền hoặc đã đạt cấp độ tối đa, gọi OnFalled
        OnFalled?.Invoke();
    }
}
