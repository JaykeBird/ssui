﻿using System;
using System.Collections.Generic;
using System.Windows;
using SolidShineUi;

namespace SolidShineUi.PropertyList
{
    public interface IPropertyEditor : IInputElement, IFrameworkInputElement
    {

        List<Type> ValidTypes { get; }

        bool EditorAllowsModifying { get; }

        bool IsPropertyWritable { get; set; }

        FrameworkElement GetFrameworkElement();

        ColorScheme ColorScheme { set; }

        event EventHandler ValueChanged;

#if NETCOREAPP
        public void LoadValue(object? value, Type type);

        public object? GetValue();

#else
        void LoadValue(object value, Type type);

        object GetValue();
#endif
    }
}