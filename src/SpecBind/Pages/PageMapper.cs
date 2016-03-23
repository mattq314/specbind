// <copyright file="PageMapper.cs">
//    Copyright © 2013 Dan Piessens  All rights reserved.
// </copyright>

namespace SpecBind.Pages
{
	using System;
	using System.Collections.Generic;
	using System.Configuration;
	using System.Linq;
	using System.Reflection;
	using System.Reflection.Emit;
	using System.Threading;

	using OpenQA.Selenium;

	using SpecBind.Configuration;
	using SpecBind.Helpers;
	using SpecBind.PropertyHandlers;

	/// <summary>
	/// A mapping class to process all the items.
	/// </summary>
	public class PageMapper : IPageMapper
	{
		private readonly Dictionary<string, Type> pageTypeCache;

		private static PageMapper instance;

		/// <summary>
		/// Initializes the <see cref="PageMapper" /> class.
		/// </summary>
		private PageMapper()
		{
			this.pageTypeCache = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
		}

		/// <summary>
		/// Gets the initialized <see cref="PageMapper" /> class.
		/// </summary>
		public static PageMapper Instance
		{
			get
			{
				return instance ?? (instance = new PageMapper());
			}
		}

		/// <summary>
		/// Gets the map count.
		/// </summary>
		/// <value>
		/// The map count.
		/// </value>
		public int MapCount
		{
			get
			{
				return this.pageTypeCache.Count;
			}
		}

		/// <summary>
		/// Gets the page type from the given name
		/// </summary>
		/// <param name="typeName">Name of the type.</param>
		/// <returns>The resolved type; otherwise <c>null</c>.</returns>
		public Type GetTypeFromName(string typeName)
		{
			if (string.IsNullOrWhiteSpace(typeName))
			{
				return null;
			}

			Type pageType;
			return this.pageTypeCache.TryGetValue(typeName.ToLookupKey(), out pageType) ? pageType : null;
		}

		/// <summary>
		/// Maps the loaded assemblies into the type mapper.
		/// </summary>
		/// <param name="pageBaseType">Type of the page base.</param>
		[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
		public void Initialize(Type pageBaseType)
		{
			// There are several blank catches to avoid loading bad asssemblies.
			try
			{
				foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic && !a.GlobalAssemblyCache))
				{
					try
					{
						// Map all public types.
						this.MapAssemblyTypes(assembly.GetExportedTypes(), pageBaseType);
					}
					catch (SystemException)
					{
					}
				}
			}
			catch (SystemException)
			{
			}

			var configSection = SettingHelper.GetConfigurationSection();

			foreach (var page in configSection.Pages)
			{
				var p = (PageElement)page;
				var key = Sanitize(p.Url);

				var type = CreateType(key + "Page", p.Url);
				this.pageTypeCache.Add(key, type);
			}
		}

		internal Type CreateType(string typeName, string url, IList<string> elementsById = null)
		{
			// from https://msdn.microsoft.com/en-us/library/system.reflection.emit.typebuilder.createtype(v=vs.110).aspx
			var asmName = new AssemblyName { Name = "SpecBind.Runtime" };
			var asmBuild = Thread.GetDomain().DefineDynamicAssembly(asmName, AssemblyBuilderAccess.RunAndSave);
			var modBuild = asmBuild.DefineDynamicModule("ModuleOne", asmName.Name + ".dll");
			var typeBuild = modBuild.DefineType(typeName, TypeAttributes.Public);
			var pageNavAttrType = typeof(PageNavigationAttribute);
			var paramOfTypeString = new[] { typeof(string) };
			var pageNavAttrCtor = pageNavAttrType.GetConstructor(paramOfTypeString);

			if (pageNavAttrCtor == null)
			{
				throw new ArgumentNullException("pageNavAttrCtor");
			}

			var attrBuilder = new CustomAttributeBuilder(pageNavAttrCtor, new object[] { url });
			typeBuild.SetCustomAttribute(attrBuilder);

			if (elementsById != null)
			{
				foreach (var propertyName in elementsById)
				{
					var propBuilder = typeBuild.DefineProperty(propertyName, PropertyAttributes.None, typeof(IWebElement), null);

					var elemLocAttrType = typeof(ElementLocatorAttribute);
					var elemLocAttrCtor = elemLocAttrType.GetConstructor(paramOfTypeString);

					if (elemLocAttrCtor == null)
					{
						throw new ArgumentNullException("elemLocAttrCtor");
					}

					//var locatorById = new ElementLocatorProperty("Id");
					//var locatorProperties = new PropertyInfo[] { locatorById };

					attrBuilder = new CustomAttributeBuilder(elemLocAttrCtor, new object[] { propertyName }/*, locatorProperties, new object[] { propertyName }*/);
					propBuilder.SetCustomAttribute(attrBuilder);
					//propBuilder.GetCustomAttribute<ElementLocatorAttribute>().Id = propertyName; // invoked member is not supported in a dynamic module
				}
			}

			var type = typeBuild.CreateType();

			//if (type != null)
			//{
				this.pageTypeCache.Remove(typeName);
			//}

			this.pageTypeCache.Add(typeName, type);
			return type;
		}

		/// <summary>
		/// Maps the assembly types.
		/// </summary>
		/// <param name="types">The types.</param>
		/// <param name="pageBaseType">Type of the page base.</param>
		internal void MapAssemblyTypes(IEnumerable<Type> types, Type pageBaseType)
		{
			foreach (var pageType in types.Where(t => t.IsClass &&  !t.IsAbstract
				&& (pageBaseType == null || (t.BaseType != null && pageBaseType.IsAssignableFrom(t.BaseType)))))
			{
				var initialName = pageType.Name;
				if (initialName.EndsWith("Page", StringComparison.InvariantCultureIgnoreCase))
				{
					initialName = initialName.Substring(0, initialName.Length - 4);
				}

				if (!this.pageTypeCache.ContainsKey(initialName))
				{
					this.pageTypeCache.Add(initialName, pageType);
				}

				foreach (var aliasAttribute in pageType.GetCustomAttributes(typeof(PageAliasAttribute), true).OfType<PageAliasAttribute>())
				{
					var key = aliasAttribute.Name;
					if (!this.pageTypeCache.ContainsKey(key))
					{
						this.pageTypeCache.Add(key, pageType);
					}
				}
			}
		}

		private static string Sanitize(string toSanitize)
		{
			var validationMessage = "Argument is null or whitespace";
			if (string.IsNullOrWhiteSpace(toSanitize))
			{
				throw new ArgumentException(validationMessage, nameof(toSanitize));
			}

			var listToRemove = new List<string>() { "_", @"/" };

			toSanitize = listToRemove.Aggregate(toSanitize, (current, toRemove) => current.Replace(toRemove, string.Empty));

			if (string.IsNullOrWhiteSpace(toSanitize))
			{
				throw new ArgumentException(validationMessage, nameof(toSanitize));
			}

			var extensionsToDrop = new List<string> { ".aspx", ".asp", ".jsp" };

			foreach (var toDrop in extensionsToDrop.Where(toDrop => toSanitize.EndsWith(toDrop)))
			{
				toSanitize = toSanitize.Replace(toDrop, string.Empty);
			}

			return toSanitize;
		}
	}
}