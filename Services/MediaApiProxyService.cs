﻿using Newtonsoft.Json;
using poplensMediaApi.Models;
using System.Net.Http.Headers;

namespace poplensUserProfileApi.Services {
    public interface IMediaApiProxyService {
        Task<Media> GetMediaByIdAsync(Guid id, string authorizationToken);
        Task<bool> IncrementTotalReviewCountAsync(Guid id, string authorizationToken);
    }
    public class MediaApiProxyService : IMediaApiProxyService {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _mediaApiUrl = "http://poplensMediaApi:8080/api/";

        public MediaApiProxyService(IHttpClientFactory httpClientFactory) {
            _httpClientFactory = httpClientFactory;
        }

        private HttpClient CreateHttpClientWithAuthorization(string token) {
            var client = _httpClientFactory.CreateClient();
            if (!string.IsNullOrEmpty(token)) {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            return client;
        }

        public async Task<Media> GetMediaByIdAsync(Guid id, string authorizationToken) {
            var client = CreateHttpClientWithAuthorization(authorizationToken);
            var response = await client.GetAsync($"{_mediaApiUrl}Media/{id}");

            if (!response.IsSuccessStatusCode) {
                return null;
            }

            var mediaJson = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Media>(mediaJson);
        }

        public async Task<bool> IncrementTotalReviewCountAsync(Guid id, string authorizationToken) {
            var client = CreateHttpClientWithAuthorization(authorizationToken);
            var response = await client.PostAsync($"{_mediaApiUrl}Media/IncrementTotalReviewCount/{id}", null);

            return response.IsSuccessStatusCode;
        }
    }
}
