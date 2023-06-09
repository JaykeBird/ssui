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
    /// A dialog to edit <see cref="Transform"/> objects. Many WPF objects/elements have properties that can use transforms to affect how they appear on screen.
    /// </summary>
    public partial class TransformEditDialog : FlatWindow
    {
        /// <summary>
        /// Create a TransformEditDialog.
        /// </summary>
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
            imgMoveDown.Source = IconLoader.LoadIcon("Down", ColorScheme);
            imgMoveUp.Source = IconLoader.LoadIcon("Up", ColorScheme);
        }

        void SetupWindow()
        {
            SetValue(TransformListProperty, new SelectableCollection<TransformSelectableControl>());
            TransformList.SelectionChanged += TransformList_SelectionChanged;
            TransformList.CanSelectMultiple = false;
        }

        /// <summary>
        /// Get or set the list of controls that represents each transform to display in the dialog. This is generally used internally, but this can also be used as a reference of what's currently in the dialog.
        /// </summary>
        public SelectableCollection<TransformSelectableControl> TransformList { 
            get => (SelectableCollection<TransformSelectableControl>)GetValue(TransformListProperty); set => SetValue(TransformListProperty, value); }

        public static DependencyProperty TransformListProperty
            = DependencyProperty.Register("TransformList", typeof(SelectableCollection<TransformSelectableControl>), typeof(TransformEditDialog),
            new FrameworkPropertyMetadata(new SelectableCollection<TransformSelectableControl>()));

        private void TransformList_SelectionChanged(object sender, CollectionSelectionChangedEventArgs e)
        {
            btnDelete.IsEnabled = TransformList.SelectedItems.Count > 0;
            btnMoveDown.IsEnabled = TransformList.SelectedItems.Count > 0;
            btnMoveUp.IsEnabled = TransformList.SelectedItems.Count > 0;

            LoadSelectedTransform();
        }

        void AddTransform(Transform t)
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
                    DisplayTransformControl(grdMatrix);
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
                grdMatrix.Visibility = Visibility.Collapsed;

                txtGridEmpty.Visibility = Visibility.Visible;
            }
            else if (grdHolder.Children.IndexOf(transformControl) == -1)
            {
                throw new ArgumentException();
            }
            else
            {
                foreach (UIElement item in grdHolder.Children)
                {
                    item.Visibility = item == transformControl ? Visibility.Visible : Visibility.Collapsed;
                }
                // LET'S DO IT
                //grdRotate.Visibility = (transformControl == grdRotate) ? Visibility.Visible : Visibility.Collapsed;
                //grdScale.Visibility = (transformControl == grdScale) ? Visibility.Visible : Visibility.Collapsed;
                //grdSkew.Visibility = (transformControl == grdSkew) ? Visibility.Visible : Visibility.Collapsed;
                //grdTranslate.Visibility = (transformControl == grdTranslate) ? Visibility.Visible : Visibility.Collapsed;
                //grdMatrix.Visibility = (transformControl == grdMatrix) ? Visibility.Visible : Visibility.Collapsed;

                //txtGridEmpty.Visibility = Visibility.Collapsed;
            }
        }

        #region Add menu
        private void mnuRotateAdd_Click(object sender, RoutedEventArgs e)
        {
            AddTransform(new RotateTransform());
        }

        private void mnuSkewAdd_Click(object sender, RoutedEventArgs e)
        {
            AddTransform(new SkewTransform());
        }

        private void mnuScaleAdd_Click(object sender, RoutedEventArgs e)
        {
            AddTransform(new ScaleTransform(1, 1));
        }

        private void mnuTranslateAdd_Click(object sender, RoutedEventArgs e)
        {
            AddTransform(new TranslateTransform());
        }

        private void mnuMatrixAdd_Click(object sender, RoutedEventArgs e)
        {
            AddTransform(new MatrixTransform(Matrix.Identity));
        }
        #endregion

        private void btnMoveUp_Click(object sender, RoutedEventArgs e)
        {
            selTransformList.MoveItemUp(TransformList.IndexOf(TransformList.SelectedItems[0]));
        }

        private void btnMoveDown_Click(object sender, RoutedEventArgs e)
        {
            selTransformList.MoveItemDown(TransformList.IndexOf(TransformList.SelectedItems[0]));
        }

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
                Text = "Matrix";
            }
            else if (transform is TransformGroup g)
            {
                Text = "Transform Group";
            }
        }

        public Transform ObjectTransform { get; set; } = new MatrixTransform(Matrix.Identity);

    }
}
