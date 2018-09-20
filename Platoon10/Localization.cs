using System;
using Windows.ApplicationModel.Resources;

//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

namespace Platoon10
{
	public class Localization
	{
		private static ResourceLoader resourceLoader;
		public static string GetString(string resourceName)
		{
			if (Localization.resourceLoader == null)
			{
                Localization.resourceLoader = ResourceLoader.GetForCurrentView("Resources"); // new ResourceLoader("Resources");
			}
			return Localization.resourceLoader.GetString(resourceName);
		}
	}
}
