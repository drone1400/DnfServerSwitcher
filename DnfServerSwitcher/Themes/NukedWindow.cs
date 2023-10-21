using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using DnfServerSwitcher.Views;
namespace DnfServerSwitcher.Themes {
    
    [TemplatePart(Name = "PART_backgroundLayer0", Type = typeof(Path))]
    [TemplatePart(Name = "PART_backgroundLayer1", Type = typeof(Path))]
    [TemplatePart(Name = "PART_focusButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_minimizeButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_maximizeButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_closeButton", Type = typeof(Button))]
    public class NukedWindow : Window {

        public static readonly DependencyProperty ChildContentProperty = DependencyProperty.Register(nameof(ChildContent),
            typeof(object),
            typeof(NukedWindow),
            new FrameworkPropertyMetadata(null)
            );
        
        public static readonly DependencyProperty TitleZoneHeightProperty = DependencyProperty.Register(nameof(TitleZoneHeight),
            typeof(double),
            typeof(NukedWindow),
            new FrameworkPropertyMetadata(50.0, OnWindowShapeChanged)
            );

        public static readonly DependencyProperty TitleGapLeftProperty = DependencyProperty.Register(nameof(TitleGapLeft),
            typeof(double),
            typeof(NukedWindow),
            new FrameworkPropertyMetadata(double.NaN, OnWindowShapeChanged)
            );
        public static readonly DependencyProperty TitleGapRightProperty = DependencyProperty.Register(nameof(TitleGapRight),
            typeof(double),
            typeof(NukedWindow),
            new FrameworkPropertyMetadata(180.0, OnWindowShapeChanged)
            );
        public static readonly DependencyProperty RadiusSmallProperty = DependencyProperty.Register(nameof(RadiusSmall),
            typeof(double),
            typeof(NukedWindow),
            new FrameworkPropertyMetadata(10.0, OnWindowShapeChanged)
            );

        public static readonly DependencyProperty RadiusBigProperty = DependencyProperty.Register(nameof(RadiusBig),
            typeof(double),
            typeof(NukedWindow),
            new FrameworkPropertyMetadata(16.0, OnWindowShapeChanged)
            );

        public object ChildContent {
            get => this.GetValue(NukedWindow.ChildContentProperty);
            set => this.SetValue(NukedWindow.ChildContentProperty, value);
        }
        
        public double TitleZoneHeight {
            get => (double)this.GetValue(NukedWindow.TitleZoneHeightProperty);
            set => this.SetValue(NukedWindow.TitleZoneHeightProperty, value);
        }

        public double TitleGapLeft {
            get => (double)this.GetValue(NukedWindow.TitleGapLeftProperty);
            set => this.SetValue(NukedWindow.TitleGapLeftProperty, value);
        }

        public double TitleGapRight {
            get => (double)this.GetValue(NukedWindow.TitleGapRightProperty);
            set => this.SetValue(NukedWindow.TitleGapRightProperty, value);
        }

        public double RadiusSmall {
            get => (double)this.GetValue(NukedWindow.RadiusSmallProperty);
            set => this.SetValue(NukedWindow.RadiusSmallProperty, value);
        }

        public double RadiusBig {
            get => (double)this.GetValue(NukedWindow.RadiusBigProperty);
            set => this.SetValue(NukedWindow.RadiusBigProperty, value);
        }


        private Border? part_windowContent = null;
        private StackPanel? part_windowTitleBar = null;
        private TextBlock? part_windowTitle = null;
        private Path? part_backgroundLayer0 = null;
        private Path? part_backgroundLayer1 = null;
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
            
            if (this.Template.FindName("PART_backgroundLayer0", this) is Path layer0) {
                this.part_backgroundLayer0 = layer0;
            } else {
                this.part_backgroundLayer0 = null;
            }
            
            if (this.Template.FindName("PART_backgroundLayer1", this) is Path layer1) {
                this.part_backgroundLayer1 = layer1;
            } else {
                this.part_backgroundLayer1 = null;
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
            double width = this.ActualWidth;
            double height = this.ActualHeight;
            
            double gapLeft = double.IsNaN(this.TitleGapLeft)
                ? (width - this.Padding.Left - this.Padding.Right - this.TitleGapRight) / 3.0
                : this.TitleGapLeft;

            if (this.part_backgroundLayer0 != null) {
                DrawPathLayer0(this.part_backgroundLayer0,
                    new Thickness(this.Padding.Left + this.RadiusBig, this.Padding.Top + this.RadiusSmall, + this.Padding.Right, this.Padding.Bottom + this.RadiusBig),
                    width, height, this.RadiusBig, this.RadiusSmall,
                    gapLeft,
                    1.5 * this.TitleZoneHeight, this.TitleZoneHeight,
                    this.TitleZoneHeight, this.TitleZoneHeight);
            }

            if (this.part_backgroundLayer1 != null) {
                DrawPathLayer1(this.part_backgroundLayer1,
                    new Thickness(this.Padding.Left, this.Padding.Top , this.Padding.Right + this.RadiusBig, this.Padding.Bottom),
                    width, height, this.RadiusBig, this.RadiusSmall,
                    gapLeft,
                    1.5 * this.TitleZoneHeight, this.TitleZoneHeight / 2,
                    1.5 * this.TitleZoneHeight, this.TitleZoneHeight,
                    this.TitleGapRight);
            }

            if (this.part_windowTitle != null) {
                this.part_windowTitle.Margin = new Thickness(
                    this.Padding.Left + 12, this.Padding.Top + this.TitleZoneHeight / 2 + 12, 0, 0);
            }
            if (this.part_windowTitleBar != null) {
                this.part_windowTitleBar.Margin = new Thickness(
                    0, this.Padding.Top + 12, this.Padding.Right + this.TitleZoneHeight, 0);
            }
            if (this.part_windowContent != null) {
                this.part_windowContent.Margin = new Thickness(
                    this.Padding.Left + 8, this.Padding.Top + this.TitleZoneHeight + 12, this.Padding.Right + this.RadiusBig + 8, this.Padding.Bottom + 8);
            }
        }

