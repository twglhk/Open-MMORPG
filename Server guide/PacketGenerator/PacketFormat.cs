﻿using System;
using System.Collections.Generic;
using System.Text;

namespace PacketGenerator
{
    class PacketFormat
    {
        // {0} Packet name / number list
        // {1} Packet Classes list
        public static string FileFormat =
@"using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using ServerCore;

public enum PacketID
{{
    {0}
}}

{1}
";
        // {0} : Packet name
        // {1} : Packet number
        public static string packetEnumFormat =
@"{0} = {1},";

        
        // {0} : packet name
        // {1} : member
        // {2} : read member
        // {3} : write member

        public static string packetFormat =
@"
class {0}
{{
    {1}

    public void Read(ArraySegment<byte> segment)
    {{
        ushort count = 0;

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        
        {2}
    }}

    public ArraySegment<byte> Write()
    {{
        ArraySegment<byte> sendBufferSegment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;
        Span<byte> s = new Span<byte>(sendBufferSegment.Array, sendBufferSegment.Offset,
            sendBufferSegment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.{0});
        count += sizeof(ushort);
        
        {3}

        success &= BitConverter.TryWriteBytes(s, count);    // Add Packet Size

        if (success == false) return null;

        return SendBufferHelper.Close(count);
    }}
}}
";

        // SkillInfo
        // {0} : member type
        // {1} : member name
        public static string memberFormat =
@"public {0} {1};";

        // {0} : list name [Upper case]
        // {1} : list name [Loser case]
        // {2} : member
        // {2} : read member
        // {3} : write member
        public static string memberListFormat =
@"
public List<{0}> {1}s = new List<{0}>();
public struct {0}
{{
    {2}

    public void Read(ReadOnlySpan<byte> s, ref ushort count)
    {{
        {3}
    }}

    public bool Write(Span<byte> s, ref ushort count)
    {{
        bool success = true;
        {4}
        return success;
    }}
}}";

        // {0} : member name
        // {1} : To~data type
        // {2} : member type
        public static string readFormat =
@"this.{0} = BitConverter.{1}(s.Slice(count, s.Length - count));
count += sizeof({2});
";

        // {0} : member name
        // {1} : member type
        public static string readByteFormat =
@"this.{0} = ({1})segment.Array[segment.Offset + count];
count += sizeof({1});";

        // {0} : member name
        public static string readStringFormat =
@"ushort {0}Len = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
count += sizeof(ushort);
this.{0} = Encoding.Unicode.GetString(s.Slice(count, {0}Len));
count += {0}Len;";

        // {0} : list name [Upper case]
        // {1} : list name [lower case]
        public static string readListFormat =
@"this.{1}s.Clear();
ushort {1}Len = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
count += sizeof(ushort);
for (int i = 0; i < {1}Len; ++i)
{{
    {0} {1} = new {0}();
    {1}.Read(s, ref count);
    {1}s.Add({1});
}}";

        // {0} : member name
        // {1} : member type
        public static string writeFormat =
@"success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.{0});
count += sizeof({1});";

        // {0} : member name
        // {1} : member type
        public static string writeByteFormat =
@"segment.Array[segment.Offset + count] = (byte)this.{0};
count += sizeof({1});";

        // {0} : member name
        public static string writeStringFormat =
@"ushort {0}Len = (ushort)Encoding.Unicode.GetBytes(this.{0}, 0, {0}.Length,
sendBufferSegment.Array, sendBufferSegment.Offset + count + sizeof(ushort));
success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), {0}Len);
count += sizeof(ushort);
count += {0}Len;";

        // {0} : list name [Upper case]
        // {1} : list name [lower case]
        public static string writeListFormat =
@"success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)this.{1}s.Count);
count += sizeof(ushort);
foreach({0} {1} in this.{1}s)
{{
    success &= {1}.Write(s, ref count);
}}";
    }
}
