using System;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

/// <summary>
/// シーン名を定数で管理するクラスを作成するスクリプト
/// </summary>
public static class SceneNameCreator
{
    // 無効な文字を管理する配列
    private static readonly string[] INVALUD_CHARS =
    {
        " ", "!", "\"", "#", "$",
        "%", "&", "\'", "(", ")",
        "-", "=", "^",  "~", "\\",
        "|", "[", "{",  "@", "`",
        "]", "}", ":",  "*", ";",
        "+", "/", "?",  ".", ">",
        ",", "<"
    };

    private const string MENU_PATH = "Tools/Create/Scene Name";     // コマンド名
    private const string FILE_PATH = "Assets/Scripts/Common/SceneName.cs"; // ファイルパス

    private static readonly string FILENAME = Path.GetFileName(FILE_PATH);  // ファイル名(拡張子あり)
    private static readonly string FILENAME_WITHOUT_EXTENSION = Path.GetFileNameWithoutExtension(FILE_PATH);    // ファイル名(拡張子なし)

    /// <summary>
    /// シーン名を定数で管理するクラスを作成します
    /// </summary>
    [MenuItem(MENU_PATH)]
    public static void Create()
    {
        if (!CanCreate())
        {
            return;
        }

        CreateScript();

        EditorUtility.DisplayDialog(FILENAME, $"{FILENAME}の作成が完了しました", "OK");
    }

    /// <summary>
    /// スクリプトを作成します
    /// </summary>
    public static void CreateScript()
    {
        var scenes = EditorBuildSettings.scenes
            .Select(c => Path.GetFileNameWithoutExtension(c.path))
            .Distinct()
            .Select(c => new { var = RemoveInvalidChars(c), val = c })
            .ToList();

        var builder = new StringBuilder();

        // シーンタイプのクラス
        builder.AppendLine("using System.Collections;");
        builder.AppendLine("using System.Collections.Generic; ");
        builder.AppendLine();
        builder.AppendLine("/// <summary>");
        builder.AppendLine("/// シーンタイプを定数で管理するクラス");
        builder.AppendLine("/// ※Build SettingsのScenes In Buildに対象シーンをすべて追加してから使用してください");
        builder.AppendLine("/// </summary>");
        builder.AppendFormat("public static class {0}", FILENAME_WITHOUT_EXTENSION).AppendLine();
        builder.AppendLine("{");

        // タイプ
        builder.AppendLine("\tpublic enum Type");
        builder.AppendLine("\t{");
        builder.AppendLine("\t\tNone,");
        foreach (var scene in scenes)
        {
            builder.AppendFormat("\t\t{0},", scene.var.Replace("Scene", "")).AppendLine();
        }
        builder.AppendLine("\t}");

        builder.AppendLine();

        // 名前リスト
        builder.AppendLine("\tpublic static readonly Dictionary<Type, string> NameList = new Dictionary<Type, string>()");
        builder.AppendLine("\t{");
        foreach (var scene in scenes)
        {
            builder.AppendLine("\t\t{ Type." + scene.var.Replace("Scene", "") + ", \"" + scene.var + "\" },");
        }
        builder.AppendLine("\t};");

        builder.AppendLine("}");

        var directoryName = Path.GetDirectoryName(FILE_PATH);
        if (!Directory.Exists(directoryName))
        {
            Directory.CreateDirectory(directoryName);
        }

        File.WriteAllText(FILE_PATH, builder.ToString(), Encoding.UTF8);
        AssetDatabase.Refresh(ImportAssetOptions.ImportRecursive);
    }

    /// <summary>
    /// シーン名を定数で管理するクラスを作成できるかどうかを取得します
    /// </summary>
    [MenuItem(MENU_PATH, true)]
    public static bool CanCreate()
    {
        return !EditorApplication.isPlaying && !Application.isPlaying && !EditorApplication.isCompiling;
    }

    /// <summary>
    /// 無効な文字を削除します
    /// </summary>
    public static string RemoveInvalidChars(string str)
    {
        Array.ForEach(INVALUD_CHARS, c => str = str.Replace(c, string.Empty));
        return str;
    }
}