namespace ElasticSearch.Data.Repository.Abstractions;

public interface IElasticSearchService
{
    Task<bool> CreateIndexAsync(string indexName = ElasticSearchIndexes.DefaultIndex, CancellationToken cancellationToken = default);

    Task<bool> CreateDocumentAsync<T>(T document, string indexName = ElasticSearchIndexes.DefaultIndex, CancellationToken cancellationToken = default) where T : IBaseEntity;

    Task<bool> UpdateDocumentAsync<T>(string documentId, object partialDocument, string indexName = ElasticSearchIndexes.DefaultIndex, CancellationToken cancellationToken = default);

    Task<bool> DeleteDocumentAsync<T>(string documentId, string indexName = ElasticSearchIndexes.DefaultIndex, CancellationToken cancellationToken = default);

    Task<long> CountDocumentsAsync<T>(string indexName = ElasticSearchIndexes.DefaultIndex, CancellationToken cancellationToken = default) where T : IBaseEntity;

    Task<IReadOnlyCollection<T>> GetDocumentsAsync<T>(string indexName = ElasticSearchIndexes.DefaultIndex, CancellationToken cancellationToken = default) where T : IBaseEntity;

    Task<T> GetDocumentAsync<T>(string documentId, string indexName = ElasticSearchIndexes.DefaultIndex, CancellationToken cancellationToken = default) where T : IBaseEntity;

    Task<IReadOnlyCollection<T>> SearchAsync<T>(Action<SearchRequestDescriptor<T>> searchRequestDescriptor, CancellationToken cancellationToken = default) where T : IBaseEntity;

    Task<IReadOnlyCollection<T>> MatchQueryAsync<T>(Expression<Func<T, object>> field, string queryKeyword, string indexName = ElasticSearchIndexes.DefaultIndex, CancellationToken cancellationToken = default, int from = 0, int size = 10) where T : IBaseEntity;

    Task<IReadOnlyCollection<T>> FuzzyQueryAsync<T>(Expression<Func<T, object>> field, string queryKeyword, string indexName = ElasticSearchIndexes.DefaultIndex, CancellationToken cancellationToken = default, int from = 0, int size = 10) where T : IBaseEntity;

    Task<IReadOnlyCollection<T>> WildcardQueryAsync<T>(Expression<Func<T, object>> field, string queryKeyword, string indexName = ElasticSearchIndexes.DefaultIndex, CancellationToken cancellationToken = default, int from = 0, int size = 10) where T : IBaseEntity;

    Task<IReadOnlyCollection<T>> BoolQueryAsync<T>(Expression<Func<T, object>> matchField, string matchQueryKeyword, Expression<Func<T, object>> fuzzyField, string fuzzyQueryKeyword, Expression<Func<T, object>> wildcardField, string wildcardQueryKeyword, string indexName = ElasticSearchIndexes.DefaultIndex, CancellationToken cancellationToken = default, int from = 0, int size = 10) where T : IBaseEntity;

    Task<IReadOnlyCollection<T>> TermQueryAsync<T>(Expression<Func<T, object>> field, string queryKeyword, string indexName = ElasticSearchIndexes.DefaultIndex, CancellationToken cancellationToken = default, int from = 0, int size = 10) where T : IBaseEntity;

    Task<IReadOnlyCollection<T>> ExistsQueryAsync<T>(Expression<Func<T, object>> field, string indexName = ElasticSearchIndexes.DefaultIndex, CancellationToken cancellationToken = default, int from = 0, int size = 10) where T : IBaseEntity;
}
