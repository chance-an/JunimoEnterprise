#pragma once
#using <System.Xml.dll>

using namespace System;
using namespace System::Collections::Generic;
using namespace System::Xml::Serialization;

namespace JunimoEnterpriseNative {
    generic <class TKey, class TValue>
    public ref class SerializableGenericDictionary : public Dictionary<TKey, TValue>, public IXmlSerializable
    {
    public:
        SerializableGenericDictionary();
        ~SerializableGenericDictionary();

        virtual System::Xml::Schema::XmlSchema^ GetSchema() {
            return nullptr;
        }

        virtual void ReadXml(System::Xml::XmlReader^ reader);
        virtual void WriteXml(System::Xml::XmlWriter^ writer);
    };

}



