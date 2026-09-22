using Cafe.Application;
using Cafe.Application.Dtos;
using Cafe.Application.Interfaces;
using Cafe.Application.Services;
using Cafe.Domain.Entities;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Cafe.UnitTests;

public class OrderServiceTests
{
    private readonly Mock<IOrderRepository> _orderRepoMock = new();
    private readonly Mock<IProductRepository> _productRepoMock = new();
    private readonly Mock<ILogger<OrderService>> _loggerMock = new();
    private readonly Mock<IDbContextTransaction> _transactionMock = new();

    public OrderServiceTests()
    {
        _orderRepoMock
            .Setup(r => r.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(_transactionMock.Object);

        _transactionMock
            .Setup(t => t.CommitAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
    }

    private OrderService CreateService() =>
        new(_orderRepoMock.Object, _productRepoMock.Object, _loggerMock.Object);

    private static Product CreateProduct(decimal price, int stock) => new()
    {
        Id = Guid.NewGuid(),
        Name = "Espresso",
        Price = price,
        Stock = stock,
        IsActive = true,
        CategoryId = Guid.NewGuid()
    };

    [Fact]
    public async Task CreateAsync_ComValorTotalCalculadoNoServidor()
    {
        // Arrange
        var produto = CreateProduct(price: 8.50m, stock: 100);
        _productRepoMock
            .Setup(r => r.GetByIdsAsync(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Product> { produto });

        var service = CreateService();
        var request = new CreateOrderRequest
        {
            Items = [new OrderItemRequest { ProductId = produto.Id, Quantity = 3 }]
        };

        // Act + Assert
        var result = await service.CreateAsync(Guid.NewGuid(), request);

        Assert.Equal(25.50m, result.Total);
        Assert.Equal(8.50m, result.Items[0].UnitPrice);
    }

    [Fact]
    public async Task CreateAsync_EstoqueInsuficiente_LancaConflito()
    {
        // Arrange
        var produto = CreateProduct(price: 10m, stock: 2);
        _productRepoMock
            .Setup(r => r.GetByIdsAsync(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Product> { produto });

        var service = CreateService();
        var request = new CreateOrderRequest
        {
            Items = [new OrderItemRequest { ProductId = produto.Id, Quantity = 5 }]
        };

        // Act + Assert
        await Assert.ThrowsAsync<ConflictException>(() => service.CreateAsync(Guid.NewGuid(), request));
    }

    [Fact]
    public async Task CreateAsync_ProdutoInexistente_LancaNotFound()
    {
        // Arrange
        _productRepoMock
            .Setup(r => r.GetByIdsAsync(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Product>());

        var service = CreateService();
        var request = new CreateOrderRequest
        {
            Items = [new OrderItemRequest { ProductId = Guid.NewGuid(), Quantity = 1 }]
        };

        // Act + Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => service.CreateAsync(Guid.NewGuid(), request));
    }

}