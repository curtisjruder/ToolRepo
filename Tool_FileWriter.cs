using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Data;

internal class Tool_FileWriter {
    private Tool_FileWriter() { }

    internal static void writeContent(string path, List<string> input, bool overWrite = true) {
        using(StreamWriter writer = new StreamWriter(path, !overWrite)) {
            foreach (string strX in input) {
                writer.WriteLine(strX);
            }          
        }        
    }

    internal static void writeContent(string path, string input, bool overWrite = true) {
        using (StreamWriter writer = new StreamWriter(path, !overWrite)) {
            writer.WriteLine(input);
        }
    }

    internal static void writeContent(string path, DataTable dt, bool overWrite = true) {
        List<string> results = new List<string>();

        string strX = "";
        for(int j = 0; j < dt.Columns.Count; j++) {
            if (strX == "")
                strX = dt.Columns[j].ColumnName;
            else
                strX += "," + dt.Columns[j].ColumnName;
        }

        results.Add(strX);

        foreach(DataRow rowX in dt.Rows) {
            results.Add(string.Join(",", rowX.ItemArray));
        }

        writeContent(path, results, overWrite);
    }
}

