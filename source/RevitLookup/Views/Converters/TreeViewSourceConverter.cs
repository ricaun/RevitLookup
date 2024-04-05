﻿// Copyright 2003-2024 by Autodesk, Inc.
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

using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using RevitLookup.Core.Objects;

namespace RevitLookup.Views.Converters;

public sealed class TreeViewGroupConverter : MarkupExtension, IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var viewSource = new CollectionViewSource
        {
            Source = value
        };
        viewSource.SortDescriptions.Add(new SortDescription($"{nameof(Descriptor)}.{nameof(Descriptor.Type)}", ListSortDirection.Ascending));
        viewSource.SortDescriptions.Add(new SortDescription($"{nameof(Descriptor)}.{nameof(Descriptor.Name)}", ListSortDirection.Ascending));
        viewSource.SortDescriptions.Add(new SortDescription($"{nameof(Descriptor)}.{nameof(Descriptor.Description)}", ListSortDirection.Ascending));
        viewSource.GroupDescriptions.Add(new PropertyGroupDescription($"{nameof(Descriptor)}.{nameof(Descriptor.Type)}"));
        viewSource.View.CollectionChanged += OnViewOnCollectionChanged;
        return viewSource.View.Groups;
    }
    
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
    
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return this;
    }
    
    /// <remarks>
    ///     Even an empty subscription tracks the deletion of an item and updates the TreeView. The update doesn't work without a subscription
    /// </remarks>
    private static void OnViewOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
    {
    }
}