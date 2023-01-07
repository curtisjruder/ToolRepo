using System;
using System.Collections.Generic;
using System.Text;
using System.IO;


internal class Tool_FileReader {
    private Tool_FileReader() { }

    internal static string getFileContent(string filePath) {
        if (!File.Exists(filePath)) return "";

        using (StreamReader x = new StreamReader(filePath)) {
            return x.ReadToEnd();
        }
    }
    internal static List<string> getFileContents(string filePath) {
        List<string> output = new List<string>();
        if (!File.Exists(filePath)) return output;

        using (StreamReader x = new StreamReader(filePath)) {
            while (!x.EndOfStream)
                output.Add(x.ReadLine());
        }

        return output;
    }
}
