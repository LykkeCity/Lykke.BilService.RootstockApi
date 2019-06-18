using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Lykke.Quintessence.RpcClient.Exceptions;
using Lykke.Quintessence.RpcClient.Models;
using Lykke.Quintessence.RpcClient.Strategies;
using Newtonsoft.Json;

namespace Lykke.BilService.RootstockApi.Services
{
    public class CurrentSendRpcRequestStrategy : ISendRpcRequestStrategy
    {
        private readonly Uri _apiUrl;
        private readonly TimeSpan _connectionTimeout;
        private readonly IHttpClientFactory _httpClientFactory;

        public CurrentSendRpcRequestStrategy(
            Uri apiUrl,
            TimeSpan connectionTimeout,
            IHttpClientFactory httpClientFactory)
        {
            _apiUrl = apiUrl;
            _connectionTimeout = connectionTimeout;
            _httpClientFactory = httpClientFactory;
        }

        public virtual async Task<string> ExecuteAsync(
            string requestJson)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var cts = new CancellationTokenSource();

                cts.CancelAfter(_connectionTimeout);

                var httpRequest = new StringContent(requestJson, Encoding.UTF8, "application/json");
                var httpResponse = await httpClient.PostAsync(_apiUrl, httpRequest, cts.Token);
                var responseJson = await httpResponse.Content.ReadAsStringAsync();

                if (httpResponse.StatusCode == HttpStatusCode.InternalServerError)
                {
                    var response = JsonConvert.DeserializeObject<RpcResponse>(responseJson);

                    if (response.Error != null &&
                        response.Error.Code == -32010)
                    {
                        var error = response.Error;

                        throw new RpcErrorException(requestJson, error.Code, error.Message);
                    }
                }

                httpResponse.EnsureSuccessStatusCode();

                return responseJson;
            }
            catch (TaskCanceledException)
            {
                throw new RpcTimeoutException(_connectionTimeout, requestJson);
            }
            catch (RpcErrorException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new RpcException
                (
                    "Error occurred while trying to send rpc request.",
                    requestJson,
                    e
                );
            }
        }
    }
}
