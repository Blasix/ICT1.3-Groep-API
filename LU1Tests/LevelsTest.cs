using LU1.Controllers;
using LU1.Models;
using LU1.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace LU1Tests;

public class LevelsTest
{
    private Mock<ILevelsRepository> _levelsRepositoryMock;
    private LevelController _levelController;
    private Guid _trajectId;
    private int _step;

    [SetUp]
    public void Setup()
    {
        _trajectId = Guid.NewGuid();
        _step = 1;
        _levelsRepositoryMock = new Mock<ILevelsRepository>();
        _levelController = new LevelController(_levelsRepositoryMock.Object, null);
    }

    /**
     * Acceptatiecriteria: Gebruiker haalt succesvol alle levels op voor een specifieke stap en traject via GET /Level/{step}/{trajectId},
     * resulterend in een 200 OK met een lijst van levels (Id, TrajectId, Step, Url, Tekst, TotalSteps).
     */
    [Test]
    public async Task GetLevelsByStepAndTraject_ReturnsLevels_WhenFound()
    {
        var levels = new List<Level>
        {
            new Level { Id = Guid.NewGuid(), TrajectId = _trajectId, Step = _step, Tekst = "Level 1" },
            new Level { Id = Guid.NewGuid(), TrajectId = _trajectId, Step = _step, Tekst = "Level 2" }
        };
        _levelsRepositoryMock.Setup(repo => repo.GetLevelsByStepAndTrajectId(_step, _trajectId.ToString())).ReturnsAsync(levels);

        var result = await _levelController.GetLevelsByStepAndTraject(_step, _trajectId.ToString()) as OkObjectResult;
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Value as IEnumerable<Level>, Is.EquivalentTo(levels));
    }

    /**
     * Acceptatiecriteria: Gebruiker probeert levels op te halen voor een specifieke stap en traject via GET /Level/{step}/{trajectId},
     * maar er worden geen levels gevonden, resulterend in een 404 NotFound.
     */
    [Test]
    public async Task GetLevelsByStepAndTraject_ReturnsNotFound_WhenNoLevelsFound()
    {
        _levelsRepositoryMock.Setup(repo => repo.GetLevelsByStepAndTrajectId(_step, _trajectId.ToString())).ReturnsAsync(new List<Level>());

        var result = await _levelController.GetLevelsByStepAndTraject(_step, _trajectId.ToString()) as NotFoundResult;
        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(404));
    }

    /**
     * Acceptatiecriteria: Gebruiker probeert levels op te halen voor een specifieke stap en traject via GET /Level/{step}/{trajectId},
     * maar de repository retourneert null, resulterend in een 404 NotFound.
     */
    [Test]
    public async Task GetLevelsByStepAndTraject_ReturnsNotFound_WhenRepositoryReturnsNull()
    {
        _levelsRepositoryMock.Setup(repo => repo.GetLevelsByStepAndTrajectId(_step, _trajectId.ToString())).ReturnsAsync((IEnumerable<Level>)null);

        var result = await _levelController.GetLevelsByStepAndTraject(_step, _trajectId.ToString()) as NotFoundResult;
        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(404));
    }
}