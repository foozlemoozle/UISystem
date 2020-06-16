/// Created by: Kirk George
/// Copyright: Kirk George
/// Website: https://github.com/foozlemoozle?tab=repositories
/// See upload date for date created.

/**
Created by Kirk George 05/23/2019.!--
Context for the UI.!--
Used to make dependency injections easier.!--
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.keg.uisystem
{
	public interface IUIContext
	{
		IHeapManager parent { get; }
		IHeapManager heapManager { get; }
	}
}
