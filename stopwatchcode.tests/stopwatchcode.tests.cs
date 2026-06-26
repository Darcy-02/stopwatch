using NUnit.Framework;
using StopwatchApp.Core;
using System.Threading;

namespace stopwatchcode.Tests
{
    [TestFixture]
    public class StopwatchEngineTests
    {
        private StopwatchEngine _engine = null!;

      
        [SetUp]
        public void SetUp() => _engine = new StopwatchEngine();

        [TearDown]
        public void TearDown() => _engine.Dispose();

        
        [Test]
        public void InitialState_IsIdle()
            => Assert.That(_engine.State, Is.EqualTo(StopwatchState.Idle));

       
        [Test]
        public void InitialSeconds_AreZero()
            => Assert.That(_engine.TotalSeconds, Is.EqualTo(0));

      
        [Test]
        public void InitialFormattedTime_IsZero()
            => Assert.That(_engine.FormattedTime, Is.EqualTo("00:00:00"));

     
        [Test]
        public void Start_SetsState_ToRunning()
        {
            _engine.Start();
            Assert.That(_engine.State, Is.EqualTo(StopwatchState.Running));
        }

        [Test]
        public void Start_ResetsSeconds_ToZero()
        {
            _engine.Start();
            Assert.That(_engine.TotalSeconds, Is.EqualTo(0));
        }

        [Test]
        public void Start_WhenRunning_ThrowsWithMessage()
        {
            _engine.Start();
            var ex = Assert.Throws<InvalidOperationException>(() => _engine.Start());
            Assert.That(ex!.Message, Is.EqualTo("stopwatch is running already"));
        }

     
        [Test]
        public void Start_WhenPaused_Throws()
        {
            _engine.Start();
            _engine.Pause();
            var ex = Assert.Throws<InvalidOperationException>(() => _engine.Start());
            Assert.That(ex!.Message, Is.EqualTo("stopwatch is paused"));
        }

     
     
        [Test]
        public void Pause_SetsState_ToPaused()
        {
            _engine.Start();
            _engine.Pause();
            Assert.That(_engine.State, Is.EqualTo(StopwatchState.Paused));
        }

       
        [Test]
        public void Pause_ReturnsFormattedTime()
        {
            _engine.Start();
            string result = _engine.Pause();
            Assert.That(result, Is.EqualTo(_engine.FormattedTime));
        }

       
        [Test]
        public void Pause_WhenIdle_Throws()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => _engine.Pause());
            Assert.That(ex!.Message, Is.EqualTo("stopwatch is paused"));
        }

        
        [Test]
        public void Pause_FreezesSeconds()
        {
            _engine.Start();
            Thread.Sleep(1200);
            int secondsAtPause = _engine.TotalSeconds;
            _engine.Pause();
            Thread.Sleep(1200); // time passes but timer is stopped
            Assert.That(_engine.TotalSeconds, Is.EqualTo(secondsAtPause));
        }

       
        [Test]
        public void Resume_SetsState_ToRunning()
        {
            _engine.Start();
            _engine.Pause();
            _engine.Resume();
            Assert.That(_engine.State, Is.EqualTo(StopwatchState.Running));
        }

        
        [Test]
        public void Resume_WhenIdle_Throws()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => _engine.Resume());
            Assert.That(ex!.Message, Is.EqualTo("Stopwatch is resumed"));
        }

      
        [Test]
        public void Reset_SetsState_ToIdle()
        {
            _engine.Start();
            _engine.Reset();
            Assert.That(_engine.State, Is.EqualTo(StopwatchState.Idle));
        }

       
        [Test]
        public void Reset_SetsSeconds_ToZero()
        {
            _engine.Start();
            Thread.Sleep(1200);
            _engine.Reset();
            Assert.That(_engine.TotalSeconds, Is.EqualTo(0));
        }

       
        [Test]
        public void Reset_SetsFormattedTime_ToZero()
        {
            _engine.Start();
            _engine.Reset();
            Assert.That(_engine.FormattedTime, Is.EqualTo("00:00:00"));
        }

    
        [Test]
        public void Stop_SetsState_ToStopped()
        {
            _engine.Start();
            _engine.Stop();
            Assert.That(_engine.State, Is.EqualTo(StopwatchState.Stopped));
        }

       
        [Test]
        public void Stop_ReturnsLastTime()
        {
            _engine.Start();
            Thread.Sleep(1200);
            string result = _engine.Stop();
            Assert.That(result, Is.Not.EqualTo("00:00:00"));
        }

        
        [Test]
        public void Stop_WhenIdle_Throws()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => _engine.Stop());
            Assert.That(ex!.Message, Is.EqualTo("stopwatch is has stopped"));
        }

       
        [Test]
        public void Stop_WhenPaused_Succeeds()
        {
            _engine.Start();
            _engine.Pause();
            Assert.DoesNotThrow(() => _engine.Stop());
            Assert.That(_engine.State, Is.EqualTo(StopwatchState.Stopped));
        }


        [Test]
        public void FormatTime_Zero_ReturnsAllZeroes()
            => Assert.That(StopwatchEngine.FormatTime(0), Is.EqualTo("00:00:00"));

        [Test]
        public void FormatTime_3661_Returns_010101()
            => Assert.That(StopwatchEngine.FormatTime(3661), Is.EqualTo("01:01:01"));

        [Test]
        public void FormatTime_3599_Returns_005959()
            => Assert.That(StopwatchEngine.FormatTime(3599), Is.EqualTo("00:59:59"));

        [Test]
        public void FormatTime_Negative_Throws()
            => Assert.Throws<ArgumentOutOfRangeException>(() => StopwatchEngine.FormatTime(-1));
    }
}
