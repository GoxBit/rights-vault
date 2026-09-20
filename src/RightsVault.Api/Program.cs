using RightsVault.Application.UseCases.RenewLicenseAgreement;
using RightsVault.Domain.Entities;
using RightsVault.Domain.Repositories;
using RightsVault.Domain.ValueObjects;
using RightsVault.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddSingleton<ILicenseAgreementRepository, InMemoryLicenseAgreementRepository>();
builder.Services.AddScoped<RenewLicenseAgreementHandler>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapPost("/agreements", async (
    CreateAgreementRequest req,
    ILicenseAgreementRepository repo,
    CancellationToken ct) =>
{
    var agreement = LicenseAgreement.Create(
        req.Title, new DateRange(req.Start, req.End), req.HasExclusivity);
    await repo.SaveAsync(agreement, ct);
    return Results.Created($"/agreements/{agreement.Id}", agreement.Id);
});

app.MapPut("/agreements/{id}/renew", async (
    Guid id,
    RenewRequest req,
    RenewLicenseAgreementHandler handler,
    CancellationToken ct) =>
{
    await handler.HandleAsync(
        new RenewLicenseAgreementCommand(id, req.NewStart, req.NewEnd), ct);
    return Results.NoContent();
})
.WithName("RenewAgreement");

app.Run();

record CreateAgreementRequest(
    string Title,
    DateOnly Start,
    DateOnly End,
    bool HasExclusivity);

record RenewRequest(DateOnly NewStart, DateOnly NewEnd);
