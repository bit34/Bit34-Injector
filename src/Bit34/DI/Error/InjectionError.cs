// Copyright (c) 2018 Oğuz Sandıkçı
// This code is licensed under MIT license (see LICENSE.txt for details)

using System;


namespace Bit34.DI.Error
{
    public class InjectionError
    {
        //  MEMBERS
        public readonly InjectionErrorType Error;
        public readonly string Message;


        //  CONSTRUCTOR
        public InjectionError(InjectionErrorType error, string message)
        {
            Error = error;
            Message = message;
        }
    }
}
