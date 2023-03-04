﻿using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using OpenSource.OpenAi.Models;

namespace OpenSource.OpenAi.Moderation
{
    public sealed class ModerationRequestBuilder : RequestBuilder<ModerationsRequest>
    {
        internal ModerationRequestBuilder(HttpClient client, OpenAiConfiguration configuration, string input)
            : base(client, configuration, () =>
            {
                return new ModerationsRequest()
                {
                    Input = input,
                    ModelId = ModerationModelType.TextModerationLatest.ToModel().Id
                };
            })
        {
        }
        /// <summary>
        /// Classifies if text violates OpenAI's Content Policy.
        /// </summary>
        /// <returns>Builder</returns>
        public ValueTask<ModerationsResponse> ExecuteAsync(CancellationToken cancellationToken = default)
            => _client.PostAsync<ModerationsResponse>(_configuration.GetUri(OpenAi.Moderation, _request.ModelId!), _request, cancellationToken);
        /// <summary>
        /// ID of the model to use.
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Builder</returns>
        public ModerationRequestBuilder WithModel(ModerationModelType model)
        {
            _request.ModelId = model.ToModel().Id;
            return this;
        }
        /// <summary>
        /// ID of the model to use. You can use <see cref="IOpenAiModelApi.AllAsync()"/> to see all of your available models, or use a standard model like <see cref="Model.TextModerationStable"/>.
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Builder</returns>
        public ModerationRequestBuilder WithModel(string modelId)
        {
            _request.ModelId = modelId;
            return this;
        }
    }
}
