using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Quintessence.RpcClient.Exceptions;
using Lykke.Quintessence.RpcClient.Models;
using Lykke.Quintessence.RpcClient.Strategies;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Lykke.BilService.RootstockApi.Services
{
    public class RootstockSendRpcRequestStrategy : ISendRpcRequestStrategy
    {
        private readonly Uri _apiUrl;
        private readonly TimeSpan _connectionTimeout;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILog _log;

        public RootstockSendRpcRequestStrategy(
            Uri apiUrl,
            TimeSpan connectionTimeout,
            IHttpClientFactory httpClientFactory,
            ILogFactory logFactory)
        {
            _apiUrl = apiUrl;
            _connectionTimeout = connectionTimeout;
            _httpClientFactory = httpClientFactory;
            _log = logFactory.CreateLog(this);
        }

        public virtual async Task<string> ExecuteAsync(
            string requestJson)
        {
            string responseJson = null;

            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var cts = new CancellationTokenSource();

                cts.CancelAfter(_connectionTimeout);

                var httpRequest = new StringContent(requestJson, Encoding.UTF8, "application/json");
                var httpResponse = await httpClient.PostAsync(_apiUrl, httpRequest, cts.Token);
                responseJson = await httpResponse.Content.ReadAsStringAsync();

                if (httpResponse.StatusCode == HttpStatusCode.InternalServerError)
                {
                    var response = JsonConvert.DeserializeObject<RpcResponse>(responseJson);

                    if (response.Error != null &&
                        response.Error.Code == -32010 &&
                        response.Error.Message == "pending transaction with same hash already exists")
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
            catch (RpcErrorException rpcExc)
            {
                _log.Error(rpcExc, responseJson);

                throw;
            }
            catch (Exception e)
            {
                _log.Error(e, responseJson);

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
