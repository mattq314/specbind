namespace SpecBind.Configuration
{
	using System.Configuration;

	/// <summary>
	/// A configuration element for a page.
	/// </summary>
	public class PageElement : ConfigurationElement
	{
		private const string UrlElementName = @"url";

		/// <summary>
		/// Gets or sets the URL of a page to be tested.
		/// </summary>
		/// <value>The URL of a page to be tested.</value>
		[ConfigurationProperty(UrlElementName, DefaultValue = "", IsRequired = true)]
		public string Url
		{
			get
			{
				return (string)this[UrlElementName];
			}

			set
			{
				this[UrlElementName] = value;
			}
		}

		//public override string ToString()
		//{
		//	return string.Format("{0} Url: {1}", GetType(), Url);
		//}
	}
}
