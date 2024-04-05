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

using System.IO;
using System.Windows.Interop;
using System.Windows.Media;
using Nice3point.Revit.Toolkit.External;
using Nice3point.Revit.Toolkit.External.Handlers;
using RevitLookup.Core.Objects;
using RevitLookup.Services.Contracts;
using RevitLookup.Utils;

namespace RevitLookup;

[AppLoader]
[UsedImplicitly]
public class Application : ExternalApplication
{
    public static ActionEventHandler ActionEventHandler { get; private set; }
    public static AsyncEventHandler AsyncEventHandler { get; private set; }
    public static AsyncEventHandler<IList<SnoopableObject>> ExternalElementHandler { get; private set; }
    public static AsyncEventHandler<IList<Descriptor>> ExternalDescriptorHandler { get; private set; }
    
    public override void OnStartup()
    {
        RegisterHandlers();
        Host.Start();
        
        var settingsService = Host.GetService<ISettingsService>();
        var updateService = Host.GetService<ISoftwareUpdateService>();
        
        EnableHardwareRendering(settingsService);
        //RibbonController.CreatePanel(Application, settingsService);
        
        updateService.CheckUpdates();
    }
    
    public override void OnShutdown()
    {
        SaveSettings();
        UpdateSoftware();
        Host.Stop();
    }
    
    private static void UpdateSoftware()
    {
        var updateService = Host.GetService<ISoftwareUpdateService>();
        if (File.Exists(updateService.LocalFilePath)) ProcessTasks.StartShell(updateService.LocalFilePath);
    }
    
    private static void SaveSettings()
    {
        var settingsService = Host.GetService<ISettingsService>();
        settingsService.Save();
    }
    
    public static void EnableHardwareRendering(ISettingsService settingsService)
    {
        if (!settingsService.UseHardwareRendering) return;
        
        //Revit overrides render mode during initialization
        //EventHandler is called after initialization
        ActionEventHandler.Raise(_ => RenderOptions.ProcessRenderMode = RenderMode.Default);
    }
    
    public static void DisableHardwareRendering(ISettingsService settingsService)
    {
        if (settingsService.UseHardwareRendering) return;
        RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;
    }
    
    private static void RegisterHandlers()
    {
        ActionEventHandler = new ActionEventHandler();
        AsyncEventHandler = new AsyncEventHandler();
        ExternalElementHandler = new AsyncEventHandler<IList<SnoopableObject>>();
        ExternalDescriptorHandler = new AsyncEventHandler<IList<Descriptor>>();
    }
}

internal class AppLoaderAttribute : Attribute
{
}