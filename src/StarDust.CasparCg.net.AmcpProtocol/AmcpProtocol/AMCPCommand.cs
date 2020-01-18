using StarDust.CasparCG.net.Models;

namespace StarDust.CasparCG.net.AmcpProtocol
{
    /// <summary>
    /// List of command that can be sent to a CasparCG Server
    /// </summary>
    public enum AMCPCommand
    {
        /// <summary>
        /// Not defined command
        /// </summary>
        Undefined = 0,

        /// <summary>
        /// None
        /// </summary>
        None = 1,

        /// <summary>
        /// Load command
        /// </summary>
        [AMCPCommandValue("LOAD")]
        LOAD,

        /// <summary>
        /// LOADBG command
        /// </summary>
        [AMCPCommandValue("LOADBG")]
        LOADBG,

        /// <summary>
        /// PLAY command
        /// </summary>
        [AMCPCommandValue("PLAY")]
        PLAY,

        /// <summary>
        /// STOP command
        /// </summary>
        [AMCPCommandValue("STOP")]
        STOP,

        /// <summary>
        /// DATA command
        /// </summary>
        [AMCPCommandValue("DATA")]
        DATA,

        /// <summary>
        /// CLEAR command
        /// </summary>
        [AMCPCommandValue("CLEAR")]
        CLEAR,

        /// <summary>
        /// SET command
        /// </summary>
        [AMCPCommandValue("SET")]
        SET,

        /// <summary>
        /// MIXER command
        /// </summary>
        [AMCPCommandValue("MIXER")]
        MIXER,


        /// <summary>
        /// CALL command
        /// </summary>
        [AMCPCommandValue("CALL")]
        CALL,

        /// <summary>
        /// REMOVE command
        /// </summary>
        [AMCPCommandValue("REMOVE")]
        REMOVE,

        /// <summary>
        /// ADD command
        /// </summary>
        [AMCPCommandValue("ADD")]
        ADD,

        /// <summary>
        /// SWAP command
        /// </summary>
        [AMCPCommandValue("SWAP")]
        SWAP,

        /// <summary>
        /// THUMBNAIL GENERATE command
        /// </summary>
        [AMCPCommandValue("THUMBNAIL GENERATE")]
        THUMBNAIL_GENERATE,


        /// <summary>
        /// THUMBNAIL LIST command
        /// </summary>
        [AMCPCommandValue("THUMBNAIL LIST")]
        THUMBNAIL_LIST,

        /// <summary>
        /// THUMBNAIL RETRIEVE command
        /// </summary>
        [AMCPCommandValue("THUMBNAIL RETRIEVE")]
        THUMBNAIL_RETRIEVE,

        /// <summary>
        /// THUMBNAIL GENERATEALL command
        /// </summary>
        [AMCPCommandValue("THUMBNAIL GENERATE_ALL")]
        THUMBNAIL_GENERATEALL,

        /// <summary>
        /// CINF command
        /// </summary>
        [AMCPCommandValue("CINF")]
        CINF,

        ///<summary>
        /// CLS command
        /// </summary>
        [AMCPCommandValue("CLS")]
        CLS,

        /// <summary>
        /// FLS command
        /// </summary>
        [AMCPCommandValue("FLS")]
        FLS,

        /// <summary>
        /// TLS command
        /// </summary>
        [AMCPCommandValue("TLS")]
        TLS,

        /// <summary>
        /// STATUS command
        /// </summary>
        [AMCPCommandValue("STATUS")]
        STATUS,


        /// <summary>
        /// VERSION command
        /// </summary>
        [AMCPCommandValue("VERSION")]
        VERSION,

        ///<summary>
        /// INFO command
        /// </summary>
        [AMCPCommandValue("INFO")]
        INFO,

        /// <summary>
        ///INFO TEMPLATE command
        /// </summary>
        [AMCPCommandValue("INFO TEMPLATE")]
        INFO_TEMPLATE,

        /// <summary>
        ///INFO CONFIG command
        /// </summary>
        [AMCPCommandValue("INFO CONFIG")]
        INFO_CONFIG,

        /// <summary>
        /// INFO PATHS command
        /// </summary>
        [AMCPCommandValue("INFO PATHS")]
        INFO_PATHS,

