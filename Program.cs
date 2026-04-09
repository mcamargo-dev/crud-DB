using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContext>();
var app = builder.Build();

// ADICIONANDO PRODUTO
app.MapPost("/Produtos", async (Produto produto,AppDbContext db) =>
{
    db.Produtos.Add(produto);
    await db.SaveChangesAsync();
    return Results.Created($"/produto/{produto.Id}", produto);
});

// LISTANDO PRODUTO
app.MapGet("/Produtos", async (AppDbContext db) =>
    await db.Produtos.ToListAsync());

app.MapGet("/Produtos/{Id}", async (int Id, AppDbContext db) =>
    await db.Produtos.FindAsync(Id) is Produto produto ? Results.Ok(produto) : Results.NotFound());

// DELETANDO PRODUTO
app.MapDelete("/Produtos/{id}", async (int id, AppDbContext db) =>
{
    var produto = await db.Produtos.FindAsync(id);
    if (produto is null) return Results.NotFound();

    db.Produtos.Remove(produto);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();

public class Produto
{
    public int Id { get; set; }
    public required String Nome { get; set; }
    public decimal Preco { get; set; }

}

public class AppDbContext : DbContext
{
    public DbSet<Produto> Produtos { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    => options.UseSqlite("Data Source=Loja.db");
}