using System;
using System.Linq;
using System.Threading.Tasks;
using System.Xaml;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.ComponentModel;

namespace SolidShineUi
{
    [ContentProperty("Content")]
    public class TabItem : DependencyObject
    {
        public TabItem()
        {
            //Self = this;

            InternalTitleChanged += tabItem_InternalTitleChanged;
            InternalIsDirtyChanged += tabItem_InternalIsDirtyChanged;
            InternalCanCloseChanged += tabItem_InternalCanCloseChanged;
            InternalCanSelectChanged += tabItem_InternalCanSelectChanged;
            InternalIconChanged += tabItem_InternalIconChanged;
            InternalContentChanged += tabItem_InternalContentChanged;
            InternalVisibilityChanged += tabItem_InternalVisibilityChanged;
            InternalShowIconChanged += tabItem_InternalShowIconChanged;
        }

        #region Title

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(TabItem),
            new PropertyMetadata("New Tab", new PropertyChangedCallback(OnInternalTitleChanged)));

        ///<summary>
        /// Get or set the title of this tab.
        ///</summary>
        [Category("Common")]
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        protected event DependencyPropertyChangedEventHandler InternalTitleChanged;

#if NETCOREAPP
        public event DependencyPropertyChangedEventHandler? TitleChanged;
#else
        public event DependencyPropertyChangedEventHandler TitleChanged;
#endif

        private static void OnInternalTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TabItem s)
            {
                s.InternalTitleChanged?.Invoke(s, e);
            }
        }
        private void tabItem_InternalTitleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            TitleChanged?.Invoke(this, e);
        }
        #endregion

        #region IsDirty

        public static readonly DependencyProperty IsDirtyProperty = DependencyProperty.Register("IsDirty", typeof(bool), typeof(TabItem),
            new PropertyMetadata(false, new PropertyChangedCallback(OnInternalIsDirtyChanged)));

        ///<summary>
        /// Get or set if the tab is dirty. In this current release, this does not make a visual change on the tab.
        ///</summary>
        [Category("Common")]
        public bool IsDirty
        {
            get { return (bool)GetValue(IsDirtyProperty); }
            set { SetValue(IsDirtyProperty, value); }
        }

        protected event DependencyPropertyChangedEventHandler InternalIsDirtyChanged;

#if NETCOREAPP
        public event DependencyPropertyChangedEventHandler? IsDirtyChanged;
#else
        public event DependencyPropertyChangedEventHandler IsDirtyChanged;
#endif

        private static void OnInternalIsDirtyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TabItem s)
            {
                s.InternalIsDirtyChanged?.Invoke(s, e);
            }
        }
        private void tabItem_InternalIsDirtyChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            IsDirtyChanged?.Invoke(this, e);
        }
        #endregion

        #region CanClose

        public static readonly DependencyProperty CanCloseProperty = DependencyProperty.Register("CanClose", typeof(bool), typeof(TabItem),
            new PropertyMetadata(true, new PropertyChangedCallback(OnInternalCanCloseChanged)));

        ///<summary>
        /// Get or set if this tab can be closed via the UI (i.e. the close button).
        ///</summary>
        [Category("Common")]
        public bool CanClose
        {
            get { return (bool)GetValue(CanCloseProperty); }
            set { SetValue(CanCloseProperty, value); }
        }

        protected event DependencyPropertyChangedEventHandler InternalCanCloseChanged;

#if NETCOREAPP
        public event DependencyPropertyChangedEventHandler? CanCloseChanged;
#else
        public event DependencyPropertyChangedEventHandler CanCloseChanged;
#endif

        private static void OnInternalCanCloseChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TabItem s)
            {
                s.InternalCanCloseChanged?.Invoke(s, e);
            }
        }
        private void tabItem_InternalCanCloseChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            CanCloseChanged?.Invoke(this, e);
        }
        #endregion
        
        #region ShowIcon

        public static readonly DependencyProperty ShowIconProperty = DependencyProperty.Register("ShowIcon", typeof(bool), typeof(TabItem),
            new PropertyMetadata(true, new PropertyChangedCallback(OnInternalShowIconChanged)));

        ///<summary>
        /// Get or set if an icon is shown for this tab. The icon section of the tab will be visible if this is true, even if there is no icon set.
        ///</summary>
        [Category("Common")]
        public bool ShowIcon
        {
            get { return (bool)GetValue(ShowIconProperty); }
            set { SetValue(ShowIconProperty, value); }
        }

        protected event DependencyPropertyChangedEventHandler InternalShowIconChanged;

#if NETCOREAPP
        public event DependencyPropertyChangedEventHandler? ShowIconChanged;
#else
        public event DependencyPropertyChangedEventHandler ShowIconChanged;
