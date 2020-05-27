/**
Created by Kirk George 05/23/2019.!--
Interface implemented for all UI Heap managers.!--
Should include: UIManager, UILayerManager, BaseUIView.!--
 */

namespace com.keg.uisystem
{
	public interface IHeapManager
	{
		int MIN_SORT { get; }
		int MAX_SORT { get; }
		UISortingLayer.Layers LAYER { get; }

		UIHandler<UI> Attach<UI>( Loader<UI> loader, int requiredSortOrders, ParamSet setupParams = null, CullSettings cullSettings = CullSettings.NoCullNoClear ) where UI : UIView;
		bool Remove( UIID id );
	}
}
