using CQRSDispatch;
using CQRSDispatch.Interfaces;

namespace Finance.Application.Commands.Users;

public abstract class OwnerBaseCommand<TResult> : ICommand<TResult>
    where TResult : RequestResult
{
    public Guid UserId { get; set; }
}