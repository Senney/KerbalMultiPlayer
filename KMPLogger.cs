using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace KMP
{
    public class KMPLogger
    {
        private static String logName = "kmplog.txt";
        private static readonly String EOL = "\r\n";

        public static readonly int KMP_LOG_FILE = 0x01;
        public static readonly int KMP_LOG_CONSOLE = 0x10;
        public static readonly int KMP_LOG_BOTH = 0x11;

        static int logType = KMP_LOG_BOTH;

        private static FileStream outFile = null;

        public static void setLogType(int type)
        {
            logType = type;
        }

        public static void log_color(String message, ConsoleColor color)
        {
            setConsoleColor(color);
            log(message);
            resetConsoleColor();
        }

        public static void debug(String message)
        {
            setConsoleColor(ConsoleColor.Gray);
            log("[DEBUG] " + getTimeStamp() + " ==> " + message);
            resetConsoleColor();
        }

        public static void info(String message)
        {
            setConsoleColor(ConsoleColor.White);
            log("[INFO] " + getTimeStamp() + " ==> " + message);
            resetConsoleColor();
        }

        private static void warning(String message)
        {
            setConsoleColor(ConsoleColor.Yellow);
            log("[WARN] " + getTimeStamp() + " ==> " + message);
            resetConsoleColor();
        }

        public static void error(String message)
        {
            setConsoleColor(ConsoleColor.DarkRed);
            log("[ERROR] " + getTimeStamp() + " ==> " + message);
            resetConsoleColor();
        }

        public static void fatal(String message)
        {
            setConsoleColor(ConsoleColor.Red);
            log("[FATAL] " + getTimeStamp() + " ==> " + message);
            resetConsoleColor();
        }

        private static void log(String message)
        {
            if ((logType & KMP_LOG_FILE) == KMP_LOG_FILE)
            {
                Console.WriteLine(message);
            }
            if ((logType & KMP_LOG_CONSOLE) == KMP_LOG_CONSOLE)
            {
                if (outFile == null)
                    outFile = File.Open(logName, FileMode.Append);
                Byte[] bytes = getByteArray(message + EOL);
                outFile.Write(bytes, 0, bytes.Length);
                outFile.Flush();
            }
        }

        private static void setConsoleColor(ConsoleColor color)
        {
            Console.ForegroundColor = color;
        }

        private static void resetConsoleColor()
        {
            Console.ResetColor();
        }

        private static String getTimeStamp()
        {
            return DateTime.Now.ToString("HH:mm:ss.ff");
        }

        private static Byte[] getByteArray(String str)
        {
            Byte[] bytes = new Byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }
    }
}
