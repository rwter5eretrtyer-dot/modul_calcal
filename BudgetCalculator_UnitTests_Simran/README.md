# Калькулятор бюджета. Модульное тестирование REST API

**Студентка:** Алакберова Симран Ахлиман  
**Группа:** 4П1.23  
**Предмет:** Модульное тестирование веб-приложений на Java  
**Задание:** «Провести тестирование» (50 баллов)  
**Срок сдачи:** 04.12.2026

> Важно: хотя дисциплина имеет слово Java в названии, конкретное задание содержит код C# (`decimal`, `IActionResult`, `[ApiController]`) и требует xUnit Test Project. Поэтому реализация сделана на **C# / ASP.NET Core / xUnit** и соответствует предоставленному фрагменту.

## Что готово

- Минимальный REST API-калькулятор, восстановленный по коду из задания (исходный проект не был приложен).
- Тестовый проект xUnit со ссылкой `ProjectReference` на основной API.
- 8 тест-кейсов: happy path, страховка, отрицательные и нулевые дни, нулевая стоимость, денежная точность.
- Исправление бага в `BudgetController.cs`: `total *= 1.05m;` вместо `total *= 5;`.
- Исходный ошибочный контроллер в `examples/BudgetController.BEFORE_FIX.cs.txt` для демонстрации падения теста.
- Настройка Code Coverage (Coverlet + Cobertura XML), GitHub Actions для автоматической проверки.
- Отчёт Word в `report/`.

### Структура

```text
BudgetCalculator_UnitTests_Simran/
├── BudgetCalculator.sln
├── global.json
├── coverage.runsettings
├── src/BudgetCalculator.Api/
│   ├── BudgetCalculator.Api.csproj
│   ├── Controllers/BudgetController.cs
│   ├── Models/BudgetRequest.cs
│   └── Program.cs
├── tests/BudgetCalculator.Tests/
│   ├── BudgetCalculator.Tests.csproj
│   └── BudgetControllerTests.cs
├── examples/BudgetController.BEFORE_FIX.cs.txt
├── docs/BUG_ANALYSIS.md
├── docs/TEST_CASES.md
├── scripts/coverage_summary.py
├── .github/workflows/tests.yml
└── report/Отчет_Модульное_тестирование_Симран.docx
```

## Требования

Установите **.NET 10 SDK**. Для восстановления пакетов xUnit и Coverlet понадобится интернет (NuGet). Специальный HTTP-сервер для запуска unit-тестов не нужен.

## Запуск тестов

Откройте PowerShell, терминал Visual Studio или Linux terminal в корневой папке проекта:

```bash
dotnet --version
dotnet restore BudgetCalculator.sln
dotnet test BudgetCalculator.sln --no-restore --verbosity normal
```

Ожидается 8 запущенных тест-кейсов, когда используется исправленная версия кода. **Фактический результат** следует проверить в терминале, а не считать заранее подтверждённым.

### Code Coverage

```bash
dotnet test BudgetCalculator.sln --collect:"XPlat Code Coverage" --settings coverage.runsettings --logger "trx;LogFileName=test-results.trx"
```

Результаты появятся в `tests/BudgetCalculator.Tests/TestResults/<идентификатор>/coverage.cobertura.xml`. Для краткого вывода по контроллеру:

```bash
python scripts/coverage_summary.py
```

Тестами предусмотрены все **4 логические ветви двух условий `if`** в методе `Calculate`, но настоящий процент покрытия может быть указан только после запуска инструмента.

### Демонстрация найденного бага

См. [docs/BUG_ANALYSIS.md](docs/BUG_ANALYSIS.md). Кратко: временно поменяйте `1.05m` на `5`, запустите только тест на страховку, зафиксируйте падение; восстановите `1.05m`, запустите тесты и соберите покрытие.

## Проверка REST API вручную (по желанию)

```bash
dotnet run --project src/BudgetCalculator.Api --launch-profile BudgetCalculator.Api
```

В другом терминале:

```bash
curl -X POST http://localhost:5059/api/budget/calculate -H "Content-Type: application/json" -d '{"dailyCost":100,"days":5,"includeInsurance":true}'
```

Ожидаемый JSON: `{"totalAmount":525}`. Регистр имени поля в HTTP JSON определяется настройками ASP.NET Core сериализации и может отличаться от C#-свойства `TotalAmount`.

## Изоляция тестируемого кода

Тесты вызывают `new BudgetController().Calculate(request)` напрямую. В данной версии примера нет базы данных, внешних API и других внедрённых зависимостей, поэтому создавать фиктивные mock-объекты не требуется. HTTP-сервер и сеть также не задействованы, так что проверяется именно логика контроллера.

## Важное ограничение

В архив включены исходники, тесты и настройки. В среде подготовки отсутствует .NET SDK, поэтому **фактический `dotnet test` и Code Coverage здесь не запускались**. Для сдачи практического результата исполните команды на Windows/Linux с .NET SDK или воспользуйтесь GitHub Actions, затем приложите настоящие скриншоты.
