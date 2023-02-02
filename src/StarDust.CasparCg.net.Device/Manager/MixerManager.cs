using  StarDust.CasparCG.net.AmcpProtocol;
using  StarDust.CasparCG.net.Models;
using  StarDust.CasparCG.net.Models.Info;
using  StarDust.CasparCG.net.Models.Mixer;
using System.Globalization;

namespace  StarDust.CasparCG.net.Device
{
    public class MixerManager
    {
        /// <summary>
        /// Decimal format use by caspar
        /// </summary>
        protected const string DecimalFormat = "0.00";

        /// <summary>
        /// AMCP Data Paser
        /// </summary>
        public IAmcpTcpParser AmcpTcpParser { get; }

        /// <summary>
        /// Channel where the mixer is attached
        /// </summary>
        protected readonly ChannelInfo _channel;

        public MixerManager(ChannelInfo channel, IAmcpTcpParser amcpTcpParser)
        {
            AmcpTcpParser = amcpTcpParser;
            _channel = channel;
        }


        public virtual void Keyer(int layer, bool enable, bool deffered = false)
        {
            string str1 = GetDefferMode(deffered);
            string str2 = enable ? "1" : "0";
            AmcpTcpParser.SendCommandAndGetStatus(string.Format("MIXER {0}-{1} KEYER {2}{3}", _channel.ID, layer, str2, str1));
        }

        public virtual void Chroma(int layer, bool enable, bool deffered = false)
        {
            string str1 = GetDefferMode(deffered);
            string str2 = enable ? "1" : "0";
            AmcpTcpParser.SendCommandAndGetStatus(string.Format("MIXER {0}-{1} CHROMA {2}{3}", _channel.ID, layer, str2, str1));
        }

        public virtual void Chroma(int layer, bool enable, float targetHue, float hueWidth, float minSaturation, float minBrightness, float softness, float spillSupress, float spillSuppresSaturation, bool showMask, bool deffered = false)
        {
            string str1 = GetDefferMode(deffered);
            string str2 = enable ? "1" : "0";
            string str3 = showMask ? "1" : "0";
            AmcpTcpParser.SendCommandAndGetStatus(string.Format("MIXER {0}-{1} CHROMA {2} {3} {4} {5} {6} {7} {8} {9} {10}{11}", _channel.ID, layer, str2, targetHue.ToString("0.00", CultureInfo.InvariantCulture), hueWidth.ToString("0.00", CultureInfo.InvariantCulture), minSaturation.ToString("0.00", CultureInfo.InvariantCulture), minBrightness.ToString("0.00", CultureInfo.InvariantCulture), softness.ToString("0.00", CultureInfo.InvariantCulture), spillSupress.ToString("0.00", CultureInfo.InvariantCulture), spillSuppresSaturation.ToString("0.00", CultureInfo.InvariantCulture), str3, str1));
        }

        public virtual void BlendMode(int layer, BlendMode mode, bool deffered = false)
        {
            BlendMode(layer, mode.ToString(), deffered);
        }

        public virtual void BlendMode(int layer, string mode, bool deffered = false)
        {
            AmcpTcpParser.SendCommandAndGetStatus(string.Format("MIXER {0}-{1} BLEND {2}{3}", _channel.ID, layer, mode.ToUpperInvariant(), GetDefferMode(deffered)));
        }

        public virtual void Opacity(int layer, float opacity, uint duration = 0, Easing ease = Easing.None, bool deffered = false)
        {
            AmcpTcpParser.SendCommandAndGetStatus(string.Format("MIXER {0}-{1} OPACITY {2} {3} {4}{5}", _channel.ID, layer, opacity.ToString("0.00", CultureInfo.InvariantCulture), duration, ease.ToString().ToLowerInvariant(), GetDefferMode(deffered)));
        }

        public virtual void Brightness(int layer, float brightness, uint duration = 0, Easing ease = Easing.None, bool deffered = false)
        {
            AmcpTcpParser.SendCommandAndGetStatus(string.Format("MIXER {0}-{1} BRIGHTNESS {2} {3} {4}{5}", _channel.ID, layer, brightness.ToString("0.00", CultureInfo.InvariantCulture), duration, ease.ToString().ToLowerInvariant(), GetDefferMode(deffered)));
        }

