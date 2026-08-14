## EXPLANATION: What's Wrong with `if (request is null)` and Complete Service Reference

### ❌ THE PROBLEM

```csharp
public async Task<Result<ApplicationResponse>> CreateAsync(CreateApplicationRequest request, CancellationToken ct = default)
{
	if (request is null)  // ❌ WRONG!
	{
		return Result<ApplicationResponse>.Failure("404","Not found");
	}
}
```

---

## Why This is WRONG

### 1. **ASP.NET Core Request Binding**
In ASP.NET Core, when you receive a request, the framework deserializes it BEFORE it reaches your action method.

```csharp
[HttpPost("create")]
public async Task<IActionResult> Create([FromBody] CreateApplicationRequest request)
{
	// At this point, 'request' is NEVER null
	// If the deserialization fails, ASP.NET Core returns 400 BadRequest BEFORE this code runs
}
```

**The flow:**
- Request arrives → Framework deserializes JSON → If valid, passes to method
- If deserialization fails → Framework returns 400 error (doesn't even call your method)
- If 'required' fields missing → Validation error (doesn't reach your code)

### 2. **When is `request` Actually Null?**
It would only be null if:
- You manually passed `null` (why would you?)
- There's a serious bug in the framework (not happening)
- You're not using data annotations for validation

### 3. **Error Code is WRONG**
```csharp
return Result<ApplicationResponse>.Failure("404","Not found");  // ❌ WRONG!
```

- `404` = "Resource Not Found" (for existing data, like a deleted user trying to update their app)
- `400` = "Bad Request" (for invalid request data, like missing fields)
- `400` is correct for null/missing required fields

---

## ✅ WHAT YOU ACTUALLY NEED

```csharp
// 1️⃣ Validate PROPERTIES, not the object itself
if (string.IsNullOrWhiteSpace(request.UserId))
	return Result<ApplicationResponse>.Failure("400", "UserId is required");

if (string.IsNullOrWhiteSpace(request.JobTitle))
	return Result<ApplicationResponse>.Failure("400", "JobTitle is required");

// 2️⃣ Validate BUSINESS LOGIC
var companyExists = await _companyRepository.ExistsAsync(companyId, ct);
if (!companyExists)
	return Result<ApplicationResponse>.Failure("404", "Company not found");  // ✅ NOW 404 makes sense!

// 3️⃣ Validate CONSTRAINTS
if (request.ScheduledAt <= DateTime.UtcNow)
	return Result<InterviewResponse>.Failure("400", "Interview must be scheduled for future date");
```

---

## HTTP Status Codes - How to Use Them Correctly

| Code | Meaning | When to Use |
|------|---------|------------|
| **200** | OK | Request succeeded |
| **201** | Created | Resource created successfully |
| **204** | No Content | Success but no response body |
| **400** | Bad Request | Invalid input (invalid format, missing required fields, etc.) |
| **401** | Unauthorized | User not authenticated |
| **403** | Forbidden | User authenticated but not allowed |
| **404** | Not Found | Resource doesn't exist (user not found, application doesn't exist) |
| **409** | Conflict | Business logic violation (trying to move from Applied → Wishlist) |
| **422** | Unprocessable Entity | Validation failed (semantic error in valid request) |
| **500** | Server Error | Code error, database error, etc. |

---

## Complete JobApplicationService Reference

Here's the COMPLETE implementation showing all best practices:

```csharp
public class JobApplicationService : IJobApplicationService
{
	private readonly IJobApplicationRepository _applicationRepository;
	private readonly ICompanyRepository _companyRepository;

	// ============================================================
	// 1️⃣ CREATE - Complete with validation and error handling
	// ============================================================
	public async Task<Result<ApplicationResponse>> CreateAsync(
		CreateApplicationRequest request, 
		CancellationToken ct = default)
	{
		// Step 1: Validate required fields
		if (string.IsNullOrWhiteSpace(request.UserId))
			return Result<ApplicationResponse>.Failure("400", "UserId is required");

		if (string.IsNullOrWhiteSpace(request.JobTitle))
			return Result<ApplicationResponse>.Failure("400", "JobTitle is required");

		if (string.IsNullOrWhiteSpace(request.SourceType))
			return Result<ApplicationResponse>.Failure("400", "SourceType is required");

		// Step 2: Check business constraint - company must exist
		var companyId = new CompanyId(request.CompanyId);
		var companyExists = await _companyRepository.ExistsAsync(companyId, ct);

		if (!companyExists)
			return Result<ApplicationResponse>.Failure("404", "Company not found");

		// Step 3: Parse optional salary
		Money? salaryExpectation = null;
		if (request.SalaryExpectation.HasValue && request.SalaryExpectation.Value > 0)
		{
			var currency = string.IsNullOrWhiteSpace(request.SalaryCurrency) 
				? "PHP" 
				: request.SalaryCurrency;
			salaryExpectation = new Money(request.SalaryExpectation.Value, currency);
		}

		// Step 4: Create the source value object
		ApplicationSource source;
		if (Enum.TryParse<SourceType>(request.SourceType, ignoreCase: true, out var sourceType))
		{
			source = sourceType switch
			{
				SourceType.LinkedIn => ApplicationSource.LinkedIn(request.SourceUrl ?? ""),
				SourceType.Referral => ApplicationSource.Referral(request.ReferralName ?? ""),
				_ => new ApplicationSource 
				{ 
					Type = sourceType,
					Url = request.SourceUrl,
					ReferralContactName = request.ReferralName
				}
			};
		}
		else
		{
			return Result<ApplicationResponse>.Failure("400", 
				$"Invalid SourceType: {request.SourceType}");
		}

		// Step 5: Create domain entity
		var application = JobApplication.Create(
			userId: request.UserId,
			companyId: companyId,
			jobTitle: request.JobTitle,
			source: source,
			salaryExpectation: salaryExpectation,
			workType: request.WorkType
		);

		// Step 6: Save to database
		await _applicationRepository.AddAsync(application, ct);

		// Step 7: Map and return
		var response = MapToResponse(application);
		return Result<ApplicationResponse>.Success(response);
	}

	// ============================================================
	// 2️⃣ GET BY ID - Single entity retrieval with 404 handling
	// ============================================================
	public async Task<Result<ApplicationResponse>> GetByIdAsync(
		Guid applicationId, 
		CancellationToken ct = default)
	{
		var appId = new ApplicationId(applicationId);
		var application = await _applicationRepository.GetByIdAsync(appId, ct);

		if (application is null)
			return Result<ApplicationResponse>.Failure("404", "Application not found");

		var response = MapToResponse(application);
		return Result<ApplicationResponse>.Success(response);
	}

	// ============================================================
	// 3️⃣ GET USER PIPELINE - List query with filtering
	// ============================================================
	public async Task<Result<IReadOnlyList<ApplicationResponse>>> GetUserPipelineAsync(
		string userId, 
		CancellationToken ct = default)
	{
		if (string.IsNullOrWhiteSpace(userId))
			return Result<IReadOnlyList<ApplicationResponse>>.Failure("400", "UserId is required");

		var applications = await _applicationRepository.GetByUserIdAsync(userId, ct);
		var responses = applications
			.Select(MapToResponse)
			.ToList()
			.AsReadOnly();

		return Result<IReadOnlyList<ApplicationResponse>>.Success(responses);
	}

	// ============================================================
	// 4️⃣ SCHEDULE INTERVIEW - Complex operation with validation
	// ============================================================
	public async Task<Result<InterviewResponse>> ScheduleInterviewAsync(
		Guid applicationId, 
		ScheduleInterviewRequest request, 
		CancellationToken ct = default)
	{
		// Validate business constraint - date must be in future
		if (request.ScheduledAt <= DateTime.UtcNow)
			return Result<InterviewResponse>.Failure("400", 
				"Interview must be scheduled for a future date");

		// Get the application
		var appId = new ApplicationId(applicationId);
		var application = await _applicationRepository.GetByIdAsync(appId, ct);

		if (application is null)
			return Result<InterviewResponse>.Failure("404", "Application not found");

		try
		{
			// Call domain method - it does business logic validation
			var interview = application.ScheduleInterview(
				type: request.Type,
				scheduledAt: request.ScheduledAt,
				duration: TimeSpan.FromMinutes(request.DurationMinutes),
				interviewer: request.InterviewerName != null 
					? new ContactInfo 
					{ 
						Name = request.InterviewerName, 
						Role = request.InterviewerRole, 
						Email = request.InterviewerEmail 
					}
					: null
			);

			// Save changes
			await _applicationRepository.UpdateAsync(application, ct);

			var response = MapToInterviewResponse(interview);
			return Result<InterviewResponse>.Success(response);
		}
		catch (Exception ex)
		{
			// Domain validation failed
			return Result<InterviewResponse>.Failure("400", ex.Message);
		}
	}

	// ============================================================
	// 5️⃣ MOVE STATUS - State transition with domain validation
	// ============================================================
	public async Task<Result> MoveStatusAsync(
		Guid applicationId, 
		MoveStatusRequest request, 
		CancellationToken ct = default)
	{
		var appId = new ApplicationId(applicationId);
		var application = await _applicationRepository.GetByIdAsync(appId, ct);

		if (application is null)
			return Result.Failure("404", "Application not found");

		try
		{
			// Domain method validates status transition
			application.MoveToStatus(request.NewStatus, request.Reason);

			// Save
			await _applicationRepository.UpdateAsync(application, ct);

			return Result.Success();
		}
		catch (Exception ex)
		{
			// Transition not allowed (e.g., can't go from Rejected to Applied)
			return Result.Failure("409", ex.Message);  // 409 = Conflict
		}
	}

	// ============================================================
	// HELPER METHODS - Mapping between entities and DTOs
	// ============================================================

	private static ApplicationResponse MapToResponse(JobApplication application)
	{
		return new ApplicationResponse(
			Id: application.Id.Value,
			CompanyId: application.CompanyId.Value,
			CompanyName: "", // TODO: Could load from company if needed
			JobTitle: application.JobTitle,
			Status: application.Status,
			AppliedDate: application.AppliedDate,
			CreatedAt: application.CreatedAt,
			Interviews: application.Interviews
				.Select(MapToInterviewResponse)
				.ToList()
				.AsReadOnly()
		);
	}

	private static InterviewResponse MapToInterviewResponse(Interview interview)
	{
		return new InterviewResponse(
			Id: interview.Id.Value,
			RoundNumber: interview.RoundNumber,
			Type: interview.Type,
			ScheduledAt: interview.ScheduledAt,
			Status: interview.Status,
			InterviewerName: interview.Interviewer?.Name
		);
	}
}
```

---

## Key Takeaways

✅ **DO:**
- Validate individual properties/fields
- Check business constraints (does the company exist?)
- Handle domain exceptions with appropriate HTTP codes
- Use 404 for "resource doesn't exist"
- Use 400 for "bad/invalid request data"
- Use 409 for "business logic conflict"

❌ **DON'T:**
- Check if `request is null`
- Return 404 for bad request data (use 400)
- Throw exceptions for expected business errors (catch and return Result)
- Skip validation of optional fields

---

## Usage in Controllers

```csharp
[ApiController]
[Route("api/applications")]
public class ApplicationsController : ControllerBase
{
	private readonly IJobApplicationService _service;

	[HttpPost]
	public async Task<IActionResult> Create(
		[FromBody] CreateApplicationRequest request,
		CancellationToken ct)
	{
		// Service returns Result<T> which is a type-safe response wrapper
		var result = await _service.CreateAsync(request, ct);

		return result.IsSuccess
			? CreatedAtAction(nameof(GetById), 
				new { id = result.Value!.Id }, 
				result.Value)
			: BadRequest(new { error = result.Error, code = result.ErrorCode });
	}

	[HttpGet("{id:guid}")]
	public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
	{
		var result = await _service.GetByIdAsync(id, ct);

		return result.IsSuccess
			? Ok(result.Value)
			: NotFound(new { error = result.Error });
	}
}
```

There you go! Complete reference ready to use! 🚀
