using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace XamChatLabelDecorator
{
    internal static class LabelDecoratorService
    {
        const string spanPatternBold = @"\*.*?\*";
        const string spanPatternItalic = @"_.*?_";
        const string spanPatternStrikeThrough = @"~.*?~";
        const string spanPatternColourTag = @"%%.*?%%";
        const string spanPatternUnderlineTag = @"/-.*?-/";
        const string spanPatternFontsizeTag = @"##.*?##";

        private static readonly IDictionary<string, SpanTypeEnum> SpanTypeDict = new Dictionary<string, SpanTypeEnum>(new List<KeyValuePair<string, SpanTypeEnum>>()
        {
            new KeyValuePair<string, SpanTypeEnum>("*", SpanTypeEnum.Bold),
            new KeyValuePair<string, SpanTypeEnum>("_", SpanTypeEnum.Italic),
            new KeyValuePair<string, SpanTypeEnum>("~", SpanTypeEnum.Strikethrough),
            new KeyValuePair<string, SpanTypeEnum>("%%", SpanTypeEnum.ColouredTag),
            new KeyValuePair<string, SpanTypeEnum>("/-", SpanTypeEnum.Underline),
            new KeyValuePair<string, SpanTypeEnum>("-/", SpanTypeEnum.Underline),
            new KeyValuePair<string, SpanTypeEnum>("##", SpanTypeEnum.FontSize),
        });

        private static readonly IDictionary<SpanTypeEnum, string> SpanPatternDict = new Dictionary<SpanTypeEnum, string>(new List<KeyValuePair<SpanTypeEnum, string>>()
        {
            new KeyValuePair<SpanTypeEnum, string>(SpanTypeEnum.Bold,spanPatternBold),
            new KeyValuePair<SpanTypeEnum, string>(SpanTypeEnum.Italic,spanPatternItalic),
            new KeyValuePair<SpanTypeEnum, string>(SpanTypeEnum.Strikethrough,spanPatternStrikeThrough),
            new KeyValuePair<SpanTypeEnum, string>(SpanTypeEnum.ColouredTag,spanPatternColourTag),
            new KeyValuePair<SpanTypeEnum, string>(SpanTypeEnum.Underline,spanPatternUnderlineTag),
            new KeyValuePair<SpanTypeEnum, string>(SpanTypeEnum.FontSize,spanPatternFontsizeTag),
        });

        public static SpanTypeEnum GetSpanType(string charType)
        {
            if (SpanTypeDict.Keys.Contains(charType[0].ToString()))
            {
                return SpanTypeDict[charType[0].ToString()];
            }
            else if (SpanTypeDict.Keys.Contains(charType.Substring(0,2)))
            {
                return SpanTypeDict[charType.Substring(0,2)];
            }
            return SpanTypeEnum.None;
        }

        public static string GetSpanPattern(SpanTypeEnum spanType)
        {
            return SpanPatternDict[spanType];
        }

        public static IEnumerable<MatchCollectionValue> GetMatchCollectionValues(MatchCollection matches, string rawText)
        {
            var matchValues = new List<MatchCollectionValue>();

            if (matches == null)
                return matchValues;

            foreach (Match item in matches)
            {
                var index = rawText.IndexOf(item.Value);
                matchValues.Add(new MatchCollectionValue() { MatchValue = item.Value, StartIndex = index });
            }
            return matchValues;
        }

        public static IEnumerable<MatchCollectionValue> ConcatenateCollection(List<List<MatchCollectionValue>> allcollection)
        {
            var matchValues = new List<MatchCollectionValue>();

            if (allcollection == null)
                return matchValues;

            foreach (var item in allcollection)
            {
                matchValues.Concat(item);
            }
            var orderedMatch = matchValues.OrderBy(x => x.StartIndex);
            return orderedMatch;
        }

        public static IEnumerable<MatchCollectionValue> ConcatenateCollection(List<MatchCollection> allcollection, string rawText)
        {
            IEnumerable<MatchCollectionValue> matchValues = new List<MatchCollectionValue>();

            if (allcollection == null)
                return matchValues;

            foreach (var item in allcollection)
            {
                var matchCollection = GetMatchCollectionValues(item, rawText);
                matchValues = matchValues.Concat(matchCollection);
            }
            var orderedMatch = matchValues.OrderBy(x => x.StartIndex);
            return orderedMatch;
        }

        public static string GetActualText(string rawText)
        {
            string[] allCharacter = SpanTypeDict.Select(x => x.Key).ToArray();
            foreach (var item in allCharacter)
            {
                if (rawText.Contains(item))
                {
                    rawText = rawText.Replace(item, "");
                }
            }
            return rawText;
        }
        public static string GetActualText(string rawText, SpanTypeEnum spanType)
        {
            string[] allCharacter = SpanTypeDict.Where(x => x.Value==spanType).Select(x=>x.Key).ToArray();
            foreach (var item in allCharacter)
            {
                if (rawText.Contains(item))
                {
                    rawText = rawText.Replace(item, "");
                }
            }
            return rawText;
        }
        public static string GetActualText(string rawText, IEnumerable<SpanTypeEnum> spanTypes)
        {
            string[] allCharacter = SpanTypeDict.Where(x => spanTypes.Contains(x.Value)).Select(x => x.Key).ToArray();
            foreach (var item in allCharacter)
            {
                if (rawText.Contains(item))
                {
                    rawText = rawText.Replace(item, "");
                }
            }
            return rawText;
        }
    }
}
