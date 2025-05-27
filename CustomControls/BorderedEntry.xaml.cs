using Microsoft.Maui.Controls;
using System;

namespace MessengerMiniApp.CustomControls
{
    public partial class BorderedEntry : ContentView
    {
        public static readonly BindableProperty PlaceholderProperty =
            BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(BorderedEntry), string.Empty);

        public static readonly BindableProperty IsPasswordProperty =
            BindableProperty.Create(nameof(IsPassword), typeof(bool), typeof(BorderedEntry), false);

        public static readonly BindableProperty SupportTextProperty =
            BindableProperty.Create(nameof(SupportText), typeof(string), typeof(BorderedEntry), "Supporting text");

        public string Placeholder
        {
            get => (string)GetValue(PlaceholderProperty);
            set => SetValue(PlaceholderProperty, value);
        }

        public bool IsPassword
        {
            get => (bool)GetValue(IsPasswordProperty);
            set => SetValue(IsPasswordProperty, value);
        }

        public string SupportText
        {
            get => (string)GetValue(SupportTextProperty);
            set => SetValue(SupportTextProperty, value);
        }

        public string Text
        {
            get => EntryField.Text;
            set => EntryField.Text = value;
        }

        public BorderedEntry()
        {
            InitializeComponent();
            BindingContext = this;

            EntryField.Focused += OnEntryFocused;
            EntryField.Unfocused += OnEntryUnfocused;
        }

        private void OnEntryFocused(object? sender, FocusEventArgs e)
        {
            // Change appearance when focused
            BottomLine.BackgroundColor = Colors.Purple;
            Cursor.BackgroundColor = Colors.Purple;
        }

        private void OnEntryUnfocused(object? sender, FocusEventArgs e)
        {
            // Restore appearance when unfocused
            BottomLine.BackgroundColor = Color.FromArgb("#64748B");
            Cursor.BackgroundColor = Color.FromArgb("#65558F");
        }
    }
}
