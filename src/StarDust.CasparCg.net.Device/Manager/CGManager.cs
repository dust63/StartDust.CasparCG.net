using StarDust.CasparCG.net.AmcpProtocol;
using StarDust.CasparCG.net.Datas;
using StarDust.CasparCG.net.Models.Info;

namespace StarDust.CasparCG.net.Device
{
    /// <summary>
    /// Class that manage action to the Character Generator for the given channel
    /// </summary>
    public class CGManager
    {
        #region Properties  

        /// <summary>
        /// Parser to send data and receive info;
        /// </summary>
        public IAMCPTcpParser AmcpTcpParser { get; }

        /// <summary>
        /// Channel instance
        /// </summary>
        protected ChannelInfo Channel { get; set; }

        #endregion


        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="channel">Parent channel</param>
        /// <param name="amcpTcpParser"></param>
        public CGManager(ChannelInfo channel, IAMCPTcpParser amcpTcpParser)
        {
            AmcpTcpParser = amcpTcpParser;
            Channel = channel;
        }

        #region AddCommand  

        /// <summary>
        /// Prepares a template for displaying. Do not play directly the template.
        /// </summary>
        /// <param name="cgLayer">the cg/flash layer where to add the template</param>
        /// <param name="template">template name</param>
        /// <returns></returns>
        public virtual AMCPError Add(uint cgLayer, string template)
        {
            return Add(cgLayer, template, false, string.Empty);
        }


        /// <summary>
        /// Prepares a template for displaying. Do not play directly the template.
        /// </summary>
        /// <param name="videoLayer">video layer where you want to load template</param>
        /// <param name="cgLayer">the cg/flash layer where to add the template</param>
        /// <param name="template">template name</param>
        /// <returns></returns>
        public virtual AMCPError Add(int videoLayer, uint cgLayer, string template)
        {
            return Add(videoLayer, cgLayer, template, false, string.Empty);
        }

        /// <summary>
        ///  Prepares a template for displaying.
        /// </summary>
        /// <param name="cgLayer">the cg/flash layer where to add the template</param>
        /// <param name="template">template name</param>
        /// <param name="autoPlay">true if you want to play directly the template onair</param>
        /// <returns></returns>
        public virtual AMCPError Add(uint cgLayer, string template, bool autoPlay)
        {
            return Add(cgLayer, template, autoPlay, string.Empty);
        }

        /// <summary>
        /// Prepares a template for displaying.
        /// </summary>
        /// <param name="videoLayer">video layer where you want to load template</param>
        /// <param name="cgLayer">the cg/flash layer where to add the template</param>
        /// <param name="template">template name</param>
        /// <param name="autoPlay">true if you want to play directly the template onair</param>
        /// <returns></returns>
        public virtual AMCPError Add(int videoLayer, uint cgLayer, string template, bool autoPlay)
        {
            return Add(videoLayer, cgLayer, template, autoPlay, string.Empty);
        }

        /// <summary>
        /// Prepares a template for displaying but do not play directly the template.. Provide data to the template in xml string. 
        /// </summary>
        /// <param name="cgLayer">the cg/flash layer where to add the template</param>
        /// <param name="template">template name</param>
        /// <param name="data">data in xml string</param>
        /// <returns></returns>
        public virtual AMCPError Add(uint cgLayer, string template, string data)
        {
            return Add(cgLayer, template, false, data);
        }


        /// <summary>
        /// Prepares a template for displaying but do not play directly the template. Provide data to the template in xml string. 
        /// </summary>
        /// <param name="videoLayer">video layer where you want to load template</param>
        /// <param name="cgLayer">the cg/flash layer where to add the template</param>
        /// <param name="template">template name</param>
        /// <param name="data">data in xml string</param>   
        /// <returns></returns>
        public virtual AMCPError Add(int videoLayer, uint cgLayer, string template, string data)
        {
            return Add(videoLayer, cgLayer, template, false, data);
        }


