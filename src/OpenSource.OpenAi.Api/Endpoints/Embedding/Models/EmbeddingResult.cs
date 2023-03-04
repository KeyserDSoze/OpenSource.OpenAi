﻿using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace OpenSource.OpenAi.Embedding
{
    /// <summary>
    /// Represents an embedding result returned by the Embedding API.  
    /// </summary>
    public class EmbeddingResult : ApiBaseResponse
    {
        /// <summary>
        /// List of results of the embedding
        /// </summary>
        [JsonPropertyName("data")]
        public List<EmbdegginData>? Data { get; set; }
        /// <summary>
        /// Usage statistics of how many tokens have been used for this request
        /// </summary>
        [JsonPropertyName("usage")]
        public Usage? Usage { get; set; }
    }
}
