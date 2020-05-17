using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading;
using UtilsLite.Pooling;

namespace UtilsLite.Tests.Pooling
{
    [TestClass]
    public class ObjectPoolTests
    {
        [TestMethod]
        public void TestAcquireOneAndRelease()
        {
            var op = new ObjectPool<PoolItem>(
                () => new PoolItem(42, "The Hitchhiker's Guide to the Galaxy"),
                100,
                10);

            var o = op.AcquireObject();

            Assert.AreEqual(99, op.AvailableTokenCount);

            Thread.Sleep(new Random().Next(10, 200));
            op.ReleaseObject(o);

            Assert.AreEqual(100, op.AvailableTokenCount);
        }

        [TestMethod]
        public void TestAcquireFewAndRelease()
        {
            var op = new ObjectPool<PoolItem>(
                () => new PoolItem(42, "The Hitchhiker's Guide to the Galaxy"),
                100,
                10);

            var os = op.AcquireObjects(7).ToList();

            Assert.AreEqual(93, op.AvailableTokenCount);
            Assert.AreEqual(7, os.Count);

            Thread.Sleep(new Random().Next(10, 200));
            os.ForEach(o => op.ReleaseObject(o));

            Assert.AreEqual(100, op.AvailableTokenCount);
        }

        [TestMethod]
        public void TestAcquireAllAndRelease()
        {
            var op = new ObjectPool<PoolItem>(
                () => new PoolItem(42, "The Hitchhiker's Guide to the Galaxy"),
                100,
                10);

            var os = op.AcquireAllObjects().ToList();

            Assert.AreEqual(0, op.AvailableTokenCount);

            Thread.Sleep(new Random().Next(10, 200));
            os.ForEach(o => op.ReleaseObject(o));

            Assert.AreEqual(100, op.AvailableTokenCount);
        }

        [TestMethod]
        public void TestReleaseObjectWithoutAcquiring()
        {
            var op = new ObjectPool<PoolItem>(
                () => new PoolItem(42, "The Hitchhiker's Guide to the Galaxy"),
                100,
                10);
            InvalidOperationException exception = null;

            try
            {
                op.ReleaseObject(new PoolItem(20, "ERROR ITEM"));
            }
            catch (InvalidOperationException ex)
            {
                exception = ex;
            }

            var os = op.AcquireAllObjects().ToList();

            Assert.IsNotNull(exception);

            Assert.AreEqual(100, os.Count);
            Assert.AreEqual(0, os.Count(i => i.Number == 20 || i.Text == "ERROR ITEM"));
        }

        [TestMethod]
        public void TestObjectInitialization()
        {
            var op = new ObjectPool<PoolItem>(
                () => new PoolItem(42, "The Hitchhiker's Guide to the Galaxy"),
                100,
                10);

            var o = op.AcquireObject();

            Assert.IsNotNull(o);
            Assert.AreEqual(42, o.Number);
            Assert.AreEqual("The Hitchhiker's Guide to the Galaxy", o.Text);

            Thread.Sleep(new Random().Next(10, 200));

            op.ReleaseObject(o);
        }

        [TestMethod]
        public void TestPoolSizeModification()
        {
            var op = new ObjectPool<PoolItem>(
                () => new PoolItem(42, "The Hitchhiker's Guide to the Galaxy"),
                100,
                10);

            Assert.AreEqual(100, op.AvailableTokenCount);

            op.ModifyPoolSize(150);

            Assert.AreEqual(150, op.AvailableTokenCount);

            op.ModifyPoolSize(50);

            Assert.AreEqual(50, op.AvailableTokenCount);

            op.ModifyPoolSize(50);

            Assert.AreEqual(50, op.AvailableTokenCount);
        }

        [TestMethod]
        [ExpectedException(typeof(NoAvailableItemsException))]
        public void TestNoAvailableItem()
        {
            var op = new ObjectPool<PoolItem>(
                () => new PoolItem(42, "The Hitchhiker's Guide to the Galaxy"),
                100,
                10);

            op.AcquireAllObjects();

            Assert.AreEqual(0, op.AvailableTokenCount);

            op.AcquireObject();
        }

        [TestMethod]
        public void TestExecuteWithObjectSync()
        {
            var op = new ObjectPool<PoolItem>(
                () => new PoolItem(42, "The Hitchhiker's Guide to the Galaxy"),
                100,
                10);

            var result = op.ExecuteWithObject(item =>
            {
                Assert.AreEqual(99, op.AvailableTokenCount);
                return item.Number + 8;
            });

            Assert.AreEqual(100, op.AvailableTokenCount);
            Assert.AreEqual(50, result);
        }

