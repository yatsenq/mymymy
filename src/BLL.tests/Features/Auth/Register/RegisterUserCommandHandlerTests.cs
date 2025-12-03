// <copyright file="RegisterUserCommandHandlerTests.cs" company="Muxluk/StayFit">
// Copyright (c) Muxluk/StayFit. All rights reserved.
// </copyright>

namespace StayFit.BLL.Tests.Features.Auth.Register
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Moq;
    using Moq.EntityFrameworkCore;
    using StayFit.BLL.Features.Auth.Register;
    using StayFit.DAL.Entities;
    using StayFit.DAL.Exceptions;
    using StayFit.DAL.Interfaces;
    using Xunit;

    /// <summary>
    /// Тести для обробника команди <see cref="RegisterUserCommandHandler"/>.
    /// </summary>
    public class RegisterUserCommandHandlerTests
    {
        private readonly Mock<IApplicationDbContext> contextMock;
        private readonly RegisterUserCommandHandler handler;

        /// <summary>
        /// Ініціалізує новий екземпляр класу <see cref="RegisterUserCommandHandlerTests"/>.
        /// </summary>
        public RegisterUserCommandHandlerTests()
        {
            this.contextMock = new Mock<IApplicationDbContext>();
            this.handler = new RegisterUserCommandHandler(this.contextMock.Object);
        }

        /// <summary>
        /// Тестує, що повертається новий ідентифікатор користувача, якщо електронна пошта унікальна.
        /// </summary>
        /// <returns>Завдання, що представляє асинхронну операцію тестування.</returns>
        [Fact]
        public async Task HandleShouldReturnNewUserIdWhenEmailIsUnique() // FIX CA1707
        {
            // Arrange
            var command = new RegisterUserCommand(
                firstName: "John",
                lastName: "Doe",
                email: "unique@email.com",
                password: "Password123!",
                dateOfBirth: new DateTime(1990, 1, 1),
                gender: "Male",
                height: 180f,
                weight: 80f);
            var users = new List<User>();

            this.contextMock.Setup(x => x.Users).ReturnsDbSet(users);

            // Act
            var result = await this.handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeGreaterThan(0);
            this.contextMock.Verify(x => x.Users.Add(It.IsAny<User>()), Times.Once);
            this.contextMock.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
        }

        /// <summary>
        /// Тестує, що виникає виняток <see cref="BusinessValidationException"/>, якщо електронна пошта вже існує.
        /// </summary>
        /// <returns>Завдання, що представляє асинхронну операцію тестування.</returns>
        [Fact]
        public async Task HandleShouldThrowBusinessValidationExceptionWhenEmailAlreadyExists() // FIX CA1707
        {
            // Arrange
            var command = new RegisterUserCommand(
                firstName: "Jane",
                lastName: "Doe",
                email: "existing@email.com",
                password: "Password123!",
                dateOfBirth: new DateTime(1995, 5, 15),
                gender: "Female",
                height: 170f,
                weight: 60f);
            var users = new List<User> { new User { Email = "existing@email.com" } };

            this.contextMock.Setup(x => x.Users).ReturnsDbSet(users);

            // Act
            Func<Task> act = async () => await this.handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<BusinessValidationException>()
               .WithMessage("User with this email already exists.");

            this.contextMock.Verify(x => x.Users.Add(It.IsAny<User>()), Times.Never);
            this.contextMock.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Never);
        }
    }
}