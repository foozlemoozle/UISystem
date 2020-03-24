/**
Created by Kirk George 05/23/2019.!--
Defines how adding a ui should impact the rendering of dialogs below it,
and how it should impact UI camera clear settings.!--
Values are set as bit masks.!--
 */

public enum CullSettings
{
	NoCullNoClear = 0,//lack of tags means do nothing
	CullBelow = 1,
	ClearCamera = 2,
    CullBelowClearCamera = 3,//combo of cull below and clear camera
}
