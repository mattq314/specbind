namespace SpecBind.Configuration
{
	using System.Configuration;

	/// <summary>
	/// A configuration element for the pages collection.
	/// </summary>
	[ConfigurationCollection(typeof(PageElement))]
	public class PageCollection : ConfigurationElementCollection
	{
		/// <inheritdoc/>
		protected override ConfigurationElement CreateNewElement()
		{
			return new PageElement();
		}

		/// <inheritdoc/>
		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((PageElement)element).Url;
		}
	}
}
