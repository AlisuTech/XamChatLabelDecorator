using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace XamChatLabelDecorator
{
    internal class ChatLabelDecoratorEngine
    {
        public FormattedString GetFormattedString(string rawText)
        {
            var formatted = new FormattedString();
            var matchingProcessor = new ChatLabelDecoratorProcessor();
            var AllProcessedString = matchingProcessor.ProcessString(rawText);

            foreach (var item in AllProcessedString)
            {
                var allSpans = CreateSpan(item, rawText);
                foreach (var span in allSpans)
                {
                    if (item.SpanType != SpanTypeEnum.None && item.SpanType != SpanTypeEnum.Web) 
                    { 
                        var actualText = LabelDecoratorService.GetActualText(span.Text);
                        span.Text = actualText;
                    }
                    formatted.Spans.Add(span);
                }
            }

            return formatted;
        }

        private IEnumerable<Span> CreateSpan(ChatStringSection section, string rawText)
        {
            var allNewLine = section.ActualText.Split(@"\n");
            var text = string.Join(Environment.NewLine, allNewLine);
            var span = new Span()
            {
                Text = text,
                FontSize = 16
            };

            IList<Span> spans = new List<Span>();

            if (section.DecorationSections.Count <= 1)
            {
                SetSpan(section, span, section.SpanType);
                spans.Add(span);
            }
            else
            {
                var spanIndexes = new List<SpanTypeIndexes>();
                var decorSection = section.DecorationSections;

                for (int i = 0; i < section.DecorationSections.Count; i++)
                {
                    var decorSpans = new List<SpanTypeEnum>
                    {
                        decorSection[i].SpanType
                    };
                    var startIndex = decorSection[i].Index;
                    int endIndex;
                    if (i >= 1)
                    {
                        endIndex = decorSection[i].LastItemIndex < decorSection[i - 1].LastItemIndex ? decorSection[i].LastItemIndex : decorSection[i - 1].LastItemIndex;
                        for (int j = i; j > 0; j--)
                        {
                            if (decorSection[i].Index >= decorSection[i - 1].Index && decorSection[i].LastItemIndex <= decorSection[j - 1].LastItemIndex)
                            {
                                decorSpans.Add(decorSection[i - 1].SpanType);
                            }
                        }
                    }
                    else
                    {
                        endIndex = decorSection[i + 1].Index;
                    }

                    spanIndexes.Add(new SpanTypeIndexes()
                    {
                        StartIndex = startIndex,
                        EndIndex = endIndex,
                        Spans = decorSpans
                    });
                }

                foreach (var item in spanIndexes)
                {
                    var substring = rawText.Substring(item.StartIndex, item.Length);

                    span = new Span()
                    {
                        Text = substring
                    };

                    foreach (var eachSpan in item.Spans)
                    {
                        SetSpan(section, span, eachSpan);
                    }

                    if (item.Spans.Contains(SpanTypeEnum.Italic) && item.Spans.Contains(SpanTypeEnum.Bold))
                    {
                        span.FontAttributes = FontAttributes.Bold | FontAttributes.Italic;
                    }

                    var containsUnderline = item.Spans.Contains(SpanTypeEnum.Web) || item.Spans.Contains(SpanTypeEnum.Underline);

                    if (containsUnderline && item.Spans.Contains(SpanTypeEnum.Strikethrough))
                    {
                        span.TextDecorations = TextDecorations.Strikethrough | TextDecorations.Underline;
                    }
                    //span.Text = LabelDecoratorService.GetActualText(span.Text, item.Spans);
                    spans.Add(span);
                }
            }
            return spans;
        }

        private void SetSpan(ChatStringSection section, Span span, SpanTypeEnum eachSpan)
        {
            switch (eachSpan)
            {
                case SpanTypeEnum.Bold:
                    span.FontAttributes = FontAttributes.Bold;
                    break;
                case SpanTypeEnum.Italic:
                    span.FontAttributes = FontAttributes.Italic;
                    break;
                case SpanTypeEnum.Strikethrough:
                    span.TextDecorations = TextDecorations.Strikethrough;
                    break;
                case SpanTypeEnum.Web:
                    span.TextColor = Color.Blue;
                    span.TextDecorations = TextDecorations.Underline;
                    span.GestureRecognizers.Add(new TapGestureRecognizer()
                    {
                        Command = _navigationCommand,
                        CommandParameter = section.ActualText
                    });
                    break;
                case SpanTypeEnum.ColouredTag:
                    var colourCode = LabelDecoratorService.GetActualText(span.Text.Split("||")[0]);
                    span.Text = span.Text.Replace(colourCode + "||", "");
                    var converter = new ColorTypeConverter();
                    try
                    {
                        span.TextColor = (Color)converter.ConvertFromInvariantString(colourCode);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        span.TextColor = Color.Black;
                    }
                    break;
                case SpanTypeEnum.Underline:
                    span.TextDecorations = TextDecorations.Underline;
                    break;
                case SpanTypeEnum.FontSize:
                    var fontsizeCode = LabelDecoratorService.GetActualText(span.Text.Split("||")[0]);
                    span.Text = span.Text.Replace(fontsizeCode + "||", "");

                    var isSuccessful = double.TryParse(fontsizeCode, out double fontsize);
                    if (isSuccessful)
                        span.FontSize = fontsize;
                    break;
                case SpanTypeEnum.None:
                default:
                    span.FontAttributes = FontAttributes.None;
                    break;
            }
        }

        private ICommand _navigationCommand = new Command<string>(async (url) =>
        {
            await OpenBrowser(url);
        });

        private async static Task OpenBrowser(string webpage)
        {
            if (webpage != null)
            {
                if (!webpage.Contains("http://", StringComparison.InvariantCultureIgnoreCase) &&
                    !webpage.Contains("https://", StringComparison.InvariantCultureIgnoreCase))
                {
                    webpage = "http://" + webpage;
                }
            }

            var geturi = new Uri(webpage);
            await Browser.OpenAsync(geturi, new BrowserLaunchOptions
            {
                LaunchMode = BrowserLaunchMode.SystemPreferred,
                TitleMode = BrowserTitleMode.Show,
            });
        }
    }

    internal class SpanTypeIndexes
    {
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }
        public int Length { get { return EndIndex - StartIndex; } }
        public IList<SpanTypeEnum> Spans { get; set; }
    }
}
