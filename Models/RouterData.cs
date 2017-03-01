using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vyatta_config_updater.VyattaConfig;
using vyatta_config_updater.VyattaConfig.Routing;

namespace vyatta_config_updater
{
	public class RouterData
	{
		public string Address;
		public string Username;
		public string Password;

		public ObservableCollection<StaticRoutingData> StaticRoutes = new ObservableCollection<StaticRoutingData>();
		public ASNData ASNData = new ASNData();
		public ObservableCollection <InterfaceMapping> Interfaces = new ObservableCollection <InterfaceMapping>();
		public VyattaConfigObject ConfigRoot;
		public VyattaConfigObject TemplateRoot;

		public System.String[] OldConfigLines;
		public System.String[] NewConfigLines;

		public RouterData()
		{
			StaticRoutes.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(
				delegate( object Sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs EventArgs )
				{
					if( EventArgs.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add )
					{
						foreach( var ItemObj in EventArgs.NewItems )
						{
							StaticRoutingData Item = (StaticRoutingData)ItemObj;

							string[] Splits = Item.Destination.Split( new char[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries );

							foreach( string Value in Splits )
							{
								if( Item.Type == RoutingType.Organisation )
								{
									VyattaConfigRouting.AddStaticRoutesForOrganization( ConfigRoot, Value.Trim(), this, Item.Interface, Item.Name );
								}
								else if( Item.Type == RoutingType.ASN )
								{
									int ASN = 0;
									int.TryParse( Value.Trim(), out ASN );

									VyattaConfigRouting.AddStaticRoutesForASN( ConfigRoot, ASN, this, Item.Interface, Item.Name );
								}
								else if( Item.Type == RoutingType.Netmask )
								{
									VyattaConfigRouting.AddStaticRoute( ConfigRoot, this, Value.Trim(), Item.Interface, Item.Name );
								}
								else
								{
									throw new Exception( "Unimplemented type" );
								}
						}
						}
					}
					else if( EventArgs.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove )
					{
						foreach( var ItemObj in EventArgs.OldItems )
						{
							StaticRoutingData Item = (StaticRoutingData)ItemObj;
							VyattaConfigRouting.DeleteGeneratedStaticRoutes( ConfigRoot, Item.Name );
						}
					}
				}
			);
		}
	}
}
