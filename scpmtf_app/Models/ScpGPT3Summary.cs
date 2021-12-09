namespace scpmtf_app.Models
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class ScpGpt3Summary
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("itemNo")]
        public long ItemNo { get; set; }

        [JsonProperty("combatSummary")]
        public string CombatSummary { get; set; }
    }
}
