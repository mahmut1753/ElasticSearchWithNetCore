using ElasticSearch.API.Middleware;
using ElasticSearch.Data.Models.Entity;
using ElasticSearch.Data.Models.Record;
using ElasticSearch.Data.Repository.Abstractions;
using ElasticSearch.Data.Repository.Service;
using ElasticSearch.Data.Settings;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddScoped<IElasticSearchService, ElasticsearchService>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference(options =>
    {
        options.Title = "Elastic Search";
        options.Theme = ScalarTheme.Moon;
        options.ShowSidebar = true;
        options.DefaultHttpClient = new(ScalarTarget.CSharp, ScalarClient.RestSharp);
        options.Authentication = new ScalarAuthenticationOptions
        {
            PreferredSecurityScheme = "ApiKey",
            ApiKey = new ApiKeyOptions
            {
                Token = "my-api-key"
            }
        };
    });
}

app.ConfigureExceptionHandler<Program>(app.Services.GetRequiredService<ILogger<Program>>());

#region Controllers

app.MapGet("/products", async (IElasticSearchService elasticsearchService, CancellationToken cancellationToken) =>
{
    var datas = await elasticsearchService.GetDocumentsAsync<Product>(ElasticSearchIndexes.Products, cancellationToken);

    return Results.Ok(datas);
});

app.MapGet("/products/{id}", async (string id, IElasticSearchService elasticsearchService, CancellationToken cancellationToken) =>
{
    var data = await elasticsearchService.GetDocumentAsync<Product>(id, ElasticSearchIndexes.Products, cancellationToken);

    return Results.Ok(data);
});

app.MapGet("/products-details", async (IElasticSearchService elasticsearchService, CancellationToken cancellationToken) =>
{
    var datas = await elasticsearchService.SearchAsync<Product>(product =>
                  product.Index(ElasticSearchIndexes.Products)
                         .From(0)
                         .Size(10), cancellationToken);

    return Results.Ok(datas);
});

app.MapGet("/products-match/{query_keyword}", async (string query_keyword, IElasticSearchService elasticsearchService, CancellationToken cancellationToken) =>
{
    var datas = await elasticsearchService.MatchQueryAsync<Product>(p => p.Name, query_keyword, ElasticSearchIndexes.Products, cancellationToken);

    return Results.Ok(datas);
});

app.MapGet("/products-fuzzy/{query_keyword}", async (string query_keyword, IElasticSearchService elasticsearchService, CancellationToken cancellationToken) =>
{
    var datas = await elasticsearchService.FuzzyQueryAsync<Product>(p => p.Name, query_keyword, ElasticSearchIndexes.Products, cancellationToken);

    return Results.Ok(datas);
});

app.MapGet("/products-wildcard/{query_keyword}", async (string query_keyword, IElasticSearchService elasticsearchService, CancellationToken cancellationToken) =>
{
    var datas = await elasticsearchService.WildcardQueryAsync<Product>(p => p.Name, query_keyword, ElasticSearchIndexes.Products, cancellationToken);

    return Results.Ok(datas);
});

app.MapGet("/products-exists", async (IElasticSearchService elasticsearchService, CancellationToken cancellationToken) =>
{
    var datas = await elasticsearchService.ExistsQueryAsync<Product>(p => p.Name, ElasticSearchIndexes.Products, cancellationToken);

    return Results.Ok(datas);
});

app.MapGet("/products-bool", async (IElasticSearchService elasticsearchService, CancellationToken cancellationToken) =>
{
    var datas = await elasticsearchService.BoolQueryAsync<Product>(p => p.Name, "Mouse", p => p.Name, "erasr", p => p.Name, "*se*", ElasticSearchIndexes.Products, cancellationToken);

    return Results.Ok(datas);
});

app.MapGet("/products-term/{query_keyword}", async (string query_keyword, IElasticSearchService elasticsearchService, CancellationToken cancellationToken) =>
{
    var datas = await elasticsearchService.TermQueryAsync<Product>(p => p.Name, query_keyword, ElasticSearchIndexes.Products, cancellationToken);

    return Results.Ok(datas);
});

app.MapGet("/products-count", async (IElasticSearchService elasticsearchService, CancellationToken cancellationToken) =>
{
    var count = await elasticsearchService.CountDocumentsAsync<Product>(ElasticSearchIndexes.Products, cancellationToken);

    return Results.Ok(count);
});

app.MapPost("/products", async (CreateProductVM createProductVM, IElasticSearchService elasticsearchService, CancellationToken cancellationToken) =>
{
    Product product = new()
    {
        Name = createProductVM.Name,
        Price = createProductVM.Price,
        Quantity = createProductVM.Quantity
    };
    bool result = await elasticsearchService.CreateDocumentAsync(product, ElasticSearchIndexes.Products, cancellationToken);

    return Results.Ok(result);
});

app.MapPut("/products", async (UpdateProductVM updateProductVM, IElasticSearchService elasticsearchService, CancellationToken cancellationToken) =>
{
    bool result = await elasticsearchService.UpdateDocumentAsync<Product>(updateProductVM.Id, updateProductVM, ElasticSearchIndexes.Products, cancellationToken);

    return Results.Ok(result);
});

app.MapDelete("/products/{id}", async (string id, IElasticSearchService elasticsearchService, CancellationToken cancellationToken) =>
{
    bool result = await elasticsearchService.DeleteDocumentAsync<Product>(id, ElasticSearchIndexes.Products, cancellationToken);

    return Results.Ok(result);
});

#endregion

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
