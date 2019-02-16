namespace StarDust.CasparCG.Models.Info
{
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


        public override string ToString()
        {
            return $"{Id} - {ProcesssName}";
        }
    }
}
