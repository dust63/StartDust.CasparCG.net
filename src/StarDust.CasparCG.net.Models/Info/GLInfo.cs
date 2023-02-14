using System.Xml.Serialization;

namespace StarDust.CasparCG.net.Models.Info
{
    /// <summary>
    /// Device buffer pool
    /// </summary>
    [XmlRoot(ElementName = "device_buffer_pool")]
    public class DeviceBufferPool
    {
        /// <summary>
        /// Stride
        /// </summary>
        [XmlElement(ElementName = "stride")]
        public int Stride { get; set; }

        /// <summary>
        /// Mip mapping
        /// </summary>
        [XmlElement(ElementName = "mipmapping")]
        public bool Mipmapping { get; set; }

        /// <summary>
        /// Width
        /// </summary>
        [XmlElement(ElementName = "width")]
        public int Width { get; set; }

        /// <summary>
        /// Height
        /// </summary>
        [XmlElement(ElementName = "height")]
        public int Height { get; set; }

        /// <summary>
        /// Size
        /// </summary>
        [XmlElement(ElementName = "size")]
        public int Size { get; set; }

        /// <summary>
        /// Count
        /// </summary>
        [XmlElement(ElementName = "count")]
        public int Count { get; set; }
    }

    /// <summary>
    /// Pooled device buffers information
    /// </summary>
    [XmlRoot(ElementName = "pooled_device_buffers")]
    public class PooledDeviceBuffers
    {
        /// <summary>
        /// Device buffer pool information
        /// </summary>
        [XmlElement(ElementName = "device_buffer_pool")]
        public DeviceBufferPool DeviceBufferPool { get; set; }

        /// <summary>
        /// Total count
        /// </summary>
        [XmlElement(ElementName = "total_count")]
        public int TotalCount { get; set; }

        /// <summary>
        /// Total size
        /// </summary>
        [XmlElement(ElementName = "total_size")]
        public int TotalSize { get; set; }
    }

    /// <summary>
    /// Host buffer pool informaiton
    /// </summary>
    [XmlRoot(ElementName = "host_buffer_pool")]
    public class HostBufferPool
    {
        /// <summary>
        /// Usage
        /// </summary>
        [XmlElement(ElementName = "usage")]
        public string Usage { get; set; }

        /// <summary>
        /// Size
        /// </summary>
        [XmlElement(ElementName = "size")]
        public int Size { get; set; }

        /// <summary>
        /// Count
        /// </summary>
        [XmlElement(ElementName = "count")]
        public int Count { get; set; }
    }

    /// <summary>
    /// Pooled host buffers
    /// </summary>
    [XmlRoot(ElementName = "pooled_host_buffers")]
    public class PooledHostBuffers
    {
        /// <summary>
        /// Host buffer pool
        /// </summary>
        [XmlElement(ElementName = "host_buffer_pool")]
        public HostBufferPool HostufferPool { get; set; }

        /// <summary>
        /// Total read count
        /// </summary>
        [XmlElement(ElementName = "total_read_count")]
        public int TotalReadCount { get; set; }

        /// <summary>
        /// Total write count
        /// </summary>
        [XmlElement(ElementName = "total_write_count")]
        public int TotalWriteCount { get; set; }

        /// <summary>
        /// Total read size
        /// </summary>
        [XmlElement(ElementName = "total_read_size")]
        public int TotalReadSize { get; set; }

        /// <summary>
        /// Total write size
        /// </summary>
        [XmlElement(ElementName = "total_write_size")]
        public int TotalWriteSize { get; set; }
    }

    /// <summary>
    /// Details
    /// </summary>
    [XmlRoot(ElementName = "details")]
    public class Details
    {
        /// <summary>
        /// Pool device buffers
        /// </summary>
        [XmlElement(ElementName = "pooled_device_buffers")]
        public PooledDeviceBuffers PooledDeviceBuffers { get; set; }

        /// <summary>
        /// Pooled host buffers
        /// </summary>
        [XmlElement(ElementName = "pooled_host_buffers")]
        public PooledHostBuffers PooledHostBuffers { get; set; }
    }

    /// <summary>
    /// All device buffers
    /// </summary>
    [XmlRoot(ElementName = "all_device_buffers")]
    public class AllDeviceBuffers
    {
        /// <summary>
        /// Total count
        /// </summary>
        [XmlElement(ElementName = "total_count")]
        public int TotalCount { get; set; }

        /// <summary>
        /// Total Size
        /// </summary>
        [XmlElement(ElementName = "total_size")]
        public int TotalSize { get; set; }
    }

    /// <summary>
    /// All host buffers
    /// </summary>
    [XmlRoot(ElementName = "all_host_buffers")]
    public class AllHostBuffers
    {
        /// <summary>
        /// Total read count
        /// </summary>
        [XmlElement(ElementName = "total_read_count")]
        public int TotalReadCount { get; set; }
        
        /// <summary>
        /// Total write count
        /// </summary>
        [XmlElement(ElementName = "total_write_count")]
        public int TotalWriteCount { get; set; }
        
        /// <summary>
        /// Total read size
        /// </summary>
        [XmlElement(ElementName = "total_read_size")]
        public int TotalReadSize { get; set; }
        
        /// <summary>
        /// Total write size
        /// </summary>
        [XmlElement(ElementName = "total_write_size")]
        public int TotalWriteSize { get; set; }
    }

    /// <summary>
    /// SUmmary information
    /// </summary>
    [XmlRoot(ElementName = "summary")]
    public class Summary
    {
        /// <summary>
        /// Pooled device buffers
        /// </summary>
        [XmlElement(ElementName = "pooled_device_buffers")]
        public PooledDeviceBuffers PooledDeviceBuffers { get; set; }
        
        /// <summary>
        /// All device buffers
        /// </summary>
        [XmlElement(ElementName = "all_device_buffers")]
        public AllDeviceBuffers AllDeviceBuffers { get; set; }
        
        /// <summary>
        /// Pooled host buffers
        /// </summary>
        [XmlElement(ElementName = "pooled_host_buffers")]
        public PooledHostBuffers PooledHostBuffers { get; set; }
        
        /// <summary>
        /// All host buffers
        /// </summary>
        [XmlElement(ElementName = "all_host_buffers")]
        public AllHostBuffers AllHostBuffers { get; set; }
    }

    /// <summary>
    /// OpenGL info
    /// </summary>
    [XmlRoot(ElementName = "gl")]
    public class GLInfo
    {
        /// <summary>
        /// Details
        /// </summary>
        [XmlElement(ElementName = "details")]
        public Details Details { get; set; }
        
        /// <summary>
        /// Summary information
        /// </summary>
        [XmlElement(ElementName = "summary")]
        public Summary Summary { get; set; }

        /// <summary>
        /// Xml serialzed version
        /// </summary>
        public string Xml { get; set; }
    }
}
