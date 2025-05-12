using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Gaenodap
{
    namespace Http
    {
        public enum Method
        {
            GET, POST, PUT, DELETE
        }

        public static class Client
        {
            private static Dictionary<string, string> baseHeader;
            private static Dictionary<Method, string> methodPairs;

            static Client()
            {
                baseHeader = new Dictionary<string, string>()
                    {
                        { "Content-Type", "application/json" }
                    };
                methodPairs = new Dictionary<Method, string>()
                    {
                        { Method.GET, "GET" },
                        { Method.POST, "POST" },
                        { Method.PUT, "PUT" },
                        { Method.DELETE, "DELETE" }
                    };
            }

            public static void Request<T>(
                MonoBehaviour behaviour,
                string path,
                Method method,
                Dictionary<string, string> header = null,
                HttpBody body = null,
                int timeoutSeconds = 10,
                int maxRetryCount = 0,
                Action<HttpResult<T>> onSuccess = null,
                Action<string> onFailure = null
            )
            {
                behaviour.StartCoroutine(RequestCoroutine(path, method, header, body, timeoutSeconds, maxRetryCount, onSuccess, onFailure));
            }

            public static IEnumerator RequestCoroutine<T>(
                string path,
                Method method,
                Dictionary<string, string> header = null,
                HttpBody body = null,
                int timeoutSeconds = 10,
                int maxRetryCount = 0,
                Action<HttpResult<T>> onSuccess = null,
                Action<string> onFailure = null
            )
            {
                if (header != null)
                {
                    foreach (string key in baseHeader.Keys)
                    {
                        if (!header.ContainsKey(key))
                        {
                            header[key] = baseHeader[key];
                        }
                    }
                }
                else
                {
                    header = baseHeader;
                }

                int retryAttempt = 0;
                while (true)
                {
                    string url = Util.CombineUrl(Config.ConfigLoader.GetBaseUri(), path);
                    using (UnityWebRequest webRequest = BuildRequest(url, method, header, body))
                    {
                        webRequest.timeout = timeoutSeconds;

                        yield return webRequest.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
                        if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
#else
                        if (webRequest.isNetworkError || webRequest.isHttpError)
#endif
                        {
                            retryAttempt++;
                            if (retryAttempt > maxRetryCount)
                            {
                                Debug.LogError($"HTTP {method} Failed after {retryAttempt} attempts. Last error: {webRequest.error}");

                                onFailure?.Invoke(webRequest.error);

                                yield break;
                            }

                            yield return new WaitForSeconds(0.5f);
                        }
                        else
                        {
                            onSuccess?.Invoke(JsonUtility.FromJson<HttpResult<T>>(webRequest.downloadHandler.text));

                            yield break;
                        }
                    }
                }
            }

            private static UnityWebRequest BuildRequest(
                string url,
                Method method,
                Dictionary<string, string> header,
                HttpBody body
            )
            {
                UnityWebRequest webRequest = new UnityWebRequest(method == Method.GET ? AppendQueryParameters(url, body) : url, methodPairs[method]);
                if (method != Method.GET && body != null)
                {
                    webRequest.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(SerializeToJson(body)));
                }

                webRequest.downloadHandler ??= new DownloadHandlerBuffer();
                if (header != null)
                {
                    foreach (var item in header)
                    {
                        webRequest.SetRequestHeader(item.Key, item.Value);
                    }
                }

                return webRequest;
            }

            private static string AppendQueryParameters(
                string url,
                HttpBody parameters
            )
            {
                var sb = new StringBuilder(url);
                if (parameters != null)
                {
                    if (!url.Contains("?"))
                    {
                        sb.Append("?");
                    }

                    foreach (PropertyInfo property in parameters.GetType().GetProperties())
                    {
                        if (sb[sb.Length - 1] != '?' && sb[sb.Length - 1] != '&')
                        {
                            sb.Append("&");
                        }

                        sb.Append(Uri.EscapeDataString(property.Name));
                        sb.Append("=");
                        sb.Append(Uri.EscapeDataString(property.GetValue(parameters, null)?.ToString() ?? ""));
                    }
                }

                return sb.ToString();
            }

            private static string SerializeToJson(HttpBody body)
            {
                return JsonUtility.ToJson(body);
            }
        }
    }
}
