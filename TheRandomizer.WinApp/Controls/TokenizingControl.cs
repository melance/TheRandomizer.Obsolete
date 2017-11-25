using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace TheRandomizer.WinApp.Controls
{
    public class TokenizingControl : RichTextBox
    {
        public static readonly DependencyProperty TokenTemplateProperty =
            DependencyProperty.Register("TokenTemplate", typeof (DataTemplate), typeof (TokenizingControl));
        public static readonly DependencyProperty TokenMatcherProperty =
            DependencyProperty.Register("TokenMatcher", typeof(Func<string, object>), typeof(TokenizingControl));

        public DataTemplate TokenTemplate
        {
            get { return (DataTemplate) GetValue(TokenTemplateProperty); }
            set { SetValue(TokenTemplateProperty, value); }
        }

        public Func<string, object> TokenMatcher
        {
            get { return (Func<string, object>)GetValue(TokenMatcherProperty); }
            set { SetValue(TokenMatcherProperty, value); }
        }

        public TokenizingControl()
        {
            TextChanged += OnTokenTextChanged;
        }

        private void OnTokenTextChanged(object sender, TextChangedEventArgs e)
        {
            var text = CaretPosition.GetTextInRun(LogicalDirection.Backward);
            if (TokenMatcher != null)
            {
                var token = TokenMatcher(text);
                if (token != null)
                {
                    ReplaceTextWithToken(text, token);
                }
            }
        }

        private void ReplaceTextWithToken(string inputText, object token)
        {
            // Remove the handler temporarily as we will be modifying tokens below, causing more TextChanged events
            TextChanged -= OnTokenTextChanged;

            var para = CaretPosition.Paragraph;

            var matchedRun = para.Inlines.FirstOrDefault(inline =>
            {
                var run = inline as Run;
                return (run != null && run.Text.EndsWith(inputText));
            }) as Run;
            if (matchedRun != null) // Found a Run that matched the inputText
            {
                var tokenContainer = CreateTokenContainer(inputText, token);
                para.Inlines.InsertBefore(matchedRun, tokenContainer);

                // Remove only if the Text in the Run is the same as inputText, else split up
                if (matchedRun.Text == inputText)
                {
                    para.Inlines.Remove(matchedRun);
                }
                else // Split up
                {
                    var index = matchedRun.Text.IndexOf(inputText) + inputText.Length;
                    var tailEnd = new Run(matchedRun.Text.Substring(index));
                    para.Inlines.InsertAfter(matchedRun, tailEnd);
                    para.Inlines.Remove(matchedRun);
                }
            }

            TextChanged += OnTokenTextChanged;
        }

        private InlineUIContainer CreateTokenContainer(string inputText, object token)
        {
            // Note: we are not using the inputText here, but could be used in future

            var presenter = new ContentPresenter()
            {
                Content = token,
                ContentTemplate = TokenTemplate,
            };

            // BaselineAlignment is needed to align with Run
            return new InlineUIContainer(presenter) { BaselineAlignment = BaselineAlignment.TextBottom };
        }


    }
}