# Roadmap

## Testing
* Current sign statistics (possible: show wrong count)
* Correct counting for answers on test type "AllToAll"
* Game-pad button calculation
* Game-pad support (with 10 buttons for 10 answers)

## BUG

## Version 1.x
* Add similar list for each Hiragana and each Katakana character for option "Similar answers"
* Make refresh interval for timer changeable via menu
* Prevent double-click and multi-click on correct answers to avoid wrong next answer
  * Note: Prevent it direct inside the command handlers
* On similar answers, in some circumstance it is easy to direct find the correct answer, we need a prevention for this
  * Maybe: Only the first character or last character must are the same on less then five answers
* Change test order so that all tests will be ask (based on ask counter)
* Move more program parts to separate library project in .Net Standard
* Add more menu underscores (for menu keyboard navigation)

## Versions 2.x
* Add command line project in .Net Core (usable under Windows, Linux, macOS)
* Add German language and language selector in menu
* Add tool-tips for each menu entries
* Make colors choose-able
* Export statistics (XLSX, CSV, JSON, XML)

## Version 3.x
* Start with integration of Kanji tests

## Ideas
* Import statistics (XLSX, CSV, JSON, XML)
* Ribbon menu
* Investigate in WPF - FlowDocument (for integrated zooming features)
* Auto update program
* .Net Xamarin version for Andorid and iOS
