using log4net;
using MonitorListExtractor.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace MonitorListExtractor.Utility
{
    /// <summary>
    /// The Guard Pattern using fluent syntax.
    /// </summary>
    public class Guard : IEvaluation, ITokenCriteria, ICondition, IConditionThan, IMessage, IAssertion, IVerify
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IList<string> failedChainList = new List<string>();

        private bool isConditionTrue;

        private dynamic greaterThanEvaluation;

        private ILog assertionLog;
        private ITokenManager tokenManager;

        private Exception assertionException;
        private GuardException guardException;

        private string message = string.Empty;
        private const string NULL_EVALUATION_MESSAGE = "Guard failed, evaluation was null";
        private const string NULL_LOG_MESSAGE = "Guard failed, log was null";
        private const string NULL_MESSAGE = "Guard failed, message was null";

        private Guard()
        {
            log.Info("Initializing Guard");
        }

        /// <summary>
        /// Initiate the Guard.
        /// </summary>
        public static IEvaluation Against
        {
            get
            {
                return new Guard();
            }
        }

        public ICondition AreEqual(string source, string target)
        {
            this.isConditionTrue = !source.Equals(target, StringComparison.CurrentCultureIgnoreCase);

            if (this.isConditionTrue)
            {
                this.addToFailedChainList(new StackFrame().GetMethod().Name);
            }

            return this;
        }

        public ICondition AreEqual(int source, int target)
        {
            this.isConditionTrue = source != target;

            if (this.isConditionTrue)
            {
                this.addToFailedChainList(new StackFrame().GetMethod().Name);
            }

            return this;
        }

        public ICondition IsTrue(bool evaluation)
        {
            this.isConditionTrue = evaluation != true;

            if (this.isConditionTrue)
            {
                this.addToFailedChainList(new StackFrame().GetMethod().Name);
            }

            return this;
        }

        public ICondition IsFalse(bool evaluation)
        {
            this.isConditionTrue = evaluation != false;

            if (this.isConditionTrue)
            {
                this.addToFailedChainList(new StackFrame().GetMethod().Name);
            }

            return this;
        }

        public ICondition IsNotNullOrEmpty(string evaluation)
        {
            this.isConditionTrue = !string.IsNullOrEmpty(evaluation);

            if (this.isConditionTrue)
            {
                this.addToFailedChainList(new StackFrame().GetMethod().Name);
            }

            return this;
        }

        public ICondition IsNullOrEmpty(string evaluation)
        {
            this.isConditionTrue = string.IsNullOrEmpty(evaluation);

            if (this.isConditionTrue)
            {
                this.addToFailedChainList(new StackFrame().GetMethod().Name);
            }

            return this;
        }

        public ICondition IsValidEmail(string evaluation)
        {
            //this.IsValidRegEx(Constants.RegEx.EMAIL, evaluation);

            return this;
        }

        public ICondition IsValidRegEx(string regEx, string evaluation)
        {
            MatchCollection matchCollection = Regex.Matches(evaluation, regEx);

            this.isConditionTrue = matchCollection.Count != 1;
            if (this.isConditionTrue)
            {
                this.addToFailedChainList(new StackFrame().GetMethod().Name);
            }

            return this;
        }

        public ICondition IsStringOverCertainSize(string evaluation, int maxStringLength)
        {
            if (evaluation.IsNullOrEmpty())
            {
                throw new GuardException(NULL_EVALUATION_MESSAGE);
            }

            int numberOfBytes = evaluation.Length * sizeof(char);
            this.isConditionTrue = !(maxStringLength > numberOfBytes);

            if (this.isConditionTrue)
            {
                this.addToFailedChainList(new StackFrame().GetMethod().Name);
            }

            return this;
        }

        public ICondition IsGreaterThanZero(dynamic evaluation)
        {
            this.isConditionTrue = !(evaluation > 0);

            if (this.isConditionTrue)
            {
                this.addToFailedChainList(new StackFrame().GetMethod().Name);
            }

            return this;
        }

        public ICondition Than(dynamic evaluation)
        {
            this.isConditionTrue = this.greaterThanEvaluation > evaluation;

            if (this.isConditionTrue)
            {
                this.addToFailedChainList(new StackFrame().GetMethod().Name);
            }

            return this;
        }

        public IConditionThan IsGreater(dynamic evaluation)
        {
            this.greaterThanEvaluation = evaluation;

            return this;
        }

        public ICondition IsLessThanZero(dynamic evaluation)
        {
            this.isConditionTrue = !(evaluation < 0);

            if (this.isConditionTrue)
            {
                this.addToFailedChainList(new StackFrame().GetMethod().Name);
            }

            return this;
        }

        public ICondition IsLessThanOrEqualToZero(dynamic evaluation)
        {
            this.isConditionTrue = !(evaluation <= 0);

            if (this.isConditionTrue)
            {
                this.addToFailedChainList(new StackFrame().GetMethod().Name);
            }

            return this;
        }

        public ICondition IsGreaterThanOrEqualToZero(dynamic evaluation)
        {
            this.isConditionTrue = !(evaluation >= 0);

            if (this.isConditionTrue)
            {
                this.addToFailedChainList(new StackFrame().GetMethod().Name);
            }

            return this;
        }

        public ICondition IsNotNull(object evaluation)
        {
            this.isConditionTrue = evaluation is null;

            if (this.isConditionTrue)
            {
                this.addToFailedChainList(new StackFrame().GetMethod().Name);
            }

            return this;
        }

        public ICondition IsNull(object evaluation)
        {
            this.isConditionTrue = !(evaluation is null);

            if (this.isConditionTrue)
            {
                this.addToFailedChainList(new StackFrame().GetMethod().Name);
            }

            return this;
        }

        public ICondition IsNumeric(string evaluation)
        {
            if (evaluation == null)
            {
                throw new GuardException(NULL_EVALUATION_MESSAGE);
            }

            this.isConditionTrue = !double.TryParse(evaluation, out double flag);

            if (this.isConditionTrue)
            {
                this.addToFailedChainList(new StackFrame().GetMethod().Name);
            }

            return this;
        }

        public ICondition IsNaN(object evaluation)
        {
            if (evaluation == null)
            {
                throw new GuardException(NULL_EVALUATION_MESSAGE);
            }

            string valueAsString = evaluation.ToString();
            this.isConditionTrue = !double.TryParse(valueAsString, out double flag);

            if (this.isConditionTrue)
            {
                this.addToFailedChainList(new StackFrame().GetMethod().Name);
            }

            return this;
        }

        public ICondition IsOfType<T>(object evaluation)
        {
            if (evaluation == null)
            {
                throw new GuardException(NULL_EVALUATION_MESSAGE);
            }

            this.isConditionTrue = !(evaluation is T);

            if (this.isConditionTrue)
            {
                this.addToFailedChainList(new StackFrame().GetMethod().Name);
            }

            return this;
        }

        public ICondition IsOfBase<T>(Type evaluation)
        {
            if (evaluation == null)
            {
                throw new GuardException(NULL_EVALUATION_MESSAGE);
            }

            this.isConditionTrue = Assembly.GetAssembly(typeof(T)).GetTypes()
                .Any(x => x.IsAbstract &&
                          x.IsClass &&
                          x.IsSubclassOf(typeof(T)) &&
                          x.Name.Equals(evaluation.Name));

            if (this.isConditionTrue)
            {
                this.addToFailedChainList(new StackFrame().GetMethod().Name);
            }

            return this;
        }

        public IMessage Log(ILog log)
        {
            this.assertionLog = log ?? throw new GuardException(NULL_LOG_MESSAGE);

            return this;
        }

        public IMessage Throw(ILog log)
        {
            this.assertionLog = log ?? throw new GuardException(NULL_LOG_MESSAGE);

            return this;
        }

        public IMessage Throw(ILog log, Exception exception)
        {
            this.assertionLog = log ?? throw new GuardException(NULL_LOG_MESSAGE);
            this.assertionException = exception ?? throw new GuardException(NULL_LOG_MESSAGE);

            return this;
        }

        public IAssertion WithMessage(MethodBase methodBase)
        {
            if (methodBase == null)
            {
                throw new GuardException(NULL_MESSAGE);
            }

            this.message = $"Error in method {methodBase.Name}";

            if (this.assertionException == null)
            {
                this.guardException = new GuardException(this.message);
            }

            return this;
        }

        private void addToFailedChainList(string callingMethod)
        {
            this.failedChainList.Add(callingMethod);
            log.Debug($"Failed Guard {this.failedChainList[this.failedChainList.Count - 1]}");
        }

        public IAssertion WithMessage(string message)
        {
            this.message = message ?? throw new GuardException(NULL_MESSAGE);
            this.guardException = new GuardException(message);

            return this;
        }

        public IVerify DefaultInt(ref int value, int defaultTo, Func<int, bool> condition)
        {
            if (!condition.Invoke(value))
            {
                value = defaultTo;
            }

            return this;
        }

        public IVerify DefaultIfNull<T>(ref T evaluation, T value)
        {
            if (evaluation == null)
            {
                evaluation = value;
            }

            return this;
        }

        public IVerify DefaultIfNull(ref string evaluation, string value)
        {
            if (string.IsNullOrEmpty(evaluation))
            {
                evaluation = value;
            }

            return this;
        }

        public void Verify() => log.Debug("Guard Verification Complete");

        public void Assert()
        {
            log.Debug("Guard Assertion Complete");
            if (!this.isConditionTrue)
            {
                return;
            }

            //RH Debugger.Break();

            this.assertionLog.Error(this.message);

            if (this.assertionException != null)
            {
                throw this.assertionException;
            }

            if (this.guardException != null)
            {
                throw this.guardException;
            }
        }

        public int FailureCount()
        {
            log.Debug("Guard Complete");
            return this.failedChainList.Count;
        }

        public ICondition IsValidToken(string source)
        {
            this.isConditionTrue = !this.tokenManager.IsValidToken(source);

            return this;
        }

        public ITokenCriteria TokenManager(ITokenManager manager)
        {
            this.tokenManager = manager ?? throw new GuardException(NULL_LOG_MESSAGE);

            return this;
        }
    }

    public interface ITokenCriteria
    {
        ICondition IsValidToken(string source);
    }

    public interface IEvaluation
    {
        ITokenCriteria TokenManager(ITokenManager tokenManager);

        ICondition AreEqual(string source, string target);

        ICondition AreEqual(int source, int target);

        ICondition IsTrue(bool evaluation);

        ICondition IsFalse(bool evaluation);

        ICondition IsNotNull(object evaluation);

        ICondition IsNull(object evaluation);

        ICondition IsNumeric(string evaluation);

        ICondition IsNullOrEmpty(string evaluation);

        ICondition IsValidRegEx(string regEx, string evaluation);

        ICondition IsValidEmail(string username);

        ICondition IsNotNullOrEmpty(string evaluation);

        ICondition IsNaN(object evaluation);

        ICondition IsGreaterThanZero(dynamic evaluation);

        ICondition IsLessThanZero(dynamic evaluation);

        ICondition IsLessThanOrEqualToZero(dynamic evaluation);

        IConditionThan IsGreater(dynamic evaluation);

        ICondition IsGreaterThanOrEqualToZero(dynamic evaluation);

        ICondition IsOfType<T>(object evaluation);

        ICondition IsOfBase<T>(Type evaluation);

        ICondition IsStringOverCertainSize(string evaluation, int maxStringLength);

        IVerify DefaultIfNull(ref string evaluation, string value);

        IVerify DefaultIfNull<T>(ref T evaluation, T value);

        IVerify DefaultInt(ref int value, int defaultTo, Func<int, bool> condition);
    }

    /// <summary>
    /// Message associated with condition
    /// </summary>
    public interface IMessage
    {
        IAssertion WithMessage(string message);

        IAssertion WithMessage(MethodBase methodBase);
    }

    /// <summary>
    /// Evaluate a condition then
    /// </summary>
    public interface IConditionThan
    {
        ICondition Than(dynamic evaluation);
    }

    /// <summary>
    /// Evaluate a condition
    /// </summary>
    public interface ICondition
    {
        IMessage Log(ILog log);

        IMessage Throw(ILog log);

        IMessage Throw(ILog log, Exception exception);
    }

    public interface IFailureCount
    {
        int FailureCount();
    }

    /// <summary>
    /// Verify the Guard
    /// </summary>
    public interface IVerify : IFailureCount
    {
        void Verify();
    }

    /// <summary>
    /// Assert the Guard
    /// </summary>
    public interface IAssertion : IFailureCount
    {
        void Assert();
    }
}
