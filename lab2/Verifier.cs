using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using Newtonsoft.Json.Schema.Generation;

namespace lab2
{
	class Verifier
	{
		private static string jsonSchema = @"{'type': 'array','items': {'type': ['object','null'],'properties': 
			{'Name': {'type': ['string','null']},'Age': {'type': ['integer','null']},'Country': {'type': ['string','null']}}}}";
		
		private static string xmlSchema2 =
			@"<xs:schema attributeFormDefault=""unqualified"" elementFormDefault=""qualified"" xmlns:xs=""http://www.w3.org/2001/XMLSchema"">
  <xs:element name=""ArrayOfPerson"" type=""ArrayOfPersonType""/>
  <xs:complexType name=""PersonType"">
    <xs:sequence>
      <xs:element type=""xs:string"" name=""Name""/>
      <xs:element type=""xs:byte"" name=""Age""/>
      <xs:element type=""xs:string"" name=""Country""/>
    </xs:sequence>
  </xs:complexType>
  <xs:complexType name=""ArrayOfPersonType"">
    <xs:sequence>
      <xs:element type=""PersonType"" name=""Person"" maxOccurs=""unbounded"" minOccurs=""0""/>
    </xs:sequence>
  </xs:complexType>
</xs:schema>";

		public static bool VerifyXml(string data)
		{
			bool isValid = true;
			try
			{
				XDocument xmlDoc = XDocument.Parse(data);
				XmlSchemaSet schemaSet = new XmlSchemaSet();
				schemaSet.Add(XmlSchema.Read(new StringReader(xmlSchema2), null));

				XmlReaderSettings xrs = new XmlReaderSettings();
				xrs.ValidationType = ValidationType.Schema;
				xrs.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
				xrs.Schemas = schemaSet;
				xrs.ValidationEventHandler += (o, s) =>
				{
					Console.WriteLine("{0}: {1}", s.Severity, s.Message);
					isValid = false;
				};

				using (XmlReader xr = XmlReader.Create(xmlDoc.CreateReader(), xrs))
				{
					while (xr.Read()) { }
				}
			}
			catch (Exception e)
			{
				return false;
			}
			
			return isValid;
		}

		public static bool VerifyJson(string data)
		{
			try
			{
				JsonSchema schema = JsonSchema.Parse(jsonSchema);

				JArray vData = JArray.Parse(data);
				return vData != null && (schema != null && vData.IsValid(schema));
			}
			catch (Exception e)
			{
				return false;
			}
			
		}
	}
}
