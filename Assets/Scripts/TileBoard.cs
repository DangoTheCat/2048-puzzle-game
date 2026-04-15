using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Quản lý bảng chơi 2048: tạo tile, xử lý di chuyển, hợp nhất, kiểm tra game over.
/// </summary>
public class TileBoard : MonoBehaviour
{
    public GameManager gameManager;   // Tham chiếu đến GameManager để cập nhật điểm / game over
    public Tile tilePrefab;           // Prefab mẫu để tạo tile mới
    public TileState[] tileStates;    // Mảng các trạng thái tile (màu sắc tương ứng với giá trị 2, 4, 8, ...)

    private TileGrid grid;           // Lưới ô vuông của bảng chơi
    private List<Tile> tiles;        // Danh sách tất cả tile đang tồn tại trên bảng
    private bool waiting;            // Cờ ngăn input khi animation đang chạy

    /// <summary>
    /// Khởi tạo ban đầu: lấy reference đến TileGrid con và tạo danh sách tile rỗng (capacity 16 = 4x4).
    /// </summary>
    private void Awake()
    {
        grid = GetComponentInChildren<TileGrid>();
        tiles = new List<Tile>(16);
    }

    /// <summary>
    /// Xóa toàn bộ bảng chơi:
    /// 1. Gỡ liên kết tile khỏi tất cả các ô (cell.tile = null)
    /// 2. Hủy (Destroy) tất cả GameObject tile
    /// 3. Xóa danh sách tiles
    /// </summary>
    public void ClearBoard(){
        foreach(var cell in grid.cells){
            cell.tile = null;  // Gỡ liên kết tile khỏi ô
        }

        foreach(var tile in tiles){
            Destroy(tile.gameObject); // Xóa GameObject tile khỏi scene
        }
        tiles.Clear(); // Xóa danh sách
    }

    /// <summary>
    /// Tạo một tile mới trên bảng:
    /// 1. Instantiate một bản sao từ tilePrefab, đặt làm con của grid
    /// 2. Gán trạng thái ban đầu (tileStates[0]) với giá trị 2
    /// 3. Đặt tile vào một ô trống ngẫu nhiên trên bảng
    /// 4. Thêm tile vào danh sách quản lý
    /// </summary>
    public void SpawnTile()
    {
        Tile tile = Instantiate(tilePrefab, grid.transform);
        tile.SetState(tileStates[0], 2);           // Tile mới luôn bắt đầu với giá trị 2
        tile.Spawn(grid.GetRandomEmptyCell());      // Đặt vào ô trống ngẫu nhiên
        tiles.Add(tile);                            // Thêm vào danh sách quản lý
    }

