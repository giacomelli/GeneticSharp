using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using HelperSharp;

namespace GeneticSharp.Domain.Reinsertions
{
    /// <summary>
    /// Exception throw when an error occurs during the execution of reinsert.
    /// </summary>
    [Serializable]
    public sealed class ReinsertionException : Exception
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.Domain.Reinsertions.ReinsertionException"/> class.
        /// </summary>
        /// <param name="reinsertion">The reinsertion where occurred the error.</param>
        /// <param name="message">The error message.</param>
        public ReinsertionException(IReinsertion reinsertion, string message)
            : base("{0}: {1}".With(reinsertion != null ? reinsertion.GetType().Name : String.Empty, message))
        {
            Reinsertion = reinsertion;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.Domain.Reinsertions.ReinsertionException"/> class.
        /// </summary>
        /// <param name="reinsertion">The Reinsertion where occurred the error.</param>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">The inner exception.</param>
        public ReinsertionException(IReinsertion reinsertion, string message, Exception innerException)
            : base("{0}: {1}".With(reinsertion != null ? reinsertion.GetType().Name : String.Empty, message), innerException)
        {
            Reinsertion = reinsertion;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReinsertionException"/> class.
        /// </summary>
        public ReinsertionException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReinsertionException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ReinsertionException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReinsertionException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ReinsertionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReinsertionException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        private ReinsertionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the reinsertion.
        /// </summary>
        /// <value>The reinsertion.</value>
        public IReinsertion Reinsertion { get; private set; }
        #endregion

        #region Methods
        /// <summary>
        /// Sets the <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with information about the exception.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Read="*AllFiles*" PathDiscovery="*AllFiles*" />
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="SerializationFormatter" />
        /// </PermissionSet>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Reinsertion", Reinsertion);
        }
        #endregion
    }
}