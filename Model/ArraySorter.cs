using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SortingComparison.Model
{
    public class ArraySorter
    {
        private long _totalComparisons;
        private readonly object _locker = new object();

        public delegate void SortCompletedHandler(int[] sortedArray, long comparisons, double elapsedMilliseconds);

        public event SortCompletedHandler BubbleSortCompleted;
        public event SortCompletedHandler QuickSortCompleted;
        public event SortCompletedHandler InsertionSortCompleted;

        public long TotalComparisons
        {
            get
            {
                lock (_locker)
                {
                    return _totalComparisons;
                }
            }
        }

        public int[] GenerateRandomArray(int size)
        {
            if (size < 0)
                throw new ArgumentOutOfRangeException(nameof(size));

            Random rand = new Random();
            int[] array = new int[size];
            for (int i = 0; i < array.Length; i++)
                array[i] = rand.Next(1000);
            return array;
        }

        public void ResetTotalComparisons()
        {
            lock (_locker)
            {
                _totalComparisons = 0;
            }
        }

        private int[] CopyArray(int[] source)
        {
            int[] copy = new int[source.Length];
            Array.Copy(source, copy, source.Length);
            return copy;
        }

        public void BubbleSort(int[] originalArray)
        {
            SortResult result = RunBubbleSort(originalArray);
            AddComparisons(result.Comparisons);
            BubbleSortCompleted?.Invoke(result.SortedArray, result.Comparisons, result.ElapsedMilliseconds);
        }

        public void QuickSort(int[] originalArray)
        {
            SortResult result = RunQuickSort(originalArray);
            AddComparisons(result.Comparisons);
            QuickSortCompleted?.Invoke(result.SortedArray, result.Comparisons, result.ElapsedMilliseconds);
        }

        public void InsertionSort(int[] originalArray)
        {
            SortResult result = RunInsertionSort(originalArray);
            AddComparisons(result.Comparisons);
            InsertionSortCompleted?.Invoke(result.SortedArray, result.Comparisons, result.ElapsedMilliseconds);
        }

        public Task<SortResult> BubbleSortAsync(int[] originalArray)
        {
            return Task.Run(() => RunAndCount(RunBubbleSort, originalArray));
        }

        public Task<SortResult> QuickSortAsync(int[] originalArray)
        {
            return Task.Run(() => RunAndCount(RunQuickSort, originalArray));
        }

        public Task<SortResult> InsertionSortAsync(int[] originalArray)
        {
            return Task.Run(() => RunAndCount(RunInsertionSort, originalArray));
        }

        private SortResult RunAndCount(Func<int[], SortResult> sort, int[] originalArray)
        {
            SortResult result = sort(originalArray);
            AddComparisons(result.Comparisons);
            return result;
        }

        private void AddComparisons(long comparisons)
        {
            lock (_locker)
            {
                _totalComparisons += comparisons;
            }
        }

        private SortResult RunBubbleSort(int[] originalArray)
        {
            int[] array = CopyArray(originalArray);
            long comparisons = 0;
            Stopwatch watch = Stopwatch.StartNew();

            for (int i = 0; i < array.Length - 1; i++)
            {
                for (int j = 0; j < array.Length - 1 - i; j++)
                {
                    comparisons++;
                    if (array[j] > array[j + 1])
                    {
                        int temp = array[j];
                        array[j] = array[j + 1];
                        array[j + 1] = temp;
                    }
                }
            }

            watch.Stop();
            return new SortResult(array, comparisons, watch.Elapsed.TotalMilliseconds);
        }

        private SortResult RunQuickSort(int[] originalArray)
        {
            int[] array = CopyArray(originalArray);
            long comparisons = 0;
            Stopwatch watch = Stopwatch.StartNew();

            QuickSortRecursive(array, 0, array.Length - 1, ref comparisons);

            watch.Stop();
            return new SortResult(array, comparisons, watch.Elapsed.TotalMilliseconds);
        }

        private void QuickSortRecursive(int[] array, int left, int right, ref long comparisons)
        {
            if (left < right)
            {
                int pivotIndex = Partition(array, left, right, ref comparisons);
                QuickSortRecursive(array, left, pivotIndex - 1, ref comparisons);
                QuickSortRecursive(array, pivotIndex + 1, right, ref comparisons);
            }
        }

        private int Partition(int[] array, int left, int right, ref long comparisons)
        {
            int pivot = array[right];
            int i = left - 1;
            for (int j = left; j < right; j++)
            {
                comparisons++;
                if (array[j] < pivot)
                {
                    i++;
                    int temp = array[i];
                    array[i] = array[j];
                    array[j] = temp;
                }
            }

            int last = array[i + 1];
            array[i + 1] = array[right];
            array[right] = last;
            return i + 1;
        }

        private SortResult RunInsertionSort(int[] originalArray)
        {
            int[] array = CopyArray(originalArray);
            long comparisons = 0;
            Stopwatch watch = Stopwatch.StartNew();

            for (int i = 1; i < array.Length; i++)
            {
                int key = array[i];
                int j = i - 1;
                while (j >= 0 && array[j] > key)
                {
                    comparisons++;
                    array[j + 1] = array[j];
                    j--;
                }
                comparisons++;
                array[j + 1] = key;
            }

            watch.Stop();
            return new SortResult(array, comparisons, watch.Elapsed.TotalMilliseconds);
        }
    }

    public sealed class SortResult
    {
        public SortResult(int[] sortedArray, long comparisons, double elapsedMilliseconds)
        {
            SortedArray = sortedArray;
            Comparisons = comparisons;
            ElapsedMilliseconds = elapsedMilliseconds;
        }

        public int[] SortedArray { get; }
        public long Comparisons { get; }
        public double ElapsedMilliseconds { get; }
    }
}
