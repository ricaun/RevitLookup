// Copyright 2003-2023 by Autodesk, Inc.
// 
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
// 
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
// 
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable.

using System.Collections;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using RevitLookup.Services.Contracts;
using Wpf.Ui;
using Wpf.Ui.Appearance;

namespace RevitLookup.Views;

public sealed partial class RevitLookupView : IWindow
{
    private readonly ISettingsService _settingsService;

    private RevitLookupView()
    {
        Wpf.Ui.Application.MainWindow = this;
        Wpf.Ui.Application.Windows.Add(this);
        InitializeComponent();
        AddShortcuts();
    }

    public RevitLookupView(
        INavigationService navigationService,
        IContentDialogService dialogService,
        ISnackbarService snackbarService,
        ISettingsService settingsService)
        : this()
    {
        _settingsService = settingsService;
        RootNavigation.TransitionDuration = settingsService.TransitionDuration;
        WindowBackdropType = settingsService.Background;

        navigationService.SetNavigationControl(RootNavigation);
        dialogService.SetContentPresenter(RootContentDialog);

        snackbarService.SetSnackbarPresenter(RootSnackbar);
        snackbarService.DefaultTimeOut = TimeSpan.FromSeconds(3);

        RestoreSize(settingsService);
    }

    private void AddShortcuts()
    {
        var closeCurrentCommand = new RelayCommand(Close);
        var closeAllCommand = new RelayCommand(() =>
        {
            for (var i = Wpf.Ui.Application.Windows.Count - 1; i >= 0; i--)
            {
                var window = Wpf.Ui.Application.Windows[i];
                window.Close();
            }
        });

        InputBindings.Add(new KeyBinding(closeAllCommand, new KeyGesture(Key.Escape, ModifierKeys.Shift)));
        InputBindings.Add(new KeyBinding(closeCurrentCommand, new KeyGesture(Key.Escape)));

        // ThemeChangedEvent to fix the issue with the window background not being updated when the theme is changed.
        {
            ThemeChangedEvent themeChanged = (sender, args) =>
            {
                this.Dispatcher.InvokeAsync(() =>
                {
                    foreach (var resource in this.Resources.MergedDictionaries.ToList())
                    {
                        this.Resources.MergedDictionaries.Add(resource);
                        this.Resources.MergedDictionaries.RemoveAt(0);
                    }
                });
            };
            if (this.IsLoaded)
            {
                ApplicationThemeManager.Changed += themeChanged;
            }
            this.Loaded += (s, e) =>
            {
                ApplicationThemeManager.Changed += themeChanged;
            };
            this.Unloaded += (s, e) =>
            {
                ApplicationThemeManager.Changed -= themeChanged;
            };
        }

#if DEBUG

        this.KeyDown += (s, e) =>
        {
            if (e.Key == Key.E)
            {
                var theme = ApplicationThemeManager.GetAppTheme() == ApplicationTheme.Light ? ApplicationTheme.Dark : ApplicationTheme.Light;
                ApplicationThemeManager.Apply(theme);
                ApplicationThemeManager.Apply(theme, Wpf.Ui.Controls.WindowBackdropType.Mica, forceBackground: true);
            }
            if (e.Key == Key.T)
            {
                var theme = ApplicationThemeManager.GetAppTheme() == ApplicationTheme.Light ? ApplicationTheme.Dark : ApplicationTheme.Light;
                ApplicationThemeManager.Apply(theme);
            }
            if (e.Key == Key.D)
            {
                var theme = ApplicationTheme.Dark;
                ApplicationThemeManager.Apply(theme);
                WindowBackgroundManager.UpdateBackground(this, theme, Wpf.Ui.Controls.WindowBackdropType.None, true);
            }
            if (e.Key == Key.F)
            {
                Debug.WriteLine($">>> {Wpf.Ui.Application.MainWindow}");
                foreach (DictionaryEntry resource in Wpf.Ui.Application.MainWindow.Resources.OfType<DictionaryEntry>().OrderBy(e => e.Key))
                {
                    this.Resources[resource.Key] = resource.Value;
                    Debug.WriteLine(
                        $"INFO | Copy Resource {resource.Key} - {resource.Value}",
                        "Wpf.Ui.Appearance"
                    );
                }
            }

            if (e.Key == Key.Y)
            {
                var theme = ApplicationThemeManager.GetAppTheme() == ApplicationTheme.Light ? ApplicationTheme.Dark : ApplicationTheme.Light;
                ApplicationThemeManager.Apply(theme);
                foreach (var resource in this.Resources.MergedDictionaries.ToList())
                {
                    this.Resources.MergedDictionaries.Add(resource);
                    this.Resources.MergedDictionaries.RemoveAt(0);
                }
            }

            if (e.Key == Key.P)
            {
                var theme = ApplicationThemeManager.GetAppTheme() == ApplicationTheme.Light ? ApplicationTheme.Dark : ApplicationTheme.Light;
                ApplicationThemeManager.Apply(theme);
                foreach (var resource in Wpf.Ui.Application.MainWindow.Resources.MergedDictionaries.ToList())
                {
                    Wpf.Ui.Application.MainWindow.Resources.MergedDictionaries.Add(resource);
                    Wpf.Ui.Application.MainWindow.Resources.MergedDictionaries.RemoveAt(0);
                }
            }


            if (e.Key == Key.U)
            {
                foreach (var resource in this.Resources.MergedDictionaries.ToList())
                {
                    this.Resources.MergedDictionaries.Add(resource);
                    this.Resources.MergedDictionaries.RemoveAt(0);
                }
            }

            if (e.Key == Key.R)
            {
                var resourceDictionary = new ResourceDictionary();
                foreach (var resource in Wpf.Ui.Application.MainWindow.Resources.MergedDictionaries)
                {
                    Debug.WriteLine(
                        $"INFO | Resources {resource.Source}",
                        "Wpf.Ui.Appearance"
                    );
                    resourceDictionary.MergedDictionaries.Add(resource);
                }

                //foreach (var resource in resourceDictionary.MergedDictionaries)
                //{
                //    Wpf.Ui.Application.MainWindow.Resources.MergedDictionaries.Add(resource);
                //    Wpf.Ui.Application.MainWindow.Resources.MergedDictionaries.RemoveAt(0);
                //}
            }
        };
#endif

    }

    private void RestoreSize(ISettingsService settingsService)
    {
        if (!settingsService.UseSizeRestoring) return;

        if (settingsService.WindowWidth >= MinWidth) Width = settingsService.WindowWidth;
        if (settingsService.WindowHeight >= MinHeight) Height = settingsService.WindowHeight;

        EnableSizeTracking();
    }

    public void EnableSizeTracking()
    {
        SizeChanged += OnSizeChanged;
    }

    public void DisableSizeTracking()
    {
        SizeChanged -= OnSizeChanged;
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs args)
    {
        _settingsService.WindowWidth = args.NewSize.Width;
        _settingsService.WindowHeight = args.NewSize.Height;
    }

    protected override void OnActivated(EventArgs args)
    {
        base.OnActivated(args);
        Wpf.Ui.Application.MainWindow = this;
    }

    protected override void OnClosed(EventArgs args)
    {
        base.OnClosed(args);
        Wpf.Ui.Application.Windows.Remove(this);
    }
}