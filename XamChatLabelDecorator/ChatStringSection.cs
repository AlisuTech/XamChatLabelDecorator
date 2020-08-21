using System;
using System.Collections.Generic;
using System.Text;

namespace XamChatLabelDecorator
{
    internal class ChatStringSection
    {
        public ChatStringSection()
        {
            DecorationSections = new List<DecorationSection>();
        }
        public string ActualText { get; set; }
        public string RawText { get; set; }
        public int Index { get; set; }
        public int Length { get { return ActualText.Length; } }
        public string Link { get; set; }
        public SpanTypeEnum SpanType { get; set; }
        public IList<DecorationSection> DecorationSections { get; set; }
    }
}
