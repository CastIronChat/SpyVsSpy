using System.Text.RegularExpressions;

public static class Outdent {
    private static Regex detectFirstLine = new Regex("^\r?\n( *)");
    private static Regex lastLineRe = new Regex("\r?\n *$");
    public static string trim(string d) {
        var m = detectFirstLine.Match(d);
        if(m.Length > 1) {
            // return $"{m.Captures.Count}";
            int indentLevel = m.Captures[0].Length;
            Regex removeIndentRe = new Regex("\r?\n {0," + indentLevel + "}");
            d = lastLineRe.Replace(removeIndentRe.Replace(d, "\n"), "");
        }
        d = d.TrimEnd();
        return d;
    }
}
