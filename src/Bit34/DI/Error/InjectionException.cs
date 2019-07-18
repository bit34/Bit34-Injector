// Copyright (c) 2018 Oğuz Sandıkçı
// This code is licensed under MIT license (see LICENSE.txt for details)

using System;


namespace Bit34.DI.Error
{
    public class InjectionException : Exception
    {
        //  MEMBERS
        public readonly InjectionErrorType Error;


        //  CONSTRUCTOR
        public InjectionException(InjectionErrorType error,string message) : base(message)
        {
            Error = error;
        }
    }
}
