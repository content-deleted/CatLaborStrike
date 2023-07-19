using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class GenericDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ISerializationCallbackReceiver {
    [SerializeField] List<KeyValuePair> _dictAsList = new List<KeyValuePair>();
    [SerializeField, HideInInspector] Dictionary<TKey, TValue> _dict = new Dictionary<TKey, TValue>();

    [Serializable]
    public struct KeyValuePair {
        public TKey Key;
        public TValue Value;

        public KeyValuePair(TKey Key, TValue Value) {
            this.Key = Key;
            this.Value = Value;
        }
    }

    public void OnBeforeSerialize() {
        _dictAsList = new List<KeyValuePair>();
        foreach (var kvPair in _dict) {
            _dictAsList.Add(new KeyValuePair(kvPair.Key, kvPair.Value));
        }
        _dictAsList.Add(new KeyValuePair());
    }

    public void OnAfterDeserialize() {
        _dict.Clear();

        for (int i = 0; i < _dictAsList.Count; i++) {
            var key = _dictAsList[i].Key;
            if(!_dict.ContainsKey(key) && key != null) {
                _dict.Add(key, _dictAsList[i].Value);
            }
        }
    }
    public TValue this[TKey key] {
        get => _dict[key];
        set => _dict[key] = value;
    }

    public ICollection<TKey> Keys => _dict.Keys;
    public ICollection<TValue> Values => _dict.Values;
    public int Count => _dict.Count;

    public void Add(TKey key, TValue value) {
        _dict.Add(key, value);
        _dictAsList.Add(new KeyValuePair(key, value));
    }

    public void Add(KeyValuePair<TKey, TValue> pair) {
        Add(pair.Key, pair.Value);
    }

    public bool ContainsKey(TKey key) => _dict.ContainsKey(key);

    public bool Remove(TKey key) {
        return _dict.Remove(key);
    } 
    public bool Remove(KeyValuePair<TKey, TValue> pair) {
        return Remove(pair.Key);
    }

    public bool Contains(KeyValuePair<TKey, TValue> pair) {
        TValue value;
        if (_dict.TryGetValue(pair.Key, out value)) {
            return EqualityComparer<TValue>.Default.Equals(value, pair.Value);
        } else {
            return false;
        }
    }

    public void Clear() {
        _dict.Clear();
        _dictAsList.Clear();
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
        if (array == null)
            throw new ArgumentException("The array cannot be null.");
        if (arrayIndex < 0)
           throw new ArgumentOutOfRangeException("The starting array index cannot be negative.");
        if (array.Length - arrayIndex < _dict.Count)
            throw new ArgumentException("The destination array has fewer elements than the collection.");

        foreach (var pair in _dict) {
            array[arrayIndex] = pair;
            arrayIndex++;
        }
    }

    public static GenericDictionary<TKey, TValue> FromDict (IDictionary<TKey, TValue> dict) {
        var newDict = new GenericDictionary<TKey, TValue>();
        foreach (var pair in dict) {
            newDict.Add(pair.Key, pair.Value);
        }
        return newDict;
    }

    public bool IsReadOnly { get; set; }
    public bool TryGetValue(TKey key, out TValue value) => _dict.TryGetValue(key, out value);
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _dict.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => _dict.GetEnumerator();
}