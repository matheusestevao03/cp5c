namespace SetupVault.Api.Exceptions;

/// <summary>Recurso solicitado não existe → 404 Not Found.</summary>
public class NotFoundException(string message) : Exception(message);

/// <summary>Dados válidos no formato, mas que quebram uma regra de negócio → 400 Bad Request.</summary>
public class BusinessRuleException(string message) : Exception(message);

/// <summary>Operação conflita com o estado atual do recurso → 409 Conflict.</summary>
public class ConflictException(string message) : Exception(message);
