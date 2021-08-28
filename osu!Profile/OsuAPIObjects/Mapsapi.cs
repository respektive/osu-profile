using Newtonsoft.Json;

namespace osu_Profile.OsuAPIObjects
{
    public class Mapsapi
    {
        /// <summary>
        /// The rank in score ranking of the player
        /// </summary>
        [JsonProperty("loved+ranked", NullValueHandling = NullValueHandling.Ignore)]
        public float GoodMaps { get; set; }
    }
}
