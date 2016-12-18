using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace vyatta_config_updater
{
	public class Util
	{
		//Crypto functions based on answer by Brett
		// http://stackoverflow.com/questions/202011/encrypt-and-decrypt-a-string#2791259

		//HKLM function by Palani Kumar
		// http://stackoverflow.com/questions/9491958/registry-getvalue-always-return-null

		const uint SaltLength = 32;

		public static RegistryKey GetHKLM()
		{
			if( Environment.Is64BitOperatingSystem )
			{
				return RegistryKey.OpenBaseKey( RegistryHive.LocalMachine, RegistryView.Registry64 );
			}
			else
			{
				return RegistryKey.OpenBaseKey( RegistryHive.LocalMachine, RegistryView.Registry32 );
			}
		}

		// NOTE: This function is not going to protect data
		// against a serious attacker. It is just designed to
		// prevent people getting plain text from the windows
		// registry.
		public static string EncryptString( string Input )
		{
			RegistryKey HKLM = GetHKLM();

			Microsoft.Win32.RegistryKey WindowsVersionInfo = HKLM.OpenSubKey( @"Software\Microsoft\Windows NT\CurrentVersion", RegistryKeyPermissionCheck.ReadSubTree );
			string ProductId = (string)WindowsVersionInfo.GetValue( "ProductId", "" );
			UInt64 InstallDateU64 = System.Convert.ToUInt64(WindowsVersionInfo.GetValue("InstallTime", null ));
			byte[] InstallDateSalt = Encoding.ASCII.GetBytes( InstallDateU64.ToString() );
			WindowsVersionInfo.Close();

			HKLM.Close();

			RijndaelManaged AES = null;

			try
			{
				Rfc2898DeriveBytes Key = new Rfc2898DeriveBytes( ProductId, InstallDateSalt );

				AES = new RijndaelManaged();
				AES.Key = Key.GetBytes( AES.KeySize / 8 );

				ICryptoTransform Encrypt = AES.CreateEncryptor( AES.Key, AES.IV );

				using( MemoryStream Stream = new MemoryStream() )
				{
					Stream.Write( BitConverter.GetBytes( (Int32)AES.IV.Length ), 0, sizeof(Int32) );
					Stream.Write( AES.IV, 0, AES.IV.Length );
					using( CryptoStream CryptoStream = new CryptoStream( Stream, Encrypt, CryptoStreamMode.Write ) )
					{
						using( StreamWriter Writer = new StreamWriter( CryptoStream ) )
						{
							Writer.Write( Input );
						}
					}

					string Result = Convert.ToBase64String( Stream.ToArray() );
					return Result;
				}
			}
			finally
			{
				if( AES != null )
				{
					AES.Clear();
				}
			}
		}

		public static string DecryptString( string Input )
		{
			RegistryKey HKLM = GetHKLM();

			Microsoft.Win32.RegistryKey WindowsVersionInfo = HKLM.OpenSubKey( @"Software\Microsoft\Windows NT\CurrentVersion", RegistryKeyPermissionCheck.ReadSubTree );
			string ProductId = (string)WindowsVersionInfo.GetValue( "ProductId", "" );
			UInt64 InstallDateU64 = System.Convert.ToUInt64(WindowsVersionInfo.GetValue("InstallTime", null ));
			byte[] InstallDateSalt = Encoding.ASCII.GetBytes( InstallDateU64.ToString() );
			WindowsVersionInfo.Close();

			HKLM.Close();

			RijndaelManaged AES = null;

			try
			{
				Rfc2898DeriveBytes Key = new Rfc2898DeriveBytes( ProductId, InstallDateSalt );

				byte[] Bytes = Convert.FromBase64String( Input );

				using( MemoryStream Stream = new MemoryStream( Bytes ) )
				{
					AES = new RijndaelManaged();
					AES.Key = Key.GetBytes( AES.KeySize / 8 );
					AES.IV = ReadByteArray( Stream );

					ICryptoTransform Decrypt = AES.CreateDecryptor( AES.Key, AES.IV );
					
					using( CryptoStream CryptoStream = new CryptoStream( Stream, Decrypt, CryptoStreamMode.Read ) )
					{
						using( StreamReader Reader = new StreamReader( CryptoStream ) )
						{
							return Reader.ReadToEnd();
						}
					}
				}
			}
			finally
			{
				if( AES != null )
				{
					AES.Clear();
				}
			}
		}

		private static byte[] ReadByteArray( Stream Stream )
		{
			byte[] LengthBytes = new byte[ sizeof(Int32) ];

			if( Stream.Read( LengthBytes, 0, LengthBytes.Length ) != LengthBytes.Length )
			{
				throw new SystemException("Unexpected end of stream.");
			}

			byte[] Buffer = new byte[ BitConverter.ToInt32( LengthBytes, 0 ) ];
			if( Stream.Read( Buffer, 0, Buffer.Length ) != Buffer.Length )
			{
				throw new SystemException("Not all bytes could be read.");
			}

			return Buffer;
		}
	}
}
