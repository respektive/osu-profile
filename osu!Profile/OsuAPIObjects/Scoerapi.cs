using Newtonsoft.Json;

namespace osu_Profile.OsuAPIObjects
{
    public class Scoerapi
    {
        /// <summary>
        /// The rank in score ranking of the player
        /// </summary>
        [JsonProperty("rank")]
        public int ScoreRank { get; set; }

        /// <summary>
        /// The user ID of the player from score ranking API
        /// </summary>
        [JsonProperty("user_id")]
        public int ID { get; set; }

        /// <summary>
        /// The username of the player from score ranking API
        /// </summary>
        [JsonProperty("username")]
        public string Scoer_username { get; set; }

        /// <summary>
        /// Ranked Score of the player from score ranking API
        [JsonProperty("score")]
        public long SCOER { get; set; }
    }
}