        /// <summary>
        /// Prepares a template for displaying. Provide data to the template in xml string. 
        /// </summary>      
        /// <param name="cgLayer">the cg/flash layer where to add the template</param>
        /// <param name="autoPlay">true if you want to play directly the template onair</param>
        /// <param name="template">template name</param>
        /// <param name="data">data in xml string</param>   
        /// <returns></returns>
        public virtual AMCPError Add(uint cgLayer, string template, bool autoPlay, string data)
        {

            var command = $"CG {Channel.ID} ADD {cgLayer} \"{template}\" {(autoPlay ? 1 : 0)} {(string.IsNullOrEmpty(data) ? string.Empty : $"\"{data}\"")}";
            return AmcpTcpParser.SendCommandAndGetStatus(command);
        }

        /// <summary>
        /// Prepares a template for displaying. Provide data to the template in xml string. 
        /// </summary>      
        /// <param name="videoLayer">video layer where you want to load template</param>
        /// <param name="cgLayer">the cg/flash layer where to add the template</param>
        /// <param name="autoPlay">true if you want to play directly the template onair</param>
        /// <param name="template">template name</param>
        /// <param name="data">data in xml string</param>   
        /// <returns></returns>
        public virtual AMCPError Add(int videoLayer, uint cgLayer, string template, bool autoPlay, string data)
        {
            var command = $"CG {Channel.ID}-{videoLayer} ADD {cgLayer} \"{template}\" {(autoPlay ? 1 : 0)} {(string.IsNullOrEmpty(data) ? string.Empty : $"\"{data}\"")}";
            return AmcpTcpParser.SendCommandAndGetStatus(command);
        }

        /// <summary>
        /// Prepares a template for displaying but do not play directly the template.. Provide data will be serialize to xml. 
        /// </summary>      
        /// <param name="videoLayer"></param>
        /// <param name="template">template name</param>
        /// <param name="data">data in xml string</param>       
        /// <returns></returns>
        public virtual AMCPError Add(uint videoLayer, string template, ICGDataContainer data)
        {
            return Add(videoLayer, template, false, data);
        }

        /// <summary>
        /// Prepares a template for displaying but do not play directly the template.. Provide data will be serialize to xml. 
        /// </summary>      
        /// <param name="videoLayer">video layer where you want to load template</param>
        /// <param name="cgLayer">the cg/flash layer where to add the template</param>
        /// <param name="template">template name</param>
        /// <param name="data">data in xml string</param>       
        /// <returns></returns>
        public virtual AMCPError Add(int videoLayer, uint cgLayer, string template, ICGDataContainer data)
        {
            return Add(videoLayer, cgLayer, template, false, data);
        }

        /// <summary>
        /// Prepares a template for displaying. Provide data will be serialize to xml. 
        /// </summary>           
        /// <param name="videoLayer"></param>
        /// <param name="template">template name</param>
        /// <param name="autoPlay">true if you want to play directly the template onair</param>
        /// <param name="data">data in xml string</param>       
        /// <returns></returns>
        public virtual AMCPError Add(uint videoLayer, string template, bool autoPlay, ICGDataContainer data)
        {
            var command = $"CG {Channel.ID}-{videoLayer} ADD 1 \"{template}\" {(autoPlay ? 1 : 0)} {(data == null ? new CasparCGDataCollection().ToXml() : $"\"{data.ToXml()}\"")}";
            return AmcpTcpParser.SendCommandAndGetStatus(command);
        }


        /// <summary>
        /// Prepares a template for displaying. Provide data will be serialize to xml. 
        /// </summary>           
        /// <param name="videoLayer">video layer where you want to load template</param>
        /// <param name="cgLayer">the cg/flash layer where to add the template</param>
        /// <param name="template">template name</param>
        /// <param name="autoPlay">true if you want to play directly the template onair</param>
        /// <param name="data">data in xml string</param>       
        /// <returns></returns>
        public virtual AMCPError Add(int videoLayer, uint cgLayer, string template, bool autoPlay, ICGDataContainer data)
        {
            var command = $"CG {Channel.ID}-{videoLayer} ADD {cgLayer} \"{template}\" {(autoPlay ? 1 : 0)} {(data == null ? string.Empty : $"\"{data.ToXml()}\"")}";
            return AmcpTcpParser.SendCommandAndGetStatus(command);
        }


        #endregion

        #region Remove      

        /// <summary>
        /// Removes the template from the specified layer.
        /// </summary>
        /// <param name="cgLayer">the cg/flash layer where to remove the template</param>
        /// <returns></returns>
        public virtual AMCPError Remove(uint cgLayer)
        {
            return AmcpTcpParser.SendCommandAndGetStatus($"CG {Channel.ID} REMOVE {cgLayer}");
        }

        /// <summary>
        /// Removes the template from the specified layer.
        /// </summary>
        /// <param name="videoLayer">video layer where you want to remove the template</param>
        /// <param name="cgLayer">the cg/flash layer where to remove the template</param>
        /// <returns></returns>
        public virtual AMCPError Remove(int videoLayer, uint cgLayer)
        {
            return AmcpTcpParser.SendCommandAndGetStatus($"CG {Channel.ID}-{videoLayer} REMOVE {cgLayer}");
        }

        #endregion

        #region Clear

        /// <summary>
        /// Removes all templates on the channel. The entire cg producer will be removed.
        /// </summary>
        /// <returns></returns>
        public virtual AMCPError Clear()
        {
            return AmcpTcpParser.SendCommandAndGetStatus($"CG {Channel.ID} CLEAR");
        }

        /// <summary>
        /// Removes all templates on a video layer. The entire cg producer will be removed.
        /// </summary>
        /// <param name="videoLayer">video layer where you want to clear template</param>
        /// <returns></returns>
        public virtual AMCPError Clear(int videoLayer)
        {
            return AmcpTcpParser.SendCommandAndGetStatus($"CG {Channel.ID}-{videoLayer} CLEAR");
        }

        #endregion

        #region Play

        /// <summary>
        /// Plays and displays the template in the specified layer.
        /// </summary>
        /// <returns></returns>
        public virtual AMCPError Play()
        {
            return AmcpTcpParser.SendCommandAndGetStatus($"CG {Channel.ID} PLAY 1");
        }

        /// <summary>
        /// Plays and displays the template in the specified layer.
        /// </summary>
        /// <param name="videoLayer"></param>
        /// <returns></returns>
        public virtual AMCPError Play(uint videoLayer)
        {
            return AmcpTcpParser.SendCommandAndGetStatus($"CG {Channel.ID}-{videoLayer} PLAY 1");
        }

        /// <summary>
        /// Plays and displays the template in the specified layer.
        /// </summary>
        /// <param name="videoLayer">>the video layer where you wantto play the template</param>
        /// <param name="cgLayer">the cg/flash layer where you want to play the template</param>
        /// <returns></returns>
        public virtual AMCPError Play(int videoLayer, uint cgLayer)
        {
            return AmcpTcpParser.SendCommandAndGetStatus($"CG {Channel.ID}-{videoLayer} PLAY {cgLayer}");
        }

        /// <summary>
        /// Plays and displays the template in the specified layer.
        /// </summary>
        /// <param name="videoLayer">>the video layer where you want to play the template</param>
        /// <param name="cgLayer">the cg/flash layer where you want to play the template</param>
        /// <param name="data">data to sent to the template</param>
        /// <returns></returns>
        public virtual AMCPError Play(int videoLayer, uint cgLayer, ICGDataContainer data)
        {
            return AmcpTcpParser.SendCommandAndGetStatus($"CG {Channel.ID}-{videoLayer} PLAY {cgLayer} \"{data.ToXml()}\"");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="videoLayer">>the video layer where you want to play the template</param>
        /// <param name="cgLayer">the cg/flash layer where you want to play the template</param>
        /// <param name="data">data to sent to the template in xml</param>
        /// <returns></returns>
        public virtual AMCPError Play(int videoLayer, uint cgLayer, string data)
        {
            return AmcpTcpParser.SendCommandAndGetStatus($"CG {Channel.ID}-{videoLayer} PLAY {cgLayer} \"{data}\"");
        }


        #endregion

        #region Stop

        /// <summary>
        /// Stops and removes the template from the specified layer. This is different from REMOVE in that the template gets a chance to animate out when it is stopped.
        /// </summary>
        /// <param name="cgLayer"></param>
        /// <returns></returns>
        public virtual AMCPError Stop(uint cgLayer)
        {
            return AmcpTcpParser.SendCommandAndGetStatus($"CG {Channel.ID} STOP {cgLayer}");
        }

        /// <summary>
        /// Stops and removes the template from the specified layer. This is different from REMOVE in that the template gets a chance to animate out when it is stopped.
        /// </summary>
        /// <param name="videoLayer"></param>
        /// <param name="cgLayer"></param>
        /// <returns></returns>
        public virtual AMCPError Stop(int videoLayer, uint cgLayer)
        {
            return AmcpTcpParser.SendCommandAndGetStatus($"CG {Channel.ID}-{videoLayer} STOP {cgLayer}");
        }

        #endregion

        #region Next

        /// <summary>
        /// Triggers a "continue" in the template on the specified layer. This is used to control animations that has multiple discreet steps.
        /// </summary>
        /// <param name="cgLayer"></param>
        /// <returns></returns>
        public virtual AMCPError Next(uint cgLayer)
        {
            return AmcpTcpParser.SendCommandAndGetStatus($"CG {Channel.ID} NEXT {cgLayer}");
        }


        /// <summary>
        /// Triggers a "continue" in the template on the specified layer. This is used to control animations that has multiple discreet steps.
        /// </summary>
        /// <param name="videoLayer"></param>
        /// <param name="cgLayer"></param>
        /// <returns></returns>
        public virtual AMCPError Next(int videoLayer, uint cgLayer)
        {
            return AmcpTcpParser.SendCommandAndGetStatus($"CG {Channel.ID}-{videoLayer} NEXT {cgLayer}");
        }

        #endregion

        #region Update

        /// <summary>
        /// Sends new data to the template on specified layer. Data is either inline XML or a reference to a saved dataset.
        /// </summary>
        /// <param name="videoLayer"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual AMCPError Update(uint videoLayer, string data)
        {
            return AmcpTcpParser.SendCommandAndGetStatus($"CG {Channel.ID}-{videoLayer} UPDATE 1 \"{data}\"");
        }

        /// <summary>
        /// Sends new data to the template on specified layer. Data is either inline XML or a reference to a saved dataset.
        /// </summary>
        /// <param name="videoLayer"></param>
        /// <param name="cgLayer"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual AMCPError Update(int videoLayer, uint cgLayer, string data)
        {
            return AmcpTcpParser.SendCommandAndGetStatus($"CG {Channel.ID}-{videoLayer} UPDATE {cgLayer} \"{data}\"");
        }

        /// <summary>
        /// Sends new data to the template on specified layer. Data is either inline XML or a reference to a saved dataset.
        /// </summary>
        /// <param name="videoLayer"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual AMCPError Update(uint videoLayer, ICGDataContainer data)
        {
            return AmcpTcpParser.SendCommandAndGetStatus($"CG {Channel.ID}-{videoLayer} UPDATE 1 \"{data.ToXml()}\"");
        }

        /// <summary>
        /// Sends new data to the template on specified layer. Data is either inline XML or a reference to a saved dataset.
        /// </summary>
        /// <param name="videoLayer"></param>
        /// <param name="cgLayer"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual AMCPError Update(int videoLayer, uint cgLayer, ICGDataContainer data)
        {
            return AmcpTcpParser.SendCommandAndGetStatus($"CG {Channel.ID}-{videoLayer} UPDATE {cgLayer} \"{data.ToXml()}\"");
        }

        #endregion

        #region Invoke         

        /// <summary>
        /// Invokes the given method on the template on the specified layer. Can be used to jump the playhead to a specific label.
        /// </summary>
        /// <param name="cgLayer"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public virtual AMCPError Invoke(uint cgLayer, string method)
        {
            return AmcpTcpParser.SendCommandAndGetStatus($"CG {Channel.ID} INVOKE {cgLayer} {method}");
        }

        /// <summary>
        /// Invokes the given method on the template on the specified layer. Can be used to jump the playhead to a specific label.
        /// </summary>
        /// <param name="videoLayer"></param>
        /// <param name="cgLayer"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public virtual AMCPError Invoke(int videoLayer, uint cgLayer, string method)
        {
            return AmcpTcpParser.SendCommandAndGetStatus($"CG {Channel.ID}-{videoLayer} INVOKE {cgLayer} {method}");
        }

        #endregion

        #region Info

        /// <summary>
        /// Retrieves information about the template host
        /// </summary>
        /// <returns></returns>
        public virtual AMCPError Info()
        {
            return AmcpTcpParser.SendCommandAndGetStatus($"CG {Channel.ID} INFO");
        }


        /// <summary>
        /// Retrieves information about the template host
        /// </summary>
        /// <param name="videoLayer"></param>
        /// <returns></returns>
        public virtual AMCPError Info(int videoLayer)
        {
            return AmcpTcpParser.SendCommandAndGetStatus($"CG {Channel.ID}-{videoLayer} INFO");
        }

        /// <summary>
        /// Retrieves information about the template on the specified layer.
        /// </summary>
        /// <param name="videoLayer"></param>
        /// <param name="cgLayer"></param>
        /// <returns></returns>
        public virtual AMCPError Info(int videoLayer, uint cgLayer)
        {
            return AmcpTcpParser.SendCommandAndGetStatus($"CG {Channel.ID}-{videoLayer} INFO {cgLayer}");
        }

        #endregion
    }
}
