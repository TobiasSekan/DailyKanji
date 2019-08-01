using System.Windows;
using System.Windows.Controls;

namespace DailyKanji.Mvvm.Model
{
    internal class AnswerViewElement
    {
        internal TextBlock AnswerText { get; }

        internal UIElement Button { get; }

        internal ColumnDefinition AnswerButtonColumn { get; }

        internal TextBlock AnswerHintText { get; }

        internal TextBlock AnswerShortCutText { get; }

        internal AnswerViewElement(TextBlock answerText, UIElement button, ColumnDefinition answerButtonColumn, TextBlock answerHintText, TextBlock answerShortCutText)
        {
            AnswerText         = answerText;
            Button             = button;
            AnswerButtonColumn = answerButtonColumn;
            AnswerHintText     = answerHintText;
            AnswerShortCutText = answerShortCutText;
        }
    }
}
