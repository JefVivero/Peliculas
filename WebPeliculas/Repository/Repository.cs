using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebPeliculas.Repositorio.IRepositorio;

namespace WebPeliculas.Repositorio
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public Repository(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<bool> CreateAsync(string url, T item)
        {
            try
            {
                var peticion = new HttpRequestMessage(HttpMethod.Post, url);

                if (item != null)
                {
                    peticion.Content = new StringContent(
                        JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json"
                        );

                    var cliente = _httpClientFactory.CreateClient();

                    HttpResponseMessage resp = await cliente.SendAsync(peticion);

                    if (resp.StatusCode == HttpStatusCode.Created)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error CreateAsync - {e.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteAsync(string url, int id)
        {
            try
            {
                var peticion = new HttpRequestMessage(HttpMethod.Delete, url + id);

                var cliente = _httpClientFactory.CreateClient();

                HttpResponseMessage resp = await cliente.SendAsync(peticion);

                if (resp.StatusCode == HttpStatusCode.NoContent)
                {
                    return true;
                }
                
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error DeleteAsync - {e.Message}");
                return false;
            }
        }

        public async Task<IEnumerable> GetAllAsync(string url)
        {
            try
            {
                var peticion = new HttpRequestMessage(HttpMethod.Get, url);

                var cliente = _httpClientFactory.CreateClient();

                HttpResponseMessage resp = await cliente.SendAsync(peticion);

                if (resp.StatusCode == HttpStatusCode.OK)
                {
                    var jsonString = await resp.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<IEnumerable>(jsonString);
                }

                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error GetAsync - {e.Message}");
                return null;
            }
        }

        public async Task<T> GetAsync(string url, int id)
        {
            try
            {
                var peticion = new HttpRequestMessage(HttpMethod.Get, url + id);

                var cliente = _httpClientFactory.CreateClient();

                HttpResponseMessage resp = await cliente.SendAsync(peticion);

                if (resp.StatusCode == HttpStatusCode.OK)
                {
                    var jsonString = await resp.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<T>(jsonString);
                }

                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error GetAsync - {e.Message}");
                return null;
            }
        }

        public async Task<bool> UpdateAsync(string url, T item)
        {
            try
            {
                var peticion = new HttpRequestMessage(HttpMethod.Patch, url);

                if (item != null)
                {
                    peticion.Content = new StringContent(
                        JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json"
                        );

                    var cliente = _httpClientFactory.CreateClient();

                    HttpResponseMessage resp = await cliente.SendAsync(peticion);

                    if (resp.StatusCode == HttpStatusCode.NoContent)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error UpdateAsync - {e.Message}");
                return false;
            }                                         
        }
    }
}
