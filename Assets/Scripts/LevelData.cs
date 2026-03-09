/// <summary>
/// Dữ liệu tĩnh cho 6 màn chơi của game Endless Runner HPC.
/// Mỗi màn có số tile (mỗi tile = 30m) và tên hiển thị.
/// </summary>
public static class LevelData
{
    public const int TOTAL_LEVELS = 6;

    /// <summary>
    /// Số tile tới đích cho mỏi màn (index 0 = Màn 1).
    /// Tile dài 30m → Màn 1: 17×30 ≈ 510m, Màn 6: 100×30 = 3000m
    /// </summary>
    public static readonly int[] TileCounts = { 17, 33, 50, 67, 83, 100 };

    /// <summary>
    /// Tên hiển thị cho từng màn.
    /// </summary>
    public static readonly string[] LevelNames =
    {
        "Màn 1 - Khởi Đầu",
        "Màn 2 - Tăng Tốc",
        "Màn 3 - Thử Thách",
        "Màn 4 - Gian Nan",
        "Màn 5 - Siêu Tốc",
        "Màn 6 - Cổng Trường HPC"
    };

    /// <summary>
    /// Khoảng cách tương đương (m) cho mỗi màn.
    /// </summary>
    public static readonly int[] DistanceMeters = { 510, 990, 1500, 2010, 2490, 3000 };

    /// <summary>
    /// Mô tả độ khó.
    /// </summary>
    public static readonly string[] DifficultyLabels =
    {
        "Dễ",
        "Trung Bình",
        "Trung Bình+",
        "Khó",
        "Rất Khó",
        "Boss"
    };
}
