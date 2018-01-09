using Hooks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Hooks
{
	[RuntimeHook]
	class GwentHook
	{
		private bool _reentrant;

		public GwentHook()
		{
			HookRegistry.Register(OnCall);
		}

		private void InitDynamicTypes() { }

		public static string[] GetExpectedMethods()
		{
			return new string[] { "GwentWebServiceClient.Services.GOGRest.LiveDataService::GetLiveDataPackage" };
		}

		object OnCall(string typeName, string methodName, object thisObj, object[] args, IntPtr[] refArgs, int[] refIdxMatch)
		{
			Debug.WriteLine("On Call");
			if (args != null)
			{
				var sw = new StreamWriter(@"D:\gwent.txt");
				sw.WriteLine(typeName);
				sw.WriteLine(methodName);
				for(int i = 0; i < 4; i++)
				{
					sw.WriteLine((string) args[i]);
				}
				sw.Flush();
				sw.Close();

				File.WriteAllText("gwent.txt", methodName);
			}
			return new object();
		}
	}
}
