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

using Autodesk.Revit.UI;

namespace RevitLookup;

public class App : IExternalApplication
{
    private static IExternalApplication _externalApplication;
    public Result OnStartup(UIControlledApplication application)
    {
        if (Utils.AssemblyContext.IsDefault())
        {
            _externalApplication = Utils.AssemblyContext.InstanceFrom<App, IExternalApplication>();
            return _externalApplication.OnStartup(application);
        }

        Console.WriteLine("OnStartup");
        Application.RegisterHandlers();
        Host.Start();

        return Result.Succeeded;
    }
    public Result OnShutdown(UIControlledApplication application)
    {
        if (Utils.AssemblyContext.IsDefault())
        {
            return _externalApplication.OnShutdown(application);
        }

        Console.WriteLine("OnShutdown");
        Host.Stop();

        Utils.AssemblyContext.Unload();

        return Result.Succeeded;
    }

}
internal class AppLoaderAttribute : Attribute
{
}