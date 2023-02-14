namespace StarDust.CasparCG.net.Models.Info
{
    /// <summary>
    /// Threads information
    /// </summary>
    public class ThreadsInfo
    {
        /// <summary>
        /// Id of the thread    
        /// </summary>
        public int? Id { get; set; }


        /// <summary>
        /// Name of the process
        /// </summary>
        public string ProcesssName { get; set; }

        /// <summary>
        /// Generate string representation
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Id} - {ProcesssName}";
        }
    }
}
