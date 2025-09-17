/*
Poor Man's T-SQL Formatter - a small free Transact-SQL formatting
library for .Net 2.0 and JS, written in C#.
Copyright (C) 2011-2025 Tao Klerks

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace TSqlFormatter.ParseStructure
{
    /// <summary>
    /// Thread-safe implementation of Node for parse tree operations.
    /// Uses ReaderWriterLockSlim for optimal read performance.
    /// </summary>
    internal class NodeImplThreadSafe : Node, IDisposable
    {
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        private readonly Dictionary<string, string> _attributes;
        private readonly List<Node> _children;
        private Node _parent;

        public NodeImplThreadSafe()
        {
            _attributes = new Dictionary<string, string>();
            _children = new List<Node>();
        }

        public string Name { get; set; }
        public string TextValue { get; set; }

        public Node Parent
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    return _parent;
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
            internal set
            {
                _lock.EnterWriteLock();
                try
                {
                    _parent = value;
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }
        }

        public IDictionary<string, string> Attributes
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    // Return a copy to prevent external modifications
                    return new Dictionary<string, string>(_attributes);
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
        }

        public IEnumerable<Node> Children
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    // Return a copy to prevent external modifications
                    return _children.ToList();
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
        }

        public void AddChild(Node child)
        {
            if (child == null)
                throw new ArgumentNullException(nameof(child));

            _lock.EnterWriteLock();
            try
            {
                SetParentOnChildInternal(child);
                _children.Add(child);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public void InsertChildBefore(Node newChild, Node existingChild)
        {
            if (newChild == null)
                throw new ArgumentNullException(nameof(newChild));
            if (existingChild == null)
                throw new ArgumentNullException(nameof(existingChild));

            _lock.EnterWriteLock();
            try
            {
                SetParentOnChildInternal(newChild);
                int index = _children.IndexOf(existingChild);
                if (index < 0)
                    throw new ArgumentException("Existing child not found in children list");
                _children.Insert(index, newChild);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        private void SetParentOnChildInternal(Node child)
        {
            // This method assumes the write lock is already held
            if (child.Parent != null)
                throw new ArgumentException("Child cannot already have a parent!");

            if (child is NodeImplThreadSafe threadSafeChild)
            {
                threadSafeChild._parent = this;
            }
            else if (child is NodeImpl legacyChild)
            {
                legacyChild.Parent = this;
            }
        }

        public void RemoveChild(Node child)
        {
            if (child == null)
                throw new ArgumentNullException(nameof(child));

            _lock.EnterWriteLock();
            try
            {
                if (_children.Remove(child))
                {
                    if (child is NodeImplThreadSafe threadSafeChild)
                    {
                        threadSafeChild._parent = null;
                    }
                    else if (child is NodeImpl legacyChild)
                    {
                        legacyChild.Parent = null;
                    }
                }
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public string GetAttributeValue(string aName)
        {
            if (string.IsNullOrEmpty(aName))
                return null;

            _lock.EnterReadLock();
            try
            {
                string outVal;
                _attributes.TryGetValue(aName, out outVal);
                return outVal;
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        public void SetAttribute(string name, string value)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Attribute name cannot be null or empty", nameof(name));

            _lock.EnterWriteLock();
            try
            {
                _attributes[name] = value;
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public void RemoveAttribute(string name)
        {
            if (string.IsNullOrEmpty(name))
                return;

            _lock.EnterWriteLock();
            try
            {
                _attributes.Remove(name);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        // Implement IDisposable to properly clean up the lock
        private bool _disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _lock?.Dispose();
                }
                _disposed = true;
            }
        }

        ~NodeImplThreadSafe()
        {
            Dispose(false);
        }
    }
}
