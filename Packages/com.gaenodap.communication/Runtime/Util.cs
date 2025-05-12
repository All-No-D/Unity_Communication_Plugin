using System;

namespace Gaenodap
{
    class Util
    {
        public static string CombineUrl(string root, string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return root;
            }

            Uri combinedPaths = new(new(root), path);

            return combinedPaths.AbsoluteUri;
        }
    }

    [Serializable]
    public class HttpBody
    {

    }

    [Serializable]
    public class HttpResult<T>
    {
        public string statusCode;
        public string msg;
        public T data;
    }
}