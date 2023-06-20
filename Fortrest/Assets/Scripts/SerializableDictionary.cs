using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SerializableDictionary<TKey, TValue>
{
    [SerializeField]
    private List<TKey> keys = new List<TKey>();

    [SerializeField]
    private List<TValue> values = new List<TValue>();

    public int Count => keys.Count;

    public TValue this[TKey key]
    {
        get
        {
            int index = keys.IndexOf(key);
            return (index != -1) ? values[index] : default(TValue);
        }
        set
        {
            int index = keys.IndexOf(key);
            if (index != -1)
            {
                values[index] = value;
            }
            else
            {
                keys.Add(key);
                values.Add(value);
            }
        }
    }

    public List<TKey> Keys => keys;

    public List<TValue> Values => values;
}