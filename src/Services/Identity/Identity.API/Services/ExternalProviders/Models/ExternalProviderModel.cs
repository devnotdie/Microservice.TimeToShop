namespace Identity.API.Services.ExternalProviders.Models
{
	public class ExternalProviderModel
	{
		public string DisplayName { get; set; }

		public string AuthenticationScheme { get; set; }

		public string Icon => AuthenticationScheme switch
		{
			"Google" => "bi-google",
			_ => ""
		};
	}
}
