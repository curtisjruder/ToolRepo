using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Data.SQLite;  // Nuget package "System.Data.SQLite" (not the one with core suffix).  
using System.Data;

internal static class Tool_SQLite {
    private static Dictionary<string, DB> dbConns = new Dictionary<string, DB>();

    internal static DB getDB(string dbPath) {
        if (dbConns.ContainsKey(dbPath)) return dbConns[dbPath];
        return new DB(dbPath);
    }

    internal static DB addDB(string dbPath) {
        if (!dbConns.ContainsKey(dbPath)) dbConns.Add(dbPath, new DB(dbPath));
        return dbConns[dbPath];
    }

    internal static void dropDB(string dbPath) {
        if (!dbConns.ContainsKey(dbPath)) return;
        dbConns.Remove(dbPath);
    }
}


// DB CLASSES
internal class DB {
    private SQLiteConnection _db;
    private SQLiteTransaction _dbTrans;
    private string _dbPath;

    private Dictionary<string, Dictionary<string, DBCol>> schema;

    public DB(string dbPath) {
        if (!File.Exists(dbPath)) throw new Exception("Error - Invalid DB Path");

        _db = new SQLiteConnection();
        _db.ConnectionString = "Data Source=" + dbPath + ";";
        _db.Open();

        if (_db.State != System.Data.ConnectionState.Open) throw new Exception("ERROR - Invalid DB Connection");

        _dbPath = dbPath;

        processTableSchema();
        processColumnSchema();
    }

    private void processTableSchema() {
        schema = new Dictionary<string, Dictionary<string, DBCol>>();

        // Required call to enforce foreign key constraints
        using (SQLiteCommand x = _db.CreateCommand()) {
            x.CommandText = "PRAGMA foreign_keys = ON";
            x.ExecuteReader();
        }

        using SQLiteCommand cmd = _db.CreateCommand();

        // Build out schema
        cmd.CommandText = "PRAGMA table_list";
        using SQLiteDataReader reader = cmd.ExecuteReader();

        while (reader.Read()) {
            string name = reader.GetString(1);
            if (!name.StartsWith("sqlite")) schema.Add(name, new Dictionary<string, DBCol>());
        }
    }

    private void processColumnSchema() {
        foreach (string tbl in schema.Keys) {
            processColumnSchema(tbl);
        }
    }

    private void processColumnSchema(string tableName) {
        using SQLiteCommand cmd = _db.CreateCommand();
        cmd.CommandText = "PRAGMA table_info(" + tableName + ")";
        using SQLiteDataReader reader = cmd.ExecuteReader();

        while (reader.Read()) {
            DBCol x = new DBCol(reader);
            schema[tableName].Add(x.getName(), x);
        }
    }

    public void executeCommand(string cmd) {
        if (_dbTrans is null) _dbTrans = _db.BeginTransaction();

        using SQLiteCommand sqlCmd = _db.CreateCommand();

        sqlCmd.CommandText = cmd;

        try {
            sqlCmd.ExecuteNonQuery();
        } catch (Exception e) {
            writeErrorMsg(cmd, e.Message);
        }
    }

    public DataTable executeQuery(string query) {
        using SQLiteCommand sqlCmd = _db.CreateCommand();
        sqlCmd.CommandText = query;

        SQLiteDataReader reader;
        try {
            reader = sqlCmd.ExecuteReader();
        } catch (Exception e) {
            writeErrorMsg(query, e.Message);
            return new DataTable();
        }

        return convertReaderToDataTable(reader);
    }

    private DataTable convertReaderToDataTable(SQLiteDataReader reader) {
        DataTable table = new DataTable();
        if (reader is null) return table;
        if (reader.FieldCount == 0) return table;

        int jEnd = reader.FieldCount;

        // Create table schema
        for (int j = 0; j < jEnd; j++)
            table.Columns.Add(reader.GetName(j), convertSQLDataType(reader.GetDataTypeName(j)));

        // Add data rows
        while (reader.Read()) {
            DataRow x = table.NewRow();
            for (int j = 0; j < jEnd; j++)
                x[j] = reader.GetValue(j);

            table.Rows.Add(x);
        }

        table.AcceptChanges();
        return table;
    }

    private Type convertSQLDataType(string input) {
        input = input.ToLower();
        if (input.Contains("real")) return typeof(Double);
        if (input.Contains("int")) return typeof(Int64);
        return typeof(String);
    }

    private void writeErrorMsg(string cmd, string err) {
        using (StreamWriter output = File.AppendText(getPath() + "DBError.txt")) {
            output.WriteLine(new String('-', 90));
            output.WriteLine(new String('-', 90));
            output.WriteLine("");
            output.WriteLine(cmd);
            output.WriteLine("");
            output.WriteLine(err);
            output.WriteLine("");
        }
    }

    private string getPath() {
        return _dbPath.Substring(0, _dbPath.LastIndexOf('\\') + 1);
    }

    public void commitUpdate() {
        if (_dbTrans is null) return;
        _dbTrans.Commit();
        _dbTrans = null;
    }

    public bool isDBColumn(string tblName, string colName) {
        return schema[tblName].ContainsKey(colName);
    }

    public void insertValues(string tableName, List<string> colNames, List<string> values) {
        for (int i = 0; i < colNames.Count; i++) {
            values[i] = schema[tableName][colNames[i]].convertValue(values[i]);
        }

        executeCommand("INSERT OR IGNORE into " + tableName + convertColNames(colNames) + " values (" + String.Join(",", values) + ")");
    }


    private string convertColNames(IEnumerable<string> colNames) {
        return "(" + String.Join(",", colNames) + ")";

    }
}

internal class DBCol {
    private string _colName;
    private string _dataType;
    public DBCol(SQLiteDataReader reader) {
        _colName = reader.GetString(1).ToUpper();
        _dataType = reader.GetString(2).ToUpper();
    }

    private bool isText() {
        return _dataType.Contains("TEXT");
    }

    private bool isDate() {
        return _colName.Contains("DATE");
    }

    public string convertValue(string input) {
        if (input.Length == 0) return "null";
        input = input.Replace("'", "");

        DateTime dt;
        if (isDate() && DateTime.TryParse(input, out dt)) return dt.ToOADate().ToString();

        if (isText()) return "'" + input + "'";
        return input;
    }

    public string getName() {
        return _colName;
    }
}