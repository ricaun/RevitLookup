// Copyright 2003-2024 by Autodesk, Inc.
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

using System.Reflection;
using System.Windows.Controls;
using Microsoft.Extensions.Logging;
using RevitLookup.Core.Contracts;
using RevitLookup.Core.Objects;
using RevitLookup.ViewModels.Contracts;
using RevitLookup.Views.Dialogs;
using RevitLookup.Views.Extensions;

namespace RevitLookup.Core.ComponentModel.Descriptors;

public sealed class ParameterDescriptor : Descriptor, IDescriptorResolver, IDescriptorExtension, IDescriptorConnector
{
    private readonly Parameter _parameter;
    
    public ParameterDescriptor(Parameter parameter)
    {
        _parameter = parameter;
        Name = parameter.Definition.Name;
    }
    
    public void RegisterExtensions(IExtensionManager manager)
    {
        manager.Register(_parameter, extension =>
        {
            extension.Name = nameof(ParameterExtensions.AsBool);
            extension.Result = extension.Value.AsBool();
        });
        
        manager.Register(_parameter, extension =>
        {
            extension.Name = nameof(ParameterExtensions.AsColor);
            extension.Result = extension.Value.AsColor();
        });
        
        if (manager.Context.IsFamilyDocument)
        {
            manager.Register(_parameter, extension =>
            {
                extension.Name = nameof(FamilyManager.GetAssociatedFamilyParameter);
                extension.Result = extension.Context.FamilyManager.GetAssociatedFamilyParameter(extension.Value);
            });
        }
    }
    
    public ResolveSet Resolve(Document context, string target, ParameterInfo[] parameters)
    {
        return target switch
        {
            nameof(Parameter.ClearValue) when parameters.Length == 0 => ResolveSet.Append(false, "Overridden"),
            _ => null
        };
    }
    
    public void RegisterMenu(ContextMenu contextMenu)
    {
        contextMenu.AddMenuItem()
            .SetHeader("Edit value")
            .SetAvailability(!_parameter.IsReadOnly && _parameter.StorageType != StorageType.None)
            .SetCommand(_parameter, async parameter =>
            {
                var context = (ISnoopViewModel) contextMenu.DataContext;
                try
                {
                    var modulesDialog = new EditParameterDialog(context.ServiceProvider, parameter);
                    var result = await modulesDialog.ShowAsync();
                    if (result)
                    {
                        context.RefreshMembersCommand.Execute(null);
                    }
                }
                catch (Exception exception)
                {
                    var logger = context.ServiceProvider.GetService<ILogger<ParameterDescriptor>>();
                    logger.LogError(exception, "Initialize EditParameterDialog error");
                }
            });
    }
}