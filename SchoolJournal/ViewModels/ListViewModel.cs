namespace SchoolJournal.ViewModels;

/// <summary>Обёртка для списка строк таблицы: выводимые строки + общее число записей.</summary>
public class ListViewModel<T>
{
    public ListViewModel(IReadOnlyList<T> items, int totalCount)
    {
        Items = items;
        TotalCount = totalCount;
    }

    public IReadOnlyList<T> Items { get; }
    public int TotalCount { get; }
}
