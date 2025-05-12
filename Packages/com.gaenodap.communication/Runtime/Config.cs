using System;
using UnityEngine;

namespace Gaenodap
{
    namespace Config
    {
        [Serializable]
        public class ApiConfig
        {
            public string baseUri;
        }

        public static class ConfigLoader
        {
            private static ApiConfig _config;

            public static string GetBaseUri()
            {
                if (_config == null)
                {
                    TextAsset json = Resources.Load<TextAsset>("Config/api_config");
                    _config = JsonUtility.FromJson<ApiConfig>(json.text);
                }

                return _config.baseUri;
            }
        }
    }
}