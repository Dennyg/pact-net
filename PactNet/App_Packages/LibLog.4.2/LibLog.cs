//===============================================================================
// LibLog
//
// https://github.com/damianh/LibLog
//===============================================================================
// Copyright © 2011-2015 Damian Hickey.  All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
//===============================================================================

// ReSharper disable PossibleNullReferenceException

// Define LIBLOG_PORTABLE conditional compilation symbol for PCL compatibility
//
// Define LIBLOG_PUBLIC to enable ability to GET a logger (LogProvider.For<>() etc) from outside this library. NOTE:
// this can have unintendend consequences of consumers of your library using your library to resolve a logger. If the
// reason is because you want to open this functionality to other projects within your solution,
// consider [InternalVisibleTo] instead.
// 
// Define LIBLOG_PROVIDERS_ONLY if your library provides its own logging API and you just want to use the
// LibLog providers internally to provide built in support for popular logging frameworks.

#pragma warning disable 1591

// If you copied this file manually, you need to change all "YourRootNameSpace" so not to clash with other libraries
// that use LibLog
#if LIBLOG_PROVIDERS_ONLY
namespace PactNet.LibLog
#else
namespace PactNet.Logging
#endif
{
    using System.Collections.Generic;
#if LIBLOG_PROVIDERS_ONLY
    using PactNet.LibLog.LogProviders;
#else
    using PactNet.Logging.LogProviders;
#endif
    using System;
#if !LIBLOG_PROVIDERS_ONLY
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
#endif

#if LIBLOG_PROVIDERS_ONLY
    internal
#else
    public
#endif
    delegate bool Logger(LogLevel logLevel, Func<string> messageFunc, Exception exception = null, params object[] formatParameters);

#if !LIBLOG_PROVIDERS_ONLY
    /// <summary>
    /// Simple interface that represent a logger.
    /// </summary>
#if LIBLOG_PUBLIC
    public
#else
    internal
#endif
    interface ILog
    {
        /// <summary>
        /// Log a message the specified log level.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <param name="messageFunc">The message function.</param>
        /// <param name="exception">An optional exception.</param>
        /// <param name="formatParameters">Optional format parameters for the message generated by the messagefunc. </param>
        /// <returns>true if the message was logged. Otherwise false.</returns>
        /// <remarks>
        /// Note to implementers: the message func should not be called if the loglevel is not enabled
        /// so as not to incur performance penalties.
        /// 
        /// To check IsEnabled call Log with only LogLevel and check the return value, no event will be written.
        /// </remarks>
        bool Log(LogLevel logLevel, Func<string> messageFunc, Exception exception = null, params object[] formatParameters );
    }
#endif

    /// <summary>
    /// The log level.
    /// </summary>
#if LIBLOG_PROVIDERS_ONLY
    internal
#else
    public
#endif
    enum LogLevel
    {
        Trace,
        Debug,
        Info,
        Warn,
        Error,
        Fatal
    }

#if !LIBLOG_PROVIDERS_ONLY
#if LIBLOG_PUBLIC
    public
#else
    internal
#endif
    static partial class LogExtensions
    {
        public static bool IsDebugEnabled(this ILog logger)
        {
            GuardAgainstNullLogger(logger);
            return logger.Log(LogLevel.Debug, null);
        }

        public static bool IsErrorEnabled(this ILog logger)
        {
            GuardAgainstNullLogger(logger);
            return logger.Log(LogLevel.Error, null);
        }

        public static bool IsFatalEnabled(this ILog logger)
        {
            GuardAgainstNullLogger(logger);
            return logger.Log(LogLevel.Fatal, null);
        }

        public static bool IsInfoEnabled(this ILog logger)
        {
            GuardAgainstNullLogger(logger);
            return logger.Log(LogLevel.Info, null);
        }

        public static bool IsTraceEnabled(this ILog logger)
        {
            GuardAgainstNullLogger(logger);
            return logger.Log(LogLevel.Trace, null);
        }

        public static bool IsWarnEnabled(this ILog logger)
        {
            GuardAgainstNullLogger(logger);
            return logger.Log(LogLevel.Warn, null);
        }

        public static void Debug(this ILog logger, Func<string> messageFunc)
        {
            GuardAgainstNullLogger(logger);
            logger.Log(LogLevel.Debug, messageFunc);
        }

        public static void Debug(this ILog logger, string message)
        {
            if (logger.IsDebugEnabled())
            {
                logger.Log(LogLevel.Debug, message.AsFunc());
            }
        }

        public static void DebugFormat(this ILog logger, string message, params object[] args)
        {
            if (logger.IsDebugEnabled())
            {
                logger.LogFormat(LogLevel.Debug, message, args);
            }
        }

        public static void DebugException(this ILog logger, string message, Exception exception)
        {
            if (logger.IsDebugEnabled())
            {
                logger.Log(LogLevel.Debug, message.AsFunc(), exception);
            }
        }

        public static void DebugException(this ILog logger, string message, Exception exception, params object[] formatParams)
        {
            if (logger.IsDebugEnabled())
            {
                logger.Log(LogLevel.Debug, message.AsFunc(), exception, formatParams);
            }
        }

        public static void Error(this ILog logger, Func<string> messageFunc)
        {
            logger.Log(LogLevel.Error, messageFunc);
        }

        public static void Error(this ILog logger, string message)
        {
            if (logger.IsErrorEnabled())
            {
                logger.Log(LogLevel.Error, message.AsFunc());
            }
        }

        public static void ErrorFormat(this ILog logger, string message, params object[] args)
        {
            if (logger.IsErrorEnabled())
            {
                logger.LogFormat(LogLevel.Error, message, args);
            }
        }

        public static void ErrorException(this ILog logger, string message, Exception exception, params object[] formatParams)
        {
            if (logger.IsErrorEnabled())
            {
                logger.Log(LogLevel.Error, message.AsFunc(), exception, formatParams);
            }
        }

        public static void Fatal(this ILog logger, Func<string> messageFunc)
        {
            logger.Log(LogLevel.Fatal, messageFunc);
        }

        public static void Fatal(this ILog logger, string message)
        {
            if (logger.IsFatalEnabled())
            {
                logger.Log(LogLevel.Fatal, message.AsFunc());
            }
        }

        public static void FatalFormat(this ILog logger, string message, params object[] args)
        {
            if (logger.IsFatalEnabled())
            {
                logger.LogFormat(LogLevel.Fatal, message, args);
            }
        }

        public static void FatalException(this ILog logger, string message, Exception exception, params object[] formatParams)
        {
            if (logger.IsFatalEnabled())
            {
                logger.Log(LogLevel.Fatal, message.AsFunc(), exception, formatParams);
            }
        }

        public static void Info(this ILog logger, Func<string> messageFunc)
        {
            GuardAgainstNullLogger(logger);
            logger.Log(LogLevel.Info, messageFunc);
        }

        public static void Info(this ILog logger, string message)
        {
            if (logger.IsInfoEnabled())
            {
                logger.Log(LogLevel.Info, message.AsFunc());
            }
        }

        public static void InfoFormat(this ILog logger, string message, params object[] args)
        {
            if (logger.IsInfoEnabled())
            {
                logger.LogFormat(LogLevel.Info, message, args);
            }
        }

        public static void InfoException(this ILog logger, string message, Exception exception, params object[] formatParams)
        {
            if (logger.IsInfoEnabled())
            {
                logger.Log(LogLevel.Info, message.AsFunc(), exception, formatParams);
            }
        }

        public static void Trace(this ILog logger, Func<string> messageFunc)
        {
            GuardAgainstNullLogger(logger);
            logger.Log(LogLevel.Trace, messageFunc);
        }

        public static void Trace(this ILog logger, string message)
        {
            if (logger.IsTraceEnabled())
            {
                logger.Log(LogLevel.Trace, message.AsFunc());
            }
        }

        public static void TraceFormat(this ILog logger, string message, params object[] args)
        {
            if (logger.IsTraceEnabled())
            {
                logger.LogFormat(LogLevel.Trace, message, args);
            }
        }

        public static void TraceException(this ILog logger, string message, Exception exception, params object[] formatParams)
        {
            if (logger.IsTraceEnabled())
            {
                logger.Log(LogLevel.Trace, message.AsFunc(), exception, formatParams);
            }
        }

        public static void Warn(this ILog logger, Func<string> messageFunc)
        {
            GuardAgainstNullLogger(logger);
            logger.Log(LogLevel.Warn, messageFunc);
        }

        public static void Warn(this ILog logger, string message)
        {
            if (logger.IsWarnEnabled())
            {
                logger.Log(LogLevel.Warn, message.AsFunc());
            }
        }

        public static void WarnFormat(this ILog logger, string message, params object[] args)
        {
            if (logger.IsWarnEnabled())
            {
                logger.LogFormat(LogLevel.Warn, message, args);
            }
        }

        public static void WarnException(this ILog logger, string message, Exception exception, params object[] formatParams)
        {
            if (logger.IsWarnEnabled())
            {
                logger.Log(LogLevel.Warn, message.AsFunc(), exception, formatParams);
            }
        }

        // ReSharper disable once UnusedParameter.Local
        private static void GuardAgainstNullLogger(ILog logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }
        }

        private static void LogFormat(this ILog logger, LogLevel logLevel, string message, params object[] args)
        {
            logger.Log(logLevel, message.AsFunc(), null, args);
        }

        // Avoid the closure allocation, see https://gist.github.com/AArnott/d285feef75c18f6ecd2b
        private static Func<T> AsFunc<T>(this T value) where T : class
        {
            return value.Return;
        }

        private static T Return<T>(this T value)
        {
            return value;
        }
    }
