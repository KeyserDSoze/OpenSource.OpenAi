﻿namespace OpenSource.OpenAi.Models
{
    public static class EmbeddingModelTypeExtensions
    {
        private static readonly Model s_adaTextEmbedding = new Model("text-embedding-ada-002");
        public static Model ToModel(this EmbeddingModelType type)
        {
            switch (type)
            {
                default:
                case EmbeddingModelType.AdaTextEmbedding:
                    return s_adaTextEmbedding;
            }
        }
    }
}
