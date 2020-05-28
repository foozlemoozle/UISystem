
using Stack = System.Collections.Generic.Stack<int>;

namespace com.keg.utils
{
    public class HashMap<T>
    {
        private T[] _map;
        private int _mapSize;

        private Stack _freedIds;
        private int _nextId;

        public HashMap( int size = 10 )
        {
            _mapSize = 10;
            _nextId = 0;
            _freedIds = new Stack( _mapSize );

            CreateMap();
        }

        private void ResizeMap()
        {
            _mapSize *= 2;
        }

        private void CreateMap()
        {
            int count = _map != null ? _map.Length : 0;
            T[] newMap = new T[ _mapSize ];

            for( int i = 0; i < count; ++i )
            {
                newMap[ i ] = _map[ i ];
            }

            _map = newMap;
        }

        public int GetNextId()
        {
            if( IsNextIdRecycled() )
            {
                return _freedIds.Peek();
            }
            else
            {
                return _nextId;
            }
        }

        public bool IsNextIdRecycled()
        {
            return _freedIds.Count > 0;
        }

        public int Add( T thing )
        {
            bool isIdRecycled = IsNextIdRecycled();
            int id = isIdRecycled ? _freedIds.Peek() : _nextId;

            if( id >= _map.Length )
            {
                ResizeMap();
            }

            _map[ id ] = thing;

            if( isIdRecycled )
            {
                _freedIds.Pop();
            }
            else
            {
                _nextId++;
            }

            return id;
        }

        public void Remove( int id )
        {
            if( IsIdValid( id ) )
            {
                _freedIds.Push( id );
                _map[ id ] = default( T );
            }
        }

        public T this[ int id ]
        {
            get
            {
                if( !IsIdValid( id ) )
                {
                    return default( T );
                }
                else
                {
                    return _map[ id ];
                }
            }
        }

        private bool IsIdValid( int id )
        {
            return id < _map.Length && id < _nextId;
        }
    }
}
