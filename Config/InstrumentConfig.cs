using System.Text.Json.Serialization;

namespace 五通道自动测试.Config
{
    public class InstrumentConfig
    {
        [JsonPropertyName("Instruments")]
        public InstrumentsConfig Instruments { get; set; } = new();
    }

    public class InstrumentsConfig
    {
        [JsonPropertyName("SpectrumAnalyzer")]
        public InstrumentInfo SpectrumAnalyzer { get; set; } = new();

        [JsonPropertyName("SignalGenerator")]
        public InstrumentInfo SignalGenerator { get; set; } = new();

        [JsonPropertyName("ZNB8")]
        public InstrumentInfo ZNB8 { get; set; } = new();

        [JsonPropertyName("ODP3063")]
        public InstrumentInfo ODP3063 { get; set; } = new();
    }

    public class InstrumentInfo
    {
        [JsonPropertyName("Address")]
        public string Address { get; set; } = string.Empty;

        [JsonPropertyName("Name")]
        public string Name { get; set; } = string.Empty;
    }
}
