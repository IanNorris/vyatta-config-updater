using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Renci.SshNet;
using Renci.SshNet.Common;

namespace vyatta_config_updater
{
	public partial class Busy : Form
	{
		private bool Completed = false;
		private BusyWorkInterface Work;

		delegate void WriteToStream( string Input ); 

		public Busy( BusyWorkInterface Work )
		{
			this.Work = Work;

			InitializeComponent();
		}

		private void SetStatus( string NewStatus, int NewProgress )
		{
			BeginInvoke( new Action(
				() => 
				{
					Progress.Value = NewProgress;
					Status.Text = NewStatus;
					Progress.Refresh();
					Status.Refresh();
				}
			) );
		}

		private void BusyBackgroundWorker_DoWork( object sender, DoWorkEventArgs e )
		{
			SetStatus( "Connecting over SSH...", 0 );

			Util.UpdateStatusDelegate Delegate = ( string NewStatus, int NewProgress ) =>
			{
				SetStatus( NewStatus, NewProgress );
			};

			try
			{
				if( Work.DoWork( Delegate ) )
				{
					Completed = true;
				}
			}
			catch( Exception Exception )
			{
				BeginInvoke( new Action(
					() => 
					{
						MessageBox.Show( Exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
					}
				) );
			}
		}

		private void BusyBackgroundWorker_Completed( object sender, RunWorkerCompletedEventArgs e )
		{
			DialogResult = DialogResult.OK;
			Close();
		}

		private void Cancel_Click( object sender, EventArgs e )
		{
		}

		private void Busy_FormClosing( object sender, FormClosingEventArgs e )
		{
			if( BusyBackgroundWorker.IsBusy )
			{
				e.Cancel = true;
				BusyBackgroundWorker.CancelAsync();
			}
			else
			{
				DialogResult = Completed ? DialogResult.OK : DialogResult.Cancel;
			}
		}

		private void Busy_Load( object sender, EventArgs e )
		{
			BusyBackgroundWorker.RunWorkerAsync();
		}
	}
}
