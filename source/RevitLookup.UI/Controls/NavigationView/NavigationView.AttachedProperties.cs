// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

// ReSharper disable once CheckNamespace
namespace Wpf.Ui.Controls;

public partial class NavigationView
{
    public static readonly DependencyProperty HeaderContentProperty = DependencyProperty.RegisterAttached(
        "HeaderContentNavigationView",
        typeof(object),
        typeof(NavigationView),
        new FrameworkPropertyMetadata(null)
    );

    [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
    public static object? GetHeaderContent(FrameworkElement target) => target.GetValue(HeaderContentProperty);

    public static void SetHeaderContent(FrameworkElement target, object headerContent) =>
        target.SetValue(HeaderContentProperty, headerContent);
}
