# Roadmap

## Version 0.1
* Add menu entry to deactivate timeout (hide visible timer too)
* Add main counter for each test (negative/positive)
  * on right answers +1 on wrong answers - 1
  * use this counter to calculate count of same tests
  * use this count to order bottom test table
* Add menu underscores (for menu keyboard navigation)
* Add option to deactivate error highlight

## Version 1.0
* Add test type for all -> "Hiragana, Katakana or Roomaji to Hiragana, Katakana or Roomaji"
* Make refresh interval for timer changeable via menu
* Recalculate buttons (button width), when window is resized
* Avoid rebuild of answer buttons and answer menu entries
* Prevent double-click and multi-click on correct answers to avoid wrong next answer
  * Note: Prevent it direct inside the command handlers
* On similar answers, in some circumstance it is easy to direct find the correct answer, we need a prevention for this 
  * Maybe: Only the first character or last character must are the same on less then five answers
* Add similar list for each Hiragana and each Katakana character for option "Similar answers"
* Change test order so that all tests will be ask (based on ask counter)
* Move more program parts to separate library project in .Net Standard
* Check for new version on start-up
* Export (CSV, JSON)

## Versions 2.0
* Add command line project in .Net Core (usable under Windows, Linux, macOS)
* Add German language and language selector in menu
* Make colours choose-able
* Check for new version on start-up

## Version 3.0
* Start with integration of Kanji tests

## Ideas
* Import (XLSX, CSV, JSON, XML)
* Export (XLSX, XML)
* Gamepad support
* Ribbon menu
* Investigate in WPF - FlowDocument (for integrated zooming features)
* Auto update program
* .Net Xamarin version for Andorid and iOS
