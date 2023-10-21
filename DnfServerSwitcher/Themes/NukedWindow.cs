using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using DnfServerSwitcher.Views;
namespace DnfServerSwitcher.Themes {
    
    [TemplatePart(Name = "PART_backgroundLayer1", Type = typeof(Path))]
    [TemplatePart(Name = "PART_backgroundLayer2", Type = typeof(Path))]
    [TemplatePart(Name = "PART_focusButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_minimizeButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_maximizeButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_closeButton", Type = typeof(Button))]
    public class NukedWindow : Window {
        
        public static readonly DependencyProperty TitleZoneHeightProperty = DependencyProperty.Register(nameof(TitleZoneHeight),
            typeof(double),
            typeof(NukedWindow),
            new FrameworkPropertyMetadata(40.0, OnWindowShapeChanged)
            );
        
        public static readonly DependencyProperty TitleGapLeftProperty = DependencyProperty.Register(nameof(TitleGapLeft),
            typeof(double),
            typeof(NukedWindow),
            new FrameworkPropertyMetadata(double.NaN, OnWindowShapeChanged)
            );
        public static readonly DependencyProperty TitleBezierLeftWidthProperty = DependencyProperty.Register(nameof(TitleBezierLeftWidth),
            typeof(double),
            typeof(NukedWindow),
            new FrameworkPropertyMetadata(60.0, OnWindowShapeChanged)
            );
        public static readonly DependencyProperty TitleBezierLeftHeightProperty = DependencyProperty.Register(nameof(TitleBezierLeftHeight),
            typeof(double),
            typeof(NukedWindow),
            new FrameworkPropertyMetadata(16.0, OnWindowShapeChanged)
            );
        public static readonly DependencyProperty TitleBezierLeftOffsetProperty = DependencyProperty.Register(nameof(TitleBezierLeftOffset),
            typeof(double),
            typeof(NukedWindow),
            new FrameworkPropertyMetadata(20.0, OnWindowShapeChanged)
            );
        
        public static readonly DependencyProperty TitleGapRightProperty = DependencyProperty.Register(nameof(TitleGapRight),
            typeof(double),
            typeof(NukedWindow),
            new FrameworkPropertyMetadata(160.0, OnWindowShapeChanged)
            );
        public static readonly DependencyProperty TitleBezierRightWidthProperty = DependencyProperty.Register(nameof(TitleBezierRightWidth),
            typeof(double),
            typeof(NukedWindow),
            new FrameworkPropertyMetadata(60.0, OnWindowShapeChanged)
            );
        public static readonly DependencyProperty TitleBezierRightHeightProperty = DependencyProperty.Register(nameof(TitleBezierRightHeight),
            typeof(double),
            typeof(NukedWindow),
            new FrameworkPropertyMetadata(36.0, OnWindowShapeChanged)
            );
        public static readonly DependencyProperty TitleBezierRightOffsetProperty = DependencyProperty.Register(nameof(TitleBezierRightOffset),
            typeof(double),
            typeof(NukedWindow),
            new FrameworkPropertyMetadata(20.0, OnWindowShapeChanged)
            );
        
        public static readonly DependencyProperty TitleCornerRightWidthProperty = DependencyProperty.Register(nameof(TitleCornerRightWidth),
            typeof(double),
            typeof(NukedWindow),
            new FrameworkPropertyMetadata(40.0, OnWindowShapeChanged)
            );
        public static readonly DependencyProperty TitleCornerRightHeightProperty = DependencyProperty.Register(nameof(TitleCornerRightHeight),
            typeof(double),
            typeof(NukedWindow),
            new FrameworkPropertyMetadata(40.0, OnWindowShapeChanged)
            );
        public static readonly DependencyProperty TitleCornerRightQuadBezierRadiusProperty = DependencyProperty.Register(nameof(TitleCornerRightQuadBezierRadius),
            typeof(double),
            typeof(NukedWindow),
            new FrameworkPropertyMetadata(5.0, OnWindowShapeChanged)
            );

