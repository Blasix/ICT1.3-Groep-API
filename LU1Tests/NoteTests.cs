using System.Security.Claims;
using LU1.Controllers;
using LU1.Models;
using LU1.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace LU1Tests;

public class NoteTests
{
    private Mock<INoteRepository> _noteRepositoryMock;
    private Mock<IChildRepository> _childRepositoryMock;
    private NoteController _noteController;
    private Guid _userId;
    private Guid _childId;

    [SetUp]
    public void Setup()
    {
        _userId = Guid.NewGuid();
        _childId = Guid.NewGuid();
        _noteRepositoryMock = new Mock<INoteRepository>();
        _childRepositoryMock = new Mock<IChildRepository>();
        _noteController = new NoteController(_noteRepositoryMock.Object, _childRepositoryMock.Object);

        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, _userId.ToString()) };
        var identity = new ClaimsIdentity(claims);
        var principal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = principal };
        _noteController.ControllerContext = new ControllerContext { HttpContext = httpContext };
    }

    /**
     * Acceptatiecriteria: Gebruiker haalt succesvol alle notities op voor een specifiek kind via GET /Note/{childId},
     * resulterend in een 200 OK met een lijst van notities (Id, ChildId, NoteDate, Title, Content).
     */
    [Test]
    public async Task Get_ReturnsChildNotes()
    {
        var notes = new List<Note>
        {
            new Note { Id = Guid.NewGuid(), ChildId = _childId, Title = "Note 1" },
            new Note { Id = Guid.NewGuid(), ChildId = _childId, Title = "Note 2" }
        };
        _childRepositoryMock.Setup(repo => repo.GetByUserId(_userId.ToString()))
            .ReturnsAsync(new List<Child> { new Child { Id = _childId, UserId = _userId } });
        _noteRepositoryMock.Setup(repo => repo.GetByChildId(_childId.ToString())).ReturnsAsync(notes);

        var result = (await _noteController.GetAll(_childId.ToString())).Result as OkObjectResult;
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Value as IEnumerable<Note>, Is.EquivalentTo(notes));
    }

    /**
     * Acceptatiecriteria: Gebruiker probeert notities op te halen voor een kind dat niet bestaat of niet aan hem gekoppeld is via GET /Note/{childId},
     * resulterend in een 400 BadRequest met de melding "Child not found".
     */
    [Test]
    public async Task Get_ReturnsBadRequest_WhenChildNotFound()
    {
        _childRepositoryMock.Setup(repo => repo.GetByUserId(_userId.ToString()))
            .ReturnsAsync(new List<Child>()); // Geen kinderen voor de gebruiker

        var result = (await _noteController.GetAll(Guid.NewGuid().ToString())).Result as BadRequestObjectResult;
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Value, Is.EqualTo("Child not found"));
        Assert.That(result.StatusCode, Is.EqualTo(400));
    }

    /**
     * Acceptatiecriteria: Gebruiker maakt succesvol een nieuwe notitie aan via POST /Note,
     * resulterend in een 201 Created met de aangemaakte notitie (Id, ChildId, NoteDate, Title, Content).
     */
    [Test]
    public async Task Create_CreatesNote_WhenValidChild()
    {
        var newNote = new Note { ChildId = _childId, Title = "New Note", Content = "Some content" };
        _childRepositoryMock.Setup(repo => repo.GetByUserId(_userId.ToString()))
            .ReturnsAsync(new List<Child> { new Child { Id = _childId, UserId = _userId } });
        _noteRepositoryMock.Setup(repo => repo.Add(It.IsAny<Note>())).Returns(Task.CompletedTask);

        var result = (await _noteController.Create(newNote)).Result as CreatedAtActionResult;
        Assert.That(result, Is.Not.Null);
        var createdNote = result.Value as Note;
        Assert.That(createdNote, Is.Not.Null);
        Assert.That(createdNote.Title, Is.EqualTo(newNote.Title));
        Assert.That(createdNote.Content, Is.EqualTo(newNote.Content));
        Assert.That(result.StatusCode, Is.EqualTo(201));
        Assert.That(result.ActionName, Is.EqualTo("GetAll"));
    }

    /**
     * Acceptatiecriteria: Gebruiker probeert een notitie aan te maken voor een kind dat niet bestaat of niet aan hem gekoppeld is via POST /Note,
     * resulterend in een 400 BadRequest met de melding "Child not found".
     */
    [Test]
    public async Task Create_ReturnsBadRequest_WhenChildNotFound()
    {
        var newNote = new Note { ChildId = Guid.NewGuid(), Title = "New Note" };
        _childRepositoryMock.Setup(repo => repo.GetByUserId(_userId.ToString()))
            .ReturnsAsync(new List<Child>());

        var result = (await _noteController.Create(newNote)).Result as BadRequestObjectResult;
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Value, Is.EqualTo("Child not found"));
        Assert.That(result.StatusCode, Is.EqualTo(400));
        _noteRepositoryMock.Verify(repo => repo.Add(It.IsAny<Note>()), Times.Never);
    }

    /**
     * Acceptatiecriteria: Gebruiker update succesvol een bestaande notitie via PUT /Note/{id},
     * resulterend in een 200 OK.
     */
    [Test]
    public async Task Update_UpdatesNote_WhenValidChildAndNoteExists()
    {
        var existingNoteId = Guid.NewGuid().ToString();
        var updatedNote = new Note { ChildId = _childId, Title = "Updated Note", Content = "Updated content" };
        _childRepositoryMock.Setup(repo => repo.GetByUserId(_userId.ToString()))
            .ReturnsAsync(new List<Child> { new Child { Id = _childId, UserId = _userId } });
        _noteRepositoryMock.Setup(repo => repo.GetByChildId(_childId.ToString()))
            .ReturnsAsync(new List<Note> { new Note { Id = Guid.Parse(existingNoteId), ChildId = _childId, Title = "Old Note" } });
        _noteRepositoryMock.Setup(repo => repo.Update(It.IsAny<Note>())).Returns(Task.CompletedTask);

        var result = await _noteController.Update(existingNoteId, updatedNote) as OkResult;
        Assert.That(result, Is.Not.Null);
        _noteRepositoryMock.Verify(repo => repo.Update(It.Is<Note>(n => n.Id == Guid.Parse(existingNoteId) && n.Title == "Updated Note")), Times.Once);
    }

    /**
     * Acceptatiecriteria: Gebruiker probeert een notitie te updaten die niet bestaat via PUT /Note/{id},
     * resulterend in een 404 NotFound.
     */
    [Test]
    public async Task Update_ReturnsNotFound_WhenNoteDoesNotExist()
    {
        var nonExistingNoteId = Guid.NewGuid().ToString();
        var updatedNote = new Note { ChildId = _childId, Title = "Updated Note" };
        _childRepositoryMock.Setup(repo => repo.GetByUserId(_userId.ToString()))
            .ReturnsAsync(new List<Child> { new Child { Id = _childId, UserId = _userId } });
        _noteRepositoryMock.Setup(repo => repo.GetByChildId(_childId.ToString()))
            .ReturnsAsync(new List<Note>()); // Geen notities voor dit kind

        var result = await _noteController.Update(nonExistingNoteId, updatedNote) as NotFoundResult;
        Assert.That(result, Is.Not.Null);
        _noteRepositoryMock.Verify(repo => repo.Update(It.IsAny<Note>()), Times.Never);
    }

    /**
     * Acceptatiecriteria: Gebruiker probeert een notitie te updaten voor een kind dat niet bestaat of niet aan hem gekoppeld is via PUT /Note/{id},
     * resulterend in een 400 BadRequest met de melding "Child not found".
     */
    [Test]
    public async Task Update_ReturnsBadRequest_WhenChildNotFound()
    {
        var existingNoteId = Guid.NewGuid().ToString();
        var updatedNote = new Note { ChildId = Guid.NewGuid(), Title = "Updated Note" };
        _childRepositoryMock.Setup(repo => repo.GetByUserId(_userId.ToString()))
            .ReturnsAsync(new List<Child>()); // Geen kinderen voor de gebruiker

        var result = await _noteController.Update(existingNoteId, updatedNote);
        Assert.That(result, Is.Not.Null);
        _noteRepositoryMock.Verify(repo => repo.Update(It.IsAny<Note>()), Times.Never);
    }

    /**
     * Acceptatiecriteria: Gebruiker verwijdert succesvol een notitie via DELETE /Note/{id},
     * resulterend in een 200 OK.
     */
    [Test]
    public async Task Delete_DeletesNote_WhenValid()
    {
        var noteIdToDelete = Guid.NewGuid().ToString();
        _noteRepositoryMock.Setup(repo => repo.Delete(noteIdToDelete)).Returns(Task.CompletedTask);

        var result = await _noteController.Delete(noteIdToDelete) as OkResult;
        Assert.That(result, Is.Not.Null);
        _noteRepositoryMock.Verify(repo => repo.Delete(noteIdToDelete), Times.Once);
    }
}