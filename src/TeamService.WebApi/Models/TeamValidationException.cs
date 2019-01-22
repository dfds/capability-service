using System;
public class TeamValidationException : ArgumentException {
    public TeamValidationException(string message) : base(message)
    {        
    }
    public TeamValidationException(string message, string paramName) : base(message, paramName) {

    }
}