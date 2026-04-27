# ADR-0004: Error Handling - Results Pattern and Functional Error

## Status

Accepted

## Context

Traditionally, we used `DomainException` for business rules violations and `Auto-Validation` middleware for input errors. This led to several issues:

- Peformance: Throwing exceptions is expensive due to stack trace generation.
- Hidden Logic: Method signatures like `Task CreateStoreBrand(...)` hide the fact that they can fail.
- Inconsistent Responses: Validation errors from middleware often used a different JSON structure than custom domain exceptions.
- Flow Control: Using exceptions for "expected" errors (like validation failures) is an anti-pattern that makes debugging harder.

## Decision

We adopt `Result Pattern` to treat errors as `First-Class Data` rather than flow-control interruptions.

1. The Result Primitive

We use a base `Result` and generic `Result<T>` class to encapsulate the outcome of any operation.

- Success: Contains the produced data (`Value`).
- Failure: Contains structured `Error` record.
- Integrity: The class enforces that a `Success` state cannot have an error, and a `Failure` state must have an error.

2. The Error Record

Errors are defined as immutable `records` containing:

- Code: A machine-readable string (e.g.: store_brand.name_conflict).
- Description: A human-readable description of the error.
- StatusCode: The corresponding HTTP status code for API responses (e.g: 400, 404, 409).

3. Functional Error Flow

- Domain Layer: Static factory methods on the aggregate root return `Result` instead of throwing exceptions for business rule violations (e.g: StoreBrand.Create returns Result<StoreBrand>).
- Application Layer: Application services perform manual validation using `FluentValidation` and return `Result<T>` if validation or business rules fail.
- API Layer: Controllers use a `ToActionResult` extension method to map the `Result` into a standardized `RFC7807 Problem Details` response.

4. Layered Defense

- Validation: Peformed manually in the `Service` to ensure consistency with the `Result` pattern.
- Global Catch-all: An `IExceptionHandler` (`GlobalExceptionHandler`) acts as a safety net only for unhandled exceptions, returning a generic 500 Internal Server Error with a standard error response.

## Consequences

Positive:

- Explicitness: Method signatures now clearly communicate failure as a possibility.
- Performance: Significant reduction in overhead by eliminating stack trace generation for business logic.
- Consistency: The frontend receives a unified JSON structure (Type, Title, Detail, Status, Instance, TraceId) for every type of error.
- Testability: Service tests can now assert against `result.IsFailure` and `result.Error` without using `Assert.Throws`.

Trade-offs:

- Boilerplate: Requires manual checks (`if (result.IsFailure)`) in services and controllers.
- Manual Mapping: Developers must manually map `FluentValidation` results into the `Result` object.

## Notes

- Implicit Operators: We use implicit conversion operators in the `Result` class to allow returning an `Error` or a `Value` directly, keeping the code clean.
- Standardized Titles: The `ToActionResult` extension automatically formats the `Error.Code` into a human-readable `Title` (e.g: brand.not_found becomes BRAND NOT FOUND).