        public virtual void Saturation(int layer, float saturation, uint duration = 0, Easing ease = Easing.None, bool deffered = false)
        {
            AmcpTcpParser.SendCommandAndGetStatus(string.Format("MIXER {0}-{1} SATURATION {2} {3} {4}{5}", _channel.ID, layer, saturation.ToString("0.00", CultureInfo.InvariantCulture), duration, ease.ToString().ToLowerInvariant(), GetDefferMode(deffered)));
        }

        public virtual void Constrast(int layer, float constrast, uint duration = 0, Easing ease = Easing.None, bool deffered = false)
        {
            AmcpTcpParser.SendCommandAndGetStatus(string.Format("MIXER {0}-{1} CONTRAST {2} {3} {4}{5}", _channel.ID, layer, constrast.ToString("0.00", CultureInfo.InvariantCulture), duration, ease.ToString().ToLowerInvariant(), GetDefferMode(deffered)));
        }

        public virtual void Levels(int layer, float minInput, float maxInput, float gamma, float minOutput, float maxOutput, uint duration = 0, Easing ease = Easing.None, bool deffered = false)
        {

            AmcpTcpParser.SendCommandAndGetStatus(string.Format("MIXER {0}-{1} LEVELS {2} {3} {4} {5} {6} {7} {8}{9}", _channel.ID, layer, minInput.ToString("0.00", CultureInfo.InvariantCulture), maxInput.ToString("0.00", CultureInfo.InvariantCulture), gamma.ToString("0.00", CultureInfo.InvariantCulture), minOutput.ToString("0.00", CultureInfo.InvariantCulture), maxOutput.ToString("0.00", CultureInfo.InvariantCulture), duration, ease.ToString().ToLowerInvariant(), GetDefferMode(deffered)));
        }

        public virtual void Fill(int layer, float x, float y, float xScale, float yScale, uint duration = 0, Easing ease = Easing.None, bool deffered = false)
        {

            AmcpTcpParser.SendCommandAndGetStatus(string.Format("MIXER {0}-{1} FILL {2} {3} {4} {5} {6} {7}{8}", _channel.ID, layer, x.ToString("0.00", CultureInfo.InvariantCulture), y.ToString("0.00", CultureInfo.InvariantCulture), xScale.ToString("0.00", CultureInfo.InvariantCulture), yScale.ToString("0.00", CultureInfo.InvariantCulture), duration, ease.ToString().ToLowerInvariant(), GetDefferMode(deffered)));
        }

        public virtual void Clip(int layer, float x, float y, float width, float height, uint duration = 0, Easing ease = Easing.None, bool deffered = false)
        {

            AmcpTcpParser.SendCommandAndGetStatus(string.Format("MIXER {0}-{1} CLIP {2} {3} {4} {5} {6} {7}{8}", _channel.ID, layer, x.ToString("0.00", CultureInfo.InvariantCulture), y.ToString("0.00", CultureInfo.InvariantCulture), width.ToString("0.00", CultureInfo.InvariantCulture), height.ToString("0.00", CultureInfo.InvariantCulture), duration, ease.ToString().ToLowerInvariant(), GetDefferMode(deffered)));
        }

        public virtual void Anchor(int layer, float x, float y, uint duration = 0, Easing ease = Easing.None, bool deffered = false)
        {

            AmcpTcpParser.SendCommandAndGetStatus(string.Format("MIXER {0}-{1} ANCHOR {2} {3} {4} {5}{6}", _channel.ID, layer, x.ToString("0.00", CultureInfo.InvariantCulture), y.ToString("0.00", CultureInfo.InvariantCulture), duration, ease.ToString().ToLowerInvariant(), GetDefferMode(deffered)));
        }
        public virtual void Anchor(int layer, float topLeftX, float topLeftY, float topRightX, float topRightY, float bottomRightX, float bottomRightY, float bottomLeftX, float bottomLeftY, uint duration, Easing ease, bool deffered = false)
        {

            AmcpTcpParser.SendCommandAndGetStatus(string.Format("MIXER {0}-{1} ANCHOR {2} {3} {4} {5} {6} {7} {8} {9} {10} {11}{12}", _channel.ID, layer, topLeftX.ToString("0.00", CultureInfo.InvariantCulture), topLeftY.ToString("0.00", CultureInfo.InvariantCulture), topRightX.ToString("0.00", CultureInfo.InvariantCulture), topRightY.ToString("0.00", CultureInfo.InvariantCulture), bottomRightX.ToString("0.00", CultureInfo.InvariantCulture), bottomRightY.ToString("0.00", CultureInfo.InvariantCulture), bottomLeftX.ToString("0.00", CultureInfo.InvariantCulture), bottomLeftY.ToString("0.00", CultureInfo.InvariantCulture), duration, ease.ToString().ToLowerInvariant(), GetDefferMode(deffered)));
        }

