using ElasticSearch.Data.Repository.Abstractions;
using ElasticSearch.Data.Settings;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticSearch.Data.Repository.Service;

public class ElasticsearchService : IElasticSearchService
{
    readonly ElasticsearchClient _elasticsearchClient;

    public ElasticsearchService(IConfiguration configuration)
    {
        ElasticsearchClientSettings elasticsearchClientSettings = new(new Uri(configuration["ElasticsearchSettings:Uri"]));

        elasticsearchClientSettings.DefaultIndex(ElasticSearchIndexes.DefaultIndex);

        _elasticsearchClient = new(elasticsearchClientSettings);

        Ping();
    }

    private void Ping()
    {
        PingResponse response = _elasticsearchClient.PingAsync().Result;

        if (!response.IsValidResponse)
            throw new Exception(response.ApiCallDetails.DebugInformation);
    }


    public async Task<bool> CreateIndexAsync(string indexName = ElasticSearchIndexes.DefaultIndex, CancellationToken cancellationToken = default)
    {
        IndexResponse indexResponse = await _elasticsearchClient.IndexAsync(indexName, cancellationToken);

        return indexResponse.IsSuccess();
    }

    public async Task<bool> CreateDocumentAsync<T>(T document, string indexName = ElasticSearchIndexes.DefaultIndex, CancellationToken cancellationToken = default) where T : IBaseEntity
    {
        CreateRequest<T> createRequest = new(document, indexName, document.Id);

        CreateResponse createResponse = await _elasticsearchClient.CreateAsync(createRequest, cancellationToken);

        return createResponse.IsSuccess();
    }

    public async Task<bool> UpdateDocumentAsync<T>(string documentId, object partialDocument, string indexName = ElasticSearchIndexes.DefaultIndex, CancellationToken cancellationToken = default)
    {
        UpdateRequest<T, object> updateRequest = new(indexName, documentId)
        {
            Doc = partialDocument
        };

        UpdateResponse<T> updateResponse = await _elasticsearchClient.UpdateAsync<T, object>(updateRequest, cancellationToken);

        return updateResponse.IsSuccess();
    }

    public async Task<bool> DeleteDocumentAsync<T>(string documentId, string indexName = ElasticSearchIndexes.DefaultIndex, CancellationToken cancellationToken = default)
    {
        DeleteResponse deleteResponse = await _elasticsearchClient.DeleteAsync<T>(indexName, documentId, cancellationToken);

        return deleteResponse.IsSuccess();
    }

    public async Task<long> CountDocumentsAsync<T>(string indexName = ElasticSearchIndexes.DefaultIndex, CancellationToken cancellationToken = default) where T : IBaseEntity
    {
        CountResponse countResponse = await _elasticsearchClient.CountAsync<T>(indexName, cancellationToken);

        return countResponse.Count;
    }

    public async Task<IReadOnlyCollection<T>> SearchAsync<T>(Action<SearchRequestDescriptor<T>> searchRequestDescriptor, CancellationToken cancellationToken = default) where T : IBaseEntity
    {
        SearchResponse<T> searchResponse = await _elasticsearchClient.SearchAsync<T>(searchRequestDescriptor, cancellationToken);

        return searchResponse.Documents;
    }

    public async Task<IReadOnlyCollection<T>> GetDocumentsAsync<T>(string indexName = ElasticSearchIndexes.DefaultIndex, CancellationToken cancellationToken = default) where T : IBaseEntity
    {
        SearchResponse<T> searchResponse = await _elasticsearchClient.SearchAsync<T>(indexName, cancellationToken);

        return searchResponse.Documents;
    }

    public async Task<T> GetDocumentAsync<T>(string documentId, string indexName = ElasticSearchIndexes.DefaultIndex, CancellationToken cancellationToken = default) where T : IBaseEntity
    {
        GetResponse<T> getResponse = await _elasticsearchClient.GetAsync<T>(documentId, index => index
                                                                                                    .Index(indexName), cancellationToken);
        return getResponse.Source;
    }

    public async Task<IReadOnlyCollection<T>> MatchQueryAsync<T>(Expression<Func<T, object>> field, string queryKeyword, string indexName = ElasticSearchIndexes.DefaultIndex, CancellationToken cancellationToken = default, int from = 0, int size = 10) where T : IBaseEntity
    {
        SearchResponse<T> searchResponse = await _elasticsearchClient.SearchAsync<T>(index => index
                                                                                                .Index(indexName)
                                                                                                .Query(query => query
                                                                                                    .Match(t => t.Field(field)
                                                                                                                 .Query(queryKeyword)))
                                                                                                .From(from)
                                                                                                .Size(size), cancellationToken);

        return searchResponse.Documents;
    }

