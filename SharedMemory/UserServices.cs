using System;
using System.Numerics;
using System.Collections.Generic;
using System.Net;

using System.Security.Cryptography;

namespace SharedMemory
{
	public class UserServices
	{

		private const String AUTH_SERVER = "http://localhost:8080/";

		public static FeMoObject FetchUserInfo (String username, String password)
		{
			WebClient client = new WebClient ();
			Global.user ("Contacting UserServices...");
			byte[] data = client.DownloadData (AUTH_SERVER + "auth/" + username + "/1");
			String s = System.Text.Encoding.UTF8.GetString (data);
			Global.user ("Received Answer!");
			BigInteger[] keys = GenerateKeys (password);
			String obj = Decrypt (s, keys);
			try {
				FeMoObject[] list = FeMoUpdateStringFormatter.JSON.Parse (obj);
				if (list.Length == 0)
					Global.fail ("Not a User!");
				else {
					FeMoObject User = list [0];
					return User;
				}
			} catch  {
				Global.fail("Couldn't read: " + obj);
			}
			return null;
		}

		public static bool CheckToken (String user, String encToken)
		{
			WebClient client = new WebClient();
			Global.user("Contacting UserServices...");
			byte[] data = client.DownloadData(AUTH_SERVER + "auth/t/" + user + ";" + encToken);
			String s = System.Text.Encoding.UTF8.GetString (data);
			Global.user ("Received Answer!");
			return s == "1";
		}

		public static String Decrypt(String s, BigInteger[] keys) {
			String ret = "";
			String[] blocks = System.Text.RegularExpressions.Regex.Split(s, "0x");
			int blocks_count = blocks.Length;
			Global.user("Found " + (blocks_count - 1) + " Blocks");
			for(int i = 0; i < blocks_count; i++) {
				if(blocks[i].Length == 0)
					continue;
				//Console.Write("Block(" + i + "): " + blocks[i]);
				BigInteger val = BigInteger.Parse(blocks[i]);
				BigInteger dec = BigInteger.ModPow(val, keys[2], keys[0]);
				String var = System.Text.Encoding.UTF8.GetString(dec.ToByteArray());
				//Console.WriteLine(" => " + var);
				ret += var;
			}
			return ret;
		}

		public static String Encrypt(String message, BigInteger[] keys) {
			String ret = "";
			int bytelen = keys[0].ToByteArray().Length;
			byte[] bytes = System.Text.Encoding.UTF8.GetBytes(message);
			int blocks = (int)Math.Ceiling((double)bytes.Length / (double)bytelen);
			for(int i = 0; i < blocks; i++) {
				byte[] block = new byte[bytelen];
				Array.Copy(bytes, i * bytelen, block, 0, (i == blocks - 1 ? bytes.Length - i * bytelen : bytelen));
				BigInteger val = new BigInteger(block);
				BigInteger enc = BigInteger.ModPow(val, keys[1], keys[0]);
				String var = "0x" + enc.ToString();
				ret += var;
			}
			return ret;
		}

		public static BigInteger[] GenerateKeys (String password)
		{
			long begin = Environment.TickCount;
			BigInteger[] primes = SplitPass(password);
			long end_prime = Environment.TickCount;
			//Global.log("Prime Splitting took " + (end_prime - begin) + " ms");
			BigInteger[] keys = GenerateRSA(primes);
			BigInteger m = new BigInteger(7);
			BigInteger c = BigInteger.ModPow(m, keys[1], keys[0]);
			//Global.log("Encrypted Text: " + c);
			m = BigInteger.ModPow(c, keys[2], keys[0]);
			//Global.log("Decrypted Text: " + m + "\t Expecting: 7");
			long end = Environment.TickCount;
			Global.user("Key generation took " + (end - begin) + " ms");
			return keys;
		}

		public static BigInteger[] SplitPass (String password)
		{
			MD5 md5 = MD5.Create ();
			byte[] hash = md5.ComputeHash (System.Text.Encoding.ASCII.GetBytes (password));
//			String h = "";
//			foreach (var item in hash) {
//				h +=  (int) item + " ";
//			}
//			Global.log(h);
			byte[] no1 = new byte[(int)Math.Ceiling ((double)hash.Length / 2)], no2 = new byte[(int)Math.Floor ((double)hash.Length / 2)];
			Array.Copy (hash, no1, (int)Math.Ceiling ((double)hash.Length / 2));
			Array.Copy (hash, no1.Length, no2, 0, (int)Math.Floor ((double)hash.Length / 2));
			BigInteger[] ret = new BigInteger[] {new BigInteger (no1), new BigInteger (no2)};
			for (int i = 0; i < ret.Length; i++) {
				ret[i] = BigInteger.Abs(ret[i]);
				ret[i] = isPrime(ret[i]);
			}
			return ret;
		}

		public static BigInteger isPrime(BigInteger number) {
			Boolean prime = false;
			Random r = new Random();
			while(!prime) {
				prime = true;
				for(int j = 0; j < 100; j++) {
					BigInteger a = new BigInteger(number < Int32.MaxValue ? (int) number - 1 : r.Next(Int32.MaxValue));
					BigInteger res = BigInteger.ModPow(a, number, number);
					if(res != a) {
						prime = false;
						number += BigInteger.One;
//						Console.SetCursorPosition(0, Console.CursorTop);
//						Console.Write(new String(' ', Console.WindowWidth - 1));
//						Console.SetCursorPosition(0, Console.CursorTop - 1);
//						Global.log("New Prime: " + number);
						break;
					}
				}
			}
			return number;
		}

