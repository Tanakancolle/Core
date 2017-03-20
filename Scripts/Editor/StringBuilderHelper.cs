using System.IO;
using UnityEditor;
using System.Text;
using System;
using System.Text.RegularExpressions;

/// <summary>
/// ビルダー補助クラス
/// </summary>
public class StringBuilderHelper
{

    /// <summary>
    /// 一文字目識別子パターン
    /// </summary>
    private static readonly string firstIdentifierPattern = @"(\p{Lu}|\p{Ll}|\p{Lt}|\p{Lm}|\p{Lo}|\p{Nl})";

    /// <summary>
    /// 識別子パターン
    /// </summary>
    private const string identifierPattern = @"(\p{Mn}|\p{Mc}|\p{Nd}|\p{Pc}|\p{Cf})";

    /// <summary>
    /// 一文字目識別子正規表現
    /// </summary>
    private static Regex firstIdentifierRegex = new Regex (string.Format ("^{0}", firstIdentifierPattern));

    /// <summary>
    /// 識別子正規表現
    /// </summary>
    private static Regex identifierRegex = new Regex (string.Format ("({0}|{1})+", firstIdentifierPattern, identifierPattern)); 

    /// <summary>
    /// 置き換え文字
    /// </summary>
    private static readonly string replaceChar = "_";

    /// <summary>
    /// タブのスペーズ数
    /// </summary>
    private const int tabToSpaceNum = 4;

    /// <summary>
    /// using定義
    /// </summary>
    /// <param name="builder">ストリングビルダー</param>
    /// <param name="usings">ネームスペース名配列</param>
    public static void EditUsings(StringBuilder builder, string[] usings)
    {
        if (usings == null) {
            return;
        }

        foreach (var edit_using in usings) {
            EditUsing (builder, edit_using);
        }
    }

    /// <summary>
    /// using定義
    /// </summary>
    /// <param name="builder">ビルダー</param>
    /// <param name="type">タイプ</param>
    public static void EditUsing(StringBuilder builder, Type type)
    {
        if (type == null) {
            return;
        }

        EditUsing (builder, type.Namespace);
    }

    /// <summary>
    /// using定義
    /// </summary>
    /// <param name="builder">ビルダー</param>
    /// <param name="usings">ネームスペース名</param>
    public static void EditUsing(StringBuilder builder, string edit_using)
    {
        if (string.IsNullOrEmpty (edit_using)) {
            return;
        }              

        builder.Insert (0, string.Format ("using {0};\n", edit_using));
    }

    /// <summary>
    /// 名前空間定義 ※「}」あり
    /// </summary>
    /// <param name="builder">ビルダー</param>
    /// <param name="name_space">名前空間名</param>
    /// <param name="tab_num">タブ数</param> 
    public static void EditNameSpace(StringBuilder builder, string name_space, string content, int tab_num = 0)
    {
        // 名前空間定義
        EditNameSpace (builder, name_space, tab_num);

        // 名前空間内容定義
        builder.AppendLine (content);
        builder.AppendLine (SetTab (tab_num) + "}");
    }

    /// <summary>
    /// 名前空間定義 ※「}」なし
    /// </summary>
    /// <param name="builder">ビルダー</param>
    /// <param name="name_space">名前空間名</param>
    /// <param name="tab_num">タブ数</param>
    public static void EditNameSpace(StringBuilder builder, string name_space, int tab_num = 0)
    {
        // タブ設定
        string tab = SetTab (tab_num);

        // 名前空間定義
        builder.AppendLine (tab + "namespace " + name_space);
        builder.AppendLine (tab + "{");
    }

    /// <summary>
    /// クラス定義 ※「}」あり
    /// </summary>
    /// <param name="builder">ビルダー</param>
    /// <param name="class_name">クラス名</param> 
    /// <param name="content">内容</param>
    /// <param name="tab_num">タブ数</param>
    /// <param name="summary">説明</param>
    /// <param name="is_static">静的クラスか</param> 
    public static void EditClass(StringBuilder builder, string class_name, string content, int tab_num = 0, bool is_static = false, string summary = "")
    {
        // クラス定義
        EditClass (builder, class_name, tab_num, is_static, summary);

        // クラス内容定義
        builder.AppendLine (content);
        builder.AppendLine (SetTab (tab_num) + "}");
    }

    /// <summary>
    /// クラス定義 ※「}」なし
    /// </summary>
    /// <param name="builder">ビルダー</param>
    /// <param name="class_name">クラス名</param>
    /// <param name="tab_num">タブ数</param>
    /// <param name="summary">説明</param>
    /// <param name="is_static">静的クラスか</param>
    public static void EditClass(StringBuilder builder, string class_name, int tab_num = 0, bool is_static = false, string summary = "")
    {
        // タブ設定
        string tab = SetTab (tab_num);

        // 静的クラス
        string static_class = "";

        // 静的クラス設定
        if (is_static) {
            static_class = "static ";
        }

        // 説明定義
        EditSummary (builder, summary, tab_num);

        // クラス定義
        builder.AppendLine (tab + "public " + static_class + "class " + class_name);
        builder.AppendLine (tab + "{");
    }

