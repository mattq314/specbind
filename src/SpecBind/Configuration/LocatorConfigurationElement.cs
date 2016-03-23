using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecBind.Configuration
{
	using System.Configuration;

	/// <summary>
	/// A configuration element for a page.
	/// </summary>
	public class LocatorConfigurationElement : ConfigurationElement
	{
		private const string TypeElementName = @"type";

		/// <summary>
		/// Gets or sets the URL of a page to be tested.
		/// </summary>
		/// <value>The URL of a page to be tested.</value>
		[ConfigurationProperty(TypeElementName, DefaultValue = "id", IsRequired = true)]
		public string Type
		{
			get
			{
				return (string)this[TypeElementName];
			}

			set
			{
				this[TypeElementName] = value;
			}
		}

	}
}
