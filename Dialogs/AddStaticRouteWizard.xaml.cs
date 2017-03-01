using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Forms.Integration;
using System.Windows.Data;
using vyatta_config_updater.Routing;
using System;
using System.Windows.Controls;
using System.ComponentModel;

namespace vyatta_config_updater.Dialogs
{
	public enum StaticRouteType
	{
		All,
		DNSLog,
		Organization,
		ASN,
		Netmask,
		IP
	}

	public enum RouteCost
	{
		Low,
		Medium,
		High,
		VeryHigh,
		Extreme
	}

	public class AddStaticRouteWizardData
	{
		private const int RouteMediumCostThreshold = 3;
		private const int RouteHighCostThreshold = 6;
		private const int RouteVeryHighCostThreshold = 10;
		private const int RouteExtremeCostThreshold = 15;

		public RouteCost EstimatedRouteCost
		{
			get
			{
				if( TotalRules < RouteMediumCostThreshold )
				{
					return RouteCost.Low;
				}
				else if( TotalRules < RouteHighCostThreshold )
				{
					return RouteCost.Medium;
				}
				else if( TotalRules < RouteVeryHighCostThreshold )
				{
					return RouteCost.High;
				}
				else if( TotalRules < RouteExtremeCostThreshold )
				{
					return RouteCost.VeryHigh;
				}
				else
				{
					return RouteCost.Extreme;
				}
			}
		}

		public string EstimatedRouteCostString
		{
			get
			{
				switch( EstimatedRouteCost )
				{
					case RouteCost.Low: return "Low";
					case RouteCost.Medium: return "Medium";
					case RouteCost.High: return "High";
					case RouteCost.VeryHigh: return "Very High";
					case RouteCost.Extreme: return "Extremely High";
					default:
						return "Unknown";
				}
			}
		}

		public string EstimatedRouteColor
		{
			get
			{
				switch( EstimatedRouteCost )
				{
					case RouteCost.Low: return "Black";
					case RouteCost.Medium: return "Orange";
					case RouteCost.High: return "Orange";
					case RouteCost.VeryHigh: return "Red";
					case RouteCost.Extreme: return "Red";
					default:
						return "Unknown";
				}
			}
		}


		public delegate void UpdatedListType();
		public UpdatedListType UpdatedList;

		public delegate void UpdatedSummaryType();
		public UpdatedSummaryType UpdatedSummary;

		UInt32 TotalIPs;
		int TotalRules;

		public UInt32 TotalIPCountProperty {get { return TotalIPs; }}
		public int TotalRuleCountProperty {get { return TotalRules; }}

		int _SelectedInterface = 0;
		public int SelectedInterface
		{
			get { return _SelectedInterface; }
			set { _SelectedInterface = value; }
		}

		string _FilterValue = "";
		public string FilterValue
		{
			get { return _FilterValue; }
			set { _FilterValue = value; UpdateAddressList(); }
		}

		public bool CanContinue_FilterPage { get { return TotalRuleCountProperty > 0 && TotalRuleCountProperty <= RouteExtremeCostThreshold; } }
		public bool CanContinue_SummaryPage { get { return _RouteNameValue.Length > 0; } }
		public string ColorStatusMessage { get {  return CanContinue_FilterPage ? "Black" : "Red"; } }

		string _RouteNameValue = "";
		public string RouteNameValue
		{
			get { return _RouteNameValue; }
			set { _RouteNameValue = value; UpdatedSummary(); }
		}

		StaticRouteType _RouteType = StaticRouteType.DNSLog;
		public StaticRouteType RouteType
		{
			get { return _RouteType; }
			set { _RouteType = value; UpdateAddressList(); }
		}

		public string FilterValuePresentable
		{
			get { return _RouteType == StaticRouteType.All ? "The Whole Internet" : _FilterValue; }
		}

		public string RouteTypeName
		{
			get
			{
				switch( _RouteType )
				{
					case StaticRouteType.All:
						return "The Whole Internet";
					case StaticRouteType.DNSLog:
						return "Collected IPs";
					case StaticRouteType.Organization:
						return "Organization";
					case StaticRouteType.ASN:
						return "ASN";
					case StaticRouteType.Netmask:
						return "Specified Netmasks";
					case StaticRouteType.IP:
						return "Specific IP Addresses";
					default:
						return "Unknown";
				}
			}
		}

		StaticRouteAction _RouteAction = StaticRouteAction.ToInterface;
		public StaticRouteAction RouteAction
		{
			get { return _RouteAction; }
			set { _RouteAction = value; }
		}

		public string SummaryTotal
		{
			get { return $"Result generates {TotalRules} rules, matching {TotalIPs} IP addresses."; }
		}

		public RouterData RouterData;

		public ObservableCollection<FilterItem> Addresses = new ObservableCollection<FilterItem>();

		public AddStaticRouteWizardData( RouterData Data )
		{
			RouterData = Data;
		}

