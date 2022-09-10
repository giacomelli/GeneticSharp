using System;
using System.Diagnostics;
using NUnit.Framework;

namespace GeneticSharp.Domain.UnitTests
{
    /// <summary>
    /// Asserts for execution flows.
    /// </summary>
    public static class FlowAssert
    {
        /// <summary>
        /// Asserts if at leas one execution flow run without exception..
        /// </summary>
        /// <param name="flows">The Execution flows.</param>
        public static void IsAtLeastOneOk(params Action[] flows)
        {
            if (flows == null)
            {
                throw new ArgumentNullException("flows");
            }

            bool ok = false;
        
            foreach (var a in flows)
            {
                try
                {
                    a();
                    ok = true;
                    break;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    Debug.WriteLine(ex.StackTrace);
                    ok = false;
                }
            }

            Assert.IsTrue(ok);
        }

        /// <summary>
        /// Tries to run the execution flow until there is no exception or until hit the max attempts.
        /// </summary>
        /// <param name="maxAttempts">The max attempts.</param>
        /// <param name="flow">The execution flow.</param>
        public static void IsAtLeastOneAttemptOk(int maxAttempts, Action flow)
        {
            bool ok = false;
            string failedMessage = null;

            for(int i = 0; i < maxAttempts; i++)
            {
                try
                {
                    flow();
                    ok = true;
                    break;
                }
                catch(Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    Debug.WriteLine(ex.StackTrace);
                    ok = false;
                    failedMessage = ex.Message;
                }
            }

            Assert.IsTrue(ok, $"All {maxAttempts} attempts failed\n\n{failedMessage}");
        }
    }
}