#endif

    /// <summary>
    /// Represents a way to get a <see cref="ILog"/>
    /// </summary>
#if LIBLOG_PROVIDERS_ONLY
    internal
#else
    public
#endif
    interface ILogProvider
    {
        /// <summary>
        /// Gets the specified named logger.
        /// </summary>
        /// <param name="name">Name of the logger.</param>
        /// <returns>The logger reference.</returns>
        Logger GetLogger(string name);

        /// <summary>
        /// Opens a nested diagnostics context. Not supported in EntLib logging.
        /// </summary>
        /// <param name="message">The message to add to the diagnostics context.</param>
        /// <returns>A disposable that when disposed removes the message from the context.</returns>
        IDisposable OpenNestedContext(string message);

        /// <summary>
        /// Opens a mapped diagnostics context. Not supported in EntLib logging.
        /// </summary>
        /// <param name="key">A key.</param>
        /// <param name="value">A value.</param>
        /// <returns>A disposable that when disposed removes the map from the context.</returns>
        IDisposable OpenMappedContext(string key, string value);
    }

    /// <summary>
    /// Provides a mechanism to create instances of <see cref="ILog" /> objects.
    /// </summary>
#if LIBLOG_PROVIDERS_ONLY
    internal
#else
    public
#endif
    static class LogProvider
    {
#if !LIBLOG_PROVIDERS_ONLY
        /// <summary>
        /// The disable logging environment variable. If the environment variable is set to 'true', then logging
        /// will be disabled.
        /// </summary>
        public const string DisableLoggingEnvironmentVariable = "PactNet_LIBLOG_DISABLE";
        private const string NullLogProvider = "Current Log Provider is not set. Call SetCurrentLogProvider " +
                                               "with a non-null value first.";
        private static dynamic _currentLogProvider;
        private static Action<ILogProvider> _onCurrentLogProviderSet;

        //Pact Custom addition
        public static string LogFilePath { get; set; }

        static LogProvider()
        {
            IsDisabled = false;
        }

        /// <summary>
        /// Sets the current log provider.
        /// </summary>
        /// <param name="logProvider">The log provider.</param>
        public static void SetCurrentLogProvider(ILogProvider logProvider)
        {
            _currentLogProvider = logProvider;

            RaiseOnCurrentLogProviderSet();
        }

        /// <summary>
        /// Gets or sets a value indicating whether this is logging is disabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if logging is disabled; otherwise, <c>false</c>.
        /// </value>
        public static bool IsDisabled { get; set; }

        /// <summary>
        /// Sets an action that is invoked when a consumer of your library has called SetCurrentLogProvider. It is 
        /// important that hook into this if you are using child libraries (especially ilmerged ones) that are using
        /// LibLog (or other logging abstraction) so you adapt and delegate to them.
        /// <see cref="SetCurrentLogProvider"/> 
        /// </summary>
        internal static Action<ILogProvider> OnCurrentLogProviderSet
        {
            set
            {
                _onCurrentLogProviderSet = value;
                RaiseOnCurrentLogProviderSet();
            }
        }

        internal static ILogProvider CurrentLogProvider
        {
            get
            {
                return _currentLogProvider;
            }
        }

        /// <summary>
        /// Gets a logger for the specified type.
        /// </summary>
        /// <typeparam name="T">The type whose name will be used for the logger.</typeparam>
        /// <returns>An instance of <see cref="ILog"/></returns>
#if LIBLOG_PUBLIC
        public
#else
        internal
#endif
        static ILog For<T>()
        {
            return GetLogger(typeof(T));
        }

#if !LIBLOG_PORTABLE
        /// <summary>
        /// Gets a logger for the current class.
        /// </summary>
        /// <returns>An instance of <see cref="ILog"/></returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
#if LIBLOG_PUBLIC
        public
#else
        internal
#endif
        static ILog GetCurrentClassLogger()
        {
            var stackFrame = new StackFrame(1, false);
            return GetLogger(stackFrame.GetMethod().DeclaringType);
        }
#endif

        /// <summary>
        /// Gets a logger for the specified type.
        /// </summary>
        /// <param name="type">The type whose name will be used for the logger.</param>
        /// <returns>An instance of <see cref="ILog"/></returns>
#if LIBLOG_PUBLIC
        public
#else
        internal
#endif
        static ILog GetLogger(Type type)
        {
            return GetLogger(type.FullName);
        }

        /// <summary>
        /// Gets a logger with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>An instance of <see cref="ILog"/></returns>
#if LIBLOG_PUBLIC
        public
#else
        internal
#endif
        static ILog GetLogger(string name)
        {
            ILogProvider logProvider = CurrentLogProvider ?? ResolveLogProvider();
            return logProvider == null 
                ? NoOpLogger.Instance
                : (ILog)new LoggerExecutionWrapper(logProvider.GetLogger(name), () => IsDisabled);
        }

        /// <summary>
        /// Opens a nested diagnostics context.
        /// </summary>
        /// <param name="message">A message.</param>
        /// <returns>An <see cref="IDisposable"/> that closes context when disposed.</returns>
#if LIBLOG_PUBLIC
        public
#else
        internal
#endif
        static IDisposable OpenNestedContext(string message)
        {
            if(CurrentLogProvider == null)
            {
                throw new InvalidOperationException(NullLogProvider);
            }
            return CurrentLogProvider.OpenNestedContext(message);
        }

        /// <summary>
        /// Opens a mapped diagnostics context.
        /// </summary>
        /// <param name="key">A key.</param>
        /// <param name="value">A value.</param>
        /// <returns>An <see cref="IDisposable"/> that closes context when disposed.</returns>
#if LIBLOG_PUBLIC
        public
#else
        internal
#endif
        static IDisposable OpenMappedContext(string key, string value)
        {
            if (CurrentLogProvider == null)
            {
                throw new InvalidOperationException(NullLogProvider);
            }
            return CurrentLogProvider.OpenMappedContext(key, value);
        }
#endif

#if LIBLOG_PROVIDERS_ONLY
    private
#else
    internal
#endif
    delegate bool IsLoggerAvailable();

#if LIBLOG_PROVIDERS_ONLY
    private
#else
    internal
#endif
    delegate ILogProvider CreateLogProvider();

#if LIBLOG_PROVIDERS_ONLY
    private
#else
    internal
#endif
    static readonly List<Tuple<IsLoggerAvailable, CreateLogProvider>> LogProviderResolvers =
            new List<Tuple<IsLoggerAvailable, CreateLogProvider>>
        {
        };

#if !LIBLOG_PROVIDERS_ONLY
        private static void RaiseOnCurrentLogProviderSet()
        {
            if (_onCurrentLogProviderSet != null)
            {
                _onCurrentLogProviderSet(_currentLogProvider);
            }
        }
#endif

        internal static ILogProvider ResolveLogProvider()
        {
            try
            {
                foreach (var providerResolver in LogProviderResolvers)
                {
                    if (providerResolver.Item1())
                    {
                        return providerResolver.Item2();
                    }
                }
            }
            catch (Exception ex)
            {
#if LIBLOG_PORTABLE
                Debug.WriteLine(
#else
                Console.WriteLine(
#endif
                    "Exception occured resolving a log provider. Logging for this assembly {0} is disabled. {1}",
                    typeof(LogProvider).GetAssemblyPortable().FullName,
                    ex);
            }
            return null;
        }

#if !LIBLOG_PROVIDERS_ONLY
        internal class NoOpLogger : ILog
        {
            internal static readonly NoOpLogger Instance = new NoOpLogger();

            public bool Log(LogLevel logLevel, Func<string> messageFunc, Exception exception, params object[] formatParameters)
            {
                return false;
            }
        }
#endif
    }

#if !LIBLOG_PROVIDERS_ONLY
    internal class LoggerExecutionWrapper : ILog
    {
        private readonly Logger _logger;
        private readonly Func<bool> _getIsDisabled;
        internal const string FailedToGenerateLogMessage = "Failed to generate log message";

        internal LoggerExecutionWrapper(Logger logger, Func<bool> getIsDisabled = null)
        {
            _logger = logger;
            _getIsDisabled = getIsDisabled ?? (() => false);
        }

        internal Logger WrappedLogger
        {
            get { return _logger; }
        }

        public bool Log(LogLevel logLevel, Func<string> messageFunc, Exception exception = null, params object[] formatParameters)
        {
#if LIBLOG_PORTABLE
            if (_getIsDisabled())
            {
                return false;
            }
#else
            var envVar = Environment.GetEnvironmentVariable(LogProvider.DisableLoggingEnvironmentVariable);

            if (_getIsDisabled() || (envVar != null && envVar.Equals("true", StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }
#endif

            if (messageFunc == null)
            {
                return _logger(logLevel, null);
            }

            Func<string> wrappedMessageFunc = () =>
            {
                try
                {
                    return messageFunc();
                }
                catch (Exception ex)
                {
                    Log(LogLevel.Error, () => FailedToGenerateLogMessage, ex);
                }
                return null;
            };
            return _logger(logLevel, wrappedMessageFunc, exception, formatParameters);
        }
    }
#endif
}

#if LIBLOG_PROVIDERS_ONLY
namespace PactNet.LibLog.LogProviders
#else
namespace PactNet.Logging.LogProviders
#endif
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;
    using System.Text.RegularExpressions;

    internal abstract class LogProviderBase : ILogProvider
    {
        protected delegate IDisposable OpenNdc(string message);
        protected delegate IDisposable OpenMdc(string key, string value);

        private readonly Lazy<OpenNdc> _lazyOpenNdcMethod;
        private readonly Lazy<OpenMdc> _lazyOpenMdcMethod;
        private static readonly IDisposable NoopDisposableInstance = new DisposableAction();

        protected LogProviderBase()
        {
            _lazyOpenNdcMethod 
                = new Lazy<OpenNdc>(GetOpenNdcMethod);
            _lazyOpenMdcMethod
               = new Lazy<OpenMdc>(GetOpenMdcMethod);
        }

        public abstract Logger GetLogger(string name);

        public IDisposable OpenNestedContext(string message)
        {
            return _lazyOpenNdcMethod.Value(message);
        }

        public IDisposable OpenMappedContext(string key, string value)
        {
            return _lazyOpenMdcMethod.Value(key, value);
        }

        protected virtual OpenNdc GetOpenNdcMethod()
        {
            return _ => NoopDisposableInstance;
        }

        protected virtual OpenMdc GetOpenMdcMethod()
        {
            return (_, __) => NoopDisposableInstance;
        }
    }

    internal static class TraceEventTypeValues
    {
        internal static readonly Type Type;
        internal static readonly int Verbose;
        internal static readonly int Information;
        internal static readonly int Warning;
        internal static readonly int Error;
        internal static readonly int Critical;

        static TraceEventTypeValues()
        {
            var assembly = typeof(Uri).GetAssemblyPortable(); // This is to get to the System.dll assembly in a PCL compatible way.
            if (assembly == null)
            {
                return;
            }
            Type = assembly.GetType("System.Diagnostics.TraceEventType");
            if (Type == null) return;
            Verbose = (int)Enum.Parse(Type, "Verbose", false);
            Information = (int)Enum.Parse(Type, "Information", false);
            Warning = (int)Enum.Parse(Type, "Warning", false);
            Error = (int)Enum.Parse(Type, "Error", false);
            Critical = (int)Enum.Parse(Type, "Critical", false);
        }
    }

    internal static class LogMessageFormatter
    {
        private static readonly Regex Pattern = new Regex(@"\{@?\w{1,}\}");

        /// <summary>
        /// Some logging frameworks support structured logging, such as serilog. This will allow you to add names to structured data in a format string:
        /// For example: Log("Log message to {user}", user). This only works with serilog, but as the user of LibLog, you don't know if serilog is actually 
        /// used. So, this class simulates that. it will replace any text in {curlybraces} with an index number. 
        /// 
        /// "Log {message} to {user}" would turn into => "Log {0} to {1}". Then the format parameters are handled using regular .net string.Format.
        /// </summary>
        /// <param name="messageBuilder">The message builder.</param>
        /// <param name="formatParameters">The format parameters.</param>
        /// <returns></returns>
        public static Func<string> SimulateStructuredLogging(Func<string> messageBuilder, object[] formatParameters)
        {
            if (formatParameters == null || formatParameters.Length == 0)
            {
                return messageBuilder;
            }

            return () =>
            {
                string targetMessage = messageBuilder();
                int argumentIndex = 0;
                foreach (Match match in Pattern.Matches(targetMessage))
                {
                    int notUsed;
                    if (!int.TryParse(match.Value.Substring(1, match.Value.Length -2), out notUsed))
                    {
                        targetMessage = ReplaceFirst(targetMessage, match.Value,
                            "{" + argumentIndex++ + "}");
                    }
                }
                try
                {
                    return string.Format(CultureInfo.InvariantCulture, targetMessage, formatParameters);
                }
                catch (FormatException ex)
                {
                    throw new FormatException("The input string '" + targetMessage + "' could not be formatted using string.Format", ex);
                }
            };
        }

        private static string ReplaceFirst(string text, string search, string replace)
        {
            int pos = text.IndexOf(search, StringComparison.Ordinal);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }
    }

    internal static class TypeExtensions
    {
        internal static MethodInfo GetMethodPortable(this Type type, string name)
        {
#if LIBLOG_PORTABLE
            return type.GetRuntimeMethod(name, new Type[]{});
#else
            return type.GetMethod(name);
#endif
        }

        internal static MethodInfo GetMethodPortable(this Type type, string name, params Type[] types)
        {
#if LIBLOG_PORTABLE
            return type.GetRuntimeMethod(name, types);
#else
            return type.GetMethod(name, types);
#endif
        }

        internal static PropertyInfo GetPropertyPortable(this Type type, string name)
        {
#if LIBLOG_PORTABLE
            return type.GetRuntimeProperty(name);
#else
            return type.GetProperty(name);
#endif
        }

        internal static IEnumerable<FieldInfo> GetFieldsPortable(this Type type)
        {
#if LIBLOG_PORTABLE
            return type.GetRuntimeFields();
#else
            return type.GetFields();
#endif
        }

        internal static Type GetBaseTypePortable(this Type type)
        {
#if LIBLOG_PORTABLE
            return type.GetTypeInfo().BaseType;
#else
            return type.BaseType;
#endif
        }

#if LIBLOG_PORTABLE
        internal static MethodInfo GetGetMethod(this PropertyInfo propertyInfo)
        {
            return propertyInfo.GetMethod;
        }

        internal static MethodInfo GetSetMethod(this PropertyInfo propertyInfo)
        {
            return propertyInfo.SetMethod;
        }
#endif

#if !LIBLOG_PORTABLE
        internal static object CreateDelegate(this MethodInfo methodInfo, Type delegateType)
        {
            return Delegate.CreateDelegate(delegateType, methodInfo);
        }
#endif

        internal static Assembly GetAssemblyPortable(this Type type)
        {
#if LIBLOG_PORTABLE
            return type.GetTypeInfo().Assembly;
#else
            return type.Assembly;
#endif
        }
    }

    internal class DisposableAction : IDisposable
    {
        private readonly Action _onDispose;

        public DisposableAction(Action onDispose = null)
        {
            _onDispose = onDispose;
        }

        public void Dispose()
        {
            if(_onDispose != null)
            {
                _onDispose();
            }
        }
    }
}