    /// <summary>
    /// 列挙型定義
    /// </summary>
    /// <param name="builder">ビルダー</param>
    /// <param name="enum_name">列挙型名</param>
    /// <param name="contents">内容配列</param>
    /// <param name="tab_num">タブ数</param>
    /// <param name="summary">説明</param>
    public static void EditEnum(StringBuilder builder, string enum_name, string[] contents, int tab_num = 0, string summary = "")
    {
        EditEnum (builder, enum_name, JoinStrings (contents, tab_num + 1), tab_num, summary);
    }

    /// <summary>
    /// 列挙型定義 ※「}」あり
    /// </summary>
    /// <param name="builder">ビルダー</param>
    /// <param name="enum_name">列挙型名</param> 
    /// <param name="content">内容</param>
    /// <param name="tab_num">タブ数</param>
    /// <param name="summary">説明</param>
    public static void EditEnum(StringBuilder builder, string enum_name, string content, int tab_num = 0, string summary = "")
    {
        // 列挙型定義
        EditEnum (builder, enum_name, tab_num, summary);

        // 列挙型内容定義
        builder.AppendLine (content);
        builder.AppendLine (SetTab (tab_num) + "}");
    }

    /// <summary>
    /// 列挙型定義 ※「}」なし
    /// </summary>
    /// <param name="builder">ビルダー</param>
    /// <param name="enum_name">列挙型名</param>
    /// <param name="tab_num">タブ数</param>
    /// <param name="summary">説明</param>
    public static void EditEnum(StringBuilder builder, string enum_name, int tab_num = 0, string summary = "")
    {
        // タブ設定
        string tab = SetTab (tab_num);

        // 説明定義
        EditSummary (builder, summary, tab_num);

        // 列挙型定義
        builder.AppendLine (tab + "public enum " + enum_name);
        builder.AppendLine (tab + "{");
    }

    /// <summary>
    /// 説明定義
    /// </summary>
    /// <param name="builder">ビルダー</param>
    /// <param name="summary">説明</param>
    /// <param name="tab_num">タブ数</param>
    public static void EditSummary(StringBuilder builder, string summary, int tab_num = 0)
    {
        if( string.IsNullOrEmpty(summary)) {
            return;
        }

        // タブ設定
        string tab = SetTab (tab_num); 

        // 説明定義
        builder.AppendLine (tab + "/// <summary>");
        builder.AppendLine (tab + "/// " + summary);
        builder.AppendLine (tab + "/// </summary>");
    }

    /// <summary>
    /// 文字列結合
    /// </summary>
    /// <param name="string_array">文字列配列</param>
    /// <param name="tab_num">タブ数</param>
    /// <param name="delimit">結合時に挿入する文字</param>
    /// <returns>結合された文字列</returns>
    public static string JoinStrings(string[] string_array, int tab_num, string delimit = ",\n", string start = "", string end = "")
    {
        StringBuilder builder = new StringBuilder ();

        // タブ設定
        string tab = SetTab (tab_num);

        // 配列全てを結合
        for (int i = 0; i < string_array.Length; ++i) {
            // 終了チェック
            if (string_array.Length - 1 == i) {
                // 最後はなにもつけない
                delimit = "";
            }

            // 結合
            builder.Append(tab + start + string_array [i] + end + delimit);
        }

        return builder.ToString ();
    }

    /// <summary>
    /// 列挙型無効文字置換
    /// </summary>
    /// <param name="str">置き換える文字列</param>
    /// <returns>置き換えた文字列</returns>
    public static string ReplaceEnumInvaild(string str)
    {   
        if( string.IsNullOrEmpty(str)) {
            return str;
        }

        var builder = new StringBuilder ();

        // 一文字目チェック
        if (!firstIdentifierRegex.IsMatch (str)) {
            str = str.Substring (1);
            builder.Append (replaceChar);
        }

        // 全体チェック
        var matches = identifierRegex.Matches (str);
        for (int i = 0; i < matches.Count; ++i) {
            if (i != 0) {
                builder.Append (replaceChar);
            }

            builder.Append (matches[i]);
        }

        return builder.ToString ();                                            
    }                                                                   


    /// <summary>
    /// タブ設定
    /// </summary>
    /// <param name="num">タブ数</param>
    /// <returns>タブが設定された文字列</returns>
    public static string SetTab(int num)
    {
        return new string (' ', num * StringBuilderHelper.tabToSpaceNum);
    }

    /// <summary>
    /// Creates the script.
    /// </summary>
    /// <returns><c>true</c>, スクリプト生成, <c>false</c> スクリプト未生成.</returns>
    /// <param name="path">生成パス</param>
    /// <param name="builder_text">スクリプト内容テキスト</param>
    /// <param name="is_overwride"><c>true</c> ファイルが存在した場合、上書き</param>
    public static bool CreateScript(string path, string builder_text, bool is_overwride)
    {
        // ディレクトリ作成
        var directoryName = Path.GetDirectoryName (path);
        if (!Directory.Exists (directoryName)) {
            Directory.CreateDirectory (directoryName);
        }

        // ファイル存在チェック
        if (!is_overwride && File.Exists (path)) {
            return false;
        }

        // スクリプト作成
        File.WriteAllText (path, builder_text, Encoding.UTF8);

        return true;
    }

    /// <summary>
    /// エディタを更新
    /// </summary>
    public static void RefreshEditor()
    {
        AssetDatabase.Refresh (ImportAssetOptions.ImportRecursive);
    }
}