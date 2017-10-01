using System;
using System.Diagnostics.Contracts;
using Castle.DynamicProxy;
using NLog;

namespace WindsorTests.InterceptorLogging
{
    public class DefaultNLogInterceptor<TKey> : DefaultNLogInterceptorBase<TKey>
    {
        private LogLevel _entryLogLevel;

        private LogLevel _exceptionLogLevel;
        private ILogger _logger;

        private LogLevel _returnLogLevel;

        public Func<TKey, DateTime, IInvocation, object, FormattableString> ReturnFormattableString;

        public DefaultNLogInterceptor(
            ILogIdentityFactory<TKey> keyFactory,
            ILogger logger = null,
            LogLevel entryLogLevel = null,
            LogLevel returnLogLevel = null,
            LogLevel exceptionLogLevel = null,
            Func<TKey, IInvocation, FormattableString> entryLogMessage = null,
            Func<TKey, DateTime, IInvocation, object, FormattableString> returnLogMessage = null,
            Func<TKey, DateTime, IInvocation, Exception, FormattableString> exceptionLogMessage = null
        )
        {
            Contract.Requires(!ReferenceEquals(keyFactory, null));
            _logger = logger;
            _entryLogLevel = entryLogLevel;
            _returnLogLevel = returnLogLevel;
            _exceptionLogLevel = exceptionLogLevel;
            KeyFactory = keyFactory;
            EntryFormattableString = entryLogMessage;
            ReturnFormattableString = returnLogMessage;
            ExceptionFormattableString = exceptionLogMessage;
        }

        public override ILogger Logger
        {
            get { return _logger ?? NLogInterceptorDefaults.Logger; }
            set { _logger = value; }
        }

        public override LogLevel EntryLogLevel
        {
            get { return _entryLogLevel ?? NLogInterceptorDefaults.EntryLogLevel; }
            set { _entryLogLevel = value; }
        }

        public override LogLevel ReturnLogLevel
        {
            get { return _returnLogLevel ?? NLogInterceptorDefaults.ReturnLogLevel; }
            set { _returnLogLevel = value; }
        }

        public override LogLevel ExceptionLogLevel
        {
            get { return _exceptionLogLevel ?? NLogInterceptorDefaults.ExceptionLogLevel; }
            set { _exceptionLogLevel = value; }
        }

        public ILogIdentityFactory<TKey> KeyFactory { get; set; }
        public Func<TKey, IInvocation, FormattableString> EntryFormattableString { get; set; }

        public Func<TKey, DateTime, IInvocation, Exception, FormattableString> ExceptionFormattableString { get; set; }

        protected override FormattableString EntryLogMessage(TKey callId, IInvocation invocation)
            => EntryFormattableString == null
                ? base.EntryLogMessage(callId, invocation)
                : EntryFormattableString(callId, invocation);

        protected override FormattableString ReturnLogMessage(TKey callId, DateTime startTime, IInvocation invocation,
            object value)
            => ReturnFormattableString == null
                ? base.ReturnLogMessage(callId, startTime, invocation, value)
                : ReturnFormattableString(callId, startTime, invocation, value);

        protected override FormattableString ExceptionLogMessage(TKey callId, DateTime startTime, IInvocation invocation,
            Exception ex)
            => ExceptionFormattableString == null
                ? base.ExceptionLogMessage(callId, startTime, invocation, ex)
                : ExceptionFormattableString(callId, startTime, invocation, ex);

        protected override TKey MakeNextCallId() => KeyFactory.Next();
    }
}