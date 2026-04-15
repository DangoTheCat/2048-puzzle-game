using UnityEngine;

/// <summary>
/// Đại diện cho một ô (cell) trên lưới bảng chơi 2048.
/// Mỗi ô có tọa độ và có thể chứa hoặc không chứa một tile.
/// </summary>
public class TileCell : MonoBehaviour
{
    /// <summary>
    /// Tọa độ (x, y) của ô trên lưới.
    /// x = cột, y = hàng. Được gán bởi TileGrid.Start().
    /// </summary>
    public Vector2Int coordinates { get; set; }

    /// <summary>
    /// Tile đang nằm trên ô này. Null nếu ô trống.
    /// Liên kết 2 chiều: tile.cell ↔ cell.tile.
    /// </summary>
    public Tile tile { get; set; }

    /// <summary>
    /// Trả về true nếu ô đang trống (không có tile nào).
    /// </summary>
    public bool empty => tile == null;

    /// <summary>
    /// Trả về true nếu ô đang có tile.
    /// </summary>
    public bool occupied => tile != null;
}
