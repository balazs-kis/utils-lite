using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UtilsLite.Collections;

namespace UtilsLite.Mapping
{
    public class GenericMapper<TSource, TTarget>
        where TSource : class, new()
        where TTarget : class, new()
    {
        private readonly Dictionary<string, PropertyInfo> _sourceProperties;
        private readonly Dictionary<string, PropertyInfo> _targetProperties;

        public GenericMapper()
        {
            _sourceProperties = typeof(TSource).GetProperties().ToDictionary(p => p.Name, p => p);
            _targetProperties = typeof(TTarget).GetProperties().ToDictionary(p => p.Name, p => p);

            var removeFromSource = new List<string>();

            _sourceProperties.ForEach(p =>
            {
                if (!_targetProperties.ContainsKey(p.Key))
                {
                    removeFromSource.Add(p.Key);
                }
            });

            removeFromSource.ForEach(k => _sourceProperties.Remove(k));
        }

        public TTarget MapByPropertyName(TSource source, Action<TSource, TTarget> customMap = null)
        {
            var result = new TTarget();

            _sourceProperties.ForEach(p =>
            {
                if (_targetProperties[p.Key].CanWrite)
                    _targetProperties[p.Key].SetValue(result, p.Value.GetValue(source));
            });

            customMap?.Invoke(source, result);

            return result;
        }
    }
}
