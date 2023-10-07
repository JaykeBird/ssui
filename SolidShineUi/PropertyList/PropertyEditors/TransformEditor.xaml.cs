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
        bool _specificType = false;

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
            else if (type == typeof(TransformGroup))
            {
                _transform = (TransformGroup)value;
            }
            else if (type == typeof(Transform))
            {
                _transform = (Transform)value;
            }
            // okay, beyond these, the property is looking for a specific type
            else if (type == typeof(MatrixTransform))
            {
                _transform = (MatrixTransform)value;
                _specificType = true;
            }
            else if (type == typeof(RotateTransform))
            {
                _transform = (RotateTransform)value;
                _specificType = true;
            }
            else if (type == typeof(SkewTransform))
            {
                _transform = (SkewTransform)value;
                _specificType = true;
            }
            else if (type == typeof(ScaleTransform))
            {
                _transform = (ScaleTransform)value;
                _specificType = true;
            }
            else if (type == typeof(TranslateTransform))
            {
                _transform = (TranslateTransform)value;
                _specificType = true;
            }
            else
            {
                //... uhhhhh...
                _transform = (Transform)value;
            }

            UpdateText();
        }

        /// <summary>
        /// Open the ListEditorDialog, with its contents being the list or collection of this property.
        /// </summary>
        public bool OpenTransformDialog()
        {
            TransformEditDialog ted = new TransformEditDialog();
            ted.ImportTransforms(new List<Transform>() { _transform });
            ted.ShowDialog();

            if (ted.DialogResult)
            {
                TransformCollection tc = ted.ExportTransforms();
                if (tc.Count == 0)
                {
                    // no transforms
                    _transform = new TransformGroup();
                }
                else if (tc.Count == 1)
                {
                    // one transform
                    _transform = tc[0];
                }
                else
                {
                    _transform = new TransformGroup() { Children = tc };
                }
            }

            return ted.DialogResult;
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            bool res = OpenTransformDialog();
            if (res)
            {
                ValueChanged?.Invoke(this, EventArgs.Empty);
            }

            // update appearance
            UpdateText();
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

            UpdateText();
        }

        void UpdateText()
        {
            Type tt = _transform.GetType();
            if (_setNull)
            {
                txtData.Text = "(null)";
            }
            else if (tt == typeof(TransformGroup))
            {
                TransformGroup tg = (TransformGroup)_transform;
                if (tg.Children.Count > 0)
                {
                    txtData.Text = $"{GROUP}: {tg.Children.Count} items";
                }
                else
                {
                    txtData.Text = $"{GROUP}: no transforms";
                }
            }
            else if (tt == typeof(RotateTransform))
            {
                txtData.Text = $"{ROTATE}: {((RotateTransform)_transform).Angle}";
            }
            else if (tt == typeof(SkewTransform))
            {
                txtData.Text = $"{SKEW}: X {((SkewTransform)_transform).AngleX}, Y {((SkewTransform)_transform).AngleY}";
            }
            else if (tt == typeof(ScaleTransform))
            {
                txtData.Text = $"{SCALE}: {((ScaleTransform)_transform).ScaleX} by {((ScaleTransform)_transform).ScaleY}";
            }
            else if (tt == typeof(TranslateTransform))
            {
                txtData.Text = $"{TRANSLATE}: {((TranslateTransform)_transform).X}, {((TranslateTransform)_transform).Y}";
            }
            else if (tt == typeof(MatrixTransform))
            {
                txtData.Text = $"{MATRIX}";
            }
            else
            {
                txtData.Text = _transform.Value.ToString();
            }
        }

        // eventually, once I figure out a localization solution, these will be moved to that spot
        /// <summary>UI text for "group"</summary>
        public const string GROUP = "group";
        /// <summary>UI text for "rotate"</summary>
        public const string ROTATE = "rotate";
        /// <summary>UI text for "skew"</summary>
        public const string SKEW = "skew";
        /// <summary>UI text for "scale"</summary>
        public const string SCALE = "scale";
        /// <summary>UI text for "translate"</summary>
        public const string TRANSLATE = "translate";
        /// <summary>UI text for "matrix"</summary>
        public const string MATRIX = "matrix";
    }
}
