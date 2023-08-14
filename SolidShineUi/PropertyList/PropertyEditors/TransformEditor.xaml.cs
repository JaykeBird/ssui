using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SolidShineUi;
using SolidShineUi.PropertyList.Dialogs;

namespace SolidShineUi.PropertyList.PropertyEditors
{
    /// <summary>
    /// An editor for WPF element transforms, of <see cref="Transform"/> or similar subtypes.
    /// </summary>
    public partial class TransformEditor : UserControl, IPropertyEditor
    {
        /// <summary>
        /// Create an TransformEditor.
        /// </summary>
        public TransformEditor()
        {
            InitializeComponent();
        }

        /// <inheritdoc/>
        public List<Type> ValidTypes => new List<Type> { typeof(Transform), typeof(TransformGroup), typeof(TransformCollection),
            typeof(TranslateTransform), typeof(RotateTransform), typeof(SkewTransform), typeof(ScaleTransform), typeof(MatrixTransform)};

        /// <inheritdoc/>
        public bool EditorAllowsModifying => true;

        bool _writable = true;

        /// <inheritdoc/>
        public bool IsPropertyWritable { get => _writable; set => _writable = value; }

        /// <inheritdoc/>
        public ExperimentalPropertyList ParentPropertyList { set { _parent = value; } }

        ColorScheme _cs = new ColorScheme();

        /// <inheritdoc/>
        public ColorScheme ColorScheme
        {
            set
            {
                _cs = value;
                btnMenu.ColorScheme = value;
                imgMenu.Source = Utils.IconLoader.LoadIcon("ThreeDots", value);
            }
        }

        /// <inheritdoc/>
        public FrameworkElement GetFrameworkElement()
        {
            return this;
        }

        Type _dataType = typeof(TransformGroup);

        Transform _transform = new TransformGroup();

        bool _setNull = false;

#if NETCOREAPP
        ExperimentalPropertyList? _parent = null;

        /// <inheritdoc/>
        public event EventHandler? ValueChanged;

        /// <inheritdoc/>
        public object? GetValue()
        {
            return _setNull ? null : _transform;
        }
#else
        ExperimentalPropertyList _parent = null;
        
        /// <inheritdoc/>
        public event EventHandler ValueChanged;
        
        /// <inheritdoc/>
        public object GetValue()
        {
            return _setNull ? null : _transform;
        }
#endif

        /// <inheritdoc/>
#if NETCOREAPP
        public void LoadValue(object? value, Type type)
#else
        public void LoadValue(object value, Type type)
#endif
        {
            // first, let's assign the property type needed based upon the dataType
            _dataType = type;

            // let's check if this is null
            if (value == null)
            {
                _transform = new TransformGroup();
                _setNull = true;
                mnuSetToNull.IsChecked = true;
                return;
            }

            if (type == typeof(TransformCollection))
            {
                _transform = new TransformGroup()
                {
                    Children = (TransformCollection)value ?? new TransformCollection(),
                };
            }
        }

        /// <summary>
        /// Open the ListEditorDialog, with its contents being the list or collection of this property.
        /// </summary>
        public bool OpenTransformDialog()
        {
            TransformEditDialog ted = new TransformEditDialog();
            ted.ImportTransforms(new List<Transform>() { _transform });
            ted.ShowDialog();

            return ted.DialogResult ?? false;
        }

        private void mnuEmptyList_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            bool res = OpenTransformDialog();
            if (res)
            {
                ValueChanged?.Invoke(this, EventArgs.Empty);
            }

            // update appearance
        }

        private void mnuSetToNull_Click(object sender, RoutedEventArgs e)
        {
            if (_setNull)
            {
                mnuSetToNull.IsChecked = false;
                _setNull = false;
            }
            else
            {
                mnuSetToNull.IsChecked = true;
                _setNull = true;
            }
        }
    }
}
