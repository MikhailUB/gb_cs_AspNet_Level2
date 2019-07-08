using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace WebStore.Clients.Base
{
	public abstract class BaseClient : IDisposable
	{
		protected readonly HttpClient _client;

		protected readonly string _serviceAddress;

		protected BaseClient(IConfiguration configuration, string serviceAddress)
		{
			_serviceAddress = serviceAddress;

			_client = new HttpClient { BaseAddress = new Uri(configuration["ClientAddress"]) };

			_client.DefaultRequestHeaders.Accept.Clear();
			_client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
		}

		protected async Task<T> GetAsync<T>(string url, CancellationToken cancel = default) where T : new()
		{
			var response = await _client.GetAsync(url, cancel);
			if (response.IsSuccessStatusCode)
				return await response.Content.ReadAsAsync<T>(cancel);

			return new T();
		}

		protected T Get<T>(string url) where T : new() => GetAsync<T>(url).Result;

		protected async Task<HttpResponseMessage> PostAsync<T>(string url, T item, CancellationToken cancel = default)
		{
			return (await _client.PostAsJsonAsync(url, item, cancel)).EnsureSuccessStatusCode();
		}

		protected HttpResponseMessage Post<T>(string url, T item) => PostAsync(url, item).Result;

		protected async Task<HttpResponseMessage> PutAsync<T>(string url, T item, CancellationToken cancel = default)
		{
			return (await _client.PutAsJsonAsync(url, item, cancel)).EnsureSuccessStatusCode();
		}

		protected HttpResponseMessage Put<T>(string url, T item) => PutAsync(url, item).Result;

		protected async Task<HttpResponseMessage> DeleteAsync(string url, CancellationToken cancel = default)
		{
			return await _client.DeleteAsync(url, cancel);
		}

		protected HttpResponseMessage Delete(string url) => DeleteAsync(url).Result;

		public void Dispose() => _client.Dispose();
	}
}
