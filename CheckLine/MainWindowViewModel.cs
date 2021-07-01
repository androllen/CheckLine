using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace CheckLine
{

    public class MainWindowViewModel : ViewModelBase
    {
        private ObservableCollection<TaskModel> _taskItems;
        public ObservableCollection<TaskModel> TaskItems
        {
            get { return _taskItems; }
            set
            {
                _taskItems = value;
                RaisePropertyChanged();
            }
        }
        private ObservableCollection<TaskModel> _targetItems;
        public ObservableCollection<TaskModel> TargetItems
        {
            get { return _targetItems; }
            set
            {
                _targetItems = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<DrawNode> DrawItems { get; set; }

        public MainWindowViewModel()
        {
            TaskItems = new ObservableCollection<TaskModel>();
            TargetItems = new ObservableCollection<TaskModel>();
            DrawItems = new ObservableCollection<DrawNode>();
            LoadData();
        }

        private bool isGroup = false;
        private double panelWidth = 100;
        public double PanelWidth
        {
            get { return panelWidth; }
            set
            {
                panelWidth = value;
                RaisePropertyChanged();
                LoadData();
            }
        }

        private void LoadData()
        {
            TaskItems.Clear();
            TargetItems.Clear();

            TaskItems.Add(new TaskModel() { SelectedIndex = 0, TaskItem = "Test a", point = new Point(0, 30) });
            TaskItems.Add(new TaskModel() { SelectedIndex = 1, TaskItem = "Test b", point = new Point(0, 90) });
            TaskItems.Add(new TaskModel() { SelectedIndex = 2, TaskItem = "Test c", point = new Point(0, 150) });
            TaskItems.Add(new TaskModel() { SelectedIndex = 3, TaskItem = "Test d", point = new Point(0, 210) });
            TaskItems.Add(new TaskModel() { SelectedIndex = 4, TaskItem = "Test e", point = new Point(0, 270) });
            TaskItems.Add(new TaskModel() { SelectedIndex = 4, TaskItem = "Test f", point = new Point(0, 330) });
            TaskItems.Add(new TaskModel() { SelectedIndex = 4, TaskItem = "Test g", point = new Point(0, 390) });
            TaskItems.Add(new TaskModel() { SelectedIndex = 4, TaskItem = "Test h", point = new Point(0, 450) });
            TaskItems.Add(new TaskModel() { SelectedIndex = 4, TaskItem = "Test i", point = new Point(0, 510) });
            TaskItems.Add(new TaskModel() { SelectedIndex = 4, TaskItem = "Test j", point = new Point(0, 570) });
            TaskItems.Add(new TaskModel() { SelectedIndex = 4, TaskItem = "Test k", point = new Point(0, 630) });
            TaskItems.Add(new TaskModel() { SelectedIndex = 4, TaskItem = "Test l", point = new Point(0, 690) });

            TargetItems.Add(new TaskModel() { SelectedIndex = 0, TaskItem = "Test 1", point = new Point(PanelWidth, 30) });
            TargetItems.Add(new TaskModel() { SelectedIndex = 1, TaskItem = "Test 2", point = new Point(PanelWidth, 90) });
            TargetItems.Add(new TaskModel() { SelectedIndex = 2, TaskItem = "Test 3", point = new Point(PanelWidth, 150) });
            TargetItems.Add(new TaskModel() { SelectedIndex = 3, TaskItem = "Test 4", point = new Point(PanelWidth, 210) });
            TargetItems.Add(new TaskModel() { SelectedIndex = 4, TaskItem = "Test 5", point = new Point(PanelWidth, 270) });
        }

        /// <summary>
        /// 左侧点击
        /// </summary>
        private RelayCommand<TaskModel> checkedCmd;
        public RelayCommand<TaskModel> CheckedCmd
        {
            get
            {
                return checkedCmd ?? (checkedCmd = new RelayCommand<TaskModel>(node =>
                {
                    isGroup = true;
                    if (!DrawItems.Any(p => p.lNode.Name == node.TaskItem))
                    {
                        var drawNode = new DrawNode()
                        {
                            lNode = new UnitNode()
                            {
                                Name = node.TaskItem,
                                TabIndex = node.SelectedIndex,
                                Point = node.point,
                                IsTab = node.IsTab
                            },
                        };

                        foreach (var item in DrawItems)
                        {
                            item.lNode.IsTab = false;
                        }

                        DrawItems.Add(drawNode);
                        TargetItems.ToList().ForEach(p => p.IsTab = false);
                        Messenger.Default.Send(drawNode, "LeftCmd");
                    }
                    else
                    {
                        TargetItems.ToList().ForEach(p => p.IsTab = false);
                        DrawItems.ToList().ForEach(p => p.lNode.IsTab = false);
                        foreach (var item in DrawItems)
                        {
                            if (item.lNode.Name == node.TaskItem)
                            {
                                foreach (var btn in item.RPoints)
                                {
                                    TargetItems.Where(b => b.TaskItem == btn.Name).ToList().ForEach(p => p.IsTab = btn.IsTab);
                                }
                                item.lNode.IsTab = true;
                                isGroup = false;
                                Messenger.Default.Send(item, "LeftCmd");
                            }
                        }
                    }

                }));
            }
        }
        /// <summary>
        /// 取消标签
        /// </summary>
        private RelayCommand<TaskModel> uncheckedCmd;
        public RelayCommand<TaskModel> UncheckedCmd
        {
            get
            {
                return uncheckedCmd ?? (uncheckedCmd = new RelayCommand<TaskModel>(node =>
                {
                    node.IsTab = false;
                }));
            }
        }

        private RelayCommand<TaskModel> drawCmd;
        public RelayCommand<TaskModel> DrawedCmd
        {
            get
            {
                return drawCmd ?? (drawCmd = new RelayCommand<TaskModel>(node =>
                {
                    if (!TaskItems.Any(p => p.IsTab))
                        return;

                    foreach (var item in DrawItems)
                    {
                        if (item.lNode.IsTab)
                        {
                            if (!item.RPoints.Any(b => b.Equals(node.point)) && !item.RPoints.Any(b => b.Equals(node.TaskItem)))
                            {
                                isGroup = false;

                                var uNode = new UnitNode();
                                uNode.Name = node.TaskItem;
                                uNode.IsTab = node.IsTab;
                                uNode.TabIndex = node.SelectedIndex;
                                uNode.Point = node.point;
                                item.RPoints.Add(uNode);

                                Messenger.Default.Send(item, "CheckedRP");
                            }

                        }
                    }

                }));
            }
        }

        private RelayCommand<TaskModel> undrawedCmd;
        public RelayCommand<TaskModel> UnDrawedCmd
        {
            get
            {
                return undrawedCmd ?? (undrawedCmd = new RelayCommand<TaskModel>(node =>
                {
                    if (isGroup)
                        return;

                    foreach (var item in DrawItems)
                    {
                        if (item.lNode.IsTab)
                        {
                            if (item.RPoints.Count > 0 && !item.RPoints.Any(b => b.Equals(node.TaskItem)))
                            {
                                item.RPoints.Remove(item.RPoints.FirstOrDefault(p => p.Name == node.TaskItem));
                                Messenger.Default.Send(node.point, "UnCheckedRP");
                            }
                        }
                    }
                }));
            }
        }



    }
}
