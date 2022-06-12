﻿// Copyright 2003-2022 by Autodesk, Inc.
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

using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using RevitLookup.UI.Common;

namespace RevitLookup.UI.Tests.ViewModels.Pages;

public sealed class DashboardViewModel : INotifyPropertyChanged
{
    private readonly RevitLookupViewModel _lookupViewModel;
    private RelayCommand _snoopSelectionCommand;

    public DashboardViewModel(RevitLookupViewModel lookupViewModel)
    {
        _lookupViewModel = lookupViewModel;
    }

    public RelayCommand SnoopSelectionCommand => _snoopSelectionCommand ??= new RelayCommand(o =>
    {
        _lookupViewModel.CurrentPageIndex = 1;
    });

    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvocator]
    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}