using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Exceptions;

namespace Restaurants.Application.Users.Commands.UpdateUsers;

public class UpdateUserDetailsComandHandler : IRequestHandler<UpdateUserDetailsCommand>
{
    private readonly ILogger<UpdateUserDetailsComandHandler> _logger;
    private readonly IUserContext _userContext;
    private readonly IUserStore<User> _userStore;
    public UpdateUserDetailsComandHandler(ILogger<UpdateUserDetailsComandHandler> logger,
                                          IUserContext userContext, IUserStore<User> userStore)
    {
        _logger = logger;
        _userContext = userContext;
        _userStore = userStore;
    }
    public async Task Handle(UpdateUserDetailsCommand request, CancellationToken cancellationToken)
    {
        var user = _userContext.GetCurrentUser();
        _logger.LogInformation("Updating User: {userId} with {@request}", user!.Id, request);

        var dbUser = await _userStore.FindByIdAsync(user!.Id, cancellationToken);

        if (dbUser == null) 
        {
            throw new ResourceNotFoundException(nameof(User), user.Id);
        }

        dbUser.Nationality = request.Nationality;
        dbUser.DateOfBirth = request.DateOfBirth;

        await _userStore.UpdateAsync(dbUser, cancellationToken);
    }
}