        public static void DrawPathLayer0(Path target, Thickness pad, double width, double height, double rb, double rs, double gapLeft, double w1, double h1, double w2, double h2) {
            double wadj = width - pad.Left - pad.Right - rb;
            double hadj = height - pad.Top - pad.Bottom - rb;

            if (w1 < 3 * rb) w1 = 3 * rb;
            if (h1 < 2 * rb) h1 = 2 * rb;
            if (gapLeft < w1 / 2 + rs) gapLeft = w1 / 2 + rs;
            if (w2 < 3 * rs) w2 = 3 * rs;
            if (h2 < 3 * rs) h2 = 3 * rs;

            Point tl = new Point(pad.Left, pad.Top);
            Point tr = new Point(pad.Left + wadj, pad.Top);
            Point bl = new Point( pad.Left, pad.Top + hadj);
            Point br = new Point( pad.Left + wadj, pad.Top + hadj);


            // bezier top left small
            Point A0 = new Point(tl.X, tl.Y + h1 + rs);
            Point A1 = new Point(tl.X, tl.Y + h1 + rs);
            Point A2 = new Point(tl.X, tl.Y + h1);
            Point A3 = new Point(tl.X + rs, tl.Y + h1);
            // beziers top / 1
            Point B0 = new Point(tl.X + gapLeft - w1 / 2, tl.Y + h1);
            Point B1 = new Point(tl.X + gapLeft - h1 / 2, tl.Y + h1);
            Point B2 = new Point(tl.X + gapLeft - h1 / 2 + rb, tl.Y + h1 - rb);
            Point B3 = new Point(tl.X + gapLeft, tl.Y + h1 / 2);
            // beziers top / 2
            //Point C0 = B3; // not used...
            Point C1 = new Point(tl.X + gapLeft + h1 / 2 - rb, tl.Y + rb);
            Point C2 = new Point(tl.X + gapLeft + h1 / 2, tl.Y);
            Point C3 = new Point(tl.X + gapLeft + w1 / 2, tl.Y);
            // beziers top \ 1
            Point D0 = new Point(tr.X - w2 - rs, tr.Y);
            Point D1 = new Point(tr.X - w2, tr.Y);
            Point D2 = new Point(tr.X - w2 + rs, tr.Y + rs);
            Point D3 = new Point(tr.X - w2 / 2 - w2 / 4, tr.Y + h2 / 4);
            // beziers top \ 2
            Point E0 = new Point(tr.X - w2 / 4, tr.Y + h2 / 2 + h2 / 4);
            Point E1 = new Point(tr.X - rs, tr.Y + h2 - rs);
            Point E2 = new Point(tr.X, tr.Y + h2);
            Point E3 = new Point(tr.X, tr.Y + h2 + rs);
            // bezier bottom right
            Point F0 = new Point(br.X, br.Y - rs);
            Point F1 = F0;
            Point F2 = new Point(br.X, br.Y);
            Point F3 = new Point(br.X - rs, br.Y);
            // bezier bottom left
            Point G0 = new Point(bl.X + rs, bl.Y);
            Point G1 = G0;
            Point G2 = new Point(bl.X, bl.Y);
            Point G3 = new Point(bl.X, bl.Y - rs);


            PathFigure fig = new PathFigure();
            fig.StartPoint = A0;
            fig.Segments.Add(new BezierSegment(A1, A2, A3, true));
            fig.Segments.Add(new LineSegment(B0, true));
            fig.Segments.Add(new BezierSegment(B1, B2, B3, true));
            fig.Segments.Add(new BezierSegment(C1, C2, C3, true));
            fig.Segments.Add(new LineSegment(D0, true));
            fig.Segments.Add(new BezierSegment(D1, D2, D3, true));
            fig.Segments.Add(new LineSegment(E0, true));
            fig.Segments.Add(new BezierSegment(E1, E2, E3, true));
            fig.Segments.Add(new LineSegment(F0, true));
            fig.Segments.Add(new BezierSegment(F1, F2, F3, true));
            fig.Segments.Add(new LineSegment(G0, true));
            fig.Segments.Add(new BezierSegment(G1, G2, G3, true));
            fig.Segments.Add(new LineSegment(A0, true));

            PathGeometry geometry0 = new PathGeometry();
            geometry0.Figures.Add(fig);
            target.Data = geometry0;
        }

