using System;
using System.Numerics;
using SharedMemory;
using System.Collections.Generic;

namespace UserServiceTest
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			String uname, pass;
			Console.Write ("Username: ");
			uname = Console.ReadLine ();
			Console.Write ("Password: ");
			pass = Console.ReadLine ();
			FeMoObject User = UserServices.FetchUserInfo (uname, pass);
			FeMoManager man = new FeMoManager ();
			User.Id = 12;
			man.CacheObject (User);
			man.SendUpdateString ();
			Global.log ("Welcome " + User.GetString ("Display"));
			String token = User.GetString ("Token");
			String encToken = UserServices.Encrypt (token, UserServices.GenerateKeys (pass));
			if (UserServices.CheckToken (uname, encToken)) {
				Global.success ("User Authentication");
			} else {
				Global.fail ("Token expired!");
			}
//			BigInteger[] keys = UserServices.GenerateKeys("test");
//			Global.log("Key Length: " + (keys[0].ToByteArray().Length * 8) + " bit");
		}
	}
}
