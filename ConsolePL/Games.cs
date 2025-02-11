using System;
using System.Collections.Generic;

namespace Games
{
    public class GuessNumberGame
    {
        private enum Difficulty { Easy = 1, Medium, Hard }
        private const int EASY_ATTEMPTS = 15;
        private const int MEDIUM_ATTEMPTS = 10;
        private const int HARD_ATTEMPTS = 5;
        
        private int maxAttempts;
        private int remainingAttempts;
        private int secretNumber;
        private int score;
        private bool gameWon;
        private List<int> guessHistory = new List<int>();

        public void StartGame()
        {
            try
            {
                Console.CursorVisible = false;
                Console.Title = "Siêu Trò Chơi Đoán Số Pro";
                ShowWelcomeScreen();
                
                // Cho phép thoát bằng phím ESC ngay từ đầu
                if (!SelectDifficulty()) return;
                GenerateSecretNumber();
                
                Console.Clear();
                Console.WriteLine($"Bắt đầu game! Bạn có {maxAttempts} lượt đoán.");
                
                while (remainingAttempts > 0 && !gameWon)
                {
                    DisplayGameStatus();
                    Console.WriteLine("Nhấn ESC để thoát, hoặc nhập số để đoán.");

                    int guess = GetValidGuess();
                    if (guess == -1) // Nếu người dùng nhấn ESC
                    {
                        Console.WriteLine("\nBạn đã thoát khỏi trò chơi.");
                        return;
                    }

                    ProcessGuess(guess);
                }

                ShowGameResult();
            }
            catch (Exception ex)
            {
                Console.Clear();
                Console.WriteLine($"Lỗi: {ex.Message}");
            }
            finally
            {
                Console.CursorVisible = true;
                Console.WriteLine("\nNhấn phím bất kỳ để thoát...");
                Console.ReadKey();
            }
        }

        private void ShowWelcomeScreen()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("====================================");
            Console.WriteLine("   CHÀO MỪNG ĐẾN VỚI TRÒ ĐOÁN SỐ     ");
            Console.WriteLine("====================================");
            Console.ResetColor();
            Console.WriteLine("\nLuật chơi:");
            Console.WriteLine("- Tôi sẽ nghĩ một số từ 1 đến 100");
            Console.WriteLine("- Bạn phải đoán số đó trong giới hạn lượt");
            Console.WriteLine("- Sau mỗi lượt, tôi sẽ gợi ý số của bạn lớn/nhỏ hơn");
            Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
            Console.ReadKey();
        }

        private bool SelectDifficulty()
        {
            Console.Clear();
            Console.WriteLine("Chọn độ khó:");
            Console.WriteLine($"1. Dễ ({EASY_ATTEMPTS} lượt)");
            Console.WriteLine($"2. Trung bình ({MEDIUM_ATTEMPTS} lượt)");
            Console.WriteLine($"3. Khó ({HARD_ATTEMPTS} lượt)");
            Console.WriteLine("Nhấn ESC để thoát.");

            while (true)
            {
                Console.Write("Lựa chọn của bạn (1-3): ");

                // Đọc phím từ người dùng
                var key = Console.ReadKey(true);

                // Kiểm tra nếu người chơi nhấn ESC
                if (key.Key == ConsoleKey.Escape)
                {
                    Console.WriteLine("\nBạn đã thoát khỏi trò chơi.");
                    return false;
                }

                // Xử lý lựa chọn độ khó
                if (char.IsDigit(key.KeyChar))
                {
                    if (int.TryParse(key.KeyChar.ToString(), out int choice) && Enum.IsDefined(typeof(Difficulty), choice))
                    {
                        maxAttempts = choice switch
                        {
                            1 => EASY_ATTEMPTS,
                            2 => MEDIUM_ATTEMPTS,
                            3 => HARD_ATTEMPTS,
                            _ => MEDIUM_ATTEMPTS
                        };
                        remainingAttempts = maxAttempts;
                        return true;
                    }
                }

                Console.WriteLine("\nLựa chọn không hợp lệ, vui lòng nhập lại!");
            }
        }

        private void GenerateSecretNumber()
        {
            Random random = new Random();
            secretNumber = random.Next(1, 101);
            score = 1000;
        }

        private void DisplayGameStatus()
        {
            Console.WriteLine("\n------------------------------------");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Lượt còn lại: {remainingAttempts} | Điểm hiện tại: {score}");
            Console.ResetColor();
            Console.WriteLine("Lịch sử đoán: " + string.Join(", ", guessHistory));
        }

        private int GetValidGuess()
{
    string input = "";
    Console.Write("\nNhập số từ 1 đến 100: "); // In thông báo MỘT LẦN
    while (true)
    {
        var key = Console.ReadKey(intercept: true); // Đọc phím không hiển thị

        // Thoát nếu nhấn ESC
        if (key.Key == ConsoleKey.Escape)
        {
            return -1;
        }

        // Xử lý Backspace
        if (key.Key == ConsoleKey.Backspace)
        {
            if (input.Length > 0)
            {
                input = input[0..^1]; // Cắt bỏ ký tự cuối
                Console.Write("\b \b"); // Xóa trên console
            }
        }
        // Xử lý phím số
        else if (char.IsDigit(key.KeyChar))
        {
            input += key.KeyChar;
            Console.Write(key.KeyChar); // Hiển thị ký tự
        }
        // Xử lý Enter
        else if (key.Key == ConsoleKey.Enter)
        {
            if (input.Length == 0) continue; // Bỏ qua nếu chưa nhập

            if (int.TryParse(input, out int guess) && guess is >= 1 and <= 100)
            {
                return guess;
            }
            
            // Xử lý lỗi
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("\nSố không hợp lệ! Nhập lại: ");
            Console.ResetColor();
            input = "";
        }
    }
}

        private void ProcessGuess(int guess)
        {
            guessHistory.Add(guess);
            remainingAttempts--;
            score -= Math.Abs(secretNumber - guess);

            if (guess == secretNumber)
            {
                gameWon = true;
                return;
            }
            
            Console.WriteLine();
            Console.ForegroundColor = guess < secretNumber ? ConsoleColor.Blue : ConsoleColor.Magenta;
            Console.WriteLine($"Số của bạn {(guess < secretNumber ? "NHỎ" : "LỚN")} hơn số bí mật!");
            Console.ResetColor();
        }

        private void ShowGameResult()
        {
            Console.Clear();
            if (gameWon)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("CHÍNH XÁC! BẠN ĐÃ CHIẾN THẮNG!");
                Console.WriteLine($"Số lần đoán: {maxAttempts - remainingAttempts}");
                Console.WriteLine($"Điểm số: {score}");
                Console.WriteLine("════════════════════════════════");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("GAME OVER! BẠN ĐÃ HẾT LƯỢT ĐOÁN");
                Console.WriteLine($"Số bí mật là: {secretNumber}");
                Console.WriteLine("════════════════════════════════");
            }
            Console.ResetColor();
        }
    }
}