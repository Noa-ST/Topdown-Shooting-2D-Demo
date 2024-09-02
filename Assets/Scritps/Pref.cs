using UnityEngine;

// Lớp Pref dùng để quản lý dữ liệu lưu trữ của trò chơi.
public class Pref
{
    // Thuộc tính 'coins' lưu trữ và lấy số lượng coin hiện tại của người chơi.
    public static int coins
    {
        // Khi thiết lập giá trị cho 'coins', giá trị sẽ được lưu vào PlayerPrefs với key là PrefConst.COIN_KEY.
        set => PlayerPrefs.SetInt(PrefConst.COIN_KEY, value);

        // Khi lấy giá trị của 'coins', giá trị sẽ được lấy từ PlayerPrefs với key là PrefConst.COIN_KEY.
        // Nếu key không tồn tại, sẽ trả về giá trị mặc định là 0.
        get => PlayerPrefs.GetInt(PrefConst.COIN_KEY, 0);
    }

    // Thuộc tính 'playerData' lưu trữ và lấy dữ liệu người chơi (có thể là điểm số, cấp độ,...).
    public static string playerData
    {
        set => PlayerPrefs.SetString(PrefConst.PLAYER_DATA_KEY, value);
        get => PlayerPrefs.GetString(PrefConst.PLAYER_DATA_KEY);
    }

    // Thuộc tính 'playerWeaponData' lưu trữ và lấy dữ liệu về vũ khí của người chơi.
    public static string playerWeaponData
    {
        set => PlayerPrefs.SetString(PrefConst.PLAYER_WEAPON_DATA_KEY, value);
        get => PlayerPrefs.GetString(PrefConst.PLAYER_WEAPON_DATA_KEY);
    }

    // Thuộc tính 'enemyData' lưu trữ và lấy dữ liệu về kẻ thù (có thể là số lượng kẻ thù đã tiêu diệt, cấp độ kẻ thù,...).
    public static string enemyData
    {
        set => PlayerPrefs.SetString(PrefConst.ENEMY_DATA_KEY, value);
        get => PlayerPrefs.GetString(PrefConst.ENEMY_DATA_KEY);
    }

    // Phương thức IsEnoughCoins kiểm tra xem người chơi có đủ số lượng coin cần thiết hay không.
    // Tham số 'coinToCheck' là số coin cần kiểm tra.
    public static bool IsEnoughCoins(int coinToCheck)
    {
        // Nếu số lượng coin hiện tại của người chơi lớn hơn hoặc bằng số coin cần kiểm tra, trả về true.
        // Ngược lại, trả về false.
        return coins >= coinToCheck;
    }
}
