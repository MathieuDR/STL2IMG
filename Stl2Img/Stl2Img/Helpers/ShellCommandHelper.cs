using System;
using ToolBox.Platform;

namespace Stl2Img.Helpers {
    public static class ShellCommandHelper {
        public static string EscapeStringForCommand(this string text) {
            switch (OS.GetCurrent()) {
                case "win":
                    return text.Replace("^", "^^").Replace("&", "^&").Replace("%", "%%")
                        .Replace("<", "^<").Replace("<", "^>").Replace("|","^|");
                case "mac":
                case "gnu":
                default:
                    throw new NotImplementedException();
            }
        }
    }
}