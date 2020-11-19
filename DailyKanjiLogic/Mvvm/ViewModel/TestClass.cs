using DailyKanjiLogic.Helper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DailyKanjiLogic.Mvvm.ViewModel
{
    public sealed class MyTestClass 
    {
        public IEnumerable<MyInternalTestClass> MyList { get; set; }

        public MyTestClass()
        {
            MyList = new Collection<MyInternalTestClass>();
        }
    }

    public sealed class MyInternalTestClass : PropertyChangedHelper, IEquatable<MyTestClass>, IFormattable, ICloneable
    {
        public object Clone() => throw new NotImplementedException();

        public bool Equals(MyTestClass other) => throw new NotImplementedException();

        public string ToString(string format, IFormatProvider formatProvider) => throw new NotImplementedException();
    }
}
