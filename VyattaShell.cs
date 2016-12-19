using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Renci.SshNet;
using Renci.SshNet.Common;

namespace vyatta_config_updater
{
	public class VyattaShell : IDisposable
	{
		SshClient Client;
		ShellStream Stream;
		bool Disposed = false;
		bool OwnClient;

		//Client is assumed to be already connected
		public VyattaShell( SshClient Client )
		{
			OwnClient = false;
			this.Client = Client;

			CreateShell();
		}

		public VyattaShell( string Address, string Username, string Password )
		{
			OwnClient = true;
			Client = new SshClient( Address, Username, Password );
			Client.Connect();

			CreateShell();
		}

		private void CreateShell()
		{
			var TermOptions = new Dictionary<TerminalModes,uint>();
			TermOptions.Add( TerminalModes.ECHO, 0 );

			Stream = Client.CreateShellStream( "bash", 80, 24, 800, 600, 64 * 1024, TermOptions );

			ReadResponse();
		}

		private string ReadResponse()
		{
			string Response = "";

			bool End = false;

			while( !End )
			{
				Stream.Expect(
					new ExpectAction( new Regex(@"\w+@\w+\:[\w~\/\\]+\$"), (Input) =>
					{
						End = true;
						Response += Input;
					} ),
					new ExpectAction( new Regex(@"\w+@\w+\:[\w~\/\\]+#"), (Input) =>
					{
						End = true;
						Response += Input;
					} ),
					new ExpectAction( new Regex(@"\n\:"), (Input) =>
					{
						Response += Input;
						Stream.Write(" ");
					} )
				);
			}

			return Response;
		}

		public string RunCommand( string Command )
		{
			System.Console.Out.WriteLine( "CL$ " + Command );

			Stream.WriteLine( Command );

			string Response = ReadResponse();

			System.Console.Out.WriteLine( Response );
			
			return Response;
		}

		// Public implementation of Dispose pattern callable by consumers.
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);           
		}

		// Protected implementation of Dispose pattern.
		protected virtual void Dispose( bool Disposing )
		{
			if( Disposed )
			{
				return;
			}

			if( Disposing )
			{
				// Free any other managed objects here.

				if( Stream != null )
				{
					Stream.Dispose();
					Stream = null;
				}
				
				if( OwnClient && Client != null )
				{
					Client.Disconnect();
					Client.Dispose();
					Client = null;
				}
			}

			// Free any unmanaged objects here.
			
			Disposed = true;
		}

		~VyattaShell()
		{
			Dispose(false);
		}
	}
}
