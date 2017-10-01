using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Castle.MicroKernel.Registration;
using FluentAssertions;
using NUnit.Framework;

namespace WindsorTests.InterceptorLogging.Tests
{
    public class CompleteLoggingTets : LoggingTests
    {
        [Test]
        public async Task TestAspectOrientedLoggingAsync()
        {
            Container.Register(
                Component.For<Foo>().LifestyleTransient()
                    .NLog(Container).Logger(Logger).Complete());
            var foo = Container.Resolve<Foo>();

            await foo.FAsync(TimeSpan.FromSeconds(0.01)).ConfigureAwait(false);
            Func<Task> a =
                async () => await foo.FAsync(TimeSpan.FromSeconds(.05)).ConfigureAwait(false);
            a.ShouldThrow<KeyNotFoundException>().And.Message.Should().Be("GAsync(2) throws");
            await foo.FAsync(TimeSpan.FromSeconds(.05)).ConfigureAwait(false);
            foo.CancellationTokenSource.Cancel();
            a = async () => await foo.FAsync(TimeSpan.FromSeconds(.05)).ConfigureAwait(false);
            a.ShouldThrow<TaskCanceledException>();
            var expect = new[]
            {
                "[1] FAsync(00:00:00.0100000)",
                "[2] GAsync(1)",
                "[2] GAsync(...) = 1",
                "[1] FAsync(...) = 1",
                "[3] FAsync(00:00:00.0500000)",
                "[4] GAsync(2)",
                "[4] GAsync(2): One or more errors occurred. (GAsync(2) throws)",
                "[3] FAsync(00:00:00.0500000): One or more errors occurred. (GAsync(2) throws)",
                "[5] FAsync(00:00:00.0500000)",
                "[6] GAsync(3)",
                "[6] GAsync(...) = 3",
                "[5] FAsync(...) = 3",
                "[7] FAsync(00:00:00.0500000)",
                "[8] GAsync(4)",
                "[8] GAsync(4): A task was canceled.",
                "[7] FAsync(00:00:00.0500000): A task was canceled."
            }.OrderBy(x => x).ToList();
            // We cannot rely strictly on the ordering of logging, since the logging async results can 
            // be reordered. So we sort the texts for efficient comparison
            var actual = Target.LogEvents
                .Select(x => x.FormattedMessage)
                .OrderBy(x => x)
                .ToList();
            Console.WriteLine("----- EXPECTED -----{0}{1}{0}----- ACTUAL -----{0}{2}{0}",
                Environment.NewLine,
                string.Join(Environment.NewLine, expect),
                string.Join(Environment.NewLine, actual));
            actual.ShouldBeEquivalentTo(expect, cfg => cfg.ExcludingMissingMembers().WithStrictOrdering());
        }

        [Test]
        public void TestAspectOrientedLogging()
        {
            Container.Register(
                Component.For<Foo>().LifestyleTransient()
                    .NLog(Container).Logger(Logger).Complete());
            var foo = Container.Resolve<Foo>();
            foo.F(TimeSpan.FromSeconds(0.01));
            Action a = () => foo.F(TimeSpan.FromSeconds(.05));
            a.ShouldThrow<KeyNotFoundException>().And.Message.Should().Be("G(2) throws");
            foo.F(TimeSpan.FromSeconds(.05));
            foo.CancellationTokenSource.Cancel();
            a = () => foo.F(TimeSpan.FromSeconds(.05));
            a.ShouldThrow<OperationCanceledException>();

            var expect = new[]
            {
                "[1] F(00:00:00.0100000)",
                "[2] G(1)",
                "[2] G(...) = 1",
                "[1] F(...) = 1",
                "[3] F(00:00:00.0500000)",
                "[4] G(2)",
                "[4] G(2): G(2) throws",
                "[3] F(00:00:00.0500000): G(2) throws",
                "[5] F(00:00:00.0500000)",
                "[6] G(3)",
                "[6] G(...) = 3",
                "[5] F(...) = 3",
                "[7] F(00:00:00.0500000)",
                "[8] G(4)",
                "[8] G(4): The operation was canceled.",
                "[7] F(00:00:00.0500000): The operation was canceled."
            }.Select(msg => new {FormattedMessage = msg}).ToList();
            Target.LogEvents
                .ShouldBeEquivalentTo(expect, cfg => cfg.ExcludingMissingMembers().WithStrictOrdering());
        }

        public class Foo
        {
            private long _cnt;
            public CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();

            public virtual async Task<string> FAsync(TimeSpan waitTime)
            {
                var v = await GAsync(Interlocked.Increment(ref _cnt)).ConfigureAwait(false);
                await Task.Delay(waitTime).ConfigureAwait(false);
                return v;
            }

            public virtual async Task<string> GAsync(long i)
            {
                CancellationTokenSource.Token.ThrowIfCancellationRequested();
                await Task.Yield();
                if (i == 2)
                    throw new KeyNotFoundException($"GAsync({i}) throws");
                return i.ToString();
            }

            public virtual string F(TimeSpan waitTime)
            {
                var v = G(Interlocked.Increment(ref _cnt));
                Thread.Sleep(waitTime);
                return v;
            }

            public virtual string G(long i)
            {
                CancellationTokenSource.Token.ThrowIfCancellationRequested();
                if (i == 2)
                    throw new KeyNotFoundException($"G({i}) throws");
                return i.ToString();
            }
        }
    }
}