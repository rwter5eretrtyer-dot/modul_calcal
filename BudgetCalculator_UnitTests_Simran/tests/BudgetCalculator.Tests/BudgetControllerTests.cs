using BudgetCalculator.Api.Controllers;
using BudgetCalculator.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace BudgetCalculator.Tests;

/// <summary>
/// Unit-тесты контроллера без HTTP-сервера и внешних зависимостей.
/// Проверяют именно действие Calculate, а не инфраструктуру ASP.NET Core.
/// </summary>
public sealed class BudgetControllerTests
{
    [Fact]
    public void Calculate_WithoutInsurance_Returns200And500()
    {
        // Arrange: позитивный сценарий по условию.
        var request = new BudgetRequest
        {
            DailyCost = 100m,
            Days = 5,
            IncludeInsurance = false
        };
        var controller = new BudgetController();

        // Act.
        var result = controller.Calculate(request);

        // Assert: статус 200 и итоговая сумма 500.
        var ok = AssertOk(result);
        Assert.Equal(500m, ReadTotalAmount(ok));
    }

    [Fact]
    public void Calculate_WithInsurance_AddsOnlyFivePercent()
    {
        // Arrange: тест, который падал до исправления бага.
        var request = new BudgetRequest
        {
            DailyCost = 100m,
            Days = 5,
            IncludeInsurance = true
        };
        var controller = new BudgetController();

        // Act.
        var result = controller.Calculate(request);

        // Assert: 500 + 5% = 525, а не 500 * 5 = 2500.
        var ok = AssertOk(result);
        Assert.Equal(525m, ReadTotalAmount(ok));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public void Calculate_WithNonPositiveDays_Returns400(int days)
    {
        // Arrange: отрицательное и нулевое количество дней.
        var request = new BudgetRequest
        {
            DailyCost = 100m,
            Days = days,
            IncludeInsurance = false
        };
        var controller = new BudgetController();

        // Act.
        var result = controller.Calculate(request);

        // Assert: ошибка валидации 400 Bad Request.
        var bad = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal<int?>(StatusCodes.Status400BadRequest, bad.StatusCode);
        Assert.Equal("Количество дней должно быть больше нуля.", bad.Value);
    }

    [Fact]
    public void Calculate_WithZeroDailyCost_Returns200AndZero()
    {
        // Arrange: стоимость дня может быть нулевой.
        var request = new BudgetRequest
        {
            DailyCost = 0m,
            Days = 5,
            IncludeInsurance = false
        };
        var controller = new BudgetController();

        // Act.
        var result = controller.Calculate(request);

        // Assert.
        var ok = AssertOk(result);
        Assert.Equal(0m, ReadTotalAmount(ok));
    }

    [Fact]
    public void Calculate_WithZeroDailyCostAndInsurance_ReturnsZero()
    {
        // Arrange: страховка 5% от нуля тоже равна нулю.
        var request = new BudgetRequest
        {
            DailyCost = 0m,
            Days = 5,
            IncludeInsurance = true
        };
        var controller = new BudgetController();

        // Act.
        var result = controller.Calculate(request);

        // Assert.
        var ok = AssertOk(result);
        Assert.Equal(0m, ReadTotalAmount(ok));
    }

    [Fact]
    public void Calculate_WithFractionalDailyCost_PreservesKopecks()
    {
        // Arrange: точное десятичное вычисление 100.50 * 3.
        var request = new BudgetRequest
        {
            DailyCost = 100.50m,
            Days = 3,
            IncludeInsurance = false
        };
        var controller = new BudgetController();

        // Act.
        var result = controller.Calculate(request);

        // Assert: точная сумма 301.50, без ошибки binary float.
        var ok = AssertOk(result);
        Assert.Equal(301.50m, ReadTotalAmount(ok));
    }

    [Fact]
    public void Calculate_WithSmallFraction_PreservesDecimalPrecision()
    {
        // Arrange: 0.10 * 3 = 0.30 в десятичной арифметике.
        var request = new BudgetRequest
        {
            DailyCost = 0.10m,
            Days = 3,
            IncludeInsurance = false
        };
        var controller = new BudgetController();

        // Act.
        var result = controller.Calculate(request);

        // Assert.
        var ok = AssertOk(result);
        Assert.Equal(0.30m, ReadTotalAmount(ok));
    }

    private static OkObjectResult AssertOk(IActionResult result)
    {
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal<int?>(StatusCodes.Status200OK, ok.StatusCode);
        return ok;
    }

    private static decimal ReadTotalAmount(OkObjectResult ok)
    {
        // Контроллер из задания возвращает анонимный объект.
        // Рефлексия позволяет проверить поле без изменения контракта API.
        Assert.NotNull(ok.Value);
        var property = ok.Value!.GetType().GetProperty("TotalAmount");
        Assert.NotNull(property);
        return Assert.IsType<decimal>(property!.GetValue(ok.Value));
    }
}
