using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Đại diện cho một ô số (tile) trên bảng chơi 2048.
/// Chứa giá trị số, trạng thái hiển thị, và xử lý animation di chuyển/merge.
/// </summary>
public class Tile : MonoBehaviour
{
    public TileState state { get; private set; }  // Trạng thái hiện tại (màu nền, màu chữ)
    public TileCell cell { get; private set; }    // Ô (cell) mà tile đang nằm trên
    public int number { get; private set; }       // Giá trị số của tile (2, 4, 8, 16, ...)
    public bool locked { get; set; }              // Cờ khóa: true = tile đã merge trong lượt này, không được merge thêm

    private Image background;          // Component Image (ảnh nền) của tile
    private TextMeshProUGUI text;      // Component Text hiển thị giá trị số

    /// <summary>
    /// Khởi tạo: lấy reference đến các component UI (Image và TextMeshProUGUI).
    /// </summary>
    private void Awake()
    {
        background = GetComponent<Image>();
        text = GetComponentInChildren<TextMeshProUGUI>();
    }

    /// <summary>
    /// Cập nhật trạng thái hiển thị của tile:
    /// - state: chứa thông tin màu nền và màu chữ
    /// - number: giá trị số mới
    /// Đổi màu nền, màu chữ, và hiển thị số tương ứng.
    /// </summary>
    public void SetState(TileState state, int number)
    {
        this.state = state;
        this.number = number;

        background.color = state.backgroundColor; // Đổi màu nền
        text.color = state.textColor;             // Đổi màu chữ
        text.text = number.ToString();            // Hiển thị số
    }

    /// <summary>
    /// Đặt tile vào một ô (cell) khi vừa được tạo (spawn):
    /// 1. Nếu tile đang nằm ở ô cũ → gỡ liên kết (cell cũ.tile = null)
    /// 2. Gán tile vào ô mới (liên kết 2 chiều: tile ↔ cell)
    /// 3. Đặt vị trí transform trực tiếp (không có animation)
    /// </summary>
    public void Spawn(TileCell cell)
    {
        if (this.cell != null)
        {
            this.cell.tile = null; // Gỡ tile khỏi ô cũ
        }
        this.cell = cell;          // Gán ô mới cho tile
        this.cell.tile = this;     // Gán tile cho ô mới

        transform.position = cell.transform.position; // Đặt vị trí ngay lập tức
    }
    
    /// <summary>
    /// Di chuyển tile đến một ô mới (có animation trượt):
    /// 1. Gỡ liên kết tile khỏi ô cũ
    /// 2. Liên kết tile với ô mới
    /// 3. Chạy animation di chuyển mượt mà đến vị trí ô mới
    /// </summary>
    public void MoveTo(TileCell cell)
    {
        if (this.cell != null)
        {
            this.cell.tile = null; // Gỡ tile khỏi ô cũ
        }
        this.cell = cell;          // Gán ô mới
        this.cell.tile = this;     // Liên kết ô mới với tile này

        StartCoroutine(Animate(cell.transform.position, false)); // Animation trượt, không hủy
    }

    /// <summary>
    /// Hợp nhất (merge) tile này vào một ô đích:
    /// 1. Gỡ liên kết tile khỏi ô hiện tại
    /// 2. Đặt cell = null (tile này sẽ bị hủy sau animation)
    /// 3. Khóa tile đích (cell.tile.locked = true) để không merge lần nữa trong lượt này
    /// 4. Chạy animation di chuyển đến ô đích, sau đó tự hủy (Destroy)
    /// </summary>
    public void Merge(TileCell cell)
    {
        if (this.cell != null)
        {
            this.cell.tile = null; // Gỡ liên kết ô cũ
        }
        
        this.cell = null;           // Tile này sẽ bị hủy, không thuộc ô nào nữa
        cell.tile.locked = true;    // Khóa tile đích (ngăn merge chuỗi)

        StartCoroutine(Animate(cell.transform.position, true)); // Animation + hủy sau khi xong
    }

    /// <summary>
    /// Coroutine tạo animation di chuyển mượt mà cho tile:
    /// - to: vị trí đích
    /// - merging: nếu true → hủy tile sau khi animation kết thúc
    /// 
    /// Sử dụng Vector3.Lerp để nội suy vị trí trong 0.1 giây.
    /// Sau khi hoàn tất, đặt chính xác vị trí đích.
    /// Nếu đang merge, Destroy(gameObject) để xóa tile khỏi scene.
    /// </summary>
    private IEnumerator Animate(Vector3 to, bool merging)
    {
        float elapsed = 0f;
        float duration = 0.1f;                  // Animation kéo dài 0.1 giây
        Vector3 from = transform.position;      // Vị trí xuất phát
        while (elapsed < duration)
        {
            // Nội suy vị trí từ from → to
            transform.position = Vector3.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null; // Chờ frame tiếp theo
        }
        transform.position = to; // Đảm bảo đúng vị trí đích
        if(merging){
            Destroy(gameObject); // Hủy tile nếu đang merge
        }
    }
}
