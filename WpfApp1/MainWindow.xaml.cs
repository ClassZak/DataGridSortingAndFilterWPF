using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Resources;
using System.Collections.ObjectModel;












namespace WpfApp1
{public enum DaysOfWeekEnum
		{
			None=-1,
			Monday=0,
			Tuesday,
			Wednesday,
			Thursday,
			Friday,
			Saturday,
			Sunday
		}public enum TaskStatusEnum
		{
			None=-1,
			Failed = 0,
			Doing = 1,
			Done = 2
		}
	public class DayData
	{
		

		public DaysOfWeekEnum Day { get; set; }
		public string DayTranslation => DayTranslations[(int)Day];
		public int TaskCount { get; set; }

		// Статический массив переводов
		public static readonly string[] DayTranslations =
		{
			"Понедельник", // Monday
			"Вторник",     // Tuesday
			"Среда",       // Wednesday
			"Четверг",     // Thursday
			"Пятница",     // Friday
			"Суббота",     // Saturday
			"Воскресенье" // Sunday
		};
		

		public TaskStatusEnum Status { get; set; }
		public string StatusTranslation => TaskStatusTranslations[(int)Status];

		// Статический массив переводов для статусов задач
		public static readonly string[] TaskStatusTranslations =
		{
		"Не выполнено", // Failed
        "В процессе",   // Doing
        "Выполнено"     // Done
    };
	}

public class ViewModel : INotifyPropertyChanged
	{
		public ICollectionView DaysView { get; }

		private string _taskCountFilterText;
		public string TaskCountFilterText
		{
			get => _taskCountFilterText;
			set
			{
				_taskCountFilterText = value;
				OnPropertyChanged(nameof(TaskCountFilterText));
				ApplyFilters();
			}
		}

		private DaysOfWeekEnum? _dayFilter;
		public DaysOfWeekEnum? DayFilter
		{
			get => _dayFilter;
			set
			{
				_dayFilter = value == DaysOfWeekEnum.None ? null : value;
				OnPropertyChanged(nameof(DayFilter));
				ApplyFilters();
			}
		}

		private TaskStatusEnum? _statusFilter;
		public TaskStatusEnum? StatusFilter
		{
			get => _statusFilter;
			set
			{
				_statusFilter = value == TaskStatusEnum.None ? null : value;
				OnPropertyChanged(nameof(StatusFilter));
				ApplyFilters();
			}
		}

		public ViewModel()
		{
			DaysView = CollectionViewSource.GetDefaultView(DayDataRepository.Days);

			DaysView.SortDescriptions.Add(new SortDescription(nameof(DayData.Day), ListSortDirection.Ascending));
			DaysView.Filter = FilterDays;
		}

		private bool FilterDays(object item)
		{
			if (item is not DayData day) return false;

			bool matchesTaskCount = true;
			if (!string.IsNullOrWhiteSpace(TaskCountFilterText) &&
				int.TryParse(TaskCountFilterText, out int taskCountFilterValue))
			{
				matchesTaskCount = day.TaskCount == taskCountFilterValue;
			}

			bool matchesDay = !DayFilter.HasValue || day.Day == DayFilter.Value;
			bool matchesStatus = !StatusFilter.HasValue || day.Status == StatusFilter.Value;

			return matchesTaskCount && matchesDay && matchesStatus;
		}

		private void ApplyFilters()
		{
			DaysView.Refresh();
		}

		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}


	public static class DayDataRepository
	{
		public static DayData[] Days = new[]
		{
			new DayData { Day = DaysOfWeekEnum.Monday, TaskCount = 5,Status=TaskStatusEnum.Failed },
			new DayData { Day = DaysOfWeekEnum.Thursday, TaskCount = 10,Status=TaskStatusEnum.Doing},
			new DayData { Day = DaysOfWeekEnum.Wednesday, TaskCount = 8,Status=TaskStatusEnum.Done },
			new DayData { Day = DaysOfWeekEnum.Tuesday, TaskCount = 10,Status=TaskStatusEnum.Failed },
			new DayData { Day = DaysOfWeekEnum.Friday, TaskCount = 8,Status=TaskStatusEnum.Done},
			new DayData { Day = DaysOfWeekEnum.Saturday, TaskCount = 8,Status=TaskStatusEnum.Failed },
			new DayData { Day = DaysOfWeekEnum.Sunday, TaskCount = 3,Status=TaskStatusEnum.Failed }
		};
	}


	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void DataGrid_Sorting(object sender, DataGridSortingEventArgs e)
		{
			var dataGrid = sender as DataGrid;
			if (dataGrid == null) return;

			e.Handled = true;

			var column = e.Column;
			var collectionView = CollectionViewSource.GetDefaultView(dataGrid.ItemsSource);

			// Определяем направление сортировки
			ListSortDirection direction = column.SortDirection != ListSortDirection.Ascending
				? ListSortDirection.Ascending
				: ListSortDirection.Descending;

			collectionView.SortDescriptions.Clear();

			if (column.SortMemberPath == "Day")
			{
				collectionView.SortDescriptions.Add(new SortDescription("Day", direction));
			}
			else if (column.SortMemberPath == "Status")
			{
				collectionView.SortDescriptions.Add(new SortDescription("Status", direction));
			}
			else if (column.SortMemberPath == "TaskCount")
			{
				collectionView.SortDescriptions.Add(new SortDescription("TaskCount", direction));
			}

			column.SortDirection = direction;
		}
	}
}