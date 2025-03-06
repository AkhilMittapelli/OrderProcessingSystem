# Order Processing System

## Building and Running the Application

### Prerequisites
- .NET 9.0 SDK
- Visual Studio 2022 or VS Code

### Building the Application
1. Clone the repository
2. Open a terminal in the root directory
3. Run `dotnet build` to build all projects
4. Run `dotnet test` to execute unit tests
5. Run `dotnet run --project OrderProcessingSystem.Api` to start the API

### Running Unit Tests

#### Using Command Line
- Execute `dotnet test` in the root directory to run all tests
- Tests are organized by service (OrderService, ProductService)
- Each test class focuses on a specific component with clear test names describing the scenario
- Tests verify both success cases and error handling

#### Using Visual Studio 2022
1. Open the solution in Visual Studio 2022
2. Open Test Explorer:
   - Go to View -> Test Explorer (or press Ctrl+E, T)
   - Wait for Visual Studio to discover all tests

3. Running Tests:
   - Click "Run All" in Test Explorer to run all tests
   - Right-click on a specific test class or test to run individual tests
   - Use the search box to filter tests by name
   - Click on test results to view detailed output and any failure messages

4. Debugging Tests:
   - Right-click on a test and select "Debug Selected Tests"
   - Set breakpoints in the test code or application code
   - Step through the code to investigate issues

5. Test Organization:
   - Tests are grouped by class and namespace
   - Green checkmarks indicate passing tests
   - Red X marks indicate failing tests
   - Blue icons indicate tests in progress

6. Available Test Categories:
   - OrderServiceTests:
     * PlaceOrder_WithValidProducts_ShouldSucceed
     * PlaceOrder_WithInsufficientStock_ShouldThrowException
     * PlaceOrder_WithNonExistentProduct_ShouldThrowException
     * GetOrderById_WithExistingOrder_ShouldReturnOrder
     * GetOrderById_WithNonExistentOrder_ShouldThrowException
     * CancelOrder_WithPendingOrder_ShouldSucceed
     * CancelOrder_WithNonPendingOrder_ShouldThrowException

7. Test Output:
   - View detailed test results in the Test Detail Summary
   - Check the Output window for debug messages
   - Review Error List for any test failures

## Key Design Decisions

### Architecture
1. Layered Architecture
   - Core: Domain entities, interfaces, and business logic
   - Infrastructure: Data access, external services
   - API: REST endpoints and controllers

2. Domain-Driven Design
   - Rich domain models (Order, Product)
   - Business logic encapsulated in domain entities
   - Value objects for immutable concepts

3. SOLID Principles
   - Dependency Injection for loose coupling
   - Interface segregation (IOrderService, IProductService)
   - Single Responsibility in services and controllers

### Error Handling
1. Custom Exceptions
   - InsufficientStockException
   - OrderNotFoundException
   - ProductNotFoundException
   - Meaningful error messages and data

2. Global Error Handling Middleware
   - Consistent error responses
   - Proper HTTP status codes
   - Logging of errors

## Concurrency and Asynchronous Processing

### Concurrency Control
1. Optimistic Concurrency
   - SemaphoreSlim for critical sections
   - Atomic operations for stock updates
   - Transaction management in repositories

2. Background Processing
   - OrderFulfillmentBackgroundService for async order processing
   - Fire-and-forget pattern for non-critical operations
   - Queued processing of pending orders

### Asynchronous Operations
1. Task-based Asynchronous Pattern (TAP)
   - All I/O operations are async
   - Proper use of async/await
   - Cancellation support where appropriate

2. Repository Pattern
   - Async CRUD operations
   - Connection management
   - Proper disposal of resources

## API Endpoints

### Orders
- GET /api/orders - List all orders
- GET /api/orders/{id} - Get order by ID
- POST /api/orders - Create new order
- POST /api/orders/{id}/cancel - Cancel order
- POST /api/orders/{id}/fulfill - Fulfill order

### Products
- GET /api/products - List all products
- GET /api/products/{id} - Get product by ID
- POST /api/products - Create new product
- PUT /api/products/{id} - Update product
- DELETE /api/products/{id} - Delete product

## Testing Strategy

### Unit Tests
- Mocking of dependencies
- Clear test names describing scenarios
- Coverage of edge cases
- Verification of business rules

### Test Categories
1. Order Processing
   - Order placement
   - Stock management
   - Order status transitions
   - Concurrent operations

2. Product Management
   - CRUD operations
   - Stock updates
   - Validation rules

3. Integration Points
   - Repository operations
   - Service interactions
   - API endpoints
