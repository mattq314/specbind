using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecBind.PropertyHandlers
{
	using System.Globalization;
	using System.Reflection;

	internal class ElementLocatorProperty : PropertyInfo
	{
		public ElementLocatorProperty(string name)
		{
			this.Name = name;
		}

		public override object[] GetCustomAttributes(bool inherit)
		{
			//throw new NotImplementedException();
			return null;
		}

		public override bool IsDefined(Type attributeType, bool inherit)
		{
			//throw new NotImplementedException();
			return true;
		}

		public override object GetValue(object obj, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
		{
			//throw new NotImplementedException();
			return null;
		}

		public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
		{
			//throw new NotImplementedException();
		}

		public override MethodInfo[] GetAccessors(bool nonPublic)
		{
			//throw new NotImplementedException();
			return null;
		}

		public override MethodInfo GetGetMethod(bool nonPublic)
		{
			//throw new NotImplementedException();
			return null;
		}

		public override MethodInfo GetSetMethod(bool nonPublic)
		{
			//throw new NotImplementedException();
			return null;
		}

		public override ParameterInfo[] GetIndexParameters()
		{
			//throw new NotImplementedException();
			return null;
		}

		public override string Name { get; }

		public override Type DeclaringType { get; }

		public override Type ReflectedType { get; }

		public override Type PropertyType { get; }

		public override PropertyAttributes Attributes { get; }

		public override bool CanRead { get; }

		public override bool CanWrite { get; }

		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			//throw new NotImplementedException();
			return null;
		}
	}
}
