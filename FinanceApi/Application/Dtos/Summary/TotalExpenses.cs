namespace FinanceApi.Application.Dtos.Summary;

public class TotalExpenses
{
    private readonly List<Expense> expenses;
    public TotalExpenses()
    {
        expenses = new List<Expense>();
    }

    public List<Expense> Expenses { get => expenses; }

    public void Add(Expense expense)
    {
        this.expenses.Add(expense);
    }
}