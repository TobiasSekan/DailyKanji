# Daily Kanji

## Requirements for usage
* Windows
  * Windows 7 SP1 with [Extended Security Updates](https://docs.microsoft.com/en-us/troubleshoot/windows-client/windows-7-eos-faq/windows-7-extended-security-updates-faq)
  * Windows 8.1
  * Windows 10 Version 1607 or higher
  * Windows Server 2012 R2, 2016 or 2019
* [.Net Core 3.1](https://dotnet.microsoft.com/download/dotnet-core/3.1)

## Current status and version
* Status: in (slow) development

| Type        | Version  | Git-Tag | Branch                                                               |
| ----------- | -------- | ------- | -------------------------------------------------------------------- |
| Release     | 0.7.0    | v0.7.0  | [master](https://github.com/TobiasSekan/DailyKanji/commits/master)   |
| Development | 1.0.0    | -       | [develop](https://github.com/TobiasSekan/DailyKanji/commits/develop) |

## Current features
* 10 Test types
  * Hiragana or Katakana to Roomaji, Only Hiragana to Roomaji, Only Katakana to Roomaji
  * Roomaji to Hiragana or Katakana, Only Roomaji to Hiragana, Only Roomaji to Katakana
  * Hiragana to Katakana or Katakana to Hiragana, Only Hiragana to Katakana, Only Katakana to Hiragana
  * All to All
* 6 Kana types
  * Gojuuon, Gojuuon with dakuten, Gojuuon with handakuten
  * Yooon, Yooon with dakuten, Yooon with handakuten
* Test can be answered via mouse left-click, number key, menu entry or gamepad buttons
* Switch to previous test or next test
* Running answer timer, when time is over the test is automatically answered wrong
* Mark possible wrong answers via mouse right-click, shift + number key or menu entry
* Highlight wrong and correct answers, when answer was wrong and/or correct
* Show hint of all possible answers, when answer was wrong (can be hide and change via menu)
* Wrong answered tests will be ask more often
* Can show only similar answers
* Changeable answer count, answer time, error highlight, error highlight time, ...
* Count right and wrong answers and answer times (separates counter for each Hiragana and Katakana)
* Separate window for statistics, statistics can individual reset via menu

## Pictures (under Windows 10) - Version 0.7.0
![Daily Kanji 1](Documentation/Pictures/DailyKanji1.png)
![Daily Kanji 2](Documentation/Pictures/DailyKanji2.png)

## Found a bug or missing a feature?
* Create a new [Bug report](https://github.com/TobiasSekan/DailyKanji/issues/new?template=bug_report.md)
* Create a new [Feature request](https://github.com/TobiasSekan/DailyKanji/issues/new?template=feature_request.md)

## Whats next?
* [Version 1.0](https://github.com/TobiasSekan/DailyKanji/milestone/1)
* [Version 2.0](https://github.com/TobiasSekan/DailyKanji/milestone/4)
* [Version 3.0](https://github.com/TobiasSekan/DailyKanji/milestone/2)
* [Ideas](https://github.com/TobiasSekan/DailyKanji/milestone/3)

## Requirements for debug, test and contributing
* [Visual Studio 2019](https://visualstudio.microsoft.com/vs/), [Visual Studio Code](https://code.visualstudio.com/), [JetBrains Rider](https://www.jetbrains.com/rider/) or similar
* [.Net Core 3.1](https://dotnet.microsoft.com/download/dotnet-core/3.1)
* C# 9.0

## Project structure
| Project             | Target framework | Operating system             |
| ------------------- | ---------------- | ---------------------------- |
| DailyKanji          | .Net Core 3.1    | Windows (because WPF)        |
| DailyKanjiLogic     | .Net Core 3.1    | Operating system independent |
| DailyKanjiLogicTest | .Net Core 3.1    | Operating system independent |

## Used NuGet packages
| Package                       | Version    | Reason                                     |
| ----------------------------- | ---------- | ------------------------------------------ |
| Extended.Wpf.Toolkit          | 4.0.1      | Additional WPF elements (e.g. SpinUpDown)  |
| Newtonsoft.Json               | 12.0.3     | Load and write JSON files                  |
| Microsoft.NET.Test.Sdk        | 16.8.0     | Test SDK for .NET                          |
| NUnit                         | 3.12.0     | NUnit test framework                       |
| NUnit3TestAdapter             | 3.17.0     | Test adapter for NUnit                     |
| Roslynator.Analyzers          | 3.0.0      | Code guidelines                            |
| SharpDX.DirectInput           | 4.2.0      | Game-pad support via DirectInput           |
| StyleCop.Analyzers            | 1.2.0-beta | Code guidelines                            |
| System.Diagnostics.StackTrace | 4.3.0      | StracTrace output                          |
