using System.Xml.Serialization;
using System.Collections.Generic;
using System.Xml.Schema;
using System.Xml;

[XmlRoot("Counters")]
public class XmlSerializableCounters : Dictionary<string, int>, IXmlSerializable
{
    public XmlSchema GetSchema()
    {
        return null;
    }

    public void ReadXml(XmlReader reader)
    {
        while (reader.Read())
        {
            if (reader.HasAttributes)
            {
                string key = reader.GetAttribute("Key");
                int value = int.Parse(reader.GetAttribute("Value"));
                this.Add(key, value);
            }
            reader.Read();
        }
    }

    public void WriteXml(XmlWriter writer)
    {
        foreach(string key in this.Keys)
        {
            writer.WriteStartElement("Entry");
            writer.WriteAttributeString("Key", key);
            writer.WriteStartAttribute("Value");
            writer.WriteValue(this[key]);
            writer.WriteEndAttribute();
            writer.WriteEndElement();
        }
    }
}
