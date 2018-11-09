# Current state
in development

# Requirements for usage
* Windows 7 or newer (8, 8.1, 10)
* .Net Framework 4.7 or newer (4.7.1, 4.7.2)

# Current features
* 8 Test types
  * Hiragana or Katakana to Roomaji, Only Hiragana to Roomaji, Only Katakana to Roomaji
  * Roomaji to Hiragana or Katakana, Only Roomaji to Hiragana, Only Roomaji to Katakana
  * Only Hiragana to Katakana, Only Katakana to Hiragana
* Highlight wrong and correct answers, when answer was wrong
* Count answer time (separates counter for Hiragana and Katakana)
* Can show only similar answers
* Changeable answer count (from two answers up to ten answers)
* Changeable error time (highlight when answer was wrong)
* Automatically load and save test statistics and settings

# Missing a feature or found a bug?
Open a issue ticket or make a pull request

# Pictures
![Daily Kanji](DailyKanji.png)
![Daily Kanji - Error](DailyKanji-Error.png)

# Requirements for debug, test and contributing
* Visual Studio 2017 Version 15.7 or newer to build
* C# 7.3

# Goals

#### Next
* Show Roomaji on wrong answer test of type "Hiragana to Katakana" and "Katakana to Hiragana"
* Show Hiragana on wrong answer test of type "Katakana to Roomaji" and "Roomaji to Katakana"
* Show Katakana on wrong answer test of type "Katakana to Roomaji" and "Roomaji to Hiragana"
* Add test type for "Hiragana or Katakana to Katakana or Hiragana"
* Add test type for all -> "Hiragana, Katakana or Roomaji to Hiragana, Katakana or Roomaji"
* Add menu entry to reset the statistics
* Add new answers sub-menu (show current answer inside menu entry with shortcut)
* Recalculate buttons (button width), when window is resized
* Visible timer in 0.1 second (can be deactivated via menu)

#### Near future
* Prevent double-click and multi-click on correct answers to avoid wrong next answer
  * Note: Prevent it direct inside the command handlers
* On similar answers, in some circumstance it is easy to direct find the correct answer, we need a prevention for this 
  * Maybe: Only the first character or last character must are the same on less then five answers
* Move logic to separate library project in .Net Standard 2.0
* Add command line project in .Net Core 2.1 (usable under Windows, Linux, macOS)
* Export (XLSX, CSV, JSON, XML)

#### Later
* Change test order so that all tests will be ask (based on ask counter)
* Make colours choose-able
* Integrate Kanji tests

#### Ideas
* Import (XLSX, CSV, JSON, XML)