		public void UpdateAddressList( )
		{
			Addresses.Clear();

			TotalIPs = 0;
			TotalRules = 0;

			string ActualValue = FilterValue;
			StaticRouteType ActualRouteType = RouteType;
			if( RouteType == StaticRouteType.All )
			{
				ActualValue = "0.0.0.0/1,1.0.0.0/1";
				ActualRouteType = StaticRouteType.Netmask;
			}
			
			var Split = ActualValue.Split( new char[] { ',', ';' } );
			foreach( var ValueUntrimmed in Split )
			{
				string Value = ValueUntrimmed.Trim();

				switch( ActualRouteType )
				{
					case StaticRouteType.ASN:
						{
							int Temp;
							if( int.TryParse( Value, out Temp ) )
							{
								var Netmasks = RouterData.ASNData.GetNetmasksFromASN( Temp );
								foreach( var Netmask in Netmasks )
								{
									var Elem = new FilterItem();
									Elem.ItemName = Netmask.NetmaskString;
									Elem.Owner = "Unknown";
									RouterData.ASNData.ASNToOwner.TryGetValue( Temp, out Elem.Owner );
									Elem.IPCount = Netmask.IPCount;
									Addresses.Add( Elem );
								}
							}
							break;
						}

					case StaticRouteType.IP:
						{
							if( vyatta_config_updater.Routing.Netmask.IsValidIP( Value ) )
							{
								string ASN, Owner, Netmask;
								if( !RouterData.ASNData.GetMatchingASNAndOrgForIP( Value, out ASN, out Owner, out Netmask ) )
								{
									Owner = "Unassigned";
								}
							
								var Elem = new FilterItem();
								Elem.ItemName = Value + "/32";
								Elem.Owner = Owner;
								Elem.IPCount = 1;
								Addresses.Add( Elem );
							}
							else
							{
								Addresses.Clear();
								return;
							}

							break;
						}

					case StaticRouteType.Organization:
						if( Value.Length >= 2 )
						{
							var Netmasks = RouterData.ASNData.GetNetmasksFromOrganization( Value );
							foreach( var Netmask in Netmasks )
							{
								var Elem = new FilterItem();
								Elem.ItemName = Netmask.NetmaskString;
							
								if( !RouterData.ASNData.ASNToOwner.TryGetValue( (int)Netmask.ASN, out Elem.Owner ) )
								{
									Elem.Owner = "Unassigned";
								}

								Elem.IPCount = Netmask.IPCount;
								Addresses.Add( Elem );
							}
						}
						break;

					case StaticRouteType.Netmask:
						{
							Netmask? NetmaskValue = vyatta_config_updater.Routing.Netmask.GetNetmaskFromString( Value );
							if( NetmaskValue.HasValue )
							{
								var Elem = new FilterItem();
								Elem.ItemName = NetmaskValue.Value.NetmaskString;
								if( !RouterData.ASNData.ASNToOwner.TryGetValue( (int)NetmaskValue.Value.ASN, out Elem.Owner ) )
								{
									Elem.Owner = "Unassigned";
								}
								Elem.IPCount = NetmaskValue.Value.IPCount;
								Addresses.Add( Elem );
							}
							else
							{
								Addresses.Clear();
								return;
							}
							break;
						}
				}
			}

			foreach( var Item in Addresses )
			{
				TotalIPs += Item.IPCount;
				TotalRules += 1;
			}

			UpdatedList();
		}
	}

	/// <summary>
	/// Interaction logic for AddStaticRouteWizard.xaml
	/// </summary>
	public partial class AddStaticRouteWizard : Window
	{
		public AddStaticRouteWizard( RouterData Data )
		{
			InitializeComponent();
			ElementHost.EnableModelessKeyboardInterop(this);

			var DC = new AddStaticRouteWizardData( Data );
			DataContext = DC;
			DC.UpdatedList += Addresses_CollectionChanged;
			DC.UpdatedSummary += Summary_Changed;

			AddressList.ItemsSource = DC.Addresses;
			Interface.ItemsSource = DC.RouterData.Interfaces;
		}

		private void Addresses_CollectionChanged()
		{
			Summary.GetBindingExpression(TextBlock.TextProperty).UpdateTarget();
			Summary.GetBindingExpression(TextBlock.ForegroundProperty).UpdateTarget();

			FilterPage.GetBindingExpression( AvalonWizard.WizardPage.AllowNextProperty ).UpdateTarget();
		}

		private void Summary_Changed()
		{
			SummaryPage.GetBindingExpression( AvalonWizard.WizardPage.AllowFinishProperty ).UpdateTarget();
		}

		private void Wizard_Finished( object sender, RoutedEventArgs e )
		{
			DialogResult = true;
			Close();
		}
	}

	public class FilterItem
	{
		public RoutingType Type;
		public string ItemName;
		public string Owner;
		public UInt32 IPCount;

		public override string ToString()
		{
			return $"{ItemName} ({Owner}) - {IPCount} IPs";
		}
	}

	class EnableListBoxConverter : IValueConverter
	{
		public object Convert( object Value, Type TargetType, object Parameter, System.Globalization.CultureInfo Culture )
		{
			return !((StaticRouteType)Value == StaticRouteType.All || (StaticRouteType)Value == StaticRouteType.DNSLog);
		}

		public object ConvertBack( object Value, Type TargetType, object Parameter, System.Globalization.CultureInfo Culture )
		{
			return new NotImplementedException();
		}
	}

	class EnableInterfaceComboConverter : IValueConverter
	{
		public object Convert( object Value, Type TargetType, object Parameter, System.Globalization.CultureInfo Culture )
		{
			return (StaticRouteAction)Value == StaticRouteAction.ToInterface;
		}

		public object ConvertBack( object Value, Type TargetType, object Parameter, System.Globalization.CultureInfo Culture )
		{
			return new NotImplementedException();
		}
	}
}
