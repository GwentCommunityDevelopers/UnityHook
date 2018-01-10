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
				// Taken from GwentWebServiceClient.Services.GOGRest.LiveDataService.GET_LIVE_DATA_URL
				string baseUrl = "http://seawolf-config.gog.com/game/{0}_{1}/data/{2}_{3}/data_definitions.zip";

				string hash = ConfigManager.Data.hash;
				string version = ConfigManager.Data.version;
				string clientVersion = GwentSettings.Instance.ClientVersion;
				string secret = GwentSettings.Instance.Secret;

				DateTime localDate = DateTime.Now;
				string date = localDate.ToString();
				string clientMessage = String.Format("Client Version: {0}", clientVersion);
				string dataMessage = String.Format("Data Version: {0}", version);
				string hashMessage = String.Format("Hash: {0}", hash);
				string secretMessage = String.Format("Secret: {0}", secret);

				string completeUrl = String.Format(baseUrl, clientVersion, secret, version, hash);

				File.WriteAllLines("gwent_data.txt", new string[] { date, clientMessage, dataMessage, hashMessage, secretMessage, completeUrl });
			}

			return null;
		}
	}
}
