using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.Json;


internal static class Tool_JSONConvert {
    public static e getObjectFromJSON<e>(string inputJSON) {
        if (inputJSON == "") return default(e);
        return JsonSerializer.Deserialize<e>(inputJSON);
    }

    public static List<e> getObjectsFromJSON<e>(string inputJSON) {
        if (inputJSON == "") return null;
        return JsonSerializer.Deserialize<List<e>>(inputJSON);
    }
}

