// <copyright file="LoginUserQueryValidator.cs" company="Muxluk/StayFit">
// Copyright (c) Muxluk/StayFit. All rights reserved.
// </copyright>

using FluentValidation;

namespace StayFit.BLL.Features.Auth.Login;

/// <summary>
/// Валідатор для запиту входу користувача.
/// </summary>
public class LoginUserQueryValidator : AbstractValidator<LoginUserQuery>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LoginUserQueryValidator"/> class.
    /// </summary>
    public LoginUserQueryValidator()
    {
        this.RuleFor(x => x.email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(100);

        this.RuleFor(x => x.password)
            .NotEmpty()
            .MinimumLength(6);
    }
}
