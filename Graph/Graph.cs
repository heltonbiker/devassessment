using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace Graph
{
    public interface IGraph<T>
    {
        IObservable<IEnumerable<T>> RoutesBetween(T source, T target);
    }

    public class Graph<T> : IGraph<T>
    {
        readonly Dictionary<T, List<T>> _dict;

        public Graph(IEnumerable<ILink<T>> links)
        {
            _dict = new Dictionary<T, List<T>>();
            foreach (var link in links)
            {
                var source = link.Source;
                if (!_dict.ContainsKey(source))
                {
                    _dict[source] = new List<T>();    ////////// FAZER ISSO TUDO USANDO GROUPBY (VER COMO FICA)
                    
                }
                _dict[source].Add(link.Target);
            }
        }

        public IObservable<IEnumerable<T>> RoutesBetween(T source, T target)
        {
            var result = new List<List<T>>();
            var queue = new Queue<List<T>>();
            queue.Enqueue(new List<T> { source });
            while (queue.Any())
            {
                var currentList = queue.Dequeue();
                var currentLastNode = currentList.Last();
                if (_dict.TryGetValue(currentLastNode, out var targets))
                {
                    foreach (var newTarget in targets)
                    {
                        if (!currentList.Contains(newTarget))
                        {
                            var nextList = new List<T>(currentList) { newTarget };

                            if (EqualityComparer<T>.Default.Equals(newTarget, target))
                                result.Add(nextList);
                            else
                                queue.Enqueue(nextList);
                        }
                    }
                }
            }
            return result.ToObservable();
        }
    }
}
