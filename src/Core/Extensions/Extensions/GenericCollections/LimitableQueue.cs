using System.Collections.Generic;

namespace Devmonster.Core.Extensions.GenericCollections;

public class LimitableQueue<T>
{

    private Queue<T> _queue = new Queue<T>();
    private int _maxItems;

    public LimitableQueue(int max)
    {
        _maxItems = max;
    }

    public LimitableQueue(int max, List<T> source)
    {
        _maxItems = max;
        foreach (var item in source) _queue.Enqueue(item);
        TrimExcess();
    }

    public void Enqueue(T item)
    {
        TrimExcess();
        _queue.Enqueue(item);
    }

    public void Resize(int newMax)
    {
        _maxItems = newMax;
        TrimExcess();
    }

    private void TrimExcess()
    {
        while (_queue.Count > _maxItems) _queue.Dequeue();
    }

    public T[] ToArray()
    {
        return _queue.ToArray();
    }
}
