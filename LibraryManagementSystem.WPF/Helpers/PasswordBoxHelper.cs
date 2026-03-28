using System.Windows;
using System.Windows.Controls;

namespace LibraryManagementSystem.WPF.Helpers
{
	public static class PasswordBoxHelper
	{
		public static readonly DependencyProperty BoundPasswordProperty =
			DependencyProperty.RegisterAttached(
				"BoundPassword",
				typeof(string),
				typeof(PasswordBoxHelper),
				new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnBoundPasswordChanged));

		public static readonly DependencyProperty BindPasswordProperty =
			DependencyProperty.RegisterAttached(
				"BindPassword",
				typeof(bool),
				typeof(PasswordBoxHelper),
				new PropertyMetadata(false, OnBindPasswordChanged));

		private static readonly DependencyProperty UpdatingPasswordProperty =
			DependencyProperty.RegisterAttached(
				"UpdatingPassword",
				typeof(bool),
				typeof(PasswordBoxHelper),
				new PropertyMetadata(false));

		public static void SetBindPassword(DependencyObject dp, bool value)
		{
			dp.SetValue(BindPasswordProperty, value);
		}

		public static bool GetBindPassword(DependencyObject dp)
		{
			return (bool)dp.GetValue(BindPasswordProperty);
		}

		public static void SetBoundPassword(DependencyObject dp, string value)
		{
			dp.SetValue(BoundPasswordProperty, value);
		}

		public static string GetBoundPassword(DependencyObject dp)
		{
			return (string)dp.GetValue(BoundPasswordProperty);
		}

		private static void SetUpdatingPassword(DependencyObject dp, bool value)
		{
			dp.SetValue(UpdatingPasswordProperty, value);
		}

		private static bool GetUpdatingPassword(DependencyObject dp)
		{
			return (bool)dp.GetValue(UpdatingPasswordProperty);
		}

		private static void OnBindPasswordChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
		{
			if (dp is not PasswordBox passwordBox)
				return;

			if ((bool)e.OldValue)
				passwordBox.PasswordChanged -= HandlePasswordChanged;

			if ((bool)e.NewValue)
				passwordBox.PasswordChanged += HandlePasswordChanged;
		}

		private static void OnBoundPasswordChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
		{
			if (dp is not PasswordBox passwordBox)
				return;

			if (GetUpdatingPassword(passwordBox))
				return;

			passwordBox.Password = e.NewValue?.ToString() ?? string.Empty;
		}

		private static void HandlePasswordChanged(object sender, RoutedEventArgs e)
		{
			if (sender is not PasswordBox passwordBox)
				return;

			SetUpdatingPassword(passwordBox, true);
			SetBoundPassword(passwordBox, passwordBox.Password);
			SetUpdatingPassword(passwordBox, false);
		}
	}
}
