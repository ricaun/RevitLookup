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

using RevitLookup.Core.Contracts;
using RevitLookup.Core.Objects;

namespace RevitLookup.Core.ComponentModel.Descriptors;

public sealed class ElementIdDescriptor : Descriptor, IDescriptorRedirection
{
    private readonly ElementId _elementId;

    public ElementIdDescriptor(ElementId elementId)
    {
        _elementId = elementId;
        Name = _elementId.ToString();
    }

    public bool TryRedirect(Document context, string target, out object output)
    {
        output = null;
        if (target == nameof(Element.Id)) return false;
        if (_elementId == ElementId.InvalidElementId) return false;

#if REVIT2024_OR_GREATER
        if (_elementId.Value is > -3000000 and < -2000000)
#else
        if (_elementId.IntegerValue is > -3000000 and < -2000000)
#endif
        {
            var element = Category.GetCategory(context, _elementId);
            if (element is null) return false;

            output = element;
            return true;
        }
        else
        {
            var element = _elementId.ToElement(context);
            if (element is null) return false;

            output = element;
            return true;
        }
    }
}