using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SolidShineUi.Toolbars.Ribbon
{
    /// <summary>
    /// A tab for a <see cref="Ribbon"/>, which can display a list of commands that a user can select.
    /// </summary>
    public class RibbonTab : DependencyObject
    {
        public RibbonTab()
        {
            SetValue(ItemsPropertyKey, new ObservableCollection<RibbonGroup>());
        }

        public string Title { get => (string)GetValue(TitleProperty); set => SetValue(TitleProperty, value); }

        public static DependencyProperty TitleProperty
            = DependencyProperty.Register("Title", typeof(string), typeof(RibbonTab),
            new FrameworkPropertyMetadata("Tab"));

        public bool FitContentsToWidth { get => (bool)GetValue(FitContentsToWidthProperty); set => SetValue(FitContentsToWidthProperty, value); }

        public static DependencyProperty FitContentsToWidthProperty
            = DependencyProperty.Register("FitContentsToWidth", typeof(bool), typeof(RibbonTab),
            new FrameworkPropertyMetadata(true));

        public Visibility Visibility { get => (Visibility)GetValue(VisibilityProperty); set => SetValue(VisibilityProperty, value); }

        public static DependencyProperty VisibilityProperty
            = DependencyProperty.Register("Visibility", typeof(Visibility), typeof(RibbonTab),
            new FrameworkPropertyMetadata(Visibility.Visible));

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        private static readonly DependencyPropertyKey ItemsPropertyKey
            = DependencyProperty.RegisterReadOnly("Items", typeof(ObservableCollection<RibbonGroup>), typeof(RibbonTab),
            new FrameworkPropertyMetadata(new ObservableCollection<RibbonGroup>()));

        public static readonly DependencyProperty ItemsProperty = ItemsPropertyKey.DependencyProperty;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Get or set the list of items in this RibbonGroup. This Items property can be used to add and remove items.
        /// </summary>
        [Category("Common")]
        public ObservableCollection<RibbonGroup> Items
        {
            get { return (ObservableCollection<RibbonGroup>)GetValue(ItemsProperty); }
            private set { SetValue(ItemsPropertyKey, value); }
        }

        #region Contextual Tab
        public bool IsContextual { get => (bool)GetValue(IsContextualProperty); set => SetValue(IsContextualProperty, value); }

        public static DependencyProperty IsContextualProperty
            = DependencyProperty.Register("IsContextual", typeof(bool), typeof(RibbonTab),
            new FrameworkPropertyMetadata(false));

        public Color ContextualColor { get => (Color)GetValue(ContextualColorProperty); set => SetValue(ContextualColorProperty, value); }

        public static DependencyProperty ContextualColorProperty
            = DependencyProperty.Register("ContextualColor", typeof(Color), typeof(RibbonTab),
            new FrameworkPropertyMetadata(Colors.Blue));
        #endregion
    }
}
