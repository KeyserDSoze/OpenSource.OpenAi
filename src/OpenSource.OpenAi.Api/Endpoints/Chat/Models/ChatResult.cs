﻿using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenSource.OpenAi.Chat
{
    /// <summary>
    /// Represents a result from calling the Completion API
    /// </summary>
    public class ChatResult : ApiBaseResponse
    {
        /// <summary>
        /// The identifier of the result, which may be used during troubleshooting
        /// </summary>
        [JsonPropertyName("id")]
        public string? Id { get; set; }
        /// <summary>
        /// The completions returned by the API.  Depending on your request, there may be 1 or many choices.
        /// </summary>
        [JsonPropertyName("choices")]
        public List<ChatChoice>? Completions { get; set; }
        /// <summary>
        /// API token usage as reported by the OpenAI API for this request
        /// </summary>
        [JsonPropertyName("usage")]
        public ChatUsage? Usage { get; set; }
    }
}
