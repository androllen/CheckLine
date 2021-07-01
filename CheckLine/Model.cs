using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using System.Windows;

namespace CheckLine
{
    public class TaskModel : ViewModelBase
    {
        public string TaskItem { get; set; }
        public int SelectedIndex { get; set; }

        private bool isTab;
        public bool IsTab
        {
            get { return isTab; }
            set
            {
                isTab = value;
                RaisePropertyChanged();
            }
        }
        public Point point { get; set; }
    }

    public class DrawNode
    {
        public UnitNode lNode { get; set; }
        public ObservableCollection<UnitNode> RPoints { get; set; } = new ObservableCollection<UnitNode>();
    }

    public class UnitNode : ViewModelBase
    {
        public string Name { get; set; }
        public int TabIndex { get; set; }
        public Point Point { get; set; }

        private bool isTab;
        public bool IsTab
        {
            get { return isTab; }
            set
            {
                isTab = value;
                RaisePropertyChanged();
            }
        }
    }
}
