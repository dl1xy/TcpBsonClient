//------------------------------------------------------------------------------
// <auto-generated>
//     Dieser Code wurde von einem Tool generiert.
//     Laufzeitversion:4.0.30319.18444
//
//     Änderungen an dieser Datei können falsches Verhalten verursachen und gehen verloren, wenn
//     der Code erneut generiert wird.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using Kernys.Bson;


namespace TcpBsonClient
{
	public class ClientCmdMessageException: BaseClientCmd
	{
		public string message;
		public ClientCmdMessageException()
		{
		}

		override public void decode(BSONObject obj)
		{			
			this.id = obj["id"].int32Value;
			this.meta = obj["meta"].stringValue;
			this.message = obj ["message"].stringValue;
		}
	}
}

