namespace BudgetCalculator.Api.Models;

/// <summary>Входные данные для расчёта бюджета.</summary>
public sealed class BudgetRequest
{
    public decimal DailyCost { get; set; }
    public int Days { get; set; }
    public bool IncludeInsurance { get; set; }
}