        ///<summary>
        /// INFO SYSTEM command
        /// </summary>
        [AMCPCommandValue("INFO SYSTEM")]
        INFO_SYSTEM,

        /// <summary>
        /// INFO SYSTEM command
        /// </summary>
        [AMCPCommandValue("INFO SERVER")]
        INFO_SERVER,


        /// <summary>
        /// INFO QUEUES command
        /// </summary>
        [AMCPCommandValue("INFO QUEUES")]
        INFO_QUEUES,

        /// <summary>
        /// INFO THREADS command
        /// </summary>
        [AMCPCommandValue("INFO THREADS")]
        INFO_THREADS,

        /// <summary>
        /// INFO DELAY command
        /// </summary>
        [AMCPCommandValue("INFO DELAY")]
        INFO_DELAY,

        /// <summary>
        /// DIAG command
        /// </summary>
        [AMCPCommandValue("DIAG")]
        DIAG,

        /// <summary>
        /// GLGC command
        /// </summary>
        [AMCPCommandValue("GLGC")]
        GLGC,

        /// <summary>
        /// BYE command
        /// </summary>
        [AMCPCommandValue("BYE")]
        BYE,


        /// <summary>
        /// KILL command
        /// </summary>
        [AMCPCommandValue("KILL")]
        KILL,

        /// <summary>
        /// RESTART command
        /// </summary>
        [AMCPCommandValue("RESTART")]
        RESTART,

        /// <summary>
        /// HELP command
        /// </summary>
        [AMCPCommandValue("HELP")]
        HELP,

        /// <summary>
        /// HELP PRODUCER command
        /// </summary>
        [AMCPCommandValue("HELP PRODUCER")]
        HELP_PRODUCER,

        /// <summary>
        /// HELP_CONSUMER command
        /// </summary>
        [AMCPCommandValue("HELP CONSUMER")]
        HELP_CONSUMER,

        /// <summary>
        /// CHANNEL GRID command
        /// </summary>
        [AMCPCommandValue("CHANNEL_GRID")]
        CHANNEL_GRID,

        /// <summary>
        /// MIXER_COMMIT command
        /// </summary>
        [AMCPCommandValue("MIXER COMMIT")]
        MIXER_COMMIT,

        /// <summary>
        /// MIXER_GRID command
        /// </summary>
        [AMCPCommandValue("MIXER GRID")]
        MIXER_GRID,

        /// <summary>
        /// MIXER_STRAIGHT ALPHA OUTPUT command
        /// </summary>
        [AMCPCommandValue("MIXER STRAIGHT_ALPHA_OUTPUT")]
        MIXER_STRAIGHT_ALPHA_OUTPUT,

        /// <summary>
        /// MIXER MASTERVOLUME command
        /// </summary>
        [AMCPCommandValue("MIXER MASTERVOLUME")]
        MIXER_MASTERVOLUME,

        /// <summary>
        /// MIXER_VOLUME command
        /// </summary>
        [AMCPCommandValue("MIXER VOLUME")]
        MIXER_VOLUME,

        /// <summary>
        /// MIXER MIPMAP command
        /// </summary>
        [AMCPCommandValue("MIXER MIPMAP")]
        MIXER_MIPMAP,

        /// <summary>
        /// MIXER ROTATION command
        /// </summary>
        [AMCPCommandValue("MIXER ROTATION")]
        MIXER_ROTATION,

        /// <summary>
        /// MIXER CROP command
        /// </summary>
        [AMCPCommandValue("MIXER CROP")]
        MIXER_CROP,

        /// <summary>
        /// MIXER PERSPECTIVE command
        /// </summary>
        [AMCPCommandValue("MIXER PERSPECTIVE")]
        MIXER_PERSPECTIVE,

        /// <summary>
        /// MIXER ANCHOR command
        /// </summary>
        [AMCPCommandValue("MIXER ANCHOR")]
        MIXER_ANCHOR,

        /// <summary>
        /// MIXER CLIP command
        /// </summary>
        [AMCPCommandValue("MIXER CLIP")]
        MIXER_CLIP,

        /// <summary>
        /// MIXER FILL command
        /// </summary>
        [AMCPCommandValue("MIXER FILL")]
        MIXER_FILL,

