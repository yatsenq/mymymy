// <copyright file="GetDailySummaryQueryHandlerTests.cs" company="Muxluk/StayFit">
// Copyright (c) Muxluk/StayFit. All rights reserved.
// </copyright>

namespace StayFit.BLL.Tests.Features.FoodDiary.GetDailySummary
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Moq;
    using Moq.EntityFrameworkCore;
    using StayFit.BLL.Features.FoodDiary.GetDailySummary;
    using StayFit.DAL.Entities;
    using StayFit.DAL.Interfaces;
    using Xunit;

    /// <summary>
    /// Тести для обробника запиту <see cref="GetDailySummaryQueryHandler"/>.
    /// </summary>
    public class GetDailySummaryQueryHandlerTests
    {
        private readonly Mock<IApplicationDbContext> contextMock;
        private readonly GetDailySummaryQueryHandler handler;

        /// <summary>
        /// Ініціалізує новий екземпляр класу <see cref="GetDailySummaryQueryHandlerTests"/>.
        /// </summary>
        public GetDailySummaryQueryHandlerTests()
        {
            this.contextMock = new Mock<IApplicationDbContext>();
            this.handler = new GetDailySummaryQueryHandler(this.contextMock.Object);
        }

        /// <summary>
        /// Тестує, що повертаються правильні підсумкові значення поживних речовин, якщо записи про їжу існують.
        /// </summary>
        /// <returns>Завдання, що представляє асинхронну операцію тестування.</returns>
        [Fact]
        public async Task HandleShouldReturnCorrectNutritionalTotalsWhenFoodEntriesExist() // FIX CA1707
        {
            // Arrange
            var query = new GetDailySummaryQuery(1, new DateTime(2025, 10, 27));
            var dailySummaries = new List<DailySummary>
            {
                new DailySummary
                {
                    UserId = 1,
                    Date = new DateTime(2025, 10, 27),
                    TotalCalories = 2000,
                    TotalProtein = 150,
                    TotalFat = 50,
                    TotalCarbs = 200,
                },
            };

            this.contextMock.Setup(x => x.DailySummaries).ReturnsDbSet(dailySummaries);

            // Act
            var result = await this.handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.TotalCalories.Should().Be(2000);
            result.TotalProtein.Should().Be(150);
            result.TotalFat.Should().Be(50);
            result.TotalCarbs.Should().Be(200);
        }

        /// <summary>
        /// Тестує, що повертається нульове зведення, якщо записи про їжу відсутні.
        /// </summary>
        /// <returns>Завдання, що представляє асинхронну операцію тестування.</returns>
        [Fact]
        public async Task HandleShouldReturnZeroSummaryWhenNoFoodEntriesExist() // FIX CA1707
        {
            // Arrange
            var query = new GetDailySummaryQuery(1, new DateTime(2025, 10, 28));
            var dailySummaries = new List<DailySummary>();

            this.contextMock.Setup(x => x.DailySummaries).ReturnsDbSet(dailySummaries);

            // Act
            var result = await this.handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.TotalCalories.Should().Be(0);
            result.TotalProtein.Should().Be(0);
            result.TotalFat.Should().Be(0);
            result.TotalCarbs.Should().Be(0);
        }
    }
}