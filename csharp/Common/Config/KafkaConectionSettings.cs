namespace Common.Config
{
    internal class KafkaConectionSettings
    {
        public string BootStrapServers { get; set; }
        public int DefaultPartitionsCount { get; set; }
        public string ConsumptionGroupId { get; set; }
    }
}
