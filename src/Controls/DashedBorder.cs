using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace OkoloIt.Wpf.Controls.Controls
{
    /// <summary>
    /// Пунктирная граница.
    /// </summary>
    public sealed class DashedBorder : Border
    {
        #region Public Fields

        /// <summary>
        /// Настройки кисти пунктирной границы.
        /// </summary>
        public static readonly DependencyProperty DashedBorderBrushProperty =
            DependencyProperty.Register(nameof(DashedBorderBrush),
                typeof(Brush),
                typeof(DashedBorder),
                new FrameworkPropertyMetadata(null)
            );

        /// <summary>
        /// Настройки массива пунктирной границы.
        /// </summary>
        public static readonly DependencyProperty StrokeDashArrayProperty =
            DependencyProperty.Register(nameof(StrokeDashArray),
                typeof(DoubleCollection),
                typeof(DashedBorder),
                new FrameworkPropertyMetadata(EmptyDoubleCollection())
            );

        /// <summary>
        /// Настройки использования пунктирной границы.
        /// </summary>
        public static readonly DependencyProperty UseDashedBorderProperty =
            DependencyProperty.Register(nameof(UseDashedBorder),
                typeof(bool),
                typeof(DashedBorder),
                new FrameworkPropertyMetadata(false, OnUseDashedBorderChanged)
            );

        #endregion Public Fields

        #region Private Fields

        private static DoubleCollection? emptyDoubleCollection;

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// Кисть.
        /// </summary>
        public Brush DashedBorderBrush {
            get { return (Brush)GetValue(DashedBorderBrushProperty); }
            set { SetValue(DashedBorderBrushProperty, value); }
        }

        /// <summary>
        /// Массив пунктиров.
        /// </summary>
        public DoubleCollection StrokeDashArray {
            get { return (DoubleCollection)GetValue(StrokeDashArrayProperty); }
            set { SetValue(StrokeDashArrayProperty, value); }
        }

        /// <summary>
        /// Использовать пунктирную границу.
        /// </summary>
        public bool UseDashedBorder {
            get { return (bool)GetValue(UseDashedBorderProperty); }
            set { SetValue(UseDashedBorderProperty, value); }
        }

        #endregion Public Properties

        #region Private Methods

        private static DoubleCollection EmptyDoubleCollection()
        {
            if (emptyDoubleCollection == null) {
                DoubleCollection doubleCollection = new DoubleCollection();
                doubleCollection.Freeze();
                emptyDoubleCollection = doubleCollection;
            }
            return emptyDoubleCollection;
        }

        private static void OnUseDashedBorderChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            DashedBorder dashedBorder = (DashedBorder)target;
            dashedBorder.UseDashedBorderChanged();
        }

        private VisualBrush CreateDashedBorderBrush()
        {
            VisualBrush dashedBorderBrush = new();
            Grid grid = new();
            Rectangle backgroundRectangle = GetBackgroundRectangle();
            Rectangle dashedRectangle = GetDashedRectangle();
            grid.Children.Add(backgroundRectangle);
            grid.Children.Add(dashedRectangle);
            dashedBorderBrush.Visual = grid;
            return dashedBorderBrush;
        }

        private Rectangle GetBackgroundRectangle()
        {
            Rectangle rectangle = GetBoundRectangle();

            rectangle.SetBinding(Rectangle.StrokeProperty, new Binding() {
                Source = this,
                Path = new PropertyPath(BackgroundProperty)
            });

            return rectangle;
        }

        private Rectangle GetBoundRectangle()
        {
            Rectangle rectangle = new();

            rectangle.SetBinding(Rectangle.StrokeThicknessProperty, new Binding() {
                Source = this,
                Path = new PropertyPath(nameof(BorderThickness.Left))
            });

            rectangle.SetBinding(Rectangle.RadiusXProperty, new Binding() {
                Source = this,
                Path = new PropertyPath(nameof(CornerRadius.TopLeft))
            });

            rectangle.SetBinding(Rectangle.RadiusYProperty, new Binding() {
                Source = this,
                Path = new PropertyPath(nameof(CornerRadius.TopLeft))
            });

            rectangle.SetBinding(Rectangle.WidthProperty, new Binding() {
                Source = this,
                Path = new PropertyPath(ActualWidthProperty)
            });

            rectangle.SetBinding(Rectangle.HeightProperty, new Binding() {
                Source = this,
                Path = new PropertyPath(ActualHeightProperty)
            });

            return rectangle;
        }

        private Rectangle GetDashedRectangle()
        {
            Rectangle rectangle = GetBoundRectangle();

            rectangle.SetBinding(Rectangle.StrokeDashArrayProperty, new Binding() {
                Source = this,
                Path = new PropertyPath(StrokeDashArrayProperty)
            });

            rectangle.SetBinding(Rectangle.StrokeProperty, new Binding() {
                Source = this,
                Path = new PropertyPath(DashedBorderBrushProperty)
            });

            Panel.SetZIndex(rectangle, 2);
            return rectangle;
        }

        private void UseDashedBorderChanged()
        {
            if (UseDashedBorder)
                BorderBrush = CreateDashedBorderBrush();
            else
                ClearValue(BorderBrushProperty);
        }

        #endregion Private Methods
    }
}