    public async Task<IReadOnlyCollection<T>> FuzzyQueryAsync<T>(Expression<Func<T, object>> field, string queryKeyword, string indexName = ElasticSearchIndexes.DefaultIndex, CancellationToken cancellationToken = default, int from = 0, int size = 10) where T : IBaseEntity
    {
        SearchResponse<T> searchResponse = await _elasticsearchClient.SearchAsync<T>(index => index
                                                                                                .Index(indexName)
                                                                                                .Query(query => query
                                                                                                    .Fuzzy(t => t.Field(field)
                                                                                                                 .Value(queryKeyword)))
                                                                                                .From(from)
                                                                                                .Size(size), cancellationToken);

        return searchResponse.Documents;
    }

    public async Task<IReadOnlyCollection<T>> WildcardQueryAsync<T>(Expression<Func<T, object>> field, string queryKeyword, string indexName = ElasticSearchIndexes.DefaultIndex, CancellationToken cancellationToken = default, int from = 0, int size = 10) where T : IBaseEntity
    {
        SearchResponse<T> searchResponse = await _elasticsearchClient.SearchAsync<T>(index => index
                                                                                                .Index(indexName)
                                                                                                .Query(query => query
                                                                                                    .Wildcard(t => t.Field(field)
                                                                                                                    .Value(queryKeyword)))
                                                                                                .From(from)
                                                                                                .Size(size), cancellationToken);

        return searchResponse.Documents;
    }

    public async Task<IReadOnlyCollection<T>> BoolQueryAsync<T>(Expression<Func<T, object>> matchField, string matchQueryKeyword, Expression<Func<T, object>> fuzzyField, string fuzzyQueryKeyword, Expression<Func<T, object>> wildcardField, string wildcardQueryKeyword, string indexName = ElasticSearchIndexes.DefaultIndex, CancellationToken cancellationToken = default, int from = 0, int size = 10) where T : IBaseEntity
    {
        SearchResponse<T> searchResponse = await _elasticsearchClient.SearchAsync<T>(index => index
                                                                                                .Index(indexName)
                                                                                                .Query(query => query
                                                                                                    .Bool(t => t.Should(
                                                                                                        match => match.Match(t => t.Field(matchField).Query(matchQueryKeyword)),
                                                                                                        fuzzy => fuzzy.Fuzzy(p => p.Field(fuzzyField).Value(fuzzyQueryKeyword)),
                                                                                                        wildcard => wildcard.Wildcard(p => p.Field(wildcardField).Value(wildcardQueryKeyword))
                                                                                                        )))
                                                                                                .From(from)
                                                                                                .Size(size), cancellationToken);

        return searchResponse.Documents;
    }

    public async Task<IReadOnlyCollection<T>> TermQueryAsync<T>(Expression<Func<T, object>> field, string queryKeyword, string indexName = ElasticSearchIndexes.DefaultIndex, CancellationToken cancellationToken = default, int from = 0, int size = 10) where T : IBaseEntity
    {
        SearchResponse<T> searchResponse = await _elasticsearchClient.SearchAsync<T>(index => index
                                                                                        .Index(indexName)
                                                                                        .Query(query => query
                                                                                            .Term(t => t.Field(field)
                                                                                                        .Value(queryKeyword)))
                                                                                        .From(from)
                                                                                        .Size(size), cancellationToken);

        return searchResponse.Documents;
    }

    public async Task<IReadOnlyCollection<T>> ExistsQueryAsync<T>(Expression<Func<T, object>> field, string indexName = ElasticSearchIndexes.DefaultIndex, CancellationToken cancellationToken = default, int from = 0, int size = 10) where T : IBaseEntity
    {
        SearchResponse<T> searchResponse = await _elasticsearchClient.SearchAsync<T>(index => index
                                                                                        .Index(indexName)
                                                                                        .Query(query => query
                                                                                            .Exists(t => t.Field(field)))
                                                                                        .From(from)
                                                                                        .Size(size), cancellationToken);


        return searchResponse.Documents;
    }
}