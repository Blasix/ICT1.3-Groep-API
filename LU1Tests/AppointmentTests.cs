using System.Security.Claims;
using LU1.Controllers;
using LU1.Models;
using LU1.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace LU1Tests;

public class AppointmentTests
{
    private Mock<IAppointmentRepository> _appointmentRepositoryMock;
    private AppointmentController _appointmentController;
    private Guid _userId;
    private string _childName;

    [SetUp]
    public void Setup()
    {
        _userId = Guid.NewGuid();
        _childName = "TestChild";
        _appointmentRepositoryMock = new Mock<IAppointmentRepository>();
        _appointmentController = new AppointmentController(_appointmentRepositoryMock.Object, null);

        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, _userId.ToString()) };
        var identity = new ClaimsIdentity(claims);
        var principal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = principal };
        _appointmentController.ControllerContext = new ControllerContext { HttpContext = httpContext };
    }

    /**
     * Acceptatiecriteria: Gebruiker haalt succesvol alle afspraken voor een specifiek kind op via GET /appointments/{childName},
     * resulterend in een 200 OK met een lijst van afspraken (id, appointmentName, date, childId, levelId, statusLevel, LevelStep).
     */
    [Test]
    public async Task Get_ReturnsChildAppointments()
    {
        var appointments = new List<AppointmentItem>
        {
            new AppointmentItem { id = Guid.NewGuid().ToString(), appointmentName = "Ap1", childId = Guid.NewGuid().ToString() },
            new AppointmentItem { id = Guid.NewGuid().ToString(), appointmentName = "Ap2", childId = Guid.NewGuid().ToString() }
        };
        _appointmentRepositoryMock.Setup(repo => repo.GetAppointmentsByUserIdAndChildName(_userId.ToString(), _childName)).ReturnsAsync(appointments);

        var result = (await _appointmentController.GetAll(_childName)).Result as OkObjectResult;
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Value as IEnumerable<AppointmentItem>, Is.EquivalentTo(appointments));
    }

    /**
     * Acceptatiecriteria: Gebruiker maakt succesvol een nieuwe afspraak aan via POST /appointments,
     * resulterend in een 200 OK.
     */
    [Test]
    public async Task PostAppointment_CreatesAppointment_WhenValid()
    {
        var appointment = new AppointmentItem { appointmentName = "New Appointment", childId = Guid.NewGuid().ToString() };
        _appointmentRepositoryMock.Setup(repo => repo.Add(appointment)).Returns(Task.CompletedTask);

        var result = await _appointmentController.PostAppointment(appointment) as OkResult;
        Assert.That(result, Is.Not.Null);
        _appointmentRepositoryMock.Verify(repo => repo.Add(appointment), Times.Once);
    }

    /**
     * Acceptatiecriteria: Gebruiker verwijdert succesvol een afspraak voor een specifiek kind via DELETE /appointments/{childName}/{appointmentName},
     * resulterend in een 200 OK.
     */
    [Test]
    public async Task DeleteAppointment_DeletesAppointment_WhenValid()
    {
        var appointmentId = Guid.NewGuid();
        var appointment = new AppointmentItem { id = appointmentId.ToString(), appointmentName = "ToDelete", childId = Guid.NewGuid().ToString() };
        _appointmentRepositoryMock.Setup(repo => repo.GetAppointmentIdByUserIdChildNameAndAppointmentName(_userId.ToString(), _childName, "ToDelete"))
            .ReturnsAsync(appointment);
        _appointmentRepositoryMock.Setup(repo => repo.Delete(_userId.ToString(), _childName, appointmentId.ToString())).Returns(Task.CompletedTask);

        var result = await _appointmentController.Delete(_childName, "ToDelete") as OkResult;
        Assert.That(result, Is.Not.Null);
        _appointmentRepositoryMock.Verify(repo => repo.Delete(_userId.ToString(), _childName, appointmentId.ToString()), Times.Once);
    }

    /**
     * Acceptatiecriteria: Gebruiker valideert succesvol een bestaande afspraaknaam voor een specifiek kind via GET /appointments/validation/{childName}/{appointmentName},
     * resulterend in een 200 OK met een 'true' boolean.
     */
    [Test]
    public async Task ValidateAppointmentName_ReturnsTrue_WhenAppointmentExists()
    {
        _appointmentRepositoryMock.Setup(repo => repo.GetAppointmentIdByUserIdChildNameAndAppointmentName(_userId.ToString(), _childName, "Existing"))
            .ReturnsAsync(new AppointmentItem { id = Guid.NewGuid().ToString() });

        var result = await _appointmentController.ValidateAppointmentName(_childName, "Existing");
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Value, Is.EqualTo(true));
    }

    /**
     * Acceptatiecriteria: Gebruiker valideert een niet-bestaande afspraaknaam voor een specifiek kind via GET /appointments/validation/{childName}/{appointmentName},
     * resulterend in een 200 OK met een 'false' boolean.
     */
    [Test]
    public async Task ValidateAppointmentName_ReturnsFalse_WhenAppointmentDoesNotExist()
    {
        _appointmentRepositoryMock.Setup(repo => repo.GetAppointmentIdByUserIdChildNameAndAppointmentName(_userId.ToString(), _childName, "NonExisting"))
            .ReturnsAsync((AppointmentItem)null);

        var result = await _appointmentController.ValidateAppointmentName(_childName, "NonExisting");
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Value, Is.EqualTo(false));
    }
}