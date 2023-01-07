using System;
using System.Collections.Generic;
using System.Text;

internal class Tool_Logger {
    private Tool_Logger() { }

    internal static void H1(string content) {
        br();
        content = expandLength("---   " + content + "   ---", 80);

        dashes(content, 0);
        Console.WriteLine(content);
        dashes(content, 0);
        br();

    }

    private static void dashes(string content, int spaceCnt) {
        Console.WriteLine(spaces(spaceCnt) + new String('-', content.Length));
    }

    private static string expandLength(string content, int minLength) {
        while (content.Length < minLength) {
            content = "-" + content + "-";
        }
        return content;
    }

    internal static void H2(string content) {
        br();
        content = expandLength("--  " + content + "  --", 60);

        dashes(content, 10);
        Console.WriteLine(spaces(10) + content);
        dashes(content, 10);
        br();
    }

    internal static void H3(string content) {
        br();
        content = expandLength("--  " + content + "  --", 50);
        Console.WriteLine(spaces(15) + content);
        br();
    }

    private static string spaces(int cnt) {
        return new String(' ', cnt);
    }

    internal static void p(string content) {
        Console.WriteLine("\t\t" + content);
    }

    internal static void br() {
        Console.WriteLine("");
    }
}

