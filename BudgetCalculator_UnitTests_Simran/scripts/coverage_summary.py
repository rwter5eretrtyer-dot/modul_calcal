#!/usr/bin/env python3
"""Вывод реального покрытия из последнего Cobertura-отчёта.
Использование: python scripts/coverage_summary.py
"""
from pathlib import Path
from xml.etree import ElementTree as ET

root = Path(__file__).resolve().parent.parent
files = list((root / "tests" / "BudgetCalculator.Tests" / "TestResults").glob("**/coverage.cobertura.xml"))
if not files:
    raise SystemExit("Отчёт покрытия отсутствует. Сначала запустите: dotnet test --collect:"XPlat Code Coverage" --settings coverage.runsettings")
latest = max(files, key=lambda x: x.stat().st_mtime)
xml = ET.parse(latest).getroot()

print(f"Файл покрытия: {latest.relative_to(root)}")
print(f"Покрытие всего измеряемого кода: строк {float(xml.attrib.get('line-rate', '0')) * 100:.1f}%, веток {float(xml.attrib.get('branch-rate', '0')) * 100:.1f}%")
found = False
for cls in xml.findall('.//class'):
    if cls.attrib.get('name', '').endswith('BudgetController'):
        found = True
        print(f"BudgetController: строк {float(cls.attrib.get('line-rate', '0')) * 100:.1f}%, веток {float(cls.attrib.get('branch-rate', '0')) * 100:.1f}%")
if not found:
    print('BudgetController отсутствует в отчёте. Проверьте фильтры и запуск тестов.')
