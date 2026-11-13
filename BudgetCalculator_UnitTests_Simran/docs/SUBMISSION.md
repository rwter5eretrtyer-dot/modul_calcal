# Как сдать работу и подтвердить результаты

**Алакберова Симран Ахлиман, группа 4П1.23**

## Подготовка

1. Скачайте архив проекта и распакуйте его.
2. Установите .NET 10 SDK и запустите Visual Studio или терминал.
3. Откройте `BudgetCalculator.sln`.
4. Восстановите зависимости командой `dotnet restore BudgetCalculator.sln`.

## Демонстрация найденной ошибки

1. Откройте `src/BudgetCalculator.Api/Controllers/BudgetController.cs`.
2. Временно замените строку `total *= 1.05m;` на `total *= 5;`.
3. Запустите:

   ```bash
   dotnet test BudgetCalculator.sln --filter "FullyQualifiedName~Calculate_WithInsurance_AddsOnlyFivePercent"
   ```

4. Сделайте скриншот **реального** падения теста (ожидалось 525, получено 2500).
5. Верните строку `total *= 1.05m;`.

## Демонстрация исправления

1. Выполните `dotnet test BudgetCalculator.sln --verbosity normal`.
2. Зафиксируйте скриншот с фактическим числом и статусом тестов.
3. Выполните сбор покрытия:

   ```bash
   dotnet test BudgetCalculator.sln --collect:"XPlat Code Coverage" --settings coverage.runsettings
   ```

4. Проверьте файл `coverage.cobertura.xml` в `tests/BudgetCalculator.Tests/TestResults/`.
5. Если установлен Python, запустите `python scripts/coverage_summary.py` и сделайте скриншот действительного процента покрытия.
6. Убедитесь, что в отчёте сборщика представлены обе ветви каждого условия метода Calculate.

## Публикация

Создайте публичный репозиторий на GitHub (например, `budget-calculator-unit-tests`) и загрузите файлы проекта. После публикации GitHub Actions сможет запустить тесты и приложить TRX/Cobertura-отчёты как артефакты конкретного запуска. Включать в ZIP папки `bin/`, `obj/` и результаты со своего компьютера не обязательно.

**На сдачу**: ссылка на репозиторий, Word-отчёт, скриншоты «красного» и «зелёного» запуска, фактический Code Coverage. Не используйте примеры вывода вместо настоящих результатов.
