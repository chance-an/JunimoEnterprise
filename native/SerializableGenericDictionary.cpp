#include "stdafx.h"
#include "SerializableGenericDictionary.h"

using namespace JunimoEnterpriseNative;

generic <class TKey, class TValue>
SerializableGenericDictionary<TKey, TValue>::SerializableGenericDictionary()
{
}

generic <class TKey, class TValue>
SerializableGenericDictionary<TKey, TValue>::~SerializableGenericDictionary()
{
}

generic <class TKey, class TValue>
void SerializableGenericDictionary<TKey, TValue>::ReadXml(System::Xml::XmlReader^ reader)
{
    array<Type^>^ extraTypes = gcnew array<Type^>(additionalSerializeTypes->Count);
    additionalSerializeTypes->CopyTo(extraTypes, 0);

    XmlSerializer^ keySerializer = gcnew XmlSerializer(TKey::typeid);
    XmlSerializer^ valueSerializer = gcnew XmlSerializer(TValue::typeid, extraTypes);

    bool wasEmpty = reader->IsEmptyElement;
    reader->Read();

    if (wasEmpty)
        return;

    while (reader->NodeType != System::Xml::XmlNodeType::EndElement)
    {
        reader->ReadStartElement("item");
        reader->ReadStartElement("key");
        TKey key = (TKey)keySerializer->Deserialize(reader);
        reader->ReadEndElement();
        reader->ReadStartElement("value");

        TValue value = (TValue)valueSerializer->Deserialize(reader);

        reader->ReadEndElement();
        this->Add(key, value);

        reader->ReadEndElement();
        reader->MoveToContent();
    }

    reader->ReadEndElement();
}

generic <class TKey, class TValue>
void SerializableGenericDictionary<TKey, TValue>::WriteXml(System::Xml::XmlWriter^ writer)
{
    array<Type^>^ extraTypes = gcnew array<Type^>(additionalSerializeTypes->Count);
    additionalSerializeTypes->CopyTo(extraTypes, 0);

    XmlSerializer^ keySerializer = gcnew XmlSerializer(TKey::typeid);
    XmlSerializer^ valueSerializer = gcnew XmlSerializer(TValue::typeid, extraTypes);

    for each(TKey key in this->Keys)
    {
        writer->WriteStartElement("item");
        writer->WriteStartElement("key");

        keySerializer->Serialize(writer, key);

        writer->WriteEndElement();
        writer->WriteStartElement("value");
        TValue value = this[key];
        valueSerializer->Serialize(writer, value);
        writer->WriteEndElement();
        writer->WriteEndElement();
    }
}