        public static readonly DependencyProperty QuadBezierRadiusProperty = DependencyProperty.Register(nameof(QuadBezierRadius),
            typeof(double),
            typeof(NukedWindow),
            new FrameworkPropertyMetadata(10.0, OnWindowShapeChanged)
            );
        
        public static readonly DependencyProperty SecondLayerPaddingProperty = DependencyProperty.Register(nameof(SecondLayerPadding),
            typeof(Thickness),
            typeof(NukedWindow),
            new FrameworkPropertyMetadata( new Thickness(0,0,12,0), OnWindowShapeChanged)
            );
        
        public static readonly DependencyProperty FirstLayerPaddingProperty = DependencyProperty.Register(nameof(FirstLayerPadding),
            typeof(Thickness),
            typeof(NukedWindow),
            new FrameworkPropertyMetadata(new Thickness(8,8,0,8), OnWindowShapeChanged)
            );
        public static readonly DependencyProperty ContentPaddingProperty = DependencyProperty.Register(nameof(ContentPadding),
            typeof(Thickness),
            typeof(NukedWindow),
            new FrameworkPropertyMetadata(new Thickness(8,8,8,8), OnWindowShapeChanged)
            );
        
        public double TitleZoneHeight {
            get => (double)this.GetValue(NukedWindow.TitleZoneHeightProperty);
            set => this.SetValue(NukedWindow.TitleZoneHeightProperty, value);
        }
        
        public double TitleGapLeft {
            get => (double)this.GetValue(NukedWindow.TitleGapLeftProperty);
            set => this.SetValue(NukedWindow.TitleGapLeftProperty, value);
        }
        public double TitleBezierLeftHeight {
            get => (double)this.GetValue(NukedWindow.TitleBezierLeftHeightProperty);
            set => this.SetValue(NukedWindow.TitleBezierLeftHeightProperty, value);
        }
        public double TitleBezierLeftWidth {
            get => (double)this.GetValue(NukedWindow.TitleBezierLeftWidthProperty);
            set => this.SetValue(NukedWindow.TitleBezierLeftWidthProperty, value);
        }
        public double TitleBezierLeftOffset {
            get => (double)this.GetValue(NukedWindow.TitleBezierLeftOffsetProperty);
            set => this.SetValue(NukedWindow.TitleBezierLeftOffsetProperty, value);
        }
        
        public double TitleGapRight {
            get => (double)this.GetValue(NukedWindow.TitleGapRightProperty);
            set => this.SetValue(NukedWindow.TitleGapRightProperty, value);
        }
        public double TitleBezierRightHeight {
            get => (double)this.GetValue(NukedWindow.TitleBezierRightHeightProperty);
            set => this.SetValue(NukedWindow.TitleBezierRightHeightProperty, value);
        }
        
        public double TitleBezierRightWidth {
            get => (double)this.GetValue(NukedWindow.TitleBezierRightWidthProperty);
            set => this.SetValue(NukedWindow.TitleBezierRightWidthProperty, value);
        }
        
        public double TitleBezierRightOffset {
            get => (double)this.GetValue(NukedWindow.TitleBezierRightOffsetProperty);
            set => this.SetValue(NukedWindow.TitleBezierRightOffsetProperty, value);
        }

        public double QuadBezierRadius {
            get => (double)this.GetValue(NukedWindow.QuadBezierRadiusProperty);
            set => this.SetValue(NukedWindow.QuadBezierRadiusProperty, value);
        }
        
        
        public double TitleCornerRightWidth {
            get => (double)this.GetValue(NukedWindow.TitleCornerRightWidthProperty);
            set => this.SetValue(NukedWindow.TitleCornerRightWidthProperty, value);
        }
        public double TitleCornerRightHeight {
            get => (double)this.GetValue(NukedWindow.TitleCornerRightHeightProperty);
            set => this.SetValue(NukedWindow.TitleCornerRightHeightProperty, value);
        }
        public double TitleCornerRightQuadBezierRadius {
            get => (double)this.GetValue(NukedWindow.TitleCornerRightQuadBezierRadiusProperty);
            set => this.SetValue(NukedWindow.TitleCornerRightQuadBezierRadiusProperty, value);
        }
        