    /// <summary>
    /// Chạy mỗi frame - xử lý input bàn phím của người chơi:
    /// - W / Mũi tên lên:    di chuyển tất cả tile lên
    /// - S / Mũi tên xuống:  di chuyển tất cả tile xuống
    /// - A / Mũi tên trái:   di chuyển tất cả tile sang trái
    /// - D / Mũi tên phải:   di chuyển tất cả tile sang phải
    /// 
    /// Nếu đang chờ animation (waiting = true) thì bỏ qua input.
    /// 
    /// Tham số truyền vào MoveTiles xác định thứ tự duyệt ô:
    /// - startX, incrementX: vị trí bắt đầu và bước nhảy theo trục X
    /// - startY, incrementY: vị trí bắt đầu và bước nhảy theo trục Y
    /// Mục đích: duyệt tile theo hướng di chuyển để tile gần cạnh đích được xử lý trước.
    /// </summary>
    private void Update() {
        if(!waiting){
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            // Di chuyển lên: duyệt từ hàng trên (y=1) xuống dưới
            MoveTiles(Vector2Int.up, 0, 1, 1, 1);
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            // Di chuyển xuống: duyệt từ hàng dưới (y=height-2) lên trên
            MoveTiles(Vector2Int.down, 0, 1, grid.height - 2, -1);
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            // Di chuyển trái: duyệt từ cột trái (x=1) sang phải
            MoveTiles(Vector2Int.left, 1, 1, 0, 1);
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            // Di chuyển phải: duyệt từ cột phải (x=width-2) sang trái
            MoveTiles(Vector2Int.right, grid.width - 2, -1, 0, 1);
        }}
    }

    /// <summary>
    /// Di chuyển tất cả tile theo một hướng nhất định.
    /// 
    /// - direction: hướng di chuyển (up/down/left/right)
    /// - startX, incrementX: xác định thứ tự duyệt các cột
    ///   (bắt đầu từ cột startX, mỗi lần tăng/giảm incrementX)
    /// - startY, incrementY: xác định thứ tự duyệt các hàng
    ///   (bắt đầu từ hàng startY, mỗi lần tăng/giảm incrementY)
    /// 
    /// Duyệt qua từng ô, nếu ô có tile thì cố gắng di chuyển tile đó.
    /// Nếu có bất kỳ tile nào thay đổi vị trí, gọi WaitForChanges()
    /// để chờ animation và xử lý hậu kỳ (spawn tile mới, kiểm tra game over).
    /// </summary>
    private void MoveTiles(Vector2Int direction, int startX, int incrementX, int startY, int incrementY)
    {   
        bool changed = false; // Theo dõi xem có tile nào thực sự di chuyển không
        for (int x = startX; x >= 0 && x < grid.width; x += incrementX)
        {
            for (int y = startY; y >= 0 && y < grid.height; y += incrementY)
            {
                TileCell cell = grid.GetCell(x, y);
                if (cell.occupied) // Nếu ô có tile
                {
                    changed |= MoveTile(cell.tile, direction); // Thử di chuyển tile
                }
            }
        }
        if(changed){
            StartCoroutine(WaitForChanges()); // Chờ animation rồi xử lý tiếp
        }
    }

    /// <summary>
    /// Di chuyển một tile đơn lẻ theo hướng cho trước.
    /// 
    /// Logic:
    /// 1. Lấy ô kề (adjacent) theo hướng di chuyển
    /// 2. Nếu ô kề trống → ghi nhớ ô đó và tiếp tục tìm ô xa hơn
    /// 3. Nếu ô kề có tile và có thể hợp nhất → thực hiện merge, trả về true
    /// 4. Nếu ô kề có tile nhưng không merge được → dừng lại
    /// 5. Cuối cùng, nếu tìm được ô trống mới → di chuyển tile đến đó
    /// 
    /// Trả về true nếu tile đã di chuyển hoặc merge, false nếu không thay đổi.
    /// </summary>
    private bool MoveTile(Tile tile, Vector2Int direction)
    {
        TileCell newCell = null;
        TileCell adjacent = grid.GetAdjacentCell(tile.cell, direction);

        while (adjacent != null) // Duyệt qua các ô liền kề theo hướng di chuyển
        {
            if (adjacent.occupied) // Ô kề có tile
            {
                if (CanMerge(tile, adjacent.tile)) // Kiểm tra có merge được không
                {
                    Merge(tile, adjacent.tile); // Thực hiện merge
                    return true;
                }
                break; // Không merge được → dừng
            }

            newCell = adjacent; // Ghi nhớ ô trống xa nhất
            adjacent = grid.GetAdjacentCell(adjacent, direction); // Tiếp tục kiểm tra ô tiếp theo
        }

        if (newCell != null)
        {
            tile.MoveTo(newCell); // Di chuyển tile đến ô trống xa nhất
            return true;
        }
        return false; // Tile không thể di chuyển
    }

    /// <summary>
    /// Kiểm tra xem hai tile có thể hợp nhất (merge) không.
    /// Điều kiện: cùng giá trị số VÀ tile đích chưa bị khóa (chưa merge trong lượt này).
    /// locked ngăn một tile merge nhiều lần trong cùng một lượt di chuyển.
    /// </summary>
    private bool CanMerge(Tile a, Tile b){
        return a.number == b.number && !b.locked;
    }

    /// <summary>
    /// Hợp nhất tile a vào tile b:
    /// 1. Xóa tile a khỏi danh sách (sẽ bị Destroy sau animation)
    /// 2. Chạy animation di chuyển tile a đến vị trí tile b
    /// 3. Tính trạng thái mới: lấy index tiếp theo trong mảng tileStates (màu mới)
    /// 4. Cập nhật tile b với giá trị gấp đôi và trạng thái mới
    /// 5. Tăng điểm bằng giá trị mới
    /// </summary>
    private void Merge(Tile a, Tile b){
        tiles.Remove(a);        // Xóa tile a khỏi danh sách quản lý
        a.Merge(b.cell);        // Chạy animation merge (tile a di chuyển đến b rồi tự hủy)

        // Lấy trạng thái tiếp theo (index + 1), clamp để không vượt mảng
        int index = Mathf.Clamp(IndexOf(b.state) + 1, 0, tileStates.Length - 1);
        int number = b.number * 2; // Giá trị mới = gấp đôi

        b.SetState(tileStates[index], number); // Cập nhật tile b
        
        gameManager.IncreaseScore(number); // Tăng điểm
    }

    /// <summary>
    /// Tìm vị trí (index) của một TileState trong mảng tileStates.
    /// Dùng để xác định trạng thái tiếp theo khi merge.
    /// Trả về -1 nếu không tìm thấy.
    /// </summary>
    private int IndexOf(TileState state){
        for(int i = 0; i < tileStates.Length; i++){
            if(tileStates[i] == state){
                return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// Coroutine xử lý sau khi di chuyển:
    /// 1. Bật cờ waiting = true → chặn input trong khi animation chạy
    /// 2. Chờ 0.1 giây cho animation hoàn tất
    /// 3. Mở khóa tất cả tile (locked = false) cho lượt tiếp theo
    /// 4. Nếu còn ô trống → tạo tile mới
    /// 5. Kiểm tra game over → nếu hết nước đi → kết thúc game
    /// </summary>
    private IEnumerator WaitForChanges(){
        waiting = true;
        yield return new WaitForSeconds(0.1f); // Chờ animation
        waiting = false;

        foreach(var tile in tiles){
            tile.locked = false; // Mở khóa để tile có thể merge ở lượt sau
        }

        if(tiles.Count < grid.size){
            SpawnTile(); // Tạo tile mới nếu còn ô trống
        }

        if(CheckForGameOver()){
            gameManager.GameOver(); // Kết thúc game nếu hết nước đi
        }
    }

    /// <summary>
    /// Kiểm tra xem game đã kết thúc chưa.
    /// 
    /// Game chưa kết thúc nếu:
    /// - Số tile < số ô (vẫn còn ô trống)
    /// - Hoặc có ít nhất một cặp tile kề nhau cùng giá trị (có thể merge)
    /// 
    /// Game kết thúc khi: bảng đầy VÀ không còn nước merge nào.
    /// Duyệt 4 hướng (trên/dưới/trái/phải) cho mỗi tile để kiểm tra.
    /// </summary>
    private bool CheckForGameOver(){
        if(tiles.Count != grid.size){
            return false; // Còn ô trống → chưa game over
        }

        foreach(var tile in tiles){
            // Lấy tile ở 4 hướng kề
            TileCell up = grid.GetAdjacentCell(tile.cell, Vector2Int.up);
            TileCell down = grid.GetAdjacentCell(tile.cell, Vector2Int.down);
            TileCell left = grid.GetAdjacentCell(tile.cell, Vector2Int.left);
            TileCell right = grid.GetAdjacentCell(tile.cell, Vector2Int.right);

            // Nếu có bất kỳ hướng nào có thể merge → chưa game over
            if(up != null && CanMerge(tile, up.tile)){
                return false;
            }
            if(down != null && CanMerge(tile, down.tile)){
                return false;
            }
            if(left != null && CanMerge(tile, left.tile)){
                return false;
            }
            if(right != null && CanMerge(tile, right.tile)){
                return false;
            }
        }
        return true; // Bảng đầy + không merge được → Game Over
    }

}