        /// <summary>
        /// MIXER_CONTRAST command
        /// </summary>
        [AMCPCommandValue("MIXER CONTRAST")]
        MIXER_CONTRAST,

        /// <summary>
        /// MIXER SATURATION command
        /// </summary>
        [AMCPCommandValue("MIXER SATURATION")]
        MIXER_SATURATION,

        /// <summary>
        /// MIXER BRIGHTNESS command
        /// </summary>
        [AMCPCommandValue("MIXER BRIGHTNESS")]
        MIXER_BRIGHTNESS,

        /// <summary>
        /// MIXER OPACITY command
        /// </summary>
        [AMCPCommandValue("MIXER OPACITY")]
        MIXER_OPACITY,

        /// <summary>
        /// MIXER_BLEND command
        /// </summary>
        [AMCPCommandValue("MIXER BLEND")]
        MIXER_BLEND,

        /// <summary>
        /// MIXER CHROMA command
        /// </summary>
        [AMCPCommandValue("MIXER CHROMA")]
        MIXER_CHROMA,

        /// <summary>
        /// MIXER KEYER command
        /// </summary>
        [AMCPCommandValue("MIXER KEYER")]
        MIXER_KEYER,

        /// <summary>
        /// CG INFO command
        /// </summary>
        [AMCPCommandValue("CG INFO")]
        CG_INFO,

        /// <summary>
        /// CG INVOKE command
        /// </summary>
        [AMCPCommandValue("CG INVOKE")]
        CG_INVOKE,

        /// <summary>
        /// RESUME command
        /// </summary>
        [AMCPCommandValue("RESUME")]
        RESUME,

        /// <summary>
        /// PAUSE command
        /// </summary>
        [AMCPCommandValue("PAUSE")]
        PAUSE,

        /// <summary>
        /// PRINT command
        /// </summary>
        [AMCPCommandValue("PRINT")]
        PRINT,

        /// <summary>
        /// LOG LEVEL command
        /// </summary>
        [AMCPCommandValue("LOG LEVEL")]
        LOG_LEVEL,

        /// <summary>
        /// CG UPDATE command
        /// </summary>
        [AMCPCommandValue("CG UPDATE")]
        CG_UPDATE,

        /// <summary>
        /// CG CLEAR command
        /// </summary>
        [AMCPCommandValue("CG CLEAR")]
        CG_CLEAR,

        /// <summary>
        /// CG REMOVE command
        /// </summary>
        [AMCPCommandValue("CG REMOVE")]
        CG_REMOVE,

        /// <summary>
        /// CG NEXT command
        /// </summary>
        [AMCPCommandValue("CG NEXT")]
        CG_NEXT,

        /// <summary>
        /// CG STOP command
        /// </summary>
        [AMCPCommandValue("CG STOP")]
        CG_STOP,

        /// <summary>
        /// CG PLAY command
        /// </summary>
        [AMCPCommandValue("CG PLAY")]
        CG_PLAY,

        /// <summary>
        /// CG ADD command
        /// </summary>
        [AMCPCommandValue("CG ADD")]
        CG_ADD,

        /// <summary>
        /// DATA REMOVE command
        /// </summary>
        [AMCPCommandValue("DATA REMOVE")]
        DATA_REMOVE,

        /// <summary>
        /// DATA LIST command
        /// </summary>
        [AMCPCommandValue("DATA LIST")]
        DATA_LIST,

        /// <summary>
        /// DATA RETRIEVE command
        /// </summary>
        [AMCPCommandValue("DATA RETRIEVE")]
        DATA_RETRIEVE,

        /// <summary>
        /// LOCK command
        /// </summary>
        [AMCPCommandValue("LOCK")]
        LOCK,

        /// <summary>
        /// LOG CATEGORY command
        /// </summary>
        [AMCPCommandValue("LOG CATEGORY")]
        LOG_CATEGORY,

        /// <summary>
        /// DATA STORE command
        /// </summary>
        [AMCPCommandValue("DATA STORE")]
        DATA_STORE,

        /// <summary>
        /// MIXER CLEAR command
        /// </summary>
        [AMCPCommandValue("MIXER CLEAR")]
        MIXER_CLEAR,

        /// <summary>
        /// GLINFO command
        /// </summary>
        [AMCPCommandValue("GL INFO")]
        GLINFO
    }



}