        public Thickness SecondLayerPadding {
            get => (Thickness)this.GetValue(NukedWindow.SecondLayerPaddingProperty);
            set => this.SetValue(NukedWindow.SecondLayerPaddingProperty, value);
        }
        public Thickness FirstLayerPadding {
            get => (Thickness)this.GetValue(NukedWindow.FirstLayerPaddingProperty);
            set => this.SetValue(NukedWindow.FirstLayerPaddingProperty, value);
        }
        public Thickness ContentPadding {
            get => (Thickness)this.GetValue(NukedWindow.ContentPaddingProperty);
            set => this.SetValue(NukedWindow.ContentPaddingProperty, value);
        }


        private Border? part_windowContent = null;
        private StackPanel? part_windowTitleBar = null;
        private TextBlock? part_windowTitle = null;
        private Path? part_backgroundLayer1 = null;
        private Path? part_backgroundLayer2 = null;
        private Button? part_focusButton = null;
        private Button? part_minimizeButton = null;
        private Button? part_maximizeButton = null;
        private Button? part_closeButton = null;

        private const string _StylesSource = "pack://application:,,,/DnfServerSwitcher;component/Themes/Styles.xaml";
        
        public NukedWindow() {
            this.StateChanged += this.OnStateChanged;
            this.SizeChanged += this.OnSizeChanged;

            ResourceDictionary rd = new ResourceDictionary() {
                Source = new Uri(NukedWindow._StylesSource, UriKind.Absolute),
            };
            this.Resources.MergedDictionaries.Add(rd);
            this.SetResourceReference(NukedWindow.StyleProperty, "NukedStyleWindow");
        }
        
        public override void OnApplyTemplate() {
            base.OnApplyTemplate();

            if (this.Template == null) { return; }

            if (this.Template.FindName("PART_focusButton", this) is Button focusButton) {
                this.part_focusButton = focusButton;
            } else {
                this.part_focusButton = null;
            }
            
            if (this.Template.FindName("PART_minimizeButton", this) is Button minimizeButton) {
                if (this.part_minimizeButton != minimizeButton) {
                    if (this.part_minimizeButton != null) {
                        this.part_minimizeButton.Click -= this.MinimizeWindow;
                    }
                    this.part_minimizeButton = minimizeButton;
                    this.part_minimizeButton.Click += this.MinimizeWindow;
                }
            } else {
                if (this.part_minimizeButton != null) {
                    this.part_minimizeButton.Click -= this.MinimizeWindow;
                }
                this.part_minimizeButton = null;
            }
            
            if (this.Template.FindName("PART_maximizeButton", this) is Button maximizeButton) {
                if (this.part_maximizeButton != maximizeButton) {
                    if (this.part_maximizeButton != null) {
                        this.part_maximizeButton.Click -= this.MaximizeWindow;
                    }
                    this.part_maximizeButton = maximizeButton;
                    this.part_maximizeButton.Click += this.MaximizeWindow;
                }
            } else {
                if (this.part_maximizeButton != null) {
                    this.part_maximizeButton.Click -= this.MaximizeWindow;
                }
                this.part_maximizeButton = null;
            }
            
            if (this.Template.FindName("PART_closeButton", this) is Button closeButton) {
                if (this.part_closeButton != closeButton) {
                    if (this.part_closeButton != null) {
                        this.part_closeButton.Click -= this.CloseWindow;
                    }
                    this.part_closeButton = closeButton;
                    this.part_closeButton.Click += this.CloseWindow;
                }
            } else {
                if (this.part_closeButton != null) {
                    this.part_closeButton.Click -= this.CloseWindow;
                }
                this.part_closeButton = null;
            }
            
            if (this.Template.FindName("PART_backgroundLayer1", this) is Path layer0) {
                this.part_backgroundLayer1 = layer0;
            } else {
                this.part_backgroundLayer1 = null;
            }
            
            if (this.Template.FindName("PART_backgroundLayer2", this) is Path layer1) {
                this.part_backgroundLayer2 = layer1;
            } else {
                this.part_backgroundLayer2 = null;
            }
            
            if (this.Template.FindName("PART_windowTitle", this) is TextBlock windowTitle) {
                this.part_windowTitle = windowTitle;
            } else {
                this.part_windowTitle = null;
            }
            
            if (this.Template.FindName("PART_windowTitleBar", this) is StackPanel windowTitleBar) {
                this.part_windowTitleBar = windowTitleBar;
            } else {
                this.part_windowTitleBar = null;
            }
            
            if (this.Template.FindName("PART_windowContent", this) is Border windowContent) {
                this.part_windowContent = windowContent;
            } else {
                this.part_windowContent = null;
            }
            
            this.RecalculatePaths();
        }
        
