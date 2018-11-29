# Daily Kanji

## Current state
* in development
* First version will released in December 2018

## Requirements for usage
* Windows (Vista SP2, 7 SP1, 8, 8.1, 10) or Windows Server (2008 SP2, 2008 R2 SP1, 2012, 2012 SP2)
* [.Net Framework 4.6](https://www.microsoft.com/en-US/download/details.aspx?id=48130)

## Current features
* 9 Test types
  * Hiragana or Katakana to Roomaji, Only Hiragana to Roomaji, Only Katakana to Roomaji
  * Roomaji to Hiragana or Katakana, Only Roomaji to Hiragana, Only Roomaji to Katakana
  * Hiragana to Katakana or Katakana to Hiragana, Only Hiragana to Katakana, Only Katakana to Hiragana
* Test can be answered via left mouse click, number key and menu
* Switch to previous test or next test
* Running answer timer, when time is over the test is automatically answered wrong
* Highlight wrong and correct answers, when answer was wrong 
* Show hint of all possible answers, when answer was wrong (can be hide and change via menu)
* Count answer times (separates counter for Hiragana and Katakana)
* Can show only similar answers
* Changeable answer count (from two answers up to ten answers)
* Changeable answer time (from one seconds up to fifteen seconds)
* Changeable error time (from one point five seconds to ten seconds)
* Statistics can individual reset via menu
* Automatically load and save test statistics and settings

## Pictures (under Windows 10)
![Daily Kanji 1](Documentation/Pictures/DailyKanji1.png)
![Daily Kanji 2](Documentation/Pictures/DailyKanji2.png)

## Missing a feature or found a bug?
Open a issue ticket or make a pull request

## Notes about support for Windows Vista and Window 8
The support for Windows Vista SP2 and Windows 8 will be maintained as long as possible.
But when I start with next major version of this project it is most likely, that I must drop this support.
Because I need to switch to a higher version of .Net Standard (1.6, 2.0 or higher).

## Whats next?
see [Roadmap](Documentation/Roadmap.md)

## Requirements for debug, test and contributing
* [Visual Studio 2017](https://visualstudio.microsoft.com/en/downloads/), [Visual Studio Code](https://visualstudio.microsoft.com/en/downloads/), [JetBrains Rider](https://www.jetbrains.com/rider/) or similar
* [.Net Framework 4.6](https://www.microsoft.com/en-US/download/details.aspx?id=48130) (include .Net Standard 1.3)
* [Roslynator (Visual Studio Extension)](https://github.com/JosefPihrt/Roslynator) for code rules
* C# 7.3

## Project overview
| Project         | Used framework     | Operating system   |
| --------------- | ------------------ | ------------------ |
| DailyKanji      | .Net Framework 4.6 | Windows            |
| DailyKanjiLogic | .Net Standard 1.3  | *function library* |
