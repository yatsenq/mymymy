// <copyright file="RegisterUserCommandValidator.cs" company="Muxluk/StayFit">
// Copyright (c) Muxluk/StayFit. All rights reserved.
// </copyright>

using FluentValidation;

namespace StayFit.BLL.Features.Auth.Register;

/// <summary>
/// Валідатор для команди реєстрації користувача.
/// </summary>
public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RegisterUserCommandValidator"/> class.
    /// </summary>
    public RegisterUserCommandValidator()
    {
        this.RuleFor(x => x.firstName)
            .NotEmpty()
            .MaximumLength(50);

        this.RuleFor(x => x.lastName)
            .NotEmpty()
            .MaximumLength(50);

        this.RuleFor(x => x.email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(100);

        this.RuleFor(x => x.password)
            .NotEmpty()
            .MinimumLength(6);
    }
}
