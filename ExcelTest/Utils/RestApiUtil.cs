using ExcelTest.Models;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ExcelTest.Utils
{
    public class RestApiUtil<T> where T : new()
    {
        public T Get(RequestParameter requestParameter)
        {
            var type = Method.GET;
            IRestResponse<T> reval = GetApiInfo(requestParameter, type);
            return reval.Data;
        }

        public async Task<T> GetAsync(RequestParameter requestParameter)
        {
            var type = Method.GET;
            IRestResponse<T> reval = await GetApiInfoAsync(requestParameter, type);
            return reval.Data;
        }

        public string GetContent(RequestParameter requestParameter)
        {
            var type = Method.GET;
            IRestResponse<T> reval = GetApiInfo(requestParameter, type);
            return reval.Content;
        }

        public async Task<string> GetContentAsync(RequestParameter requestParameter)
        {
            var type = Method.GET;
            IRestResponse<T> reval = await GetApiInfoAsync(requestParameter, type);
            return reval.Content;
        }

        /// <summary>
        /// Http Post请求
        /// </summary>
        /// <param name="requestParameter">请求配置实体</param>
        /// <returns>实体序列化后的Response信息</returns>
        public T Post(RequestParameter requestParameter)
        {
            var type = Method.POST;
            IRestResponse<T> reval = GetApiInfo(requestParameter, type);
            return reval.Data;
        }

        /// <summary>
        /// 异步Post请求
        /// </summary>
        /// <param name="requestParameter">请求配置实体</param>
        /// <returns>实体序列化后的Response信息</returns>
        public async Task<T> PostAsync(RequestParameter requestParameter)
        {
            var type = Method.POST;
            IRestResponse<T> reval = await GetApiInfoAsync(requestParameter, type);
            return reval.Data;
        }

        /// <summary>
        /// Post请求
        /// </summary>
        /// <param name="requestParameter">请求配置实体</param>
        /// <returns>Response文本信息</returns>
        public string PostContent(RequestParameter requestParameter)
        {
            var type = Method.POST;
            IRestResponse<T> reval = GetApiInfo(requestParameter, type);
            return reval.Content;
        }

        /// <summary>
        /// 异步Http Post请求
        /// </summary>
        /// <param name="requestParameter">请求配置实体</param>
        /// <returns>Response文本信息</returns>
        public async Task<string> PostContentAsync(RequestParameter requestParameter)
        {
            var type = Method.POST;
            IRestResponse<T> reval = await GetApiInfoAsync(requestParameter, type);
            return reval.Content;
        }

        public T Delete(RequestParameter requestParameter)
        {
            var type = Method.DELETE;
            IRestResponse<T> reval = GetApiInfo(requestParameter, type);
            return reval.Data;
        }

        public async Task<T> DeleteAsync(RequestParameter requestParameter)
        {
            var type = Method.DELETE;
            IRestResponse<T> reval = await GetApiInfoAsync(requestParameter, type);
            return reval.Data;
        }

        public T Put(RequestParameter requestParameter)
        {
            var type = Method.PUT;
            IRestResponse<T> reval = GetApiInfo(requestParameter, type);
            return reval.Data;
        }

        public async Task<T> PutAsync(RequestParameter requestParameter)
        {
            var type = Method.PUT;
            IRestResponse<T> reval = await GetApiInfoAsync(requestParameter, type);
            return reval.Data;
        }

        /// <summary>
        /// 同步Http请求
        /// </summary>
        /// <param name="requestParameter"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private static IRestResponse<T> GetApiInfo(RequestParameter requestParameter, Method type)
        {
            IRestResponse<T> reval;
            var request = SetRestApiRequestParameter(requestParameter, type);

            var stopWatch = new Stopwatch();
            stopWatch.Start();
            try
            {
                RestClient client = new RestClient(requestParameter.Url);

                if (requestParameter.Authenticator != null)
                    client.Authenticator = requestParameter.Authenticator;

                reval = client.Execute<T>(request);
                stopWatch.Stop();

                if (reval.ErrorException != null && reval.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    RestApiInfoLog(requestParameter.Url, request, reval, stopWatch.ElapsedMilliseconds);
                    throw new Exception($"接口请求出错，错误信息：{reval.ErrorException.Message}");
                }

                RestApiInfoLog(requestParameter.Url, request, reval, stopWatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                if (stopWatch.IsRunning)
                    stopWatch.Stop();
                RestApiInfoLog(requestParameter.Url, request, null, stopWatch.ElapsedMilliseconds, ex);
                throw new Exception($"HTTP请求链路故障，错误信息：{ex.Message}");
            }

            return reval;
        }

        /// <summary>
        /// 异步Http请求
        /// </summary>
        /// <param name="requestParameter">请求配置参数实体</param>
        /// <param name="type">请求类型</param>
        /// <returns>异步Http请求结果</returns>
        /// <exception cref="Exception">Http请求时产生的异常</exception>
        private static async Task<IRestResponse<T>> GetApiInfoAsync(RequestParameter requestParameter, Method type)
        {
            IRestResponse<T> reval;
            var request = SetRestApiRequestParameter(requestParameter, type);

            var stopWatch = new Stopwatch();
            stopWatch.Start();
            try
            {
                RestClient client = new RestClient(requestParameter.Url);

                reval = await client.ExecuteAsync<T>(request);
                stopWatch.Stop();

                if (reval.ErrorException != null && reval.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    RestApiInfoLog(requestParameter.Url, request, reval, stopWatch.ElapsedMilliseconds);
                    throw new Exception($"接口请求出错，错误信息：{reval.ErrorException.Message}");
                }

                RestApiInfoLog(requestParameter.Url, request, reval, stopWatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                if (stopWatch.IsRunning)
                    stopWatch.Stop();

                RestApiInfoLog(requestParameter.Url, request, null, stopWatch.ElapsedMilliseconds, ex);
                throw new Exception($"HTTP请求链路故障，错误信息：{ex.Message}");
            }

            return reval;
        }

        private static RestRequest SetRestApiRequestParameter(RequestParameter requestParameter, Method type)
        {
            var request = new RestRequest(type);

            // 设置请求体
            if (requestParameter.RequestBodyData != null)
                request.AddParameter(requestParameter.ContentType, requestParameter.RequestBodyData, ParameterType.RequestBody);

            // 设置拼接参数
            if (requestParameter != null && requestParameter.Parameters.Count > 0)
            {
                foreach (KeyValuePair<string, object> param in requestParameter.Parameters)
                    request.AddParameter(param.Key, param.Value, ParameterType.QueryString);
            }

            if (requestParameter.Headers != null)
            {
                request.AddHeaders((Dictionary<string, string>)requestParameter.Headers);
            }

            request.Timeout = requestParameter.Timeout;

            return request;
        }

        private static void RestApiInfoLog(string url, IRestRequest request, IRestResponse response, long durationMs, Exception exception = null)
        {
            var requestToLog = new
            {
                resource = request?.Resource,
                parameters = request?.Parameters.Select(parameter => new
                {
                    name = parameter?.Name,
                    value = parameter?.Value,
                    type = parameter?.Type.ToString()
                }),
                method = request?.Method.ToString(),
                uri = url,
            };

            var responseToLog = new object();
            if (exception != null)
            {
                responseToLog = new
                {
                    statusCode = "SystemError",
                    content = "HTTP请求链路故障",
                    errorMessage = exception.Message,
                    errorInner = exception.InnerException,
                    errorStackTrace = exception.StackTrace
                };
            }
            else
            {
                responseToLog = new
                {
                    statusCode = response?.StatusCode,
                    content = response?.Content,
                    headers = response?.Headers,
                    responseUri = response?.ResponseUri,
                    errorMessage = response?.ErrorMessage,
                };
            }

            // 序列化HTTP请求日志
            string logStr = string.Format("Request completed in {0} ms,\n Request: {1},\n Response: {2}",
                durationMs, JsonConvert.SerializeObject(requestToLog), JsonConvert.SerializeObject(responseToLog));

            SysLogUtil logHelper = new SysLogUtil(SysLogUtil.LogTagType.LogToFile);
            if (exception != null)
                logHelper.Error(logStr, url, "WebAPIRequest");
            else
                logHelper.Info(logStr, url, "WebAPIRequest");
        }
    }
}