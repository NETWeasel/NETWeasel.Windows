using System;
using System.Collections.Generic;
using System.IO;

namespace NETWeasel.Windows
{
    internal class WxsComposer
    {
        private readonly string _pathToWxs;

        private readonly Dictionary<string, string> _replaceMap = new Dictionary<string, string>();

        internal WxsComposer(string pathToWxs)
        {
            _pathToWxs = pathToWxs ?? throw new ArgumentNullException(nameof(pathToWxs));
        }

        internal WxsComposer Replace(string key, string replaceWith)
        {
            if(_replaceMap.ContainsKey(key))
                throw new InvalidOperationException($"Cannot add key to replace that has already been defined to be replaced, duplicate key: {key}");

            _replaceMap[key] = replaceWith;

            return this;
        }

        internal string Compose()
        {
            if (!File.Exists(_pathToWxs))
            {
                throw new InvalidOperationException("Cannot compose WXS with invalid path, the path either does not exist or hasn't been found");
            }

            var wxs = File.ReadAllText(_pathToWxs);

            foreach (var map in _replaceMap)
            {
                wxs = wxs.Replace(map.Key, map.Value);
            }

            return wxs;
        }
    }
}