        private void OnSizeChanged(object sender, SizeChangedEventArgs e) {
            this.RecalculatePaths();
        }
        private void OnStateChanged(object sender, EventArgs e) {
            this.RecalculatePaths();
        }
        
        private static void OnWindowShapeChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e) {
            if (d is not NukedWindow win) return;
            win.RecalculatePaths();
        }

        private void CloseWindow(object sender, RoutedEventArgs e) {
            this.part_focusButton?.Focus();
            Application.Current.Shutdown();
        }
        
        private void MaximizeWindow(object sender, RoutedEventArgs e) {
            this.part_focusButton?.Focus();
            switch (this.WindowState) {
                case WindowState.Maximized:
                    this.WindowState = WindowState.Normal;
                    break;
                case WindowState.Normal:
                    this.WindowState = WindowState.Maximized;
                    break;
            }
        }
        private void MinimizeWindow(object sender, RoutedEventArgs e) {
            this.part_focusButton?.Focus();
            this.WindowState = WindowState.Minimized;
        }

        private void RecalculatePaths() {
            if (this.ActualWidth == 0 || this.ActualHeight == 0 ||
                double.IsNaN(this.ActualWidth) || double.IsNaN(this.ActualHeight)) {
                if (this.part_backgroundLayer1 != null) {
                    this.part_backgroundLayer1.Data = Geometry.Empty;
                }
                if (this.part_backgroundLayer2 != null) {
                    this.part_backgroundLayer2.Data = Geometry.Empty;
                }
                return;
            }
            double width = this.ActualWidth - this.Padding.Left - this.Padding.Right;
            double height = this.ActualHeight - this.Padding.Top - this.Padding.Bottom;
            
            double gapLeft = double.IsNaN(this.TitleGapLeft)
                ? (width - this.TitleGapRight) / 3.0
                : this.TitleGapLeft;
            double gapRight = double.IsNaN(this.TitleGapRight) 
                ? this.TitleBezierRightWidth * 2
                : this.TitleGapRight;

            if (this.part_backgroundLayer1 != null) {
                DrawPathLayer1(this.part_backgroundLayer1,
                    this.FirstLayerPadding,
                    width, height, 
                    this.QuadBezierRadius,
                    gapLeft,
                    this.TitleBezierLeftWidth, this.TitleBezierLeftHeight, this.TitleBezierLeftOffset,
                    this.TitleCornerRightWidth, this.TitleCornerRightHeight, this.TitleCornerRightQuadBezierRadius);
            }

            if (this.part_backgroundLayer2 != null) {
                DrawPathLayer2(this.part_backgroundLayer2,
                    this.SecondLayerPadding,
                    width, height, this.QuadBezierRadius,
                    gapLeft, this.TitleBezierLeftWidth, this.TitleBezierLeftHeight, this.TitleBezierLeftOffset,
                    gapRight, this.TitleBezierRightWidth, this.TitleBezierRightHeight, this.TitleBezierRightOffset
                    );
            }

            if (this.part_windowTitle != null) {
                double ah = this.part_windowTitle.ActualHeight;
                double centeringOffset = (this.TitleZoneHeight - this.TitleBezierLeftHeight - ah) /2;
                this.part_windowTitle.Margin = new Thickness(
                    this.SecondLayerPadding.Left + this.ContentPadding.Left, this.SecondLayerPadding.Top + this.TitleBezierLeftHeight + centeringOffset, 0, 0);
            }
            if (this.part_windowTitleBar != null) {
                this.part_windowTitleBar.Margin = new Thickness(
                    0, this.FirstLayerPadding.Top, this.FirstLayerPadding.Right + this.TitleCornerRightWidth, 0);
            }
            if (this.part_windowContent != null) {
                this.part_windowContent.Margin = new Thickness(
                    this.ContentPadding.Left + this.SecondLayerPadding.Left, 
                    this.ContentPadding.Top + this.SecondLayerPadding.Top + this.TitleZoneHeight, 
                    this.ContentPadding.Right + this.SecondLayerPadding.Right, 
                    this.ContentPadding.Bottom + this.SecondLayerPadding.Bottom);
            }
        }

