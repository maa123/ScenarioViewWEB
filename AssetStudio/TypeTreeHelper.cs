using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AssetStudio
{
    public static class TypeTreeHelper
    {
        public static void ReadTypeString(StringBuilder sb, List<TypeTreeNode> members, BinaryReader reader)
        {
            var jo = new Dictionary<string, object>();
            for (int i = 0; i < members.Count; i++)
            {
                ReadStringValue(ref jo, sb, members, reader, ref i);
            }
            //sb.AppendLine(JsonConvert.SerializeObject(jo, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
            sb.AppendLine(JObject.FromObject(jo).ToString());
        }

        private static void ReadStringValue(ref Dictionary<string, object> jo, StringBuilder sb, List<TypeTreeNode> members, BinaryReader reader, ref int i)
        {
            var member = members[i];
            var level = member.m_Level;
            var varTypeStr = member.m_Type;
            var varNameStr = member.m_Name;
            object value = null;
            var append = true;
            var align = (member.m_MetaFlag & 0x4000) != 0;
            switch (varTypeStr)
            {
                case "SInt8":
                    value = reader.ReadSByte();
                    break;
                case "UInt8":
                case "char":
                    value = reader.ReadByte();
                    break;
                case "short":
                case "SInt16":
                    value = reader.ReadInt16();
                    break;
                case "UInt16":
                case "unsigned short":
                    value = reader.ReadUInt16();
                    break;
                case "int":
                case "SInt32":
                    value = reader.ReadInt32();
                    break;
                case "UInt32":
                case "unsigned int":
                case "Type*":
                    value = reader.ReadUInt32();
                    break;
                case "long long":
                case "SInt64":
                    value = reader.ReadInt64();
                    break;
                case "UInt64":
                case "unsigned long long":
                case "FileSize":
                    value = reader.ReadUInt64();
                    break;
                case "float":
                    value = reader.ReadSingle();
                    break;
                case "double":
                    value = reader.ReadDouble();
                    break;
                case "bool":
                    value = reader.ReadBoolean();
                    break;
                case "string":
                    append = false;
                    var str = reader.ReadAlignedString();
                    jo[varNameStr] =  str;
                    i += 3;
                    break;
                case "map":
                    {
                        if ((members[i + 1].m_MetaFlag & 0x4000) != 0)
                            align = true;
                        append = false;
                        var size = reader.ReadInt32();
                        var map = GetMembers(members, i);
                        i += map.Count - 1;
                        var first = GetMembers(map, 4);
                        var next = 4 + first.Count;
                        var second = GetMembers(map, next);
                        Dictionary<string, object>[] nmap = new Dictionary<string, object>[size * 2];
                        for (int j = 0; j < size; j++)
                        {
                            int tmp1 = 0;
                            int tmp2 = 0;
                            nmap[j] = new Dictionary<string, object>();
                            nmap[j + 1] = new Dictionary<string, object>();
                            ReadStringValue(ref nmap[j], sb, first, reader, ref tmp1);
                            ReadStringValue(ref nmap[j + 1], sb, second, reader, ref tmp2);
                        }
                        nmap = nmap.Where(c => c != null).ToArray();
                        jo[varNameStr] = nmap;
                        break;
                    }
                case "TypelessData":
                    {
                        append = false;
                        var size = reader.ReadInt32();
                        reader.ReadBytes(size);
                        i += 2;
                        jo[varNameStr] = size;
                        break;
                    }
                default:
                    {
                        if (i < members.Count - 1 && members[i + 1].m_Type == "Array") //Array
                        {
                            if ((members[i + 1].m_MetaFlag & 0x4000) != 0)
                                align = true;
                            append = false;
                            var size = reader.ReadInt32();
                            Dictionary<string, object>[] arr = new Dictionary<string, object>[size];
                            var vector = GetMembers(members, i);
                            i += vector.Count - 1;
                            for (int j = 0; j < size; j++)
                            {
                                int tmp = 3;
                                arr[j] = new Dictionary<string, object>();
                                ReadStringValue(ref arr[j], sb, vector, reader, ref tmp);
                            }
                            arr = arr.Where(c => c != null).ToArray();
                            jo[varNameStr] = arr;
                            break;
                        }
                        else //Class
                        {
                            append = false;
                            var @class = GetMembers(members, i);
                            i += @class.Count - 1;
                            Dictionary<string, object>[] cls = new Dictionary<string, object>[@class.Count - 1];
                            for (int j = 1; j < @class.Count; j++)
                            {
                                cls[j - 1] = new Dictionary<string, object>();
                                ReadStringValue(ref cls[j - 1], sb, @class, reader, ref j);
                            }
                            cls = cls.Where(c => c != null).ToArray();
                            jo[varNameStr] = cls;
                            break;
                        }
                    }
            }
            if (append && value != null)
                jo[varNameStr] = value;
            if (align)
                reader.AlignStream();
        }

        public static UType ReadUType(List<TypeTreeNode> members, BinaryReader reader)
        {
            var obj = new UType();
            for (int i = 1; i < members.Count; i++)
            {
                var member = members[i];
                var varNameStr = member.m_Name;
                obj[varNameStr] = ReadValue(members, reader, ref i);
            }
            return obj;
        }

        private static object ReadValue(List<TypeTreeNode> members, BinaryReader reader, ref int i)
        {
            var member = members[i];
            var varTypeStr = member.m_Type;
            object value;
            var align = (member.m_MetaFlag & 0x4000) != 0;
            switch (varTypeStr)
            {
                case "SInt8":
                    value = reader.ReadSByte();
                    break;
                case "UInt8":
                case "char":
                    value = reader.ReadByte();
                    break;
                case "short":
                case "SInt16":
                    value = reader.ReadInt16();
                    break;
                case "UInt16":
                case "unsigned short":
                    value = reader.ReadUInt16();
                    break;
                case "int":
                case "SInt32":
                    value = reader.ReadInt32();
                    break;
                case "UInt32":
                case "unsigned int":
                case "Type*":
                    value = reader.ReadUInt32();
                    break;
                case "long long":
                case "SInt64":
                    value = reader.ReadInt64();
                    break;
                case "UInt64":
                case "unsigned long long":
                case "FileSize":
                    value = reader.ReadUInt64();
                    break;
                case "float":
                    value = reader.ReadSingle();
                    break;
                case "double":
                    value = reader.ReadDouble();
                    break;
                case "bool":
                    value = reader.ReadBoolean();
                    break;
                case "string":
                    value = reader.ReadAlignedString();
                    i += 3;
                    break;
                case "map":
                    {
                        if ((members[i + 1].m_MetaFlag & 0x4000) != 0)
                            align = true;
                        var map = GetMembers(members, i);
                        i += map.Count - 1;
                        var first = GetMembers(map, 4);
                        var next = 4 + first.Count;
                        var second = GetMembers(map, next);
                        var size = reader.ReadInt32();
                        var dic = new List<KeyValuePair<object, object>>(size);
                        for (int j = 0; j < size; j++)
                        {
                            int tmp1 = 0;
                            int tmp2 = 0;
                            dic.Add(new KeyValuePair<object, object>(ReadValue(first, reader, ref tmp1), ReadValue(second, reader, ref tmp2)));
                        }
                        value = dic;
                        break;
                    }
                case "TypelessData":
                    {
                        var size = reader.ReadInt32();
                        value = reader.ReadBytes(size);
                        i += 2;
                        break;
                    }
                default:
                    {
                        if (i < members.Count - 1 && members[i + 1].m_Type == "Array") //Array
                        {
                            if ((members[i + 1].m_MetaFlag & 0x4000) != 0)
                                align = true;
                            var vector = GetMembers(members, i);
                            i += vector.Count - 1;
                            var size = reader.ReadInt32();
                            var list = new List<object>(size);
                            for (int j = 0; j < size; j++)
                            {
                                int tmp = 3;
                                list.Add(ReadValue(vector, reader, ref tmp));
                            }
                            value = list;
                            break;
                        }
                        else //Class
                        {
                            var @class = GetMembers(members, i);
                            i += @class.Count - 1;
                            var obj = new UType();
                            for (int j = 1; j < @class.Count; j++)
                            {
                                var classmember = @class[j];
                                var name = classmember.m_Name;
                                obj[name] = ReadValue(@class, reader, ref j);
                            }
                            value = obj;
                            break;
                        }
                    }
            }
            if (align)
                reader.AlignStream();
            return value;
        }

        private static List<TypeTreeNode> GetMembers(List<TypeTreeNode> members, int index)
        {
            var member2 = new List<TypeTreeNode>();
            member2.Add(members[index]);
            var level = members[index].m_Level;
            for (int i = index + 1; i < members.Count; i++)
            {
                var member = members[i];
                var level2 = member.m_Level;
                if (level2 <= level)
                {
                    return member2;
                }
                member2.Add(member);
            }
            return member2;
        }
    }
}
