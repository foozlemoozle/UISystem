/// Created by: Kirk George
/// Copyright: Kirk George
/// Website: https://github.com/foozlemoozle?tab=repositories
/// See upload date for date created.

/**
Created by Kirk George 05/23/2019.!--
Dynamic parameter set.!-- 
Used for passing a dynamic set of parameters.!--
 */

using System.Collections;
using System.Collections.Generic;

public class ParamSet 
{
	private struct SetComparer : IEqualityComparer<string>
	{
		public bool Equals( string a, string b )
		{
			return a == b;
		}

		public int GetHashCode( string a )
		{
			return a.GetHashCode();
		}
	}

	private Dictionary<string, object> _set;

	public ParamSet()
	{
		_set = new Dictionary<string, object>( new SetComparer() );
	}

	public ParamSet Add<T>( string key, T value )
	{
		if( _set.ContainsKey( key ) )
		{
			_set[ key ] = value;
		}
		else
		{
			_set.Add( key, value );
		}

		return this;
	}

	public T GetT<T>( string key )
	{
		if( !_set.ContainsKey( key ) )
		{
			return default( T );
		}

		return (T)_set[ key ];
	}
}
