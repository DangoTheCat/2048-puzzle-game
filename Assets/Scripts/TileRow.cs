using UnityEngine;

/// <summary>
/// Đại diện cho một hàng (row) trong lưới bảng chơi 2048.
/// Chứa mảng các TileCell (ô) thuộc hàng này.
/// </summary>
public class TileRow : MonoBehaviour
{
    /// <summary>
    /// Mảng các ô (TileCell) trong hàng này.
    /// Được tự động lấy từ các TileCell con trong Awake().
    /// </summary>
    public TileCell[] cells { get; private set; }

    /// <summary>
    /// Khởi tạo: tự động tìm và lưu tất cả TileCell con vào mảng cells.
    /// </summary>
    private void Awake()
    {
        cells = GetComponentsInChildren<TileCell>();
    }
}
