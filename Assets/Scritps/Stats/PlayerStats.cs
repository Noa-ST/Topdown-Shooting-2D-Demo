using System;
using UnityEngine;

// Tạo một menu tùy chỉnh trong Unity Editor để tạo đối tượng ScriptableObject
[CreateAssetMenu(fileName = "Player Stats", menuName = "Nora/TDS/Create Player Stats")]
public class PlayerStats : ActionStats
{
    [Header("Level Up Base:")]
    public int level; // Cấp độ hiện tại của người chơi
    public int maxLevel; // Cấp độ tối đa mà người chơi có thể đạt được
    public float xp; // Điểm kinh nghiệm hiện tại của người chơi
    public float levelUpXpRequied; // Số XP cần thiết để lên cấp độ tiếp theo

    [Header("Level Up:")]
    public float levelUpXpUpRequireUp; // Giá trị tăng thêm của XP cần thiết để lên cấp sau mỗi lần nâng cấp
    public float hpUp; // Giá trị HP tăng thêm mỗi lần lên cấp

    // Kiểm tra xem người chơi đã đạt cấp độ tối đa hay chưa
    public override bool IsMaxLevel()
    {
        return level >= maxLevel; // Trả về true nếu cấp độ hiện tại >= cấp độ tối đa
    }

    // Phương thức Load: Tải dữ liệu người chơi từ PlayerPrefs
    public override void Load()
    {
        // Nếu dữ liệu người chơi không rỗng, tải dữ liệu vào đối tượng này
        if (!string.IsNullOrEmpty(Pref.playerData))
        {
            JsonUtility.FromJsonOverwrite(Pref.playerData, this); // Tải dữ liệu từ JSON vào đối tượng
        }
    }

    // Phương thức Save: Lưu dữ liệu người chơi vào PlayerPrefs
    public override void Save()
    {
        Pref.playerData = JsonUtility.ToJson(this); // Chuyển đổi đối tượng này thành JSON và lưu lại
    }

    // Phương thức Upgrade: Xử lý việc nâng cấp cấp độ của người chơi
    public override void Upgrade(Action OnSuccess = null, Action OnFalled = null)
    {
        // Nếu người chơi đã đạt cấp độ tối đa, gọi OnFalled và thoát khỏi phương thức
        if (IsMaxLevel())
        {
            OnFalled?.Invoke();
            return;
        }

        // Công thức tính giá trị nâng cấp

        // Vòng lặp để kiểm tra và nâng cấp cấp độ cho đến khi không đủ XP hoặc đã đạt cấp độ tối đa
        while (xp >= levelUpXpRequied && !IsMaxLevel())
        {
            level++; // Tăng cấp độ
            xp -= levelUpXpRequied; // Trừ đi XP cần thiết cho lần nâng cấp hiện tại

            // Tăng giá trị HP và XP cần thiết cho lần nâng cấp tiếp theo bằng công thức tính giá trị nâng cấp ở class tĩnh Helper
            hp += hpUp * Helper.GetUpgradeFormuala(level);
            levelUpXpRequied += levelUpXpUpRequireUp * Helper.GetUpgradeFormuala(level);

            Save(); // Lưu lại trạng thái mới sau khi nâng cấp

            OnSuccess?.Invoke(); // Gọi callback nếu việc nâng cấp thành công
        }

        // Nếu XP không đủ hoặc đã đạt cấp độ tối đa, gọi OnFalled
        if (xp < levelUpXpRequied || IsMaxLevel())
        {
            OnFalled?.Invoke();
        }
    }
}

//levelUpXpRequied:
//Đây là giá trị XP (điểm kinh nghiệm) mà người chơi cần phải có để lên cấp từ cấp hiện tại lên cấp tiếp theo.
//Ví dụ: Nếu levelUpXpRequied là 100, người chơi cần tích lũy đủ 100 XP để từ cấp độ hiện tại lên cấp độ tiếp theo.
//Giá trị này sẽ tăng sau mỗi lần người chơi lên cấp, dựa trên công thức tính được áp dụng trong phương thức Upgrade(). Sau khi lên cấp, levelUpXpRequied sẽ tăng lên một giá trị mới để chuẩn bị cho lần lên cấp tiếp theo.

//levelUpXpUpRequireUp:
//Đây là giá trị tăng thêm của XP cần thiết (levelUpXpRequied) sau mỗi lần người chơi lên cấp.
//Ví dụ: Nếu levelUpXpUpRequireUp là 50, sau khi người chơi lên cấp, giá trị levelUpXpRequied cho lần lên cấp tiếp theo sẽ được tăng thêm 50. Nghĩa là nếu levelUpXpRequied ban đầu là 100, sau khi lên cấp, nó sẽ trở thành 150.
//Giá trị này được cộng thêm vào levelUpXpRequied để tăng độ khó của trò chơi, yêu cầu người chơi tích lũy nhiều XP hơn để lên cấp sau mỗi lần lên cấp.