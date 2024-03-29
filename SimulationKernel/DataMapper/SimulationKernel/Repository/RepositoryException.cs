﻿namespace DataMapper.SimulationKernel.Repository
{
  using System;

  /// <summary>
  /// Repository exception class.
  /// </summary>
  /// <seealso cref="System.Exception" />
  public class RepositoryException : Exception
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="RepositoryException" /> class.
    /// </summary>
    public RepositoryException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RepositoryException" /> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public RepositoryException(string message)
      : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RepositoryException" /> class.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
    /// ( <see langword="Nothing" /> in Visual Basic) if no inner exception is specified.</param>
    public RepositoryException(string message, Exception innerException)
      : base(message, innerException)
    {
    }
  }
}
