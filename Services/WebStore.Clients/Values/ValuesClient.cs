using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using WebStore.Clients.Base;
using WebStore.Interfaces.Api;

namespace WebStore.Clients.Values
{
	public class ValuesClient : BaseClient, IValuesService
	{
		public ValuesClient(IConfiguration Configuration) : base(Configuration, "api/values") { }

		public IEnumerable<string> Get()
		{
			return GetAsync().Result;
		}

		public async Task<IEnumerable<string>> GetAsync()
		{
			var response = await _client.GetAsync(_serviceAddress);
			if (response.IsSuccessStatusCode)
				return await response.Content.ReadAsAsync<List<string>>();

			return new string[0];
		}

		public string Get(int id)
		{
			return GetAsync(id).Result;
		}

		public async Task<string> GetAsync(int id)
		{
			var response = await _client.GetAsync($"{_serviceAddress}/{id}");
			if (response.IsSuccessStatusCode)
				return await response.Content.ReadAsAsync<string>();

			return "";
		}

		public Uri Post(string value)
		{
			return PostAsync(value).Result;
		}

		public async Task<Uri> PostAsync(string value)
		{
			var response = await _client.PostAsJsonAsync($"{_serviceAddress}/post", value);
			response.EnsureSuccessStatusCode();

			return response.Headers.Location;
		}

		public HttpStatusCode Put(int id, string value)
		{
			return PutAsync(id, value).Result;
		}

		public async Task<HttpStatusCode> PutAsync(int id, string value)
		{
			var response = await _client.PutAsJsonAsync($"{_serviceAddress}/put/{id}", value);
			response.EnsureSuccessStatusCode();

			return response.StatusCode;
		}

		public HttpStatusCode Delete(int id)
		{
			return DeleteAsync(id).Result;
		}

		public async Task<HttpStatusCode> DeleteAsync(int id)
		{
			var response = await _client.DeleteAsync($"{_serviceAddress}/delete/{id}");

			return response.StatusCode;
		}
	}
}
