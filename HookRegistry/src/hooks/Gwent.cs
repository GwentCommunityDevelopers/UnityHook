using System;
using GwentUnity;
using System.IO;
using RedTools;

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
			return new string[] {
				"GwentUnity.AppLogin.InitializeDefinitions::LoadDefinitions"
			};
		}

		object OnCall(string typeName, string methodName, object thisObj, object[] args, IntPtr[] refArgs, int[] refIdxMatch)
		{
			if (methodName == "LoadDefinitions") {
				string hash = ConfigManager.Data.hash;
				string version = ConfigManager.Data.version;

				string clientVersion = GwentSettings.Instance.ClientVersion;
				string secret = GwentSettings.Instance.Secret;
				string[] gwentArgs = new string[] {
					hash,
					version,
					clientVersion,
					secret
				};

				File.WriteAllLines("gwent_data_args.txt", gwentArgs);
			}

			return null;
		}
	}
}
