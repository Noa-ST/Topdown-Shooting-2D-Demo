using System;
using UnityEngine;

// Lớp Stats là một lớp trừu tượng, kế thừa từ ScriptableObject.
public abstract class Stats : ScriptableObject
{
    // Phương thức trừu tượng Save, có nhiệm vụ lưu trữ dữ liệu của Stats.
    public abstract void Save();

    // Phương thức trừu tượng Load, có nhiệm vụ tải dữ liệu của Stats.
    public abstract void Load();

    // Phương thức trừu tượng Upgrade, có nhiệm vụ nâng cấp một thuộc tính hoặc một khả năng nào đó.
    // Nó nhận vào hai tham số là các hành động (Action) sẽ được gọi khi nâng cấp thành công (OnSuccess)
    // hoặc khi thất bại (OnFailed). Cả hai tham số đều là tùy chọn (nullable).
    public abstract void Upgrade(Action OnSuccess = null, Action OnFalled = null);

    // Phương thức trừu tượng IsMaxLevel, kiểm tra xem thuộc tính này đã đạt cấp độ tối đa chưa.
    public abstract bool IsMaxLevel();
}
