using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.Json;


internal static class Tool_JSONConvert {
    public static e getObjectFromJSON<e>(string inputJSON) {
        return JsonSerializer.Deserialize<e>(inputJSON);
    }

    public static List<e> getObjectsFromJSON<e>(string inputJSON) {
        return JsonSerializer.Deserialize<List<e>>(inputJSON);
    }
}

