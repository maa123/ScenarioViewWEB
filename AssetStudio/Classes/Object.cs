﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssetStudio
{
    public class Object
    {
        public SerializedFile assetsFile;
        public ObjectReader reader;
        public long m_PathID;
        public int[] version;
        protected BuildType buildType;
        public BuildTarget platform;
        public ClassIDType type;
        public SerializedType serializedType;
        public uint byteSize;

        public Object(ObjectReader reader)
        {
            this.reader = reader;
            reader.Reset();
            assetsFile = reader.assetsFile;
            type = reader.type;
            m_PathID = reader.m_PathID;
            version = reader.version;
            buildType = reader.buildType;
            platform = reader.platform;
            serializedType = reader.serializedType;
            byteSize = reader.byteSize;

            if (platform == BuildTarget.NoTarget)
            {
                var m_ObjectHideFlags = reader.ReadUInt32();
            }
        }

        protected bool HasStructMember(string name)
        {
            return serializedType?.m_Nodes != null && serializedType.m_Nodes.Any(x => x.m_Name == name);
        }

        public string Dump()
        {
            reader.Reset();
            if (serializedType?.m_Nodes != null)
            {
                var sb = new StringBuilder();
                TypeTreeHelper.ReadTypeString(sb, serializedType.m_Nodes, reader);
                return sb.ToString();
            }
            return null;
        }
        public string DumpText() {
            reader.Reset();
            if (serializedType?.m_Nodes != null) {
                var sb = new StringBuilder();
                TypeTreeHelperText.ReadTypeString(sb, serializedType.m_Nodes, reader);
                return sb.ToString();
            }
            return null;
        }

        public byte[] GetRawData()
        {
            reader.Reset();
            return reader.ReadBytes((int)byteSize);
        }
    }
}
