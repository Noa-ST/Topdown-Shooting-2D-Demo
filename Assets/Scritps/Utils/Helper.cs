
using static Cinemachine.DocumentationSortingAttribute;

public static class Helper
{
   public static float GetUpgradeFormuala(int level)
    {
        return (level / 2 - 0.5f) * 0.5f;
    }
}

//Phân tích công thức
//level / 2:
//Ý nghĩa: Đây là bước đầu tiên của công thức, nơi giá trị của level được chia đôi.
//Mục đích: Chia đôi cấp độ này có thể là để làm chậm lại hoặc giảm bớt tốc độ tăng trưởng của giá trị khi cấp độ tăng cao

//level / 2 - 0.5f:
//Ý nghĩa: Sau khi chia cấp độ cho 2, công thức trừ đi 0.5f.
//Mục đích: Trừ 0.5f là một cách để điều chỉnh giá trị đầu ra sao cho nó thấp hơn một chút so với phép chia ban đầu, có thể để tạo ra một đường cong giảm tốc độ (làm giá trị tăng chậm lại khi cấp độ tăng lên).

//(level / 2 - 0.5f) * 0.5f:
//Ý nghĩa: Toàn bộ biểu thức được nhân với 0.5f.
//Mục đích: Nhân với 0.5f là để giảm giá trị cuối cùng của công thức xuống một nửa, giúp giảm tốc độ tăng trưởng của giá trị khi cấp độ tăng.

//Ví dụ tính toán
//Giả sử bạn có cấp độ (level) là 4:

//Bước 1: level / 2 = 4 / 2 = 2
//Bước 2: 2 - 0.5 = 1.5
//Bước 3: 1.5 * 0.5 = 0.75
//Như vậy, với level = 4, hàm GetUpgradeFormuala sẽ trả về 0.75.

//Tóm tắt
//Công thức này được thiết kế để tạo ra một hệ số nhỏ tăng chậm dần khi cấp độ tăng lên.
//Mục đích của công thức này có thể là để đảm bảo rằng sự gia tăng của các thuộc tính sau khi nâng cấp không quá nhanh, tạo ra một cảm giác cân bằng trong trò chơi.
//Nó cũng có thể được điều chỉnh để đảm bảo rằng việc nâng cấp mang lại lợi ích có thể đo lường được nhưng không quá mạnh, đảm bảo trò chơi vẫn thử thách và thú vị.
