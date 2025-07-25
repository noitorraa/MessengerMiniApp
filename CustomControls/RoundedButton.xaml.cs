using Microsoft.Maui.Controls;
using System;
using System.Windows.Input;

namespace MessengerMiniApp.CustomControls
{
    public partial class RoundedButton : ContentView
    {
        public static readonly BindableProperty TextProperty =
            BindableProperty.Create(nameof(Text), typeof(string), typeof(RoundedButton), string.Empty);

        public static readonly BindableProperty CommandProperty =
            BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(RoundedButton), null);

        public static readonly BindableProperty CommandParameterProperty =
            BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(RoundedButton), null);

        public new static readonly BindableProperty BackgroundColorProperty =
            BindableProperty.Create(nameof(BackgroundColor), typeof(Color), typeof(RoundedButton), Colors.Red);

        public new static readonly BindableProperty HeightProperty =
            BindableProperty.Create(nameof(Height), typeof(double), typeof(RoundedButton), 66.0);

        public new static readonly BindableProperty WidthProperty =
            BindableProperty.Create(nameof(Width), typeof(double), typeof(RoundedButton), 191.0);

        public static readonly BindableProperty FontSizeProperty =
            BindableProperty.Create(nameof(FontSize), typeof(double), typeof(RoundedButton), 20.0);

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        public new Color BackgroundColor
        {
            get => (Color)GetValue(BackgroundColorProperty);
            set => SetValue(BackgroundColorProperty, value);
        }

        public new double Height
        {
            get => (double)GetValue(HeightProperty);
            set => SetValue(HeightProperty, value);
        }

        public new double Width
        {
            get => (double)GetValue(WidthProperty);
            set => SetValue(WidthProperty, value);
        }

        public double FontSize
        {
            get => (double)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }

        public event EventHandler<EventArgs>? Clicked;

        public RoundedButton()
        {
            InitializeComponent();
            BindingContext = this;
        }

        private void OnButtonClicked(object sender, EventArgs e)
        {
            Command?.Execute(CommandParameter);
            Clicked?.Invoke(this, e);
        }
    }
}
