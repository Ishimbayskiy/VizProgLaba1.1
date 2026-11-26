using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SortingComparison.Model;

namespace SortingComparison.ViewModel
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly ArraySorter _sorter;
        private int[] _originalArray;

        [ObservableProperty]
        private int _arraySize = 1000;

        [ObservableProperty]
        private string _originalArrayString;

        [ObservableProperty]
        private string _bubbleSortResult;

        [ObservableProperty]
        private string _quickSortResult;

        [ObservableProperty]
        private string _insertionSortResult;

        [ObservableProperty]
        private string _totalComparisons = "Общее число сравнений: 0";

        public IAsyncRelayCommand GenerateArrayCommand { get; }
        public IAsyncRelayCommand BubbleSortCommand { get; }
        public IAsyncRelayCommand QuickSortCommand { get; }
        public IAsyncRelayCommand InsertionSortCommand { get; }

        public MainViewModel()
        {
            _sorter = new ArraySorter();
            GenerateArrayCommand = new AsyncRelayCommand(GenerateArrayAsync, CanGenerateArray);
            BubbleSortCommand = new AsyncRelayCommand(BubbleSortAsync, CanSortBubble);
            QuickSortCommand = new AsyncRelayCommand(QuickSortAsync, CanSortQuick);
            InsertionSortCommand = new AsyncRelayCommand(InsertionSortAsync, CanSortInsertion);
        }

        private bool CanGenerateArray()
        {
            return GenerateArrayCommand == null || !GenerateArrayCommand.IsRunning;
        }

        private bool CanSortBubble()
        {
            return _originalArray != null && (BubbleSortCommand == null || !BubbleSortCommand.IsRunning);
        }

        private bool CanSortQuick()
        {
            return _originalArray != null && (QuickSortCommand == null || !QuickSortCommand.IsRunning);
        }

        private bool CanSortInsertion()
        {
            return _originalArray != null && (InsertionSortCommand == null || !InsertionSortCommand.IsRunning);
        }

        private async Task GenerateArrayAsync()
        {
            await Task.Delay(100);
            _originalArray = _sorter.GenerateRandomArray(ArraySize);
            _sorter.ResetTotalComparisons();
            OriginalArrayString = "Исходный массив: " + FormatArray(_originalArray, Math.Min(20, _originalArray.Length)) + (ArraySize > 20 ? "..." : "");
            BubbleSortResult = QuickSortResult = InsertionSortResult = null;
            TotalComparisons = "Общее число сравнений: 0";
            BubbleSortCommand.NotifyCanExecuteChanged();
            QuickSortCommand.NotifyCanExecuteChanged();
            InsertionSortCommand.NotifyCanExecuteChanged();
        }

        private async Task BubbleSortAsync()
        {
            BubbleSortResult = "Сортируется...";
            SortResult result = await _sorter.BubbleSortAsync(_originalArray);
            BubbleSortResult = $"Пузырьковая: {FormatArray(result.SortedArray)}, время: {result.ElapsedMilliseconds:F2} мс, сравнений: {result.Comparisons}";
            UpdateTotalComparisons();
        }

        private async Task QuickSortAsync()
        {
            QuickSortResult = "Сортируется...";
            SortResult result = await _sorter.QuickSortAsync(_originalArray);
            QuickSortResult = $"Быстрая: {FormatArray(result.SortedArray)}, время: {result.ElapsedMilliseconds:F2} мс, сравнений: {result.Comparisons}";
            UpdateTotalComparisons();
        }

        private async Task InsertionSortAsync()
        {
            InsertionSortResult = "Сортируется...";
            SortResult result = await _sorter.InsertionSortAsync(_originalArray);
            InsertionSortResult = $"Вставками: {FormatArray(result.SortedArray)}, время: {result.ElapsedMilliseconds:F2} мс, сравнений: {result.Comparisons}";
            UpdateTotalComparisons();
        }

        private void UpdateTotalComparisons()
        {
            TotalComparisons = $"Общее число сравнений: {_sorter.TotalComparisons}";
        }

        private string FormatArray(int[] array, int count)
        {
            int[] preview = new int[count];
            Array.Copy(array, preview, count);
            return string.Join(", ", preview);
        }

        private string FormatArray(int[] array)
        {
            if (array.Length <= 10)
                return FormatArray(array, array.Length);
            return FormatArray(array, 5) + "...";
        }
    }
}
