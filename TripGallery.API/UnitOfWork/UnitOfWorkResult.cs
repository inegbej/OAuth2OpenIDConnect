using System;

namespace TripGallery.API.UnitOfWork
{

    public class UnitOfWorkResult<T> where T : class
    {
        // properties
        
        public bool HasError
        {
            get
            {
                return Status != UnitOfWork.UnitOfWorkStatus.Ok;
            }
        }

        public T Result { get; private set; }

        public UnitOfWorkStatus Status { get; private set; }

        public Exception Exception { get; private set; }


        // Ctor
        public UnitOfWorkResult(T result, UnitOfWorkStatus status)
        {
            Result = result;
            Status = status;
        }
        
        public UnitOfWorkResult(T result, UnitOfWorkStatus status, Exception exception)
            : this(result, status)
        {
            Exception = exception;
        }

    }



    public class UnitOfWorkResult
    {

        public bool HasError
        {
            get
            {
                return Status != UnitOfWork.UnitOfWorkStatus.Ok;
            }
        }

        // Properties
        public UnitOfWorkStatus Status { get; private set; }

        public Exception Exception { get; private set; }


        // Ctor
        public UnitOfWorkResult(UnitOfWorkStatus status)
        {
            Status = status;
        }

        public UnitOfWorkResult(UnitOfWorkStatus status, Exception exception)
            : this(status)
        {
            Exception = exception;
        }

    }

}
