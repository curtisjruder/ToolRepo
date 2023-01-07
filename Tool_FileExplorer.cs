using System;
using System.Collections.Generic;
using System.Text;
using System.IO;


internal class Tool_FileExplorer {
    private Tool_FileExplorer() { }

    internal static bool existsFile(string filePath) {
        return File.Exists(filePath);
    }

    internal static bool existsFolder(string filePath) {
        filePath = getFilePath(filePath);
        if (filePath == "") return false;
        return Directory.Exists(filePath);
    }

    internal static void createFolder(string folderPath) {
        if (existsFolder(folderPath)) return;
        Directory.CreateDirectory(folderPath);
    }
    internal static void deleteFolder(string folderPath) {
        if (!existsFolder(folderPath)) return;
        Directory.Delete(folderPath, true);
    }

    internal static void deleteFile(string filePath) {
        if (!existsFile(filePath)) return;
        File.Delete(filePath);
    }


    internal static string myPath() {
        return getFilePath(System.Reflection.Assembly.GetExecutingAssembly().CodeBase.Substring(8).Replace("/", "\\"));
    }

    internal static string desktopPath() {
        return Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + '\\';
    }

    internal static string documentPath() {
        return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + '\\';
    }

    internal static string getFilePath(string filePath) {
        if (filePath.IndexOf("\\") == -1) return "";// filePath + "\\";
        return filePath.Substring(0, filePath.LastIndexOf('\\')) + '\\';
    }

    internal static string getFileName(string filePath) {
        int index = filePath.LastIndexOf("\\");
        if (index == -1) return "";
        if (index == filePath.Length - 1) return "";
        return filePath.Substring(index + 1);
    }

    internal static string getParentPath(string filePath) {
        filePath = getFilePath(filePath);
        return getFilePath(filePath.Substring(0, filePath.Length - 1));
    }

    internal static string searchParentsForFile(string fileName, string path) {
        if (path == "") return "";
        if (existsFile(path + fileName)) return path;
        return searchParentsForFile(fileName, getParentPath(path));
    }
    internal static string searchParentsForFolder(string folderName, string path) {
        if (path == "") return "";
        if (existsFolder(path + folderName)) return path;
        return searchParentsForFolder(folderName, getParentPath(path));
    }

    internal static List<string> getFiles(string rootPath) {
        List<string> files = new List<string>();
        if (!existsFolder(rootPath)) return files;

        foreach (string x in Directory.EnumerateFiles(rootPath)) {
            files.Add(x);
        }
        return files;
    }

    internal static List<string> getFolders(string rootPath) {
        List<string> folders = new List<string>();
        if (!existsFolder(rootPath)) return folders;
                
        foreach (string x in Directory.EnumerateDirectories(rootPath)) {            
            folders.Add(x + "\\");
        }

        return folders;
    }
}
