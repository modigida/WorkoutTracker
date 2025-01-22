using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Windows;
using Microsoft.Xaml.Behaviors;
using System.Windows.Controls;

namespace WorkoutTracker.Behaviors;
public class IntegerInputBehavior : Behavior<TextBox>
{
    private static readonly Regex IntegerRegex = new Regex(@"^\d*$");

    protected override void OnAttached()
    {
        base.OnAttached();

        AssociatedObject.PreviewTextInput += OnPreviewTextInput;
        DataObject.AddPastingHandler(AssociatedObject, OnPaste);
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();

        AssociatedObject.PreviewTextInput -= OnPreviewTextInput;
        DataObject.RemovePastingHandler(AssociatedObject, OnPaste);
    }

    private void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        e.Handled = !IsTextValid(e.Text, ((TextBox)sender).Text);
    }

    private void OnPaste(object sender, DataObjectPastingEventArgs e)
    {
        if (e.DataObject.GetDataPresent(DataFormats.Text))
        {
            var pastedText = e.DataObject.GetData(DataFormats.Text) as string;
            var currentText = ((TextBox)sender).Text;

            if (!IsTextValid(pastedText, currentText))
            {
                e.CancelCommand();
            }
        }
        else
        {
            e.CancelCommand();
        }
    }

    private bool IsTextValid(string newText, string existingText)
    {
        var combinedText = existingText.Insert(AssociatedObject.CaretIndex, newText);

        if (string.IsNullOrWhiteSpace(combinedText))
            return true;

        return IntegerRegex.IsMatch(combinedText);
    }
}