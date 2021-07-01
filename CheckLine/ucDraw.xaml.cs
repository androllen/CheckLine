using GalaSoft.MvvmLight.Messaging;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;


namespace CheckLine
{
    /// <summary>
    /// Interaction logic for ucDraw.xaml
    /// </summary>
    public partial class ucDraw : UserControl
    {
        public ucDraw()
        {
            InitializeComponent();

            Messenger.Default.Register<DrawNode>(this, "LeftCmd", LeftEvent);
            Messenger.Default.Register<DrawNode>(this, "CheckedRP", CheckedRP);
            Messenger.Default.Register<Point>(this, "UnCheckedRP", UnCheckedRP);
            this.SizeChanged += UcDraw_SizeChanged;
        }

        private void UcDraw_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            PanelWidth = e.NewSize.Width;
            Updata();
        }
        private void Updata()
        {
            for (int i = canvas.Children.Count - 1; i >= 0; i--)
            {
                if (canvas.Children[i] is Path element)
                {
                    if (element.Data is LineGeometry Line)
                    {
                        if (Line.EndPoint.X != PanelWidth)
                        {
                            var point = new Point() { X = PanelWidth, Y = Line.EndPoint.Y };
                            Line.EndPoint = point;
                        }
                    }

                }
            }
        }
        public double PanelWidth
        {
            get { return (double)GetValue(PanelWidthProperty); }
            set { SetValue(PanelWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PanelWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PanelWidthProperty =
            DependencyProperty.Register("PanelWidth", typeof(double), typeof(ucDraw), new PropertyMetadata(300.0));


        private DrawNode drawNode { get; set; }
        private void UnCheckedRP(Point obj)
        {
            UnDrawingLine(drawNode.lNode.Point, obj);
        }

        private void CheckedRP(DrawNode obj)
        {
            DrawingLine(drawNode.lNode.Point, obj.RPoints);
        }
        private void LeftEvent(DrawNode obj)
        {
            drawNode = obj;
        }
        private void DrawingLine(Point startPt, ObservableCollection<UnitNode> endPt)
        {
            foreach (var item in endPt)
            {
                LineGeometry lineGry = new LineGeometry();
                lineGry.StartPoint = startPt;
                lineGry.EndPoint = item.Point;
                Path mPath = new Path();
                mPath.Stroke = Brushes.Red;
                mPath.StrokeThickness = 1;
                mPath.Data = lineGry;

                canvas.Children.Add(mPath);
            }
        }

        private void UnDrawingLine(Point startPt, Point endPt)
        {
            for (int i = canvas.Children.Count - 1; i >= 0; i--)
            {
                if (canvas.Children[i] is Path element)
                {
                    if (element.Data is LineGeometry Line)
                    {
                        if (Line.StartPoint.Equals(startPt) && Line.EndPoint.Equals(endPt))
                        {
                            canvas.Children.Remove(element);
                        }
                    }

                }
            }
        }
    }
}
