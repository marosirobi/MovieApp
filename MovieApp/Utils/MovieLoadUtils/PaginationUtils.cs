using System.Collections.ObjectModel;

namespace MovieApp.MVVM.Utils
{
    public static class PaginationUtils
    {
        public static void UpdateVisibleItems<T>(
            ObservableCollection<T> sourceCollection,
            ObservableCollection<T> visibleCollection,
            int currentPage,
            int itemsPerPage)
        {
            visibleCollection.Clear();
            var startIndex = currentPage * itemsPerPage;
            var itemsToShow = sourceCollection.Skip(startIndex).Take(itemsPerPage).ToList();

            foreach (var item in itemsToShow)
            {
                visibleCollection.Add(item);
            }
        }

        public static bool CanGoNext<T>(int currentPage, int itemsPerPage, ObservableCollection<T> sourceCollection)
        {
            return (currentPage + 1) * itemsPerPage < sourceCollection.Count;
        }

        public static bool CanGoPrevious(int currentPage)
        {
            return currentPage > 0;
        }
    }
}