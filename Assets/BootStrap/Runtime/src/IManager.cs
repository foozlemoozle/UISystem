/// Created by: Kirk George
/// Copyright: Kirk George
/// Website: https://github.com/foozlemoozle?tab=repositories
/// See upload date for date created.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Exception = System.Exception;
using System.Threading.Tasks;

namespace com.keg.bootstrap
{
	public interface IManager
	{
		Task Setup( BootStrap bootstrap, System.Action<IManager> onSetup, System.Action<IManager> onFail );
		void Update();
		Task Teardown( System.Action onTeardown );
	}
}
