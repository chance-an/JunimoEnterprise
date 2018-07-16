#pragma once
#pragma managed

namespace JunimoEnterpriseNative {

    generic <class TypeParameter1, class TypeParameter2>
    public ref class SerializableTuple
    {
    public:
        property TypeParameter1 Item1;
        property TypeParameter2 Item2;

        SerializableTuple()
        {

        }
        
        SerializableTuple(TypeParameter1 value1, TypeParameter2 value2)
        {
            Item1 = value1;
            Item2 = value2;
        }

        virtual inline int GetHashCode() override {
            int hash = 17;
            hash = hash * 23 + Item1->GetHashCode();
            hash = hash * 23 + Item2->GetHashCode();
            return hash;
        }

        virtual bool Equals(Object^ other) override {
            SerializableTuple<TypeParameter1, TypeParameter2>^ o
                = (SerializableTuple<TypeParameter1, TypeParameter2>^)other;
            return o->Item1->Equals(this->Item1) && o->Item2->Equals(this->Item2);
        }
    };

};