        public static void DrawPathLayer1(Path target, Thickness pad, double width, double height, double rb, double rs, double gapLeft, double w1, double h1, double w2, double h2, double gapRight) {
            double wadj = width - pad.Left - pad.Right - rb;
            double hadj = height - pad.Top - pad.Bottom - rb;

            if (w1 < 3 * rb) w1 = 3 * rb;
            if (h1 < 2 * rb) h1 = 2 * rb;
            if (gapLeft < w1 / 2 + rs) gapLeft = w1 / 2 + rs;
            if (w2 < 3 * rs) w2 = 3 * rs;
            if (h2 < 3 * rs) h2 = 3 * rs;

            Point tl = new Point(pad.Left, pad.Top);
            Point tr = new Point(pad.Left + wadj, pad.Top);
            Point bl = new Point( pad.Left, pad.Top + hadj);
            Point br = new Point( pad.Left + wadj, pad.Top + hadj);


            // bezier top left small
            Point A0 = new Point(tl.X, tl.Y + h1 + rs);
            Point A1 = new Point(tl.X, tl.Y + h1 + rs);
            Point A2 = new Point(tl.X, tl.Y + h1);
            Point A3 = new Point(tl.X + rs, tl.Y + h1);
            // beziers top / 1
            Point B0 = new Point(tl.X + gapLeft - w1 / 2, tl.Y + h1);
            Point B1 = new Point(tl.X + gapLeft - h1 / 2, tl.Y + h1);
            Point B2 = new Point(tl.X + gapLeft - h1 / 2 + rb, tl.Y + h1 - rb);
            Point B3 = new Point(tl.X + gapLeft, tl.Y + h1 / 2);
            // beziers top / 2
            Point C1 = new Point(tl.X + gapLeft + h1 / 2 - rb, tl.Y + rb);
            Point C2 = new Point(tl.X + gapLeft + h1 / 2, tl.Y);
            Point C3 = new Point(tl.X + gapLeft + w1 / 2, tl.Y);
            // beziers top \ 1
            Point D0 = new Point(tr.X - gapRight - w2 / 2, tr.Y);
            Point D1 = new Point(tr.X - gapRight - h2 / 2, tr.Y);
            Point D2 = new Point(tr.X - gapRight - h2 / 2 + rb, tr.Y + rb);
            Point D3 = new Point(tr.X - gapRight, tr.Y + h2 / 2);
            // beziers top \ 2
            Point E1 = new Point(tr.X - gapRight + h2 / 2 - rb, tr.Y + h2 - rb);
            Point E2 = new Point(tr.X - gapRight + h2 / 2, tr.Y + h2);
            Point E3 = new Point(tr.X - gapRight + w2 / 2, tr.Y + h2);
            // bezier top right
            Point F0 = new Point(tr.X - rs, tr.Y + h2);
            Point F1 = new Point(tr.X - rs, tr.Y + h2);
            Point F2 = new Point(tr.X, tr.Y + h2);
            Point F3 = new Point(tr.X, tr.Y + h2 + rs);
            // bezier bottom right
            Point G0 = new Point(br.X, br.Y - rs);
            Point G1 = G0;
            Point G2 = new Point(br.X, br.Y);
            Point G3 = new Point(br.X - rs, br.Y);
            // bezier bottom left
            Point H0 = new Point(bl.X + rs, bl.Y);
            Point H1 = H0;
            Point H2 = new Point(bl.X, bl.Y);
            Point H3 = new Point(bl.X, bl.Y - rs);


            PathFigure fig = new PathFigure();
            fig.StartPoint = A0;
            fig.Segments.Add(new BezierSegment(A1, A2, A3, true));
            fig.Segments.Add(new LineSegment(B0, true));
            fig.Segments.Add(new BezierSegment(B1, B2, B3, true));
            fig.Segments.Add(new BezierSegment(C1, C2, C3, true));
            fig.Segments.Add(new LineSegment(D0, true));
            fig.Segments.Add(new BezierSegment(D1, D2, D3, true));
            fig.Segments.Add(new BezierSegment(E1, E2, E3, true));
            fig.Segments.Add(new LineSegment(F0, true));
            fig.Segments.Add(new BezierSegment(F1, F2, F3, true));
            fig.Segments.Add(new LineSegment(G0, true));
            fig.Segments.Add(new BezierSegment(G1, G2, G3, true));
            fig.Segments.Add(new LineSegment(H0, true));
            fig.Segments.Add(new BezierSegment(H1, H2, H3, true));
            fig.Segments.Add(new LineSegment(A0, true));

            PathGeometry geometry0 = new PathGeometry();
            geometry0.Figures.Add(fig);
            target.Data = geometry0;
        }

    }
}
