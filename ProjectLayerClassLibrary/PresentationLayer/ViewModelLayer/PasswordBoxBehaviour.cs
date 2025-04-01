using System.Windows;
using System.Windows.Controls;

namespace ProjectLayerClassLibrary.PresentationLayer.ViewModelLayer
{
    public static class PasswordBoxBehavior 
    {
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.RegisterAttached(
                "Password", typeof(string), typeof(PasswordBoxBehavior),
                new PropertyMetadata(string.Empty, OnPasswordChanged));

        //public static string GetBoundPassword(DependencyObject obj)
        public static string GetPassword(DependencyObject obj)
        {
            return (string)obj.GetValue(PasswordProperty);
        }

        //public static void SetBoundPassword(DependencyObject obj, object? value)
        public static void SetPassword(DependencyObject obj, object? value)
        {
            obj.SetValue(PasswordProperty, value);
        }

        //private static void OnBoundPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        private static void OnPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PasswordBox passwordBox)
            {
                passwordBox.PasswordChanged -= PasswordBox_PasswordChanged;
                passwordBox.Password = e.NewValue?.ToString();
                passwordBox.PasswordChanged += PasswordBox_PasswordChanged;
            }
        }

        private static void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
                //SetBoundPassword(passwordBox, passwordBox.Password);
                SetPassword(passwordBox, passwordBox.Password);
            }
        }
    }
}
