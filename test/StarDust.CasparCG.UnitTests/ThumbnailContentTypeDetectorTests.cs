using StarDust.CasparCG.AspNetCore.Internal;
using Xunit;

namespace StarDust.CasparCG.UnitTests;

public sealed class ThumbnailContentTypeDetectorTests
{
    [Theory]
    [InlineData(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }, "image/png")]
    [InlineData(new byte[] { 0xFF, 0xD8, 0xFF, 0x00 }, "image/jpeg")]
    [InlineData(new byte[] { (byte)'G', (byte)'I', (byte)'F', (byte)'8', (byte)'7', (byte)'a' }, "image/gif")]
    [InlineData(new byte[] { 0x42, 0x4D, 0x00, 0x00 }, "image/bmp")]
    [InlineData(new byte[] { (byte)'R', (byte)'I', (byte)'F', (byte)'F', 0x00, 0x00, 0x00, 0x00, (byte)'W', (byte)'E', (byte)'B', (byte)'P' }, "image/webp")]
    [InlineData(new byte[] { 0x49, 0x49, 0x2A, 0x00 }, "image/tiff")]
    [InlineData(new byte[] { 0x00, 0x00, 0x01, 0x00 }, "image/x-icon")]
    public void Detect_recognizes_common_image_signatures(byte[] bytes, string expectedContentType)
    {
        var contentType = ThumbnailContentTypeDetector.Detect(bytes);

        Assert.Equal(expectedContentType, contentType);
    }

    [Fact]
    public void Detect_falls_back_to_octet_stream_for_unknown_bytes()
    {
        var contentType = ThumbnailContentTypeDetector.Detect(new byte[] { 0x01, 0x02, 0x03 });

        Assert.Equal("application/octet-stream", contentType);
    }
}
