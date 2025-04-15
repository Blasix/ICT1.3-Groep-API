using System.Security.Claims;
using LU1.Controllers;
using LU1.Models;
using LU1.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace LU1Tests;

public class ChildTests
{
    Mock<IChildRepository> _childRepositoryMock;
    private ChildController _childController;
    Guid _userId;
    
  
    [SetUp]
    public void Setup()
    {
        _childRepositoryMock = new Mock<IChildRepository>();
        _childController = new ChildController(_childRepositoryMock.Object, null); 

        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, _userId.ToString()) };
        var identity = new ClaimsIdentity(claims);
        var principal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = principal };
        _childController.ControllerContext = new ControllerContext { HttpContext = httpContext };
    }
    
    /**
     * Acceptatiecriteria: Gebruiker haalt succesvol zijn gekoppelde kinderen op via GET /children, resulterend in een 200 OK met een lijst van kinderen (Id, Name, UserId).
     */
    [Test]
    public async Task Get_ReturnsUserChildren()
    {
        var children = new List<Child>
        {
            new Child { Id = Guid.NewGuid(), Name = "c1", UserId = _userId },
            new Child { Id = Guid.NewGuid(), Name = "c2", UserId = _userId }
        };
        _childRepositoryMock.Setup(repo => repo.GetByUserId(_userId.ToString())).ReturnsAsync(children);
        
        var result = (await _childController.GetAll()).Result as OkObjectResult;
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Value as IEnumerable<Child>, Is.EquivalentTo(children));
    }
    
    /**
     * Acceptatiecriteria: Gebruiker maakt succesvol een nieuw kind aan via POST /children, resulterend in een 201 Created met het aangemaakte kind (Id, Name, UserId).
     */
    [Test]
    public async Task Create_CreatesChild_WhenValid()
    {
        var child = new Child { Name = "c1" };
        _childRepositoryMock.Setup(repo => repo.GetByUserId(_userId.ToString())).ReturnsAsync(new List<Child>());
        _childRepositoryMock.Setup(repo => repo.Add(child)).Returns(Task.CompletedTask);
        
        var result = (await _childController.Create(child)).Result as CreatedAtActionResult;
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Value, Is.EqualTo(child));
    }
    
    /**
     * Acceptatiecriteria: Gebruiker update succesvol een kind via PUT /children/{id}, resulterend in een 200 OK.
     */
    [Test]
    public async Task Update_UpdatesChild_WhenValid()
    {
        var child = new Child { Id = Guid.NewGuid(), Name = "c1" };
        _childRepositoryMock.Setup(repo => repo.GetByUserId(_userId.ToString())).ReturnsAsync(new List<Child> { child });
        _childRepositoryMock.Setup(repo => repo.Update(child)).Returns(Task.CompletedTask);
        
        var result = await _childController.Update(child.Id.ToString(), child);
        Assert.That(result, Is.TypeOf<OkResult>());
        _childRepositoryMock.Verify(repo => repo.Update(child), Times.Once);
    }
    
    /**
     * Acceptatiecriteria: Gebruiker kan een kind verwijderen via DELETE /children/{id}, resulterend in een 200 OK.
     */
    [Test]
    public async Task Delete_DeletesChild_WhenValid()
    {
        var childId = Guid.NewGuid();
        var child = new Child { Id = childId, UserId = _userId };
        _childRepositoryMock.Setup(repo => repo.GetByUserId(_userId.ToString())).ReturnsAsync(new List<Child> { child });
        _childRepositoryMock.Setup(repo => repo.Delete(childId.ToString())).Returns(Task.CompletedTask);
        
        var result = await _childController.Delete(childId.ToString());
        Assert.That(result, Is.TypeOf<OkResult>());
        _childRepositoryMock.Verify(repo => repo.Delete(childId.ToString()), Times.Once);
    }
    
    /**
     * Acceptatiecriteria: Gebruiker kan geen kind verwijderen dat hij niet bezit, resulterend in een 404 Not Found.
     */
    [Test]
    public async Task Delete_ReturnsNotAuthorized_WhenUserDoesNotOwnChild()
    {
        var childId = Guid.NewGuid();
        _childRepositoryMock.Setup(repo => repo.GetByUserId(_userId.ToString()))
            .ReturnsAsync(new List<Child>());
    
        var result = await _childController.Delete(childId.ToString());
        Assert.That(result, Is.TypeOf<NotFoundResult>());
        _childRepositoryMock.Verify(repo => repo.Delete(childId.ToString()), Times.Never);
    }
}