        [TestMethod]
        [ExpectedException(typeof(NoAvailableItemsException))]
        public void TestExecuteWithObjectSyncAllItemsTaken()
        {
            var op = new ObjectPool<PoolItem>(
                () => new PoolItem(42, "The Hitchhiker's Guide to the Galaxy"),
                100,
                10);

            op.AcquireAllObjects();
            op.ExecuteWithObject(item => item);
        }

        [TestMethod]
        public void TestExecuteWithObject()
        {
            var op = new ObjectPool<PoolItem>(
                () => new PoolItem(42, "The Hitchhiker's Guide to the Galaxy"),
                100,
                10);

            var result = op.ExecuteWithObject(item =>
            {
                Assert.AreEqual(99, op.AvailableTokenCount);
                return item.Number + 8;
            });

            Assert.AreEqual(100, op.AvailableTokenCount);
            Assert.AreEqual(50, result);
        }

        [TestMethod]
        [ExpectedException(typeof(NoAvailableItemsException))]
        public void TestExecuteWithObjectAllItemsTaken()
        {
            var op = new ObjectPool<PoolItem>(
                () => new PoolItem(42, "The Hitchhiker's Guide to the Galaxy"),
                100,
                10);

            op.AcquireAllObjects();
            op.ExecuteWithObject(item => item);
        }

        [TestMethod]
        public void TestExecuteActionWithObject()
        {
            var op = new ObjectPool<PoolItem>(
                () => new PoolItem(42, "The Hitchhiker's Guide to the Galaxy"),
                100,
                10);

            var number = 0;

            op.ExecuteWithObject(o =>
            {
                Assert.AreEqual(99, op.AvailableTokenCount);
                number = o.Number + 8;
            });

            Assert.AreEqual(100, op.AvailableTokenCount);
            Assert.AreEqual(50, number);
        }

        [TestMethod]
        public void AcquireObjectWithinAgeLimit()
        {
            var op = new ObjectPool<PoolItem>(
                () => new PoolItem(42, "The Hitchhiker's Guide to the Galaxy"),
                100,
                10,
                false,
                TimeSpan.FromHours(8));

            Assert.AreEqual(100, op.AvailableTokenCount);

            Thread.Sleep(1200);
            var o = op.AcquireObject();

            Assert.AreEqual(99, op.AvailableTokenCount);
            Assert.IsTrue(o.CreationTime < DateTime.UtcNow - TimeSpan.FromSeconds(1));

            op.ReleaseObject(o);

            Assert.AreEqual(100, op.AvailableTokenCount);
        }

        [TestMethod]
        public void AcquireObjectOverAgeLimit()
        {
            var op = new ObjectPool<PoolItem>(
                () => new PoolItem(42, "The Hitchhiker's Guide to the Galaxy"),
                100,
                10,
                false,
                TimeSpan.FromSeconds(1));

            Assert.AreEqual(100, op.AvailableTokenCount);

            Thread.Sleep(1200);
            var o = op.AcquireObject();

            Assert.AreEqual(99, op.AvailableTokenCount);
            Assert.IsTrue(o.CreationTime > DateTime.UtcNow - TimeSpan.FromSeconds(1));

            op.ReleaseObject(o);

            Assert.AreEqual(100, op.AvailableTokenCount);
        }

        [TestMethod]
        public void ReleaseObjectOverAgeLimit()
        {
            var op = new ObjectPool<PoolItem>(
                () => new PoolItem(42, "The Hitchhiker's Guide to the Galaxy"),
                100,
                10,
                false,
                TimeSpan.FromSeconds(1));

            Assert.AreEqual(100, op.AvailableTokenCount);

            var o = op.AcquireObject();
            Thread.Sleep(1200);

            Assert.AreEqual(99, op.AvailableTokenCount);

            op.ReleaseObject(o);

            Assert.AreEqual(100, op.AvailableTokenCount);
        }

        [TestMethod]
        public void Test_If_Pool_Removes_Faulted_Items_During_Acquire()
        {
            var op = new ObjectPool<PoolItem>(
                () => new PoolItem(42, "The Hitchhiker's Guide to the Galaxy"),
                100,
                10,
                false,
                TimeSpan.FromSeconds(1));

            var exceptionThrown = false;
            const string errorMessage = "Something bad happened...";

            void ThrowEx(PoolItem item)
            {
                exceptionThrown = true;
                op.OnAcquiring.Unsubscribe(ThrowEx);
                throw new Exception(errorMessage);
            }

            op.OnAcquiring.Subscribe(ThrowEx);
            var obj = op.AcquireObject();

            Assert.IsNotNull(obj);
            Assert.IsTrue(exceptionThrown);
        }
    }
}