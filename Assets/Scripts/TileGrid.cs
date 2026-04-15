using UnityEngine;

/// <summary>
/// Quản lý lưới (grid) của bảng chơi 2048.
/// Chứa các hàng (TileRow) và các ô (TileCell), cung cấp hàm truy cập ô theo tọa độ.
/// </summary>
public class TileGrid : MonoBehaviour
{
    /// <summary>Mảng các hàng trong lưới, lấy từ các TileRow con.</summary>
    public TileRow[] rows { get; private set; }

    /// <summary>Mảng tất cả các ô trong lưới, lấy từ các TileCell con.</summary>
    public TileCell[] cells { get; private set; }

    /// <summary>Tổng số ô trong lưới (ví dụ: 4x4 = 16).</summary>
    public int size => cells.Length;

    /// <summary>Chiều cao của lưới (số hàng).</summary>
    public int height => rows.Length;

    /// <summary>Chiều rộng của lưới (số cột = tổng ô / số hàng).</summary>
    public int width => size / height;

    /// <summary>
    /// Khởi tạo: lấy tất cả TileRow và TileCell từ các GameObject con.
    /// </summary>
    private void Awake()
    {
        rows = GetComponentsInChildren<TileRow>();
        cells = GetComponentsInChildren<TileCell>();
    }

    /// <summary>
    /// Gán tọa độ (x, y) cho từng ô trong lưới.
    /// Duyệt qua từng hàng (y) và từng ô trong hàng (x),
    /// gán coordinates = (x, y) để sau này có thể truy xuất ô theo vị trí.
    /// </summary>
    private void Start()
    {
        for (int y = 0; y < rows.Length; y++)
        {
            for (int x = 0; x < rows[y].cells.Length; x++)
            {
                rows[y].cells[x].coordinates = new Vector2Int(x, y);
            }
        }
    }

    /// <summary>
    /// Lấy ô tại tọa độ (x, y).
    /// Kiểm tra biên: nếu x hoặc y nằm ngoài phạm vi lưới → trả về null.
    /// Nếu hợp lệ → trả về ô ở hàng y, cột x.
    /// </summary>
    public TileCell GetCell(int x, int y)
    {
        if (x >= 0 && x < width && y >= 0 && y < height)
        {       
            return rows[y].cells[x]; // Trả về ô tại vị trí [y][x]
        }else{
            return null; // Ngoài phạm vi → null
        }
    }

    /// <summary>
    /// Lấy ô tại tọa độ Vector2Int (overload tiện lợi).
    /// Chuyển tiếp đến GetCell(int x, int y).
    /// </summary>
    public TileCell GetCell(Vector2Int coordinates)
    {
        return GetCell(coordinates.x, coordinates.y);
    }

    /// <summary>
    /// Lấy ô kề (adjacent cell) theo một hướng nhất định.
    /// - direction: hướng di chuyển (Vector2Int.up/down/left/right)
    /// 
    /// Lưu ý: trục Y bị đảo (coordinates.y -= direction.y) vì trong Unity UI,
    /// row index tăng từ trên xuống dưới, nhưng Vector2Int.up có y = 1.
    /// Nên di chuyển "lên" (up) thực tế là giảm y trong mảng rows.
    /// 
    /// Trả về null nếu ô kề nằm ngoài phạm vi lưới.
    /// </summary>
    public TileCell GetAdjacentCell(TileCell cell, Vector2Int direction)
    {
        Vector2Int coordinates = cell.coordinates;
        coordinates.x += direction.x;   // Di chuyển theo trục X (trái/phải)
        coordinates.y -= direction.y;   // Di chuyển theo trục Y (đảo ngược)
        return GetCell(coordinates);    // Trả về ô tại tọa độ mới
    }

    /// <summary>
    /// Tìm một ô trống ngẫu nhiên trên lưới.
    /// 
    /// Thuật toán:
    /// 1. Chọn một index ngẫu nhiên trong mảng cells
    /// 2. Nếu ô đó đã có tile → tăng index lên 1 (vòng lặp tròn)
    /// 3. Nếu quay lại đúng index ban đầu → không còn ô trống → trả về null
    /// 4. Nếu tìm được ô trống → trả về ô đó
    /// 
    /// Đây là thuật toán tìm kiếm tuyến tính với bắt đầu ngẫu nhiên (linear probing).
    /// </summary>
    public TileCell GetRandomEmptyCell()
    {
        int index = Random.Range(0, cells.Length);  // Chọn vị trí bắt đầu ngẫu nhiên
        int startingIndex = index;                  // Ghi nhớ vị trí bắt đầu
        while (cells[index].occupied)               // Lặp nếu ô hiện tại đã có tile
        {
            index++;

            if (index >= cells.Length)
            {
                index = 0; // Quay lại đầu mảng (vòng lặp tròn)
            }

            if (index == startingIndex)
            {
                return null; // Đã duyệt hết mà không tìm thấy ô trống
            }
        }
        return cells[index]; // Trả về ô trống tìm được
    }
}
