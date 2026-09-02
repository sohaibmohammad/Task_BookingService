A. How did you define and enforce overlapping bookings, and why?
Definition: Overlapping bookings are defined as any new reservation request where the requested time window intersects with an existing confirmed booking for the same resource (StartDateTime < ExistingEndDateTime AND EndDateTime > ExistingStartDateTime).

Enforcement: This was enforced at both the application and database layers via an explicit availability check (IsResourceAvailableAsync). To maintain absolute data integrity and prevent double-booking, the check and subsequent insertion are wrapped inside an atomic database transaction (BeginTransactionAsync).

B. What did you assume about concurrency?
We assumed that multiple users might attempt to reserve high-demand resources simultaneously during peak hours. To handle race conditions safely without relying solely on application-level checks, we relied on relational database transaction boundaries and atomicity guarantees via the UnitOfWork pattern.

C. What would break in your design at scale, and where would the first bottleneck be?
First Bottleneck: Database write contention and range-query performance on the Bookings table when executing concurrent availability checks (IsResourceAvailableAsync) on popular resources.

What would break: As historical data scales, frequent date-range queries without strict composite indexing (ResourceId + StartDateTime + EndDateTime) can exhaust database connection pools, increase query latency, and cause deadlocks under heavy write loads on a single centralized database instance.

D. How would you evolve this into a distributed system?
Distributed Locking: Implement Redis-based distributed locks (such as Redlock) keyed by ResourceId during the booking window to ensure mutual exclusion across multiple server nodes before hitting the database.

Asynchronous Queueing: Decouple incoming booking requests using a message broker (like RabbitMQ or Azure Service Bus) to process reservation spikes sequentially or in batches.

Database Sharding: Partition database tables horizontally by resource category or geographical region to distribute read/write loads.

E. Which tradeoff did you prioritize — simplicity, correctness, or performance — and why?
Correctness was prioritized over raw performance and hyper-simplicity. In a resource reservation domain, allowing a double-booking or failing to roll back a conflicting transaction is a critical business failure. Guaranteeing strict data consistency, automatic past-date status mapping, and transactional safety is far more vital than microsecond optimization.

🔌 Simple API Documentation
1. Get Bookings by Resource (with Date Range & Pagination)
Endpoint: GET /api/bookings/resource/{resourceId}

Query Parameters: StartDate (optional), EndDate (optional), Status (optional), PageNumber (default: 1), PageSize (default: 10)

Description: Retrieves paginated bookings for a specific resource. Automatically maps past-date active bookings to "Completed" dynamically.

2. Create Booking
Endpoint: POST /api/bookings

Request Body:

JSON
{
  "resourceId": "string",
  "userId": "string",
  "startDateTime": "2026-06-01T10:00:00Z",
  "endDateTime": "2026-06-01T11:00:00Z"
}
Description: Validates resource availability within a transaction and creates a new confirmed booking. Returns 400 Bad Request if a time overlap occurs.

3. Cancel Booking
Endpoint: DELETE /api/bookings/{id}

Description: Cancels an existing booking. Rejects the request with an exception if the booking is already in the past or its status is not eligible for cancellation.

🧪 Tests
Unit Tests: Developed using xUnit, Moq, and FluentAssertions.

Coverage: Covers service-layer business logic, including concurrency transaction handling, overlap validations, past-date status mapping (Completed), and pagination constraints.
