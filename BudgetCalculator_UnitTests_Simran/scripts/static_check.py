#!/usr/bin/env python3
"""Проверка целостности проекта. НЕ заменяет dotnet test."""
from pathlib import Path
from xml.etree import ElementTree as ET
import json

root = Path(__file__).resolve().parent.parent
api = root / 'src/BudgetCalculator.Api'
tests = root / 'tests/BudgetCalculator.Tests'
controller = (api / 'Controllers/BudgetController.cs').read_text(encoding='utf8')
test_file = (tests / 'BudgetControllerTests.cs').read_text(encoding='utf8')

for p in (api / 'BudgetCalculator.Api.csproj', tests / 'BudgetCalculator.Tests.csproj', root / 'coverage.runsettings'):
    ET.parse(p)
    print('[OK] XML:',p.relative_to(root))
for p in (root / 'global.json', api / 'Properties/launchSettings.json'):
    json.loads(p.read_text(encoding='utf8'))
    print('[OK] JSON:',p.relative_to(root))
assert 'ProjectReference Include="../../src/BudgetCalculator.Api/BudgetCalculator.Api.csproj"' in (tests / 'BudgetCalculator.Tests.csproj').read_text('utf8')
assert 'total *= 1.05m;' in controller and 'total *= 5;' not in controller
assert 'request.Days <= 0' in controller and 'request.IncludeInsurance' in controller
assert '[Theory]' in test_file and '[InlineData(-1)]' in test_file and '[InlineData(0)]' in test_file
assert 'Assert.Equal(525m, ReadTotalAmount(ok))' in test_file
assert 'Assert.Equal(301.50m, ReadTotalAmount(ok))' in test_file
assert 'Assert.Equal(500m, ReadTotalAmount(ok))' in test_file
assert 'StatusCodes.Status400BadRequest' in test_file
assert 'StatusCodes.Status200OK' in test_file
assert 'total *= 5;' in (root / 'examples/BudgetController.BEFORE_FIX.cs.txt').read_text('utf8')
print('[OK] Project references, controller fix, and required test case source markers')
print('IMPORTANT: Static checks do not compile C# or execute xUnit tests.')
