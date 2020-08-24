# XamChatLabelDecorator
Xamarin Chat Label Decorator

# Nugget
 [Plugin.XamChatLabelDecoratorConverter](https://www.nuget.org/packages/Plugin.XamChatLabelDecoratorConverter/1.0.3)

<img src="./image.png">


# Sample Text
string sampletext = "Hello there this is a sample of how to use \*XamChatLabelDecorator\* to decorate a \_chat label or an article\_ comprising of different formatting without having to create multiple label spans.\n \*Bold Text\* \n \*\_Italix Text\_\* \n \~Strikethrough\~ \n %%Red||Changing a colour to red%% \n /-Underline Sample-/ \n Loading http link https://github.com/AlisuTech/XamChatLabelDecorator  ##30||Changing Font Size## *XamChatLabelDecorator* also supports * adding new lines to your text \n\n\n Three lines were added. \n ##50||%%Red||Enjoy%%##"; <b>
\*?\* - Bold <br>
\_?\_ - Italic <br>
\~?\~ - Strikethrough <br>
/-?-/ - Underline <br>
\n - Creates a new line <br>
##Fontsize||actual text## - Changes font size <br>
%%Colour||actualtext%% - Changes the colour of a formatted text <br>

# Limitation
The converter cannot nest a change of colour and fontsize change in the same span.