        public static void DrawPathLayer1(Path target, Thickness pad, double width, double height, double quadRadius, 
            double gapLeft, double bezLeftWidth, double bezLeftHeight, double bezLeftCurveOffset, 
            double cornerRightWidth, double cornerRightHeight, double cornerQuadRadius) {
            
            double wadj = width - pad.Left - pad.Right;
            double hadj = height - pad.Top - pad.Bottom;

            if (gapLeft < bezLeftWidth / 2) gapLeft = bezLeftWidth / 2;

            Point tl = new Point(pad.Left, pad.Top);
            Point tr = new Point(pad.Left + wadj, pad.Top);
            Point bl = new Point( pad.Left, pad.Top + hadj);
            Point br = new Point( pad.Left + wadj, pad.Top + hadj);


            // quadratic bezier top left small
            Point A0 = new Point(tl.X, tl.Y + bezLeftHeight + quadRadius);
            Point A1 = new Point(tl.X, tl.Y + bezLeftHeight);
            Point A2 = new Point(tl.X + quadRadius, tl.Y + bezLeftHeight);
            // bezier top left
            Point B0 = new Point(tl.X + gapLeft - bezLeftWidth / 2, tl.Y + bezLeftHeight);
            Point B1 = new Point(tl.X + gapLeft + bezLeftCurveOffset, tl.Y + bezLeftHeight);
            Point B2 = new Point(tl.X + gapLeft - bezLeftCurveOffset, tl.Y);
            Point B3 = new Point(tl.X + gapLeft + bezLeftWidth / 2, tl.Y );
            // corner right - quadratic bezier 1
            Point C0 = new Point(tr.X - cornerRightWidth - cornerQuadRadius, tr.Y);
            Point C1 = new Point(tr.X - cornerRightWidth, tr.Y);
            Point C2 = new Point(tr.X - cornerRightWidth + cornerQuadRadius, tr.Y + cornerQuadRadius);
            // corner right - quadratic bezier 2
            Point D0 = new Point(tr.X - cornerQuadRadius, tr.Y + cornerRightHeight - cornerQuadRadius);
            Point D1 = new Point(tr.X, tr.Y + cornerRightHeight);
            Point D2 = new Point(tr.X, tr.Y + cornerRightHeight + cornerQuadRadius);
            // quadratic bezier bottom right
            Point E0 = new Point(br.X, br.Y - quadRadius);
            Point E1 = new Point(br.X, br.Y);
            Point E2 = new Point(br.X - quadRadius, br.Y);
            // quadratic bezier bottom left
            Point F0 = new Point(bl.X + quadRadius, bl.Y);
            Point F1 = new Point(bl.X, bl.Y);
            Point F2 = new Point(bl.X, bl.Y - quadRadius);


            PathFigure fig = new PathFigure();
            fig.StartPoint = A0;
            fig.Segments.Add(new QuadraticBezierSegment(A1, A2, true));
            fig.Segments.Add(new LineSegment(B0, true));
            fig.Segments.Add(new BezierSegment(B1, B2, B3, true));
            fig.Segments.Add(new LineSegment(C0, true));
            fig.Segments.Add(new QuadraticBezierSegment(C1, C2,true));
            fig.Segments.Add(new LineSegment(D0, true));
            fig.Segments.Add(new QuadraticBezierSegment(D1, D2, true));
            fig.Segments.Add(new LineSegment(E0, true));
            fig.Segments.Add(new QuadraticBezierSegment(E1, E2, true));
            fig.Segments.Add(new LineSegment(F0, true));
            fig.Segments.Add(new QuadraticBezierSegment(F1, F2, true));
            fig.Segments.Add(new LineSegment(A0, true));

            PathGeometry geometry0 = new PathGeometry();
            geometry0.Figures.Add(fig);
            target.Data = geometry0;
        }

