﻿using System;
using System.Collections;
using System.Collections.Generic;
 

namespace SoundMixerSoftware.Common.Collection
{
    /// <summary>
    /// List<T> thread-safe wrapper.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ConcurrentList<T> : IList<T>
    {
        #region Private Fields

        private readonly List<T> _list;
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private object _syncRoot;
        
        #endregion
        
        #region Public Properties

        /// <summary>
        /// Collection synchronization object.
        /// </summary>
        // ReSharper disable once ConvertToAutoPropertyWhenPossible
        public object SyncRoot => _syncRoot;
        
        #endregion
        
        #region Implemented Properties

        public int Count
        {
            get
            {
                lock (_syncRoot)
                    return _list.Count;
            }
        }
        
        public bool IsReadOnly => false;
        
        #endregion 
        
        #region Constructor

        public ConcurrentList() : this(3)
        {
            
        }
        
        public ConcurrentList(int capacity)
        {
            _list = new List<T>(capacity);
            _syncRoot = ((ICollection) _list).SyncRoot;
        }
        
        #endregion
        
        #region Implemented Methods
        
        public IEnumerator<T> GetEnumerator()
        {
            lock (_syncRoot)
                return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            lock (_syncRoot)
                return _list.GetEnumerator();
        }

        public void Add(T item)
        {
            lock(_syncRoot)
                _list.Add(item);
        }

        public void Clear()
        {
            lock(_syncRoot)
                _list.Clear();
        }

        public bool Contains(T item)
        {
            lock(_syncRoot)
                return _list.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            lock(_syncRoot)
                _list.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            lock(_syncRoot)
                return _list.Remove(item);
        }
        
        public int IndexOf(T item)
        {
            lock(_syncRoot)
                return _list.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            lock(_syncRoot)
                _list.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            lock(_syncRoot)
                _list.RemoveAt(index);
        }

        public T this[int index]
        {
            get
            {
                lock (_syncRoot)
                    return _list[index];
            }
            set
            {
                lock (_syncRoot)
                    _list[index] = value;
            }
        }
        
        #endregion
        
        #region Public Methods

        public void ForEach(Action<T> action)
        {
            if(action == null)
                throw new ArgumentNullException(nameof(action));
            lock (_syncRoot)
            {
                // ReSharper disable once ForCanBeConvertedToForeach
                for (var n = 0; n < _list.Count; n++)
                    action(_list[n]);
            }
        }

        #endregion
        
    }
}