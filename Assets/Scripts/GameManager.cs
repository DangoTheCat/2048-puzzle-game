using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// Quản lý logic chính của game 2048: điểm số, bắt đầu game mới, kết thúc game.
/// </summary>
public class GameManager : MonoBehaviour
{
    public TileBoard board;          // Tham chiếu đến bảng chơi (TileBoard)
    public CanvasGroup gameOver;     // UI hiển thị màn hình "Game Over"
    public TextMeshProUGUI scoreText;     // Text hiển thị điểm hiện tại
    public TextMeshProUGUI highScoreText; // Text hiển thị điểm cao nhất

    private int score; // Điểm hiện tại của người chơi

    /// <summary>
    /// Được gọi khi game bắt đầu. Tự động khởi tạo game mới.
    /// </summary>
    private void Start()
    {
        NewGame();
    }

    /// <summary>
    /// Bắt đầu một ván game mới:
    /// 1. Đặt điểm về 0
    /// 2. Hiển thị điểm cao nhất đã lưu
    /// 3. Ẩn màn hình "Game Over"
    /// 4. Xóa toàn bộ bảng rồi tạo 2 tile ban đầu
    /// 5. Bật lại input cho bảng chơi
    /// </summary>
    public void NewGame(){
        SetScore(0);
        highScoreText.text = LoadHighScore().ToString();
        
        gameOver.alpha = 0f;            // Ẩn UI game over (trong suốt)
        gameOver.interactable = false;  // Không cho tương tác với UI game over

        board.ClearBoard();   // Xóa tất cả tile cũ
        board.SpawnTile();    // Tạo tile đầu tiên
        board.SpawnTile();    // Tạo tile thứ hai
        board.enabled = true; // Cho phép người chơi thao tác
    }

    /// <summary>
    /// Xử lý khi game kết thúc:
    /// 1. Tắt input của bảng chơi (không cho di chuyển nữa)
    /// 2. Cho phép tương tác với màn hình Game Over (nút chơi lại)
    /// 3. Chạy hiệu ứng fade-in cho màn hình Game Over sau 1 giây
    /// </summary>
    public void GameOver(){
        board.enabled = false;          // Vô hiệu hóa input
        gameOver.interactable = true;   // Cho phép nhấn nút trên UI game over

        StartCoroutine(Fade(gameOver, 1f, 1f)); // Fade-in UI game over
    }

    /// <summary>
    /// Coroutine tạo hiệu ứng fade (mờ dần / hiện dần) cho một CanvasGroup.
    /// - canvasGroup: đối tượng UI cần fade
    /// - to: giá trị alpha đích (0 = trong suốt, 1 = hiện hoàn toàn)
    /// - delay: thời gian chờ trước khi bắt đầu hiệu ứng (giây)
    /// Hiệu ứng diễn ra trong 0.5 giây, sử dụng Lerp để chuyển đổi mượt mà.
    /// </summary>
    private IEnumerator Fade(CanvasGroup canvasGroup, float to, float delay){
        yield return new WaitForSeconds(delay); // Chờ trước khi bắt đầu fade

        float elapsed = 0f;
        float duration = 0.5f;              // Thời gian fade kéo dài 0.5 giây
        float from = canvasGroup.alpha;     // Lấy giá trị alpha ban đầu

        while(elapsed < duration){
            // Nội suy tuyến tính (Lerp) từ alpha hiện tại đến alpha đích
            canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null; // Chờ đến frame tiếp theo
        }

        canvasGroup.alpha = to; // Đảm bảo đạt đúng giá trị đích
    }

    /// <summary>
    /// Tăng điểm hiện tại thêm một lượng "points".
    /// Được gọi khi hai tile hợp nhất thành công.
    /// </summary>
    public void IncreaseScore(int points){
        SetScore(score + points);
    }

    /// <summary>
    /// Cập nhật điểm:
    /// 1. Gán giá trị điểm mới
    /// 2. Hiển thị điểm lên UI
    /// 3. Lưu điểm cao nếu cần
    /// </summary>
    private void SetScore(int score){
        this.score = score;
        scoreText.text = score.ToString();
        SaveHighScore();
    }

    /// <summary>
    /// Lưu điểm cao nhất vào PlayerPrefs (bộ nhớ cục bộ của Unity).
    /// Chỉ lưu khi điểm hiện tại lớn hơn điểm cao đã lưu trước đó.
    /// </summary>
    private void SaveHighScore(){
        int highScore = LoadHighScore();
        if(score > highScore){
            PlayerPrefs.SetInt("highScore", score);
        }
    }

    /// <summary>
    /// Đọc điểm cao nhất từ PlayerPrefs.
    /// Trả về 0 nếu chưa có điểm cao nào được lưu.
    /// </summary>
    private int LoadHighScore(){
        return PlayerPrefs.GetInt("highScore", 0);
    }
}
