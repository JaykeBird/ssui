using SolidShineUi.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SolidShineUi.PropertyList.Dialogs
{
    /// <summary>
    /// Interaction logic for TransformEditDialog.xaml
    /// </summary>
    public partial class TransformEditDialog : FlatWindow
    {
        public TransformEditDialog()
        {
            InitializeComponent();
            Loaded += TransformEditDialog_Loaded;
            SetupWindow();
        }

        private void TransformEditDialog_Loaded(object sender, RoutedEventArgs e)
        {
            imgAdd.Source = IconLoader.LoadIcon("Add", ColorScheme);
            imgDelete.Source = IconLoader.LoadIcon("Delete", ColorScheme);
        }

        void SetupWindow()
        {
            SetValue(TransformListProperty, new SelectableCollection<TransformSelectableControl>());
            TransformList.SelectionChanged += TransformList_SelectionChanged;
            TransformList.CanSelectMultiple = false;
        }

        public SelectableCollection<TransformSelectableControl> TransformList { 
            get => (SelectableCollection<TransformSelectableControl>)GetValue(TransformListProperty); set => SetValue(TransformListProperty, value); }

        public static DependencyProperty TransformListProperty
            = DependencyProperty.Register("TransformList", typeof(SelectableCollection<TransformSelectableControl>), typeof(TransformEditDialog),
            new FrameworkPropertyMetadata(new SelectableCollection<TransformSelectableControl>()));

        private void TransformList_SelectionChanged(object sender, CollectionSelectionChangedEventArgs e)
        {
            btnDelete.IsEnabled = TransformList.SelectedItems.Count > 0;
            LoadSelectedTransform();
        }

        /// <summary>
        /// Import a collection of transforms to edit in this TransformEditDialog. This should be called prior to showing the dialog, but multiple calls will not remove existing values.
        /// </summary>
        /// <param name="transforms">The transforms to import into the dialog.</param>
        /// <remarks>
        /// To only edit one transform in Single Edit Mode, use <see cref="ImportSingleTransform(Transform)"/>. To get the list of transforms in the dialog, use <see cref="ExportTransformCollection"/> or <see cref="TransformList"/>.
        /// </remarks>
        public void ImportTransforms(TransformCollection transforms)
        {
            foreach (Transform item in transforms)
            {
                if (item is TransformGroup tg)
                {
                    ImportTransforms(tg);
                }
                AddTransform(item);
            }
        }

        /// <summary>
        /// Import a collection of transforms to edit in this TransformEditDialog. This should be called prior to showing the dialog, but multiple calls will not remove existing values.
        /// </summary>
        /// <param name="transforms">The transforms to import into the dialog.</param>
        /// <remarks>
        /// To only edit one transform in Single Edit Mode, use <see cref="ImportSingleTransform(Transform)"/>. To get the list of transforms in the dialog, use <see cref="ExportTransformCollection"/> or <see cref="TransformList"/>.
        /// </remarks>
        public void ImportTransforms(TransformGroup transforms)
        {
            foreach (Transform item in transforms.Children)
            {
                if (item is TransformGroup tg)
                {
                    ImportTransforms(tg);
                }
                AddTransform(item);
            }
        }

        /// <summary>
        /// Import a collection of transforms to edit in this TransformEditDialog. This should be called prior to showing the dialog, but multiple calls will not remove existing values.
        /// </summary>
        /// <param name="transforms">The transforms to import into the dialog.</param>
        /// <remarks>
        /// To only edit one transform in Single Edit Mode, use <see cref="ImportSingleTransform(Transform)"/>. To get the list of transforms in the dialog, use <see cref="ExportTransformCollection"/> or <see cref="TransformList"/>.
        /// </remarks>
        public void ImportTransforms(IEnumerable<Transform> transforms)
        {
            foreach (Transform item in transforms)
            {
                if (item is TransformGroup tg)
                {
                    ImportTransforms(tg);
                }
                AddTransform(item);
            }
        }

        /// <summary>
        /// Import a single transform for editing; this activates Single Edit Mode for the TransformEditDialog, which only allows this transform to be edited; no adding or deleting.
        /// This should be called prior to showing the dialog, but multiple calls will not remove existing values.
        /// </summary>
        /// <param name="t">The one transform to edit.</param>
        /// <remarks>If the Transform <paramref name="t"/> is a <see cref="TransformGroup"/>, use <see cref="ImportTransforms(TransformGroup)"/> instead.</remarks>
        public void ImportSingleTransform(Transform t)
        {
            if (t is TransformGroup tg)
            {
                // this singlie transform is actually a TransformGroup
                // which can hold multiple transforms
                // thus, the standard format should apply instead
                ImportTransforms(tg);
            }
            else
            {
                var tsc = AddTransform(t);
                singleEditMode = true;
                btnAdd.Visibility = Visibility.Collapsed;
                btnDelete.Visibility = Visibility.Collapsed;
                btnMoveDown.Visibility = Visibility.Collapsed;
                btnMoveUp.Visibility = Visibility.Collapsed;
                tsc.IsSelected = true;
            }
        }

        /// <summary>
        /// Get all of the transforms listed in this dialog, combined together into a collection.
        /// </summary>
        /// <returns>A TransformCollection that contains each transform listed in this dialog.</returns>
        public TransformCollection ExportTransformCollection()
        {
            TransformCollection transforms = new TransformCollection();
            StoreDataToSelectedTransform();
            foreach (TransformSelectableControl item in TransformList)
            {
                transforms.Add(item.TransformValue);
            }

            return transforms;
        }

        /// <summary>
        /// Get the value of the transform object being edited in this dialog.
        /// </summary>
        /// <remarks>
        /// If this dialog is in Single Edit Mode, the transform object returned should match the type of the object passed in before via <see cref="ImportSingleTransform(Transform)"/>.
        /// Otherwise, this will return a <see cref="TransformGroup"/> that contains all listed transforms in the dialog (or if only a single transform is in the dialog, just that transform).
        /// </remarks>
        public Transform ExportSingleTransform()
        {
            if (!singleEditMode)
            {
                if (TransformList.Count == 1)
                {
                    StoreDataToSelectedTransform();
                    return TransformList[0].TransformValue;
                }
                else
                {
                    return new TransformGroup() { Children = ExportTransformCollection() };
                }
            }
            else
            {
                if (TransformList.Count >= 1)
                {
                    StoreDataToSelectedTransform();
                    return TransformList[0].TransformValue;
                }
                else
                {
                    return new TransformGroup();
                }
            }
        }

        TransformSelectableControl AddTransform(Transform t)
        {
            TransformSelectableControl control = new TransformSelectableControl(t);
            TransformList.Add(control);
        }

        void LoadSelectedTransform()
        {
            var selItems = TransformList.SelectedItems;
            if (selItems.Count > 0)
            {
                var selItem = selItems[0];

                Transform selTransform = selItem.ObjectTransform;

                if (selTransform is RotateTransform r)
                {
                    // load rotation controls
                    DisplayTransformControl(grdRotate);
                }
                else if (selTransform is ScaleTransform s)
                {
                    // load scale controls
                    DisplayTransformControl(grdScale);
                }
                else if (selTransform is TranslateTransform u)
                {
                    // load translation controls
                    DisplayTransformControl(grdTranslate);
                }
                else if (selTransform is SkewTransform k)
                {
                    // load skew controls
                    DisplayTransformControl(grdSkew);
                }
                else if (selTransform is MatrixTransform m)
                {
                    // load matrix controls
                    DisplayTransformControl(null);
                }
                else if (selTransform is TransformGroup g)
                {
                    // oh dear
                    DisplayTransformControl(null);
                }
            }
            
        }

#if NETCOREAPP
        void DisplayTransformControl(Grid? transformControl)
#else
        void DisplayTransformControl(Grid transformControl)
#endif
        {
            if (transformControl == null)
            {
                // select nothing
                grdRotate.Visibility = Visibility.Collapsed;
                grdScale.Visibility = Visibility.Collapsed;
                grdTranslate.Visibility = Visibility.Collapsed;
                grdSkew.Visibility = Visibility.Collapsed;

                txtGridEmpty.Visibility = Visibility.Visible;
            }
            else if (grdHolder.Children.IndexOf(transformControl) == -1)
            {
                throw new ArgumentException();
            }
            else
            {
                // LET'S DO IT
                grdRotate.Visibility = (transformControl == grdRotate) ? Visibility.Visible : Visibility.Collapsed;
                grdScale.Visibility = (transformControl == grdScale) ? Visibility.Visible : Visibility.Collapsed;
                grdSkew.Visibility = (transformControl == grdSkew) ? Visibility.Visible : Visibility.Collapsed;
                grdTranslate.Visibility = (transformControl == grdTranslate) ? Visibility.Visible : Visibility.Collapsed;

                txtGridEmpty.Visibility = Visibility.Collapsed;
            }
        }

        #region Add menu
        private void mnuRotateAdd_Click(object sender, RoutedEventArgs e)
        {
            TransformSelectableControl tsc = AddTransform(new RotateTransform());
            selTransformList.Items.Select(tsc);
        }

        private void mnuSkewAdd_Click(object sender, RoutedEventArgs e)
        {
            TransformSelectableControl tsc = AddTransform(new SkewTransform());
            selTransformList.Items.Select(tsc);
        }

        private void mnuScaleAdd_Click(object sender, RoutedEventArgs e)
        {
            TransformSelectableControl tsc = AddTransform(new ScaleTransform(1, 1));
            selTransformList.Items.Select(tsc);
        }

        private void mnuTranslateAdd_Click(object sender, RoutedEventArgs e)
        {
            TransformSelectableControl tsc = AddTransform(new TranslateTransform());
            selTransformList.Items.Select(tsc);
        }

        private void mnuMatrixAdd_Click(object sender, RoutedEventArgs e)
        {
            TransformSelectableControl tsc = AddTransform(new MatrixTransform(Matrix.Identity));
            selTransformList.Items.Select(tsc);
        }
        #endregion

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (TransformList.SelectedItems.Count > 0)
            {
                TransformList.Remove(TransformList.SelectedItems[0]);
                DisplayTransformControl(null);
            }
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }

    public class TransformSelectableControl : SelectableItem
    {
        public TransformSelectableControl(Transform transform) : base()
        {
            ObjectTransform = transform;

            if (transform is RotateTransform r)
            {
                Text = "Rotate";
            }
            else if (transform is ScaleTransform s)
            {
                Text = "Scale";
            }
            else if (transform is TranslateTransform u)
            {
                Text = "Translate";
            }
            else if (transform is SkewTransform k)
            {
                Text = "Skew";
            }
            else if (transform is MatrixTransform m)
            {
                Text = "Custom (Matrix)";
            }
            else if (transform is TransformGroup g)
            {
                Text = "Transform Group";
            }
        }

        public Transform ObjectTransform { get; set; } = new MatrixTransform(Matrix.Identity);

    }
}
