using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Devmonster.Core.Extensions.Compression;

public static class StringCompression
{
    public static void CopyTo(Stream src, Stream dest)
    {
        byte[] bytes = new byte[4096];

        int cnt;

        while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0)
        {
            dest.Write(bytes, 0, cnt);
        }
    }

    public static byte[] Zip(string str)
    {
        var bytes = Encoding.UTF8.GetBytes(str);

        using (var msi = new MemoryStream(bytes))
        using (var mso = new MemoryStream())
        {
            using (var gs = new GZipStream(mso, CompressionMode.Compress))
            {
                //msi.CopyTo(gs);
                CopyTo(msi, gs);
            }

            return mso.ToArray();
        }
    }

    public static string ZipForWeb(string str)
    {
        return UrlEncodeBase64(Convert.ToBase64String(Zip(str)));
    }

    public static string Unzip(byte[] bytes)
    {
        using (var msi = new MemoryStream(bytes))
        using (var mso = new MemoryStream())
        {
            using (var gs = new GZipStream(msi, CompressionMode.Decompress))
            {
                //gs.CopyTo(mso);
                CopyTo(gs, mso);
            }

            return Encoding.UTF8.GetString(mso.ToArray());
        }
    }

    public static string UnzipForWeb(string str)
    {
        return Unzip(Convert.FromBase64String(UrlDecodeBase64(str)));
    }

    public static string UrlEncodeBase64(string text)
    {
        return text.Replace("=", string.Empty)
            .Replace('+', '-')
            .Replace('/', '_')
            .PadRight(text.Length + (4 - text.Length % 4) % 4, '=');
    }

    public static string UrlDecodeBase64(string base64string)
    {
        return base64string.Replace('-', '+').Replace('_', '/');
    }

}
