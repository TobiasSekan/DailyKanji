using NUnit.Framework;
using System;
using System.Collections.ObjectModel;

namespace MyTestNamespace
{

    [TestFixture]
    public sealed class MainTestClass
    {
        public sealed class MyClass:IFormattable
        {
            public string ToString(string format, IFormatProvider formatProvider)
            {
                throw new FormatException();
            }
        }

        [Test]
        public void Test()
        {
            var myList = new Collection<MyClass>
            {
                new MyClass(),
                new MyClass()
            };

            Assert.That(myList, Has.Exactly(3).Items);
        }
    }
}
