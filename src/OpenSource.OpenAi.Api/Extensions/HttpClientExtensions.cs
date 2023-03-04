﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace OpenSource.OpenAi
{
    public static class HttpClientExtensions
    {
        internal static async Task<HttpResponseMessage> PrivatedExecuteAsync(this HttpClient client,
            string url,
            HttpMethod method,
            object? message,
            bool isStreaming,
            CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(method, url);
            if (message != null)
            {
                if (message is HttpContent httpContent)
                {
                    request.Content = httpContent;
                }
                else
                {
                    var jsonContent = JsonSerializer.Serialize(message);
                    var stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                    request.Content = stringContent;
                }
            }
            var response = await client.SendAsync(request, isStreaming ? HttpCompletionOption.ResponseHeadersRead : HttpCompletionOption.ResponseContentRead, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                return response;
            }
            else
            {
                throw new HttpRequestException(await response.Content.ReadAsStringAsync());
            }
        }
        internal static async ValueTask<TResponse> DeleteAsync<TResponse>(this HttpClient client, string url, CancellationToken cancellationToken)
        {
            var response = await client.PrivatedExecuteAsync(url, HttpMethod.Delete, null, false, cancellationToken);
            var responseAsString = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TResponse>(responseAsString)!;
        }
        internal static async ValueTask<TResponse> GetAsync<TResponse>(this HttpClient client, string url, CancellationToken cancellationToken)
        {
            var response = await client.PrivatedExecuteAsync(url, HttpMethod.Get, null, false, cancellationToken);
            var responseAsString = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TResponse>(responseAsString)!;
        }
        internal static async ValueTask<TResponse> PostAsync<TResponse>(this HttpClient client, string url, object? message, CancellationToken cancellationToken)
        {
            var response = await client.PrivatedExecuteAsync(url, HttpMethod.Post, message, false, cancellationToken);
            var responseAsString = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TResponse>(responseAsString)!;
        }
        private const string StartingWith = "data: ";
        private const string Done = "[DONE]";
        internal static async IAsyncEnumerable<TResponse> PostStreamAsync<TResponse>(this HttpClient client,
            string url,
            object? message,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var response = await client.PrivatedExecuteAsync(url, HttpMethod.Post, message, true, cancellationToken);
            using var stream = await response.Content.ReadAsStreamAsync();
            using var reader = new StreamReader(stream);
            string line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                if (line.StartsWith(StartingWith))
                    line = line.Substring(StartingWith.Length);
                if (line == Done)
                {
                    yield break;
                }
                else if (!string.IsNullOrWhiteSpace(line))
                {
                    var res = JsonSerializer.Deserialize<TResponse>(line);
                    if (res is ApiBaseResponse apiResult)
                        apiResult.SetHeaders(response);
                    yield return res!;
                }
            }
        }
        private static void SetHeaders<TResponse>(this TResponse result, HttpResponseMessage response)
            where TResponse : ApiBaseResponse
        {
            try
            {
                result.Organization = response.Headers.GetValues("Openai-Organization").FirstOrDefault();
                result.RequestId = response.Headers.GetValues("X-Request-ID").FirstOrDefault();
                result.ProcessingTime = TimeSpan.FromMilliseconds(int.Parse(response.Headers.GetValues("Openai-Processing-Ms").First()));
                result.OpenaiVersion = response.Headers.GetValues("Openai-Version").FirstOrDefault();
                result.ModelId = response.Headers.GetValues("Openai-Model").FirstOrDefault();
            }
            catch (Exception e)
            {
                Debug.Print($"Issue parsing metadata of OpenAi Response.  Error: {e.Message}.  This is probably ignorable.");
            }
        }
    }
}
