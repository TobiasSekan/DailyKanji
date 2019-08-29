# Roadmap

## Testing
* Current sign statistics (possible: show wrong count)
* Correct counting for answers on test type "AllToAll"
* Game-pad button calculation
* Game-pad support (with 10 buttons for 10 answers)

## BUG
* Fix crash on ten possible answers and when only kana with Handakuten are selected
* Avoid that "ji",  "じ", "ジ" to be taken as "ji",  "ぢ", "ヂ"

## Version 1.x
* Find a way to differentiate tests (maybe via hash)
* Add UnitTests (NUnit with `Assert.That()`)
* Add extended Katakana (see https://en.wikipedia.org/wiki/Transcription_into_Japanese#Extended_katakana_2)
* Add German language and language selector in menu
* Add tool-tips for each menu entries
* Add more menu underscores (for menu keyboard navigation)
* Add similar list for each Hiragana and each Katakana character for option "Similar answers"
* Change test order so that all tests will be ask (based on ask counter)
* Prevent double-click and multi-click on correct answers to avoid wrong next answer
  * Note: Prevent it direct inside the command handlers
* On similar answers, in some circumstance it is easy to direct find the correct answer, we need a prevention for this
  * Maybe: Only the first character or last character must are the same on less then five answers

## Versions 2.x
* Internal: DailyKanjiLogic.Mvvm.ViewModel.GetAnswerNumber -> Can we use foreach here ?
* Add command line project in .Net Core (usable under Windows, Linux, macOS)
* Move more program parts to separate library project in .Net Standard
* Export statistics (XLSX, CSV, JSON, XML)
* Import statistics (XLSX, CSV, JSON, XML)
* Investigate in WPF - FlowDocument (for integrated zooming features)
* Make colors choose-able
* Ribbon menu

## Version 3.x
* Start with integration of Kanji tests

## Ideas
* Auto update program
* .Net Xamarin version for Andorid and iOS
