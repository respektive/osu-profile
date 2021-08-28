using Newtonsoft.Json;

namespace osu_Profile.OsuAPIObjects
{
    public class Player
    {
        /// <summary>
        /// The user id of the player
        /// </summary>
        [JsonProperty("user_id", NullValueHandling = NullValueHandling.Ignore)]
        public int ID { get; set; }

        /// <summary>
        /// The user name of the player
        /// </summary>
        [JsonProperty("username")]
        public string Username { get; set; }

        /// <summary>
        /// Counts the best individual score on each ranked and approved beatmaps
        /// </summary>
        [JsonProperty("ranked_score", NullValueHandling = NullValueHandling.Ignore)]
        public long RankedScore { get; set; }

        /// <summary>
        /// Counts every score on ranked and approved beatmaps
        /// </summary>
        [JsonProperty("total_score", NullValueHandling = NullValueHandling.Ignore)]
        public long Score { get; set; }

        /// <summary>
        /// The rank in performance ranking of the player
        /// </summary>
        [JsonProperty("pp_rank", NullValueHandling = NullValueHandling.Ignore)]
        public int PPRank { get; set; }

        /// <summary>
        /// Score api return
        /// </summary>
        [JsonProperty("score_rank", NullValueHandling = NullValueHandling.Ignore)]
        public Scoerapi scoerinfo { get; set; }

        /// <summary>
        /// The rank in performance ranking of the player in the country
        /// </summary>
        [JsonProperty("pp_country_rank", NullValueHandling = NullValueHandling.Ignore)]
        public int PPCountryRank { get; set; }

        /// <summary>
        /// The level of the player
        /// </summary>
        [JsonProperty("level", NullValueHandling = NullValueHandling.Ignore)]
        public float Level { get; set; }

        /// <summary>
        /// The number of PP of the player
        /// </summary>
        [JsonProperty("pp_raw", NullValueHandling = NullValueHandling.Ignore)]
        public float PP { get; set; }

        /// <summary>
        /// The accuracy of the player
        /// </summary>
        [JsonProperty("accuracy", NullValueHandling = NullValueHandling.Ignore)]
        public float Accuracy { get; set; }

        /// <summary>
        /// The time played by the player
        /// </summary>
        [JsonProperty("total_seconds_played", NullValueHandling = NullValueHandling.Ignore)]
        public int PlayTime { get; set; }

        /// <summary>
        /// The number of maps played by the player
        /// </summary>
        [JsonProperty("playcount", NullValueHandling = NullValueHandling.Ignore)]
        public float PlayCount { get; set; }

        /// <summary>
        /// The number of total hits by the player
        /// </summary>
        [JsonProperty("count300", NullValueHandling = NullValueHandling.Ignore)]
        public float Count300 { get; set; }
        [JsonProperty("count100", NullValueHandling = NullValueHandling.Ignore)]
        public float Count100 { get; set; }
        [JsonProperty("count50", NullValueHandling = NullValueHandling.Ignore)]
        public float Count50 { get; set; }


        /// <summary>
        /// The number of total hits devided by placount by the player
        /// </summary>
        public float HitsPerPlay { get; set; }

        /// <summary>
        /// The number of A ranks made by the player
        /// </summary>
        [JsonProperty("count_rank_a", NullValueHandling = NullValueHandling.Ignore)]
        public int RankA { get; set; }

        /// <summary>
        /// The number of S ranks made by the player
        /// </summary>
        [JsonProperty("count_rank_s", NullValueHandling = NullValueHandling.Ignore)]
        public int RankS { get; set; }

        /// <summary>
        /// The number of SH ranks made by the player
        /// </summary> 
        [JsonProperty("count_rank_sh", NullValueHandling = NullValueHandling.Ignore)]
        public int RankSH { get; set; }

        /// <summary>
        /// The number of SS ranks made by the player
        /// </summary>
        [JsonProperty("count_rank_ss", NullValueHandling = NullValueHandling.Ignore)]
        public int RankSS { get; set; }

        /// <summary>
        /// The number of SSH ranks made by the player
        /// </summary>
        [JsonProperty("count_rank_ssh", NullValueHandling = NullValueHandling.Ignore)]
        public int RankSSH { get; set; }

        /// <summary>
        /// The number of total S ranks made by the player
        /// </summary>
        public int TotalS { get; set; }

        /// <summary>
        /// User's top scores
        /// <summary>
        public Score[] TopRanks { get; set; }

        /// <summary>
        /// Profile mode
        /// </summary>
        public int Mode { get; set; }

        /// <summary>
        /// Number of clears the player has
        /// </summary>
        [JsonProperty("clears", NullValueHandling = NullValueHandling.Ignore)]
        public int Clears { get; set; }

        /// <summary>
        /// Completion percentage (All Ranked and loved maps)
        /// </summary>
        [JsonProperty("completion", NullValueHandling = NullValueHandling.Ignore)]
        public int Completion { get; set; }
    }
}
