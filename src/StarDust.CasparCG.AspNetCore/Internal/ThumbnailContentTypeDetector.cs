namespace StarDust.CasparCG.AspNetCore.Internal;

internal static class ThumbnailContentTypeDetector
{
    private const string OctetStream = "application/octet-stream";
    private const string Png = "image/png";
    private const string Jpeg = "image/jpeg";
    private const string Gif = "image/gif";
    private const string Bmp = "image/bmp";
    private const string Webp = "image/webp";
    private const string Tiff = "image/tiff";
    private const string Ico = "image/x-icon";

    public static string Detect(ReadOnlySpan<byte> bytes)
    {
        if (IsPng(bytes))
        {
            return Png;
        }

        if (IsJpeg(bytes))
        {
            return Jpeg;
        }

        if (IsGif(bytes))
        {
            return Gif;
        }

        if (IsBmp(bytes))
        {
            return Bmp;
        }

        if (IsWebp(bytes))
        {
            return Webp;
        }

        if (IsTiff(bytes))
        {
            return Tiff;
        }

        if (IsIco(bytes))
        {
            return Ico;
        }

        return OctetStream;
    }

    private static bool IsPng(ReadOnlySpan<byte> bytes) =>
        bytes.Length >= 8 &&
        bytes[0] == 0x89 &&
        bytes[1] == 0x50 &&
        bytes[2] == 0x4E &&
        bytes[3] == 0x47 &&
        bytes[4] == 0x0D &&
        bytes[5] == 0x0A &&
        bytes[6] == 0x1A &&
        bytes[7] == 0x0A;

    private static bool IsJpeg(ReadOnlySpan<byte> bytes) =>
        bytes.Length >= 3 &&
        bytes[0] == 0xFF &&
        bytes[1] == 0xD8 &&
        bytes[2] == 0xFF;

    private static bool IsGif(ReadOnlySpan<byte> bytes) =>
        bytes.Length >= 6 &&
        (bytes[..6].SequenceEqual("GIF87a"u8) || bytes[..6].SequenceEqual("GIF89a"u8));

    private static bool IsBmp(ReadOnlySpan<byte> bytes) =>
        bytes.Length >= 2 &&
        bytes[0] == 0x42 &&
        bytes[1] == 0x4D;

    private static bool IsWebp(ReadOnlySpan<byte> bytes) =>
        bytes.Length >= 12 &&
        bytes[..4].SequenceEqual("RIFF"u8) &&
        bytes[8..12].SequenceEqual("WEBP"u8);

    private static bool IsTiff(ReadOnlySpan<byte> bytes) =>
        bytes.Length >= 4 &&
        ((bytes[0] == 0x49 && bytes[1] == 0x49 && bytes[2] == 0x2A && bytes[3] == 0x00) ||
         (bytes[0] == 0x4D && bytes[1] == 0x4D && bytes[2] == 0x00 && bytes[3] == 0x2A));

    private static bool IsIco(ReadOnlySpan<byte> bytes) =>
        bytes.Length >= 4 &&
        bytes[0] == 0x00 &&
        bytes[1] == 0x00 &&
        bytes[2] == 0x01 &&
        bytes[3] == 0x00;
}