        public static void DrawPathLayer2(Path target, Thickness pad, double width, double height, double quadRadius, 
            double gapLeft, double bezLeftWidth, double bezLeftHeight, double bezLeftCurveOffset,
            double gapRight, double bezRightWidth, double bezRightHeight, double bezRightCurveOffset) {
            double wadj = width - pad.Left - pad.Right;
            double hadj = height - pad.Top - pad.Bottom;

            if (gapLeft < bezLeftWidth / 2) gapLeft = bezLeftWidth / 2;
            if (gapRight < bezRightWidth / 2) gapRight = bezRightWidth / 2;

            Point tl = new Point(pad.Left, pad.Top);
            Point tr = new Point(pad.Left + wadj, pad.Top);
            Point bl = new Point( pad.Left, pad.Top + hadj);
            Point br = new Point( pad.Left + wadj, pad.Top + hadj);


            // quadratic bezier top left small
            Point A0 = new Point(tl.X, tl.Y + bezLeftHeight + quadRadius);
            Point A1 = new Point(tl.X, tl.Y + bezLeftHeight);
            Point A2 = new Point(tl.X + quadRadius, tl.Y + bezLeftHeight);
            // bezier top left
            Point B0 = new Point(tl.X + gapLeft - bezLeftWidth / 2, tl.Y + bezLeftHeight);
            Point B1 = new Point(tl.X + gapLeft + bezLeftCurveOffset, tl.Y + bezLeftHeight);
            Point B2 = new Point(tl.X + gapLeft - bezLeftCurveOffset, tl.Y);
            Point B3 = new Point(tl.X + gapLeft + bezLeftWidth / 2, tl.Y );
            // bezier top right
            Point C0 = new Point(tr.X - gapRight - bezRightWidth / 2, tr.Y);
            Point C1 = new Point(tr.X - gapRight + bezRightCurveOffset, tr.Y);
            Point C2 = new Point(tr.X - gapRight - bezRightCurveOffset, tr.Y + bezRightHeight);
            Point C3 = new Point(tr.X - gapRight + bezRightWidth / 2, tr.Y + bezRightHeight);
            // quadratic bezier top right
            Point D0 = new Point(tr.X - quadRadius, tr.Y + bezRightHeight);
            Point D1 = new Point(tr.X, tr.Y + bezRightHeight);
            Point D2 = new Point(tr.X, tr.Y + bezRightHeight + quadRadius);
            // quadratic bezier bottom right
            Point E0 = new Point(br.X, br.Y - quadRadius);
            Point E1 = new Point(br.X, br.Y);
            Point E2 = new Point(br.X - quadRadius, br.Y);
            // quadratic bezier bottom left
            Point F0 = new Point(bl.X + quadRadius, bl.Y);
            Point F1 = new Point(bl.X, bl.Y);
            Point F2 = new Point(bl.X, bl.Y - quadRadius);


            PathFigure fig = new PathFigure();
            fig.StartPoint = A0;
            fig.Segments.Add(new QuadraticBezierSegment(A1, A2, true));
            fig.Segments.Add(new LineSegment(B0, true));
            fig.Segments.Add(new BezierSegment(B1, B2, B3, true));
            fig.Segments.Add(new LineSegment(C0, true));
            fig.Segments.Add(new BezierSegment(C1, C2, C3, true));
            fig.Segments.Add(new LineSegment(D0, true));
            fig.Segments.Add(new QuadraticBezierSegment(D1, D2, true));
            fig.Segments.Add(new LineSegment(E0, true));
            fig.Segments.Add(new QuadraticBezierSegment(E1, E2, true));
            fig.Segments.Add(new LineSegment(F0, true));
            fig.Segments.Add(new QuadraticBezierSegment(F1, F2, true));
            fig.Segments.Add(new LineSegment(A0, true));

            PathGeometry geometry0 = new PathGeometry();
            geometry0.Figures.Add(fig);
            target.Data = geometry0;
        }

    }
}
