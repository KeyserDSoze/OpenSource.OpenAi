﻿using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenSource.OpenAi.Edit
{
    /// <summary>
    /// Represents an embedding result returned by the Embedding API.  
    /// </summary>
    public class EditResult : ApiBaseResponse
    {
        /// <summary>
        /// List of results of the embedding
        /// </summary>
        [JsonPropertyName("choices")]
        public List<EditChoice>? Choices { get; set; }
        /// <summary>
        /// Usage statistics of how many tokens have been used for this request
        /// </summary>
        [JsonPropertyName("usage")]
        public EditUsage? Usage { get; set; }
    }
}