        public virtual void Crop(int layer, float leftEdge, float rightEdge, float topEdge, float bottomEdge, uint duration = 0, Easing ease = Easing.None, bool deffered = false)
        {

            AmcpTcpParser.SendCommandAndGetStatus(string.Format("MIXER {0}-{1} CROP {2} {3} {4} {5} {6} {7}{8}", _channel.ID, layer, leftEdge.ToString("0.00", CultureInfo.InvariantCulture), rightEdge.ToString("0.00", CultureInfo.InvariantCulture), topEdge.ToString("0.00", CultureInfo.InvariantCulture), bottomEdge.ToString("0.00", CultureInfo.InvariantCulture), duration, ease.ToString().ToLowerInvariant(), GetDefferMode(deffered)));
        }

        public virtual void Rotation(int layer, float rotationAngle, uint duration = 0, Easing ease = Easing.None, bool deffered = false)
        {

            AmcpTcpParser.SendCommandAndGetStatus(string.Format("MIXER {0}-{1} ROTATION {2} {3} {4}{5}", _channel.ID, layer, rotationAngle.ToString("0.00", CultureInfo.InvariantCulture), duration, ease.ToString().ToLowerInvariant(), GetDefferMode(deffered)));
        }



        public virtual void MipMap(int layer, bool enable, bool deffered = false)
        {
            AmcpTcpParser.SendCommandAndGetStatus(string.Format("MIXER {0}-{1} MIPMAP {2}{3}", _channel.ID, layer, BoolToNumericString(enable), GetDefferMode(deffered)));
        }

        protected virtual string BoolToNumericString(bool enable)
        {
            return enable ? "1" : "0";
        }

        public virtual void Volume(int layer, float volume, bool deffered = false)
        {

            AmcpTcpParser.SendCommandAndGetStatus(string.Format("MIXER {0}-{1} VOLUME {2}{3}", _channel.ID, layer, volume.ToString("0.00", CultureInfo.InvariantCulture), GetDefferMode(deffered)));
        }

        public virtual void Volume(int layer, float volume, uint duration = 0, Easing ease = Easing.None, bool deffered = false)
        {

            if (ease == Easing.None || duration == 0)
                Volume(layer, volume, deffered);
            else
                AmcpTcpParser.SendCommandAndGetStatus(string.Format("MIXER {0}-{1} VOLUME {2} {3} {4}{5}", _channel.ID, layer, volume.ToString("0.00", CultureInfo.InvariantCulture), duration, ease.ToString().ToLowerInvariant(), GetDefferMode(deffered)));
        }

        public virtual void MasterVolume(int layer, float volume, bool deffered = false)
        {

            AmcpTcpParser.SendCommandAndGetStatus(string.Format("MIXER {0}-{1} MASTERVOLUME {2}{3}", _channel.ID, layer, volume.ToString("0.00", CultureInfo.InvariantCulture), GetDefferMode(deffered)));
        }

        public virtual void StraightAlphaOutput(int layer, bool enable, bool deffered = false)
        {

            string str2 = enable ? "1" : "0";
            AmcpTcpParser.SendCommandAndGetStatus(string.Format("MIXER {0}-{1} STRAIGHT_ALPHA_OUTPUT {2}{3}", _channel.ID, layer, str2, GetDefferMode(deffered)));
        }

        public virtual void StraightAlphaOutput(int layer, int gridSize, uint duration = 0, Easing ease = Easing.None, bool deffered = false)
        {

            AmcpTcpParser.SendCommandAndGetStatus(string.Format("MIXER {0}-{1} GRID {2} {3} {4}{5}", _channel.ID, layer, gridSize, duration, ease.ToString().ToLowerInvariant(), GetDefferMode(deffered)));
        }

        protected virtual string GetDefferMode(bool deffered)
        {
            return deffered ? " DEFER" : string.Empty;
        }

        public virtual void Commit(int layer)
        {
            AmcpTcpParser.SendCommandAndGetStatus(string.Format("MIXER {0}-{1} COMMIT", _channel.ID, layer));
        }
    }
}
