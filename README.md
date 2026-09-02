# Backend Booking Management API

## 📋 Table of Contents
1. [Design Write-Up](#03-design-write-up)
2. [API Documentation](#-api-documentation)
3. [Testing](#-tests)

---

## 03. Design Write-Up

### A. How did you define and enforce overlapping bookings, and why?
* **Definition:** Overlapping bookings are defined as any new reservation request where the requested time window intersects with an existing confirmed booking for the same resource (`StartDateTime < ExistingEndDateTime` AND `EndDateTime > ExistingStartDateTime`).
* **Enforcement:** This was enforced at both the application and database layers via an explicit availability check (`IsResourceAvailableAsync`). To maintain absolute data integrity and prevent double-booking, the check and subsequent insertion are wrapped inside an atomic database transaction (`BeginTransactionAsync`).

### B. What did you assume about concurrency?
We assumed that multiple users might attempt to reserve high-demand resources simultaneously during peak hours. To handle race conditions safely without relying solely on application-level checks, we relied on relational database transaction boundaries and atomicity guarantees via the `UnitOfWork` pattern.

### C. What would break in your design at scale, and where would the first bottleneck be?
* **First Bottleneck:** Initially, frequent database round-trips for resource availability checks and resource retrieval create read latency and database load on popular resources.
* **What would break / Mitigation:** As historical data scales, frequent date-range queries without strict composite indexing (`ResourceId` + `StartDateTime` + `EndDateTime`) can exhaust database connection pools and cause deadlocks. To alleviate this, a caching layer (such as Redis) can be introduced to cache resource metadata and availability states, reducing direct database read queries.

### D. How would you evolve this into a distributed system?
* **Distributed Caching & Locking:** Leverage **Redis** both as a distributed caching layer (with cache-aside/invalidation patterns for resource states) and for distributed locks (such as Redlock) keyed by `ResourceId` to ensure mutual exclusion across multiple server replicas before hitting the database.
* **Asynchronous Queueing:** Decouple incoming booking requests using a message broker (like RabbitMQ or Azure Service Bus) to process reservation spikes sequentially or in batches.
* **Database Sharding:** Partition database tables horizontally by resource category or geographical region to distribute read/write loads.

### E. Which tradeoff did you prioritize — simplicity, correctness, or performance — and why?
**Correctness** was prioritized over raw performance and hyper-simplicity. In a resource reservation domain, allowing a double-booking or failing to roll back a conflicting transaction is a critical business failure. Guaranteeing strict data consistency, automatic past-date status mapping, and transactional safety is far more vital than microsecond optimization.

---

## 🔌 API Documentation

### 1. Get All Resources (Cached for Dropdowns)
* **Endpoint:** `GET /api/resources`
* **Description:** Retrieves the list of all available resources. **(Caching & Frontend Optimization):** This endpoint is fully cached to instantly feed UI dropdowns/selectors on the frontend, eliminating repetitive database queries and ensuring blazing-fast rendering.

### 2. Get Resource Bookings (with Date Range, Pagination & Caching)
* **Endpoint:** `GET /api/bookings/resource/{resourceId}`
* **Query Parameters:** 
  * `StartDate` *(optional)*
  * `EndDate` *(optional)*
  * `Status` *(optional)*
  * `PageNumber` *(default: 1)*
  * `PageSize` *(default: 10)*
* **Description:** Retrieves paginated and filtered bookings for a specific resource. **(Performance Optimization):** Resource availability states and metadata leverage an integrated caching layer to minimize direct database read queries and boost response times. Automatically maps past-date active bookings to "Completed" dynamically.

### 3. Create Booking
* **Endpoint:** `POST /api/bookings`
* **Request Body:**
  ```json
  {
    "resourceId": "string",
    "userId": "string",
    "startDateTime": "2026-06-01T10:00:00Z",
    "endDateTime": "2026-06-01T11:00:00Z"
  }
