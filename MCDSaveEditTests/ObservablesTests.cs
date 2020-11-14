using System;
using System.Reactive;
using MCDSaveEdit;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MCDSaveEditTests
{
    [TestClass]
    public class ObservablesTests
    {
        [TestMethod]
        public void TestPropertyConstructor()
        {
            var intProp = new Property<int>(0);
            Assert.IsNotNull(intProp);
        }

        [TestMethod]
        public void TestPropertyValuePlusPlus()
        {
            var intProp = new Property<int>(0);
            intProp.value++;
            Assert.AreEqual(1, intProp.value);
        }

        [TestMethod]
        public void TestPropertySetsValueOnSetValue()
        {
            var intProp = new Property<int>(0);
            intProp.setValue = 1;
            Assert.AreEqual(1, intProp.value);
        }

        [TestMethod]
        public void TestPropertySetsValueOnValue()
        {
            var intProp = new Property<int>(0);
            intProp.value = 1;
            Assert.AreEqual(1, intProp.value);
        }

        [TestMethod]
        public void TestPropertyCallsSubscriberOnSetValue()
        {
            var intProp = new Property<int>(0);
            bool actionWasCalled = false;
            intProp.subscribe(_ => { actionWasCalled = true; });
            intProp.setValue = 1;
            Assert.IsTrue(actionWasCalled);
        }

        [TestMethod]
        public void TestPropertyDoesNotCallSubscriberUntilValue()
        {
            var intProp = new Property<int>(0);
            bool actionWasCalled = false;
            intProp.subscribe(_ => { actionWasCalled = true; });
            Assert.IsFalse(actionWasCalled);
            intProp.value = 1;
            Assert.IsTrue(actionWasCalled);
        }

        [TestMethod]
        public void TestPropertyCallsSubscriberOnValue()
        {
            var intProp = new Property<int>(0);
            bool actionWasCalled = false;
            intProp.subscribe(_ => { actionWasCalled = true; });
            intProp.value = 1;
            Assert.IsTrue(actionWasCalled);
        }

        [TestMethod]
        public void TestPropertyCallsObserverSubscriberOnValue()
        {
            var intProp = new Property<int>(0);
            bool actionWasCalled = false;
            var intObserver = Observer.Create<int>(_ => { actionWasCalled = true; });
            intProp.subscribe(intObserver);
            intProp.value = 1;
            Assert.IsTrue(actionWasCalled);
        }

        [TestMethod]
        public void TestPropertyDoesNotCallsSubscriberAfterUnsubscription()
        {
            var intProp = new Property<int>(0);
            bool actionWasCalled = false;
            var intObserver = Observer.Create<int>(_ => { actionWasCalled = true; });
            intProp.subscribe(intObserver);
            intProp.unsubscribe(intObserver);
            intProp.value = 1;
            Assert.IsFalse(actionWasCalled);
        }

        [TestMethod]
        public void TestPropertyCallsObserverSubscriberOnlyOnce()
        {
            var intProp = new Property<int>(0);
            int actionCalledCount = 0;
            var intObserver = Observer.Create<int>(_ => { actionCalledCount++; });
            intProp.subscribe(intObserver);
            intProp.value = 1;
            Assert.AreEqual(1, actionCalledCount);
        }

        [TestMethod]
        public void TestPropertyPreventsDoubleSubscription()
        {
            var intProp = new Property<int>(0);
            int actionCalledCount = 0;
            var intObserver = Observer.Create<int>(_ => { actionCalledCount++; });
            intProp.subscribe(intObserver);
            intProp.subscribe(intObserver);
            intProp.value = 1;
            Assert.AreEqual(1, actionCalledCount);
        }

        [TestMethod]
        public void TestPropertyCallsAllSubscribers()
        {
            var intProp = new Property<int>(0);
            int actionCalledCount = 0;
            var observerOne = Observer.Create<int>(_ => { actionCalledCount++; });
            var observerTwo = Observer.Create<int>(_ => { actionCalledCount++; });
            intProp.subscribe(observerOne);
            intProp.subscribe(observerTwo);
            intProp.value = 1;
            Assert.AreEqual(2, actionCalledCount);
        }

    }
}