#endif

        private static void OnInternalShowIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TabItem s)
            {
                s.InternalShowIconChanged?.Invoke(s, e);
            }
        }
        private void tabItem_InternalShowIconChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ShowIconChanged?.Invoke(this, e);
        }
        #endregion

        //internal TabItem Self { get => this; }

        #region CanSelect

        public static readonly DependencyProperty CanSelectProperty = DependencyProperty.Register("CanSelect", typeof(bool), typeof(TabItem),
            new PropertyMetadata(true, new PropertyChangedCallback(OnInternalCanSelectChanged)));

        ///<summary>
        /// Get or set whether this tab can be selected via UI.
        ///</summary>
        [Category("Common")]
        public bool CanSelect
        {
            get { return (bool)GetValue(CanSelectProperty); }
            set { SetValue(CanSelectProperty, value); }
        }

        protected event DependencyPropertyChangedEventHandler InternalCanSelectChanged;

#if NETCOREAPP
        public event DependencyPropertyChangedEventHandler? CanSelectChanged;
#else
        public event DependencyPropertyChangedEventHandler CanSelectChanged;
#endif

        private static void OnInternalCanSelectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TabItem s)
            {
                s.InternalCanSelectChanged?.Invoke(s, e);
            }
        }
        private void tabItem_InternalCanSelectChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            CanSelectChanged?.Invoke(this, e);
        }
        #endregion

        //#region TabItem

        //private static readonly DependencyProperty SelfProperty = DependencyProperty.Register("Self", typeof(TabItem), typeof(TabItem),
        //    new PropertyMetadata(null));

        ////public static readonly DependencyProperty TabItemProperty = TabItemPropertyKey.DependencyProperty;

        //public TabItem Self
        //{
        //    get { return (TabItem)GetValue(SelfProperty); }
        //    set { SetValue(SelfProperty, value); }
        //}
        //#endregion

        #region Icon

        public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(ImageSource), typeof(TabItem),
            new PropertyMetadata(null, new PropertyChangedCallback(OnInternalIconChanged)));

        ///<summary>
        /// Get or set the icon to display with this tab.
        ///</summary>
        [Category("Common")]
        public ImageSource Icon
        {
            get { return (ImageSource)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        protected event DependencyPropertyChangedEventHandler InternalIconChanged;

#if NETCOREAPP
        public event DependencyPropertyChangedEventHandler? IconChanged;
#else
        public event DependencyPropertyChangedEventHandler IconChanged;
#endif

        private static void OnInternalIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TabItem s)
            {
                s.InternalIconChanged?.Invoke(s, e);
            }
        }
        private void tabItem_InternalIconChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            IconChanged?.Invoke(this, e);
        }
        #endregion

        #region Content

        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register("Content", typeof(UIElement), typeof(TabItem),
            new PropertyMetadata(null, new PropertyChangedCallback(OnInternalContentChanged)));

        ///<summary>
        /// Get or set the content to display when this tab is selected.
        ///</summary>
        [Category("Common")]
        public UIElement Content
        {
            get { return (UIElement)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        protected event DependencyPropertyChangedEventHandler InternalContentChanged;

#if NETCOREAPP
        public event DependencyPropertyChangedEventHandler? ContentChanged;
#else
        public event DependencyPropertyChangedEventHandler ContentChanged;
#endif

        private static void OnInternalContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TabItem s)
            {
                s.InternalContentChanged?.Invoke(s, e);
            }
        }
        private void tabItem_InternalContentChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ContentChanged?.Invoke(this, e);
        }
        #endregion

        #region Visibility

        public static readonly DependencyProperty VisibilityProperty = DependencyProperty.Register("Visibility", typeof(Visibility), typeof(TabItem),
            new PropertyMetadata(System.Windows.Visibility.Visible, new PropertyChangedCallback(OnInternalVisibilityChanged)));

        ///<summary>
        /// Get or set if this tab is visually displayed in the UI.
        ///</summary>
        [Category("Appearance")]
        public Visibility Visibility
        {
            get { return (Visibility)GetValue(VisibilityProperty); }
            set { SetValue(VisibilityProperty, value); }
        }

        protected event DependencyPropertyChangedEventHandler InternalVisibilityChanged;

#if NETCOREAPP
        public event DependencyPropertyChangedEventHandler? VisibilityChanged;
#else
        public event DependencyPropertyChangedEventHandler VisibilityChanged;
#endif

        private static void OnInternalVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TabItem s)
            {
                s.InternalVisibilityChanged?.Invoke(s, e);
            }
        }
        private void tabItem_InternalVisibilityChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            VisibilityChanged?.Invoke(this, e);
        }
        #endregion

        /// <summary>
        /// Close the current tab, if it is currently in a TabControl.
        /// </summary>
        public void Close()
        {
            InternalTabClosing?.Invoke(this, EventArgs.Empty);
        }

#if NETCOREAPP
        internal protected event EventHandler? InternalTabClosing;
#else
        internal protected event EventHandler InternalTabClosing;
#endif

        ///<summary>
        /// Get or set if this tab is visible in the UI.
        ///</summary>
        [Category("Appearance")]
        public bool IsVisible
        {
            get
            {
                return Visibility == Visibility.Visible;
            }
            set
            {
                Visibility = value ? Visibility.Visible : Visibility.Collapsed;
            }
        }

    }
}
