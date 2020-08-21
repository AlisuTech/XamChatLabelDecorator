using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace XamChatLabelDecorator
{
    internal class ChatLabelDecoratorProcessor
    {
        public IList<ChatStringSection> ProcessString(string rawText)
        {
            var sections = new List<ChatStringSection>();
            var allSections = new List<ChatStringSection>();
            if (rawText != null)
            {
                var lastIndex = 0;

                MatchCollection boldCollections = Regex.Matches(rawText, LabelDecoratorService.GetSpanPattern(SpanTypeEnum.Bold), RegexOptions.Singleline);
                MatchCollection italicConnections = Regex.Matches(rawText, LabelDecoratorService.GetSpanPattern(SpanTypeEnum.Italic), RegexOptions.Singleline);
                IList<string> webcollections = new List<string>();
                MatchCollection strikethroughCollections = Regex.Matches(rawText, LabelDecoratorService.GetSpanPattern(SpanTypeEnum.Strikethrough), RegexOptions.Singleline);
                MatchCollection colourTagCollection = Regex.Matches(rawText, LabelDecoratorService.GetSpanPattern(SpanTypeEnum.ColouredTag), RegexOptions.Singleline);
                MatchCollection underlineTagCollection = Regex.Matches(rawText, LabelDecoratorService.GetSpanPattern(SpanTypeEnum.Underline), RegexOptions.Singleline);
                MatchCollection fontsizeTagCollection = Regex.Matches(rawText, LabelDecoratorService.GetSpanPattern(SpanTypeEnum.FontSize), RegexOptions.Singleline);

                foreach (var item in rawText?.Split(' '))
                {
                    if (IsUrlValid(item))
                    {
                        webcollections.Add(item);
                    }
                }

                foreach (var item in webcollections)
                {
                    var chatstringsection = new ChatStringSection() { ActualText = item, RawText = item, SpanType = SpanTypeEnum.Web, Index = rawText.IndexOf(item) };
                    chatstringsection.DecorationSections.Add(new DecorationSection()
                    {
                        SpanType = SpanTypeEnum.Web,
                        Index = chatstringsection.Index,
                        Length = chatstringsection.Length
                    });
                    sections.Add(chatstringsection);
                }


                var allMatchCollectionValue = LabelDecoratorService.ConcatenateCollection(
                                                new List<MatchCollection>()
                                                {
                                                boldCollections,
                                                italicConnections,
                                                strikethroughCollections,
                                                colourTagCollection,
                                                underlineTagCollection,
                                                    fontsizeTagCollection
                                                }, rawText);

                foreach (var item in allMatchCollectionValue)
                {
                    var foundText = item.MatchValue;
                    var itemIndex = item.StartIndex;
                    var spanText = rawText.Substring(itemIndex,2);
                    var spanType = LabelDecoratorService.GetSpanType(spanText);
                    var chatstringsection = new ChatStringSection() { ActualText = foundText, RawText = item.MatchValue, SpanType = spanType, Index = itemIndex };
                    var multiSection = sections.FirstOrDefault(x => (x.Index + x.Length) >= itemIndex && x.Index <= itemIndex);
                    if (multiSection != null)
                    {
                        var lastmultisectionIndex = multiSection.Index + multiSection.Length;
                        var currentSectionLastIndex = itemIndex + chatstringsection.Length;
                        var decorationLength = lastmultisectionIndex > currentSectionLastIndex ? currentSectionLastIndex : lastmultisectionIndex;
                        var decorationSection = new DecorationSection()
                        {
                            Index = itemIndex,
                            Length = decorationLength - itemIndex,
                            SpanType = spanType
                        };
                        multiSection.DecorationSections.Add(decorationSection);
                        if (lastmultisectionIndex < currentSectionLastIndex)
                        {
                            var currentDecorationSection = new DecorationSection()
                            {
                                Index = lastmultisectionIndex,
                                Length = currentSectionLastIndex - lastmultisectionIndex,
                                SpanType = spanType
                            };
                            var startindexsubstring = currentDecorationSection.Index - chatstringsection.Index;

                            chatstringsection = new ChatStringSection() { ActualText = foundText.Substring(startindexsubstring), RawText = spanText, SpanType = spanType, Index = currentDecorationSection.Index };

                            chatstringsection.DecorationSections.Add(currentDecorationSection);
                            sections.Add(chatstringsection);
                        }
                    }
                    else
                    {
                        chatstringsection.DecorationSections.Add(new DecorationSection()
                        {
                            SpanType = spanType,
                            Index = chatstringsection.Index,
                            Length = chatstringsection.Length
                        });
                        sections.Add(chatstringsection);
                    }

                }


                var actualSection = new List<ChatStringSection>();
                var orderedSection = sections.OrderBy(x => x.Index);
                foreach (var item in orderedSection)
                {
                    var textLength = item.Index - lastIndex;
                    if (textLength > 0)
                    {
                        var subsection = rawText.Substring(lastIndex, textLength);
                        if (!string.IsNullOrEmpty(subsection))
                        {
                            actualSection.Add(new ChatStringSection() { ActualText = subsection, SpanType = SpanTypeEnum.None, Index = lastIndex });
                        }
                        lastIndex = item.Index + item.Length;
                    }
                }

                if (lastIndex + 1 < rawText.Length && !string.IsNullOrWhiteSpace(rawText.Substring(lastIndex + 1)))
                {
                    actualSection.Add(new ChatStringSection() { ActualText = rawText.Substring(lastIndex), SpanType = SpanTypeEnum.None, Index = lastIndex });
                }

                allSections.AddRange(sections);
                allSections.AddRange(actualSection);
            }
            var mainSection = allSections.OrderBy(x => x.Index).ToList();
            return mainSection;
        }

        private static bool IsUrlValid(string url)
        {

            string pattern = @"^(http|https|ftp|)\://|[a-zA-Z0-9\-\.]+\.[a-zA-Z](:[a-zA-Z0-9]*)?/?([a-zA-Z0-9\-\._\?\,\'/\\\+&amp;%\$#\=~])*[^\.\,\)\(\s]$";
            Regex reg = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            return reg.IsMatch(url);
        }
    }

    public enum SpanTypeEnum { Bold, Italic, Strikethrough, Underline, Web, ColouredTag, FontSize, None }
}
