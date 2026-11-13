# Анализ ошибки со страховкой

**Исполнитель:** Алакберова Симран Ахлиман, 4П1.23.

## Исходная реализация

```csharp
var total = request.DailyCost * request.Days;
if (request.IncludeInsurance)
{
    total *= 5;
}
```

В условии установлена цена страховки 5% от исходной суммы. Умножение на `5` увеличивает итоговую сумму до 500% от базовой стоимости.

Для `DailyCost = 100`, `Days = 5`, `IncludeInsurance = true`:

- Базовая сумма: `100 * 5 = 500`.
- С ошибкой: `500 * 5 = 2500`.
- Правильный расчёт: `500 * 1.05 = 525`.

## Исправление

```csharp
if (request.IncludeInsurance)
{
    total *= 1.05m;
}
```

Литерал `m` указывает тип `decimal`. Входное поле `DailyCost` также имеет тип `decimal`, что подходит для десятичных денежных расчётов.

## Как показать красный тест и затем зелёный

1. В `src/BudgetCalculator.Api/Controllers/BudgetController.cs` временно замените `total *= 1.05m;` на `total *= 5;`.
2. Выполните `dotnet test BudgetCalculator.sln --filter "FullyQualifiedName~Calculate_WithInsurance_AddsOnlyFivePercent"`.
3. Ожидается падение: тест требует `525`, но код возвращает `2500`.
4. Верните исправленную строку `total *= 1.05m;`.
5. Запустите `dotnet test BudgetCalculator.sln` и зафиксируйте реальный результат.

Версия контроллера до исправления также сохранена в `examples/BudgetController.BEFORE_FIX.cs.txt` и не компилируется с проектом. Здесь не приложены выдуманные логи или скриншоты выполнения.
