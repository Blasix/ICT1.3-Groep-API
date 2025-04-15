using LU1.Controllers;
using LU1.Models;
using LU1.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace LU1Tests;

public class TrajectTest
{
    private Mock<ITrajectRepository> _trajectRepositoryMock;
    private TrajectController _trajectController;

    [SetUp]
    public void Setup()
    {
        _trajectRepositoryMock = new Mock<ITrajectRepository>();
        _trajectController = new TrajectController(_trajectRepositoryMock.Object);
    }

    /**
     * Acceptatiecriteria: Gebruiker haalt succesvol alle trajecten op via GET /Traject,
     * resulterend in een 200 OK met een lijst van trajecten (Id, Type).
     */
    [Test]
    public async Task GetAll_ReturnsAllTrajects()
    {
        var trajects = new List<Traject>
        {
            new Traject { Id = Guid.NewGuid(), type = "TypeA" },
            new Traject { Id = Guid.NewGuid(), type = "TypeB" }
        };
        _trajectRepositoryMock.Setup(repo => repo.GetAll()).ReturnsAsync(trajects);

        var result = (await _trajectController.GetAll()).Result as OkObjectResult;
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Value as IEnumerable<Traject>, Is.EquivalentTo(trajects));
    }
}