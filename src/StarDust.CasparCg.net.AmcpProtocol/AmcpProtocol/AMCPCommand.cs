using StarDust.CasparCG.net.Models;

namespace StarDust.CasparCG.net.AmcpProtocol
{
    /// <summary>
    /// List of command that can be sent to a CasparCG Server
    /// </summary>
    public enum AMCPCommand
    {

        Undefined = 0,

        None = 1,

        [AMCPCommandValue("LOAD")]
        LOAD,

        [AMCPCommandValue("LOADBG")]
        LOADBG,

        [AMCPCommandValue("PLAY")]
        PLAY,

        [AMCPCommandValue("STOP")]
        STOP,

        [AMCPCommandValue("DATA")]
        DATA,

        [AMCPCommandValue("CLEAR")]
        CLEAR,

        [AMCPCommandValue("SET")]
        SET,

        [AMCPCommandValue("MIXER")]
        MIXER,

        [AMCPCommandValue("CALL")]
        CALL,

        [AMCPCommandValue("REMOVE")]
        REMOVE,

        [AMCPCommandValue("ADD")]
        ADD,

        [AMCPCommandValue("SWAP")]
        SWAP,

        [AMCPCommandValue("THUMBNAIL GENERATE")]
        THUMBNAIL_GENERATE,

        [AMCPCommandValue("THUMBNAIL LIST")]
        THUMBNAIL_LIST,

        [AMCPCommandValue("THUMBNAIL RETRIEVE")]
        THUMBNAIL_RETRIEVE,

        [AMCPCommandValue("THUMBNAIL GENERATE_ALL")]
        THUMBNAIL_GENERATEALL,

        [AMCPCommandValue("CINF")]
        CINF,

        [AMCPCommandValue("CLS")]
        CLS,

        [AMCPCommandValue("FLS")]
        FLS,

        [AMCPCommandValue("TLS")]
        TLS,

        [AMCPCommandValue("STATUS")]
        STATUS,

        [AMCPCommandValue("VERSION")]
        VERSION,

        [AMCPCommandValue("INFO")]
        INFO,

        [AMCPCommandValue("INFO TEMPLATE")]
        INFO_TEMPLATE,

        [AMCPCommandValue("INFO CONFIG")]
        INFO_CONFIG,

        [AMCPCommandValue("INFO PATHS")]
        INFO_PATHS,

        [AMCPCommandValue("INFO SYSTEM")]
        INFO_SYSTEM,

        [AMCPCommandValue("INFO SERVER")]
        INFO_SERVER,

        [AMCPCommandValue("INFO QUEUES")]
        INFO_QUEUES,

        [AMCPCommandValue("INFO THREADS")]
        INFO_THREADS,

        [AMCPCommandValue("INFO DELAY")]
        INFO_DELAY,

        [AMCPCommandValue("DIAG")]
        DIAG,

        [AMCPCommandValue("GLGC")]
        GLGC,

        [AMCPCommandValue("BYE")]
        BYE,

        [AMCPCommandValue("KILL")]
        KILL,

        [AMCPCommandValue("RESTART")]
        RESTART,

        [AMCPCommandValue("HELP")]
        HELP,

        [AMCPCommandValue("HELP PRODUCER")]
        HELP_PRODUCER,

        [AMCPCommandValue("HELP CONSUMER")]
        HELP_CONSUMER,

        [AMCPCommandValue("CHANNEL_GRID")]
        CHANNEL_GRID,

        [AMCPCommandValue("MIXER COMMIT")]
        MIXER_COMMIT,

        [AMCPCommandValue("MIXER GRID")]
        MIXER_GRID,

        [AMCPCommandValue("MIXER STRAIGHT_ALPHA_OUTPUT")]
        MIXER_STRAIGHT_ALPHA_OUTPUT,

        [AMCPCommandValue("MIXER MASTERVOLUME")]
        MIXER_MASTERVOLUME,

        [AMCPCommandValue("MIXER VOLUME")]
        MIXER_VOLUME,

        [AMCPCommandValue("MIXER MIPMAP")]
        MIXER_MIPMAP,

        [AMCPCommandValue("MIXER ROTATION")]
        MIXER_ROTATION,

        [AMCPCommandValue("MIXER CROP")]
        MIXER_CROP,

        [AMCPCommandValue("MIXER PERSPECTIVE")]
        MIXER_PERSPECTIVE,

        [AMCPCommandValue("MIXER ANCHOR")]
        MIXER_ANCHOR,

        [AMCPCommandValue("MIXER CLIP")]
        MIXER_CLIP,

        [AMCPCommandValue("MIXER FILL")]
        MIXER_FILL,

        [AMCPCommandValue("MIXER CONTRAST")]
        MIXER_CONTRAST,

        [AMCPCommandValue("MIXER SATURATION")]
        MIXER_SATURATION,

        [AMCPCommandValue("MIXER BRIGHTNESS")]
        MIXER_BRIGHTNESS,

        [AMCPCommandValue("MIXER OPACITY")]
        MIXER_OPACITY,

        [AMCPCommandValue("MIXER BLEND")]
        MIXER_BLEND,

        [AMCPCommandValue("MIXER CHROMA")]
        MIXER_CHROMA,

        [AMCPCommandValue("MIXER KEYER")]
        MIXER_KEYER,

        [AMCPCommandValue("CG INFO")]
        CG_INFO,

        [AMCPCommandValue("CG INVOKE")]
        CG_INVOKE,

        [AMCPCommandValue("RESUME")]
        RESUME,

        [AMCPCommandValue("PAUSE")]
        PAUSE,

        [AMCPCommandValue("PRINT")]
        PRINT,

        [AMCPCommandValue("LOG LEVEL")]
        LOG_LEVEL,

        [AMCPCommandValue("CG UPDATE")]
        CG_UPDATE,

        [AMCPCommandValue("CG CLEAR")]
        CG_CLEAR,

        [AMCPCommandValue("CG REMOVE")]
        CG_REMOVE,

        [AMCPCommandValue("CG NEXT")]
        CG_NEXT,

        [AMCPCommandValue("CG STOP")]
        CG_STOP,

        [AMCPCommandValue("CG PLAY")]
        CG_PLAY,

        [AMCPCommandValue("CG ADD")]
        CG_ADD,

        [AMCPCommandValue("DATA REMOVE")]
        DATA_REMOVE,

        [AMCPCommandValue("DATA LIST")]
        DATA_LIST,

        [AMCPCommandValue("DATA RETRIEVE")]
        DATA_RETRIEVE,

        [AMCPCommandValue("LOCK")]
        LOCK,

        [AMCPCommandValue("LOG CATEGORY")]
        LOG_CATEGORY,

        [AMCPCommandValue("DATA STORE")]
        DATA_STORE,

        [AMCPCommandValue("MIXER CLEAR")]
        MIXER_CLEAR,


        [AMCPCommandValue("GL INFO")]
        GLINFO
    }



}