		public static BigInteger PrevPrime(BigInteger number) {
			Boolean prime = false;
			Random r = new Random();
			while(!prime) {
				prime = true;
				for(int j = 0; j < 100; j++) {
					BigInteger a = new BigInteger(number < Int32.MaxValue ? (int) number - 1 : r.Next(Int32.MaxValue));
					BigInteger res = BigInteger.ModPow(a, number, number);
					if(res != a) {
						prime = false;
						number -= BigInteger.One;
						break;
					}
				}
			}
			return number;
		}

		//public static RSAParameters GenerateRSA (BigInteger[] primes) {
		public static BigInteger[] GenerateRSA (BigInteger[] primes) {
			long begin = Environment.TickCount;
			Global.user("Creating RSA Key...");
			BigInteger p = primes[0];
			//Global.user("p: \t" + p);
			BigInteger q = primes[1];
			//Global.user("q: \t" + q);
			BigInteger N = p * q;
			long end_n = Environment.TickCount;
			//Global.user("N: \t" + N + "\t (p * q) " + "\t(" + (end_n - begin) + " ms)");
			BigInteger N_ = (p - 1)*(q - 1);
			long end_r = Environment.TickCount;
			//Global.user("R: \t" + N_ + "\t ((p - 1) * (q - 1))" + "\t(" + (end_r - end_n) + " ms)");
			BigInteger e = 65537;
			long end_e = Environment.TickCount;
			//Global.user("e: \t" + e + "\t(" + (end_e - end_r) + " ms)");
			BigInteger d = modInverse(e, N_);  
			long end_d = Environment.TickCount;
			//Global.user("d: \t" + d + "\t(" + (end_d - end_e) + " ms)");
			return new BigInteger[] {N, e, d};
			/*RSAParameters rsa = new RSAParameters();
			rsa.Modulus = N.ToByteArray();
			rsa.P = p.ToByteArray();
			rsa.Q = p.ToByteArray();*/
		}

		
		
		private static BigInteger modInverse(BigInteger a, BigInteger n) 
		{
			BigInteger i = n, v = 0, d = 1;
			while (a>0) {
				BigInteger t = i/a, x = a;
				a = i % x;
				i = x;
				x = d;
				d = v - t*x;
				v = x;
			}
			v %= n;
			if (v<0) v = (v+n)%n;
			return v;
		}
		


		private static BigInteger[] sub_method_1 (BigInteger N_)
		{
			BigInteger rand = N_ + 1;
			List<BigInteger> list = new List<BigInteger> ();
			while(rand == isPrime(rand)) {
				rand += N_;
			}
			sub_method_3 (rand, (BigInteger) 2, list);
			while (list.Count < 2) {
				rand += N_;
				sub_method_3 (rand, (BigInteger) 2, list);
			}
			BigInteger e = BigInteger.One, d = BigInteger.One;
			for (int i = 0; i < list.Count; i += 2) {
				if(list.Count % 2 == 0) {
					e *= list[i];
					d *= list[i + 1];
				} else {
					if(i + 1 == list.Count) {
						e *= list[i];
					} else {
						e *= list[i];
						d *= list[i + 1];
					}
				}
			}
			return new BigInteger[] {e, d};
		}

		private static BigInteger sub_method_2 (BigInteger N_)
		{
			BigInteger r = N_ / 2;
			while (BigInteger.GreatestCommonDivisor(r, N_) != 1) {
				r++;
			}
			return (BigInteger) r;
		}

		public static void sub_method_3 (BigInteger n, BigInteger factor, List<BigInteger> list)
		{
			//factor = PrevPrime(Sqrt(n));
			BigInteger max = Sqrt(n);
			BigInteger testFactor = factor;
			if (n == BigInteger.One)
				return;
			if (n % factor != 0) {
				while(n % testFactor != 0) {
					Console.SetCursorPosition(0, Console.CursorTop);
					Console.Write(new String(' ', Console.WindowWidth - 1));
					Console.SetCursorPosition(0, Console.CursorTop - 1);
					testFactor = testFactor * 2 + 1;
					testFactor = isPrime(testFactor);
					Global.log("GCD Factor: " + testFactor);
					if(testFactor > max) {
						sub_method_3(n, isPrime(factor + 1), list);
						return;
					}
				}
			}
			list.Add(testFactor);
			Console.SetCursorPosition(0, Console.CursorTop);
			Console.Write(new String(' ', Console.WindowWidth - 1));
			Console.SetCursorPosition(0, Console.CursorTop - 1);
			Global.log("Factor: " + testFactor + "\nNumber: " + (n / testFactor));
			Console.WriteLine();
			sub_method_3(n / testFactor, factor + 1, list);
		}

		public static BigInteger Sqrt(BigInteger n)
		{
			if (n == 0) return 0;
			if (n > 0)
			{
				int bitLength = Convert.ToInt32(Math.Ceiling(BigInteger.Log(n, 2)));
				BigInteger root = BigInteger.One << (bitLength / 2);
				
				while (!isSqrt(n, root))
				{
					root += n / root;
					root /= 2;
				}
				
				return root;
			}
			
			throw new ArithmeticException("NaN");
		}

		private static Boolean isSqrt(BigInteger n, BigInteger root)
		{
			BigInteger lowerBound = root*root;
			BigInteger upperBound = (root + 1)*(root + 1);
			
			return (n >= lowerBound && n < upperBound);
		}
	}
}

