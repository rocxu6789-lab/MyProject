using Config;
using UnityEngine;
using SimpleJSON;
using System.IO;

public static class TableManager
{
    private static Tables _tables;
    public static Tables Tables { get { return _tables; } }
    public static void Init()
    {
        _tables = new Tables(path => JSONNode.Parse(File.ReadAllText(Path.Combine(Application.dataPath, "Gen/json", path + ".json"))));
    }

    public static string GetNextId(this string ID, int index)
    {
        return ID + "_" + (index + 1);
    }
    public static string GetPreId(this string ID, int index)
    {
        return ID + "_" + (index - 1);
    }
}