using System;
using System.Collections.Generic;
using System.Text;
using XamarinChatLabelDecorator.ViewModels;

namespace XamarinChatLabelDecorator.ViewModel
{
    public class ChatLabelDecoratorViewModel:BaseViewModel
    {
		public ChatLabelDecoratorViewModel()
		{
			SampleText = GetSampleDecoratorText();
		}

		private string GetSampleDecoratorText()
		{
			string sampletext = "Hello there this is a sample of how to use *XamChatLabelDecorator* to decorate a _chat label or an article_ comprising of different formatting without having to create multiple label spans.\n *Bold Text* \n *_Italix Text_* \n ~Strikethrough~ \n %%Red||Changing a colour to red%% \n /-Underline Sample-/ \n Loading http link https://github.com/AlisuTech/XamChatLabelDecorator  ##30||Changing Font Size## *XamChatLabelDecorator* also supports * adding new lines to your text \n\n\n Three lines were added. \n ##50||%%Red||Enjoy%%##";

			return sampletext;
		}

		public string SampleText { get; set; }
	}
}
