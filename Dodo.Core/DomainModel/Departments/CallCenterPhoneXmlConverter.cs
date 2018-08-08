﻿using System;
using System.Linq;
using System.Xml.Linq;

namespace Dodo.Core.DomainModel.Departments
{
	public static class CallCenterPhoneXmlConverter
	{
		public static XElement CreateXmlNode(CallCenterPhone phone)
		{
			return new XElement("Phone",
				new XAttribute("number", phone.Number ?? String.Empty),
				new XAttribute("iconPath", phone.Icon.Path ?? String.Empty),
				new XAttribute("iconSitePath", phone.Icon.SitePath ?? String.Empty));
		}

		public static CallCenterPhone[] GetCallCenterPhones(this XElement container)
		{
			var phns = container.Element("CallCenterPhones");
			if(phns==null)
				return new CallCenterPhone[0];

			return phns.Elements().Select(ParseCallCenterPhone).ToArray();
		}

		private static CallCenterPhone ParseCallCenterPhone(XElement element)
		{
			return new CallCenterPhone
			{
				Number = GetElement(element, "number"),
				Icon = new Icon(GetElement(element, "iconPath"), GetElement(element, "iconSitePath"))
			};
		}

		private static string GetElement(XElement element, string attribute)
		{
			return element.Attribute(attribute)?.Value ?? "";
		}
	}
}