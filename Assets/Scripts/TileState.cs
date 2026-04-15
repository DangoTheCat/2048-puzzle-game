using UnityEngine;

/// <summary>
/// ScriptableObject lưu trạng thái hiển thị của một tile.
/// Mỗi giá trị tile (2, 4, 8, 16, ...) sẽ có một TileState riêng
/// với màu nền và màu chữ khác nhau.
/// 
/// Tạo asset mới trong Unity: Right-click → Create → Tile State
/// </summary>
[CreateAssetMenu(menuName = "Tile State")]
public class TileState : ScriptableObject
{
    /// <summary>Màu nền của tile.</summary>
    public Color backgroundColor;

    /// <summary>Màu chữ (số) hiển thị trên tile.</summary>
    public Color textColor;
}
