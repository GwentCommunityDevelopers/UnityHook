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

		private static string LOG_FILE = "gwent_data.txt";

		public GwentHook()
		{
			HookRegistry.Register(OnCall);
		}

		private void InitDynamicTypes() { }

		public static string[] GetExpectedMethods()
		{
			return new string[] {
				"GwentUnity.AppLogin.InitializeDefinitions::StartLoadDefinitions",
				"GwentUnity.AppLogin.InitializeDefinitions::StardLoadDefaultDefinitions",
				"GwentUnity.AppMenusStateV2::OnEnterState"
			};
		}

		object OnCall(string typeName, string methodName, object thisObj, object[] args, IntPtr[] refArgs, int[] refIdxMatch)
		{
			switch (methodName)
			{
				case "OnEnterState":
					// Called when Gwent Client reaches the main menu.
					// Do nothing.
					break;
				case "StartLoadDefinitions":
					// Called when Gwent loads card data from the internet.
					saveGwentDataToFile();
					break;
				case "StardLoadDefaultDefinitions":
					// Called when Gwent loads card data from the local file.
					string message = "Card data loaded from local file. Look for the data_definitions file. e.g.";
					string location = Directory.GetCurrentDirectory() + "\\GWENT_Data\\StreamingAssets\\data_definitions";
					File.WriteAllLines(LOG_FILE, new string[] { DateTime.Now.ToString(), message, location});
					break;
			}

			return null;
		}

		private void saveGwentDataToFile()
		{
			// Taken from GwentWebServiceClient.Services.GOGRest.LiveDataService.GET_LIVE_DATA_URL
			string baseUrl = "http://seawolf-config.gog.com/game/{0}_{1}/data/{2}_{3}/data_definitions.zip";

			string hash = ConfigManager.Data.hash;
			string version = ConfigManager.Data.version;
			string clientVersion = GwentSettings.Instance.ClientVersion;
			string secret = GwentSettings.Instance.Secret;

			string clientMessage = String.Format("Client Version: {0}", clientVersion);
			string dataMessage = String.Format("Data Version: {0}", version);
			string hashMessage = String.Format("Hash: {0}", hash);
			string secretMessage = String.Format("Secret: {0}", secret);

			string completeUrl = String.Format(baseUrl, clientVersion, secret, version, hash);

			File.WriteAllLines(LOG_FILE, new string[] { DateTime.Now.ToString(), clientMessage, dataMessage, hashMessage, secretMessage, completeUrl });
		}
	}
}
