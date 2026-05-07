using System.Text;

public static class KbdText
{
    public static string Kbd(string key)
    {
        return $"<mark=#0000007F padding=\"15,15,8,8\"><b>{key}</b></mark>";
    }

    public static string Compose(string before, string key, string after)
    {
        var sb = new StringBuilder();
        if (!string.IsNullOrEmpty(before)) sb.Append(before);
        sb.Append(Kbd(key));
        if (!string.IsNullOrEmpty(after)) sb.Append(after);
        return sb.ToString();
    }
}
