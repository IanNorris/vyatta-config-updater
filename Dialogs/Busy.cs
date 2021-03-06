﻿using System;
using System.ComponentModel;
using System.Windows.Forms;

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
					System.Console.Out.WriteLine( NewStatus );
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

			Util.UpdateStatusDelegate SetStatusDelegate = ( string NewStatus, int NewProgress ) =>
			{
				SetStatus( NewStatus, NewProgress );
			};

			Util.ShouldCancelDelegate CancelDelegate = () =>
			{
				return BusyBackgroundWorker.CancellationPending;
			};

			try
			{
				if( Work.DoWork( SetStatusDelegate, CancelDelegate ) )
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
