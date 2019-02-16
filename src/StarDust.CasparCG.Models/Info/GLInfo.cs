using System.Xml.Serialization;

namespace StarDust.CasparCG.Models.Info
{
    [XmlRoot(ElementName = "device_buffer_pool")]
    public class DeviceBufferPool
    {
        [XmlElement(ElementName = "stride")]
        public int Stride { get; set; }
        [XmlElement(ElementName = "mipmapping")]
        public bool Mipmapping { get; set; }
        [XmlElement(ElementName = "width")]
        public int Width { get; set; }
        [XmlElement(ElementName = "height")]
        public int Height { get; set; }
        [XmlElement(ElementName = "size")]
        public int Size { get; set; }
        [XmlElement(ElementName = "count")]
        public int Count { get; set; }
    }

    [XmlRoot(ElementName = "pooled_device_buffers")]
    public class PooledDeviceBuffers
    {
        [XmlElement(ElementName = "device_buffer_pool")]
        public DeviceBufferPool Device_buffer_pool { get; set; }
        [XmlElement(ElementName = "total_count")]
        public int Total_count { get; set; }
        [XmlElement(ElementName = "total_size")]
        public int Total_size { get; set; }
    }

    [XmlRoot(ElementName = "host_buffer_pool")]
    public class HostBufferPool
    {
        [XmlElement(ElementName = "usage")]
        public string Usage { get; set; }
        [XmlElement(ElementName = "size")]
        public int Size { get; set; }
        [XmlElement(ElementName = "count")]
        public int Count { get; set; }
    }

    [XmlRoot(ElementName = "pooled_host_buffers")]
    public class PooledHostBuffers
    {
        [XmlElement(ElementName = "host_buffer_pool")]
        public HostBufferPool Host_buffer_pool { get; set; }
        [XmlElement(ElementName = "total_read_count")]
        public int TotalReadCount { get; set; }
        [XmlElement(ElementName = "total_write_count")]
        public int TotalWriteCount { get; set; }
        [XmlElement(ElementName = "total_read_size")]
        public int TotalReadSize { get; set; }
        [XmlElement(ElementName = "total_write_size")]
        public int TotalWriteSize { get; set; }
    }

    [XmlRoot(ElementName = "details")]
    public class Details
    {
        [XmlElement(ElementName = "pooled_device_buffers")]
        public PooledDeviceBuffers PooledDeviceBuffers { get; set; }
        [XmlElement(ElementName = "pooled_host_buffers")]
        public PooledHostBuffers PooledHostBuffers { get; set; }
    }

    [XmlRoot(ElementName = "all_device_buffers")]
    public class AllDeviceBuffers
    {
        [XmlElement(ElementName = "total_count")]
        public int Total_count { get; set; }
        [XmlElement(ElementName = "total_size")]
        public int Total_size { get; set; }
    }

    [XmlRoot(ElementName = "all_host_buffers")]
    public class AllHostBuffers
    {
        [XmlElement(ElementName = "total_read_count")]
        public int TotalReadCount { get; set; }
        [XmlElement(ElementName = "total_write_count")]
        public int TotalWriteCount { get; set; }
        [XmlElement(ElementName = "total_read_size")]
        public int TotalReadSize { get; set; }
        [XmlElement(ElementName = "total_write_size")]
        public int TotalWriteSize { get; set; }
    }

    [XmlRoot(ElementName = "summary")]
    public class Summary
    {
        [XmlElement(ElementName = "pooled_device_buffers")]
        public PooledDeviceBuffers PooledDeviceBuffers { get; set; }
        [XmlElement(ElementName = "all_device_buffers")]
        public AllDeviceBuffers AllDeviceBuffers { get; set; }
        [XmlElement(ElementName = "pooled_host_buffers")]
        public PooledHostBuffers PooledHostBuffers { get; set; }
        [XmlElement(ElementName = "all_host_buffers")]
        public AllHostBuffers AllHostBuffers { get; set; }
    }

    [XmlRoot(ElementName = "gl")]
    public class GLInfo
    {
        [XmlElement(ElementName = "details")]
        public Details Details { get; set; }
        [XmlElement(ElementName = "summary")]
        public Summary Summary { get; set; }

        public string Xml { get; set; }
    }
}
