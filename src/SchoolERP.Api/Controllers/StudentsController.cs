using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERP.Application.Students.Commands.CreateStudent;
using SchoolERP.Application.Students.Commands.DeactivateStudent;
using SchoolERP.Application.Students.Commands.EnrollStudent;
using SchoolERP.Application.Students.Commands.UpdateStudent;
using SchoolERP.Application.Students.Queries.GetStudentById;
using SchoolERP.Application.Students.Queries.GetStudents;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Api.Controllers;

[Authorize]
public class StudentsController : BaseApiController
{
    /// <summary>
    /// Listar estudiantes con filtros y paginación.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] Guid? schoolId,
        [FromQuery] Guid? sectionId,
        [FromQuery] Guid? gradeLevelId,
        [FromQuery] bool? isActive,
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 25,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDesc = false,
        CancellationToken cancellationToken = default)
    {
        var query = new GetStudentsQuery
        {
            SchoolId = schoolId,
            SectionId = sectionId,
            GradeLevelId = gradeLevelId,
            IsActive = isActive ?? true,
            Search = search,
            Page = page,
            PageSize = pageSize,
            SortBy = sortBy,
            SortDesc = sortDesc
        };

        var result = await Mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Obtener estudiante por ID con detalle completo.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetStudentByIdQuery(id), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Crear nuevo estudiante.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateStudentRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateStudentCommand(
            SchoolId: request.SchoolId,
            FirstName: request.FirstName,
            LastName: request.LastName,
            DateOfBirth: request.DateOfBirth,
            Gender: request.Gender,
            SecondLastName: request.SecondLastName,
            NationalId: request.NationalId,
            Nse: request.Nse,
            StudentCode: request.StudentCode,
            Nationality: request.Nationality,
            Address: request.Address,
            Province: request.Province,
            Municipality: request.Municipality,
            Phone: request.Phone,
            BloodType: request.BloodType,
            Allergies: request.Allergies,
            MedicalNotes: request.MedicalNotes,
            HealthInsurance: request.HealthInsurance,
            HealthInsuranceNumber: request.HealthInsuranceNumber,
            HasSpecialNeeds: request.HasSpecialNeeds);

        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Actualizar datos del estudiante.
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateStudentRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateStudentCommand(
            StudentId: id,
            FirstName: request.FirstName,
            LastName: request.LastName,
            SecondLastName: request.SecondLastName,
            Address: request.Address,
            Province: request.Province,
            Municipality: request.Municipality,
            Phone: request.Phone,
            Allergies: request.Allergies,
            MedicalNotes: request.MedicalNotes,
            HasSpecialNeeds: request.HasSpecialNeeds,
            HealthInsurance: request.HealthInsurance,
            HealthInsuranceNumber: request.HealthInsuranceNumber);

        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Desactivar estudiante.
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Deactivate(Guid id, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new DeactivateStudentCommand(id), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Matricular estudiante en una sección del año académico.
    /// </summary>
    [HttpPost("{id:guid}/enroll")]
    public async Task<IActionResult> Enroll(Guid id, [FromBody] EnrollStudentRequest request, CancellationToken cancellationToken)
    {
        var command = new EnrollStudentCommand(
            StudentId: id,
            SectionId: request.SectionId,
            AcademicYearId: request.AcademicYearId,
            EnrollmentDate: request.EnrollmentDate);

        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }
}

// Request bodies
public record CreateStudentRequest(
    Guid SchoolId,
    string FirstName,
    string LastName,
    DateTime DateOfBirth,
    Gender Gender,
    string? SecondLastName = null,
    string? NationalId = null,
    string? Nse = null,
    string? StudentCode = null,
    string? Nationality = null,
    string? Address = null,
    string? Province = null,
    string? Municipality = null,
    string? Phone = null,
    string? BloodType = null,
    string? Allergies = null,
    string? MedicalNotes = null,
    string? HealthInsurance = null,
    string? HealthInsuranceNumber = null,
    bool HasSpecialNeeds = false);

public record UpdateStudentRequest(
    string FirstName,
    string LastName,
    string? SecondLastName = null,
    string? Address = null,
    string? Province = null,
    string? Municipality = null,
    string? Phone = null,
    string? Allergies = null,
    string? MedicalNotes = null,
    bool HasSpecialNeeds = false,
    string? HealthInsurance = null,
    string? HealthInsuranceNumber = null);

public record EnrollStudentRequest(
    Guid SectionId,
    Guid AcademicYearId,
    DateTime? EnrollmentDate = null);
