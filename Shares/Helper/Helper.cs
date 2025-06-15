namespace Shares.Helper
{
    public static class Logger
    {
        private static object _lock = new object();

        public static void Log(string level, ConsoleColor color, string message)
        {
            string time = DateTime.Now.ToString("HH:mm:ss");
            Console.Write($"[{time}] ");

            // เปลี่ยนสีเฉพาะคำว่า level
            Console.ForegroundColor = color;
            Console.Write($"[{level}]");

            Console.ResetColor();

            // แล้วเขียนข้อความต่อ
            Console.Write($" - {message} \n");
        }

        public static void Info(string message)
        {
            Log("INFO", ConsoleColor.Green, message);
        }

        public static void Warn(string message)
        {
            Log("WARN", ConsoleColor.Yellow, message);
        }

        public static void Error(string message)
        {
            Log("ERROR", ConsoleColor.Red, message);
        }
    }
}
