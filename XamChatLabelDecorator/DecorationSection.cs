using System;
using System.Collections.Generic;
using System.Text;

namespace XamChatLabelDecorator
{
    internal class DecorationSection
    {
        public int Index { get; set; }
        public int Length { get; set; }
        public int LastItemIndex { get { return Index + Length; } }
        public SpanTypeEnum SpanType { get; set; }
    }
}
