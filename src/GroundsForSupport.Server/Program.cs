var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
  app.MapOpenApi();
}

// We need a /create-payment-intent endpoint

app.MapPost("/create-payment-intent", static () =>
{
  // we need to get the amount and email
  // from the frontend

  // we need to validate the amount and email

  // we are going to use the Stripe API to create
  // a payment intent

  // return the secret for that intent
  // to the frontend
  return Results.Ok("Hello");
});

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseHttpsRedirection();

app.Run();