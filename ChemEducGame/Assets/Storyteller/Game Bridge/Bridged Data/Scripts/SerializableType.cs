//This script was created by Bunny83 

using System;
using System.IO;
using UnityEngine;
namespace DaiMangou.BridgedData
{
    [Serializable]
    public class SerializableType : ISerializationCallbackReceiver
    {
        public byte[] data;
        public Type type;

        public SerializableType(Type aType)
        {
            type = aType;
        }


        public void OnBeforeSerialize()
        {
            using (var stream = new MemoryStream())
            using (var w = new BinaryWriter(stream))
            {
                Write(w, type);
                data = stream.ToArray();
            }
        }

        public void OnAfterDeserialize()
        {
            using (var stream = new MemoryStream(data))
            using (var r = new BinaryReader(stream))
            {
                type = Read(r);
            }
        }

        public static Type Read(BinaryReader aReader)
        {
            var paramCount = aReader.ReadByte();
            if (paramCount == 0xFF)
                return null;
            var typeName = aReader.ReadString();
            var type = Type.GetType(typeName);
            if (type == null)
                throw new Exception("Can't find type; '" + typeName + "'");
            if (!type.IsGenericTypeDefinition || paramCount <= 0) return type;
            var p = new Type[paramCount];
            for (var i = 0; i < paramCount; i++) p[i] = Read(aReader);
            type = type.MakeGenericType(p);

            return type;
        }

        public static void Write(BinaryWriter aWriter, Type aType)
        {
            if (aType == null)
            {
                aWriter.Write((byte)0xFF);
                return;
            }

            if (aType.IsGenericType)
            {
                var t = aType.GetGenericTypeDefinition();
                var p = aType.GetGenericArguments();
                aWriter.Write((byte)p.Length);
                aWriter.Write(t.AssemblyQualifiedName);
                for (var i = 0; i < p.Length; i++) Write(aWriter, p[i]);
                return;
            }

            aWriter.Write((byte)0);
            aWriter.Write(aType.AssemblyQualifiedName);
        }
    }
}