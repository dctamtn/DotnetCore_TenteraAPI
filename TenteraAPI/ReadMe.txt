# TenteraAPI - Clean Architecture Implementation

## Overview
TenteraAPI is a .NET 9.0 Web API project that follows Clean Architecture principles. This architecture promotes separation of concerns, maintainability, and testability by organizing the codebase into distinct layers.

## Project Structure
The solution is organized into the following layers:

### 1. Domain Layer
- Contains enterprise business rules and entities
- Independent of other layers
- Defines interfaces that other layers must implement
- Houses core business logic and domain models

### 2. Application Layer
- Contains business use cases
- Orchestrates the flow of data to and from entities
- Implements interfaces defined in the Domain layer
- Contains business rules and validation logic

### 3. Infrastructure Layer
- Implements interfaces defined in the Application layer
- Contains external concerns like:
  - Database access (Entity Framework Core)
  - External services integration
  - File system operations
  - Third-party services

### 4. Presentation Layer
- Contains API controllers and endpoints
- Handles HTTP requests and responses
- Implements API versioning and documentation
- Manages authentication and authorization

## Technology Stack
- .NET 9.0
- Entity Framework Core 9.0.5
- SQL Server
- Twilio 7.11.1 (for SMS/communication services)
- OpenAPI/Swagger for API documentation

## Key Features
- Clean Architecture implementation.
- Follow SOLID principles
- Use dependency injection
- Implement proper logging.
- Write unit tests for business logic.
- RESTful API design.
- Entity Framework Core for data access.
- SMS integration with Twilio.
- Google email integration with SmtpClient.
- API documentation with Swagger/OpenAPI.

### Steps to run the Application
Step 1. UnZip project file.
Step 2. Build the solution & Restore NuGet packages
Step 3. Update the connection string in `appsettings.json`
Step 4. Configure TwilioSettings for sending SMS  in `appsettings.json`
Step 5. Configure SmtpSettings for sending email  in `appsettings.json`
Step 6. Run database migrations 'update-database' to apply database code first.
Step 7. Run the application
Step 8. You can test endoints running by swapgger or import file TenteraAPI.postman_collection.json under folder 'TenteraAPI\TenteraAPI\TenteraAPI.postman_collection.json' into postman to test endpoint.
Step 9: You can check all unittest working by open tab 'Test Explorer' then click to 'Run' to check all unitest passed & cover all API logic.
Step 10: You can check logging file under folder "TenteraAPI\TenteraAPI\logs\tentera-*.txt" when you need check more info that happened when call APIs. 


### How to use APIs: 

Each user have an IC number that was use to register/login as below:

Flow 1: 
When register user.
	-Screen1: 
		-User need input required info:
			+ Customer name.
			+ IC NUmber.
			+ Mobile numbers.
			+ Email address.
		-Then user click on 'Next' button, It will call to API '/api/account/register', 
		-This endpoint will return error message if these inputted info invalid.
		-If inputted data is valid, UI will navigate to screen2.	
		
	-Screen2:
		-On screen2 and call to API 'api/account/send-mobile-code' to get 4-digt code (It expires in 10 minutes). Then user click on 'Verify' button.
		-User check SMS and enter 4-digit then click on 'Verify' button, It will call to API '/api/account/verify-code' to check some validation for the code. 
			+ If It is invalid  => API return error 
			+ If it is valid => API return success => UI navigate to screen3
			
	-Screen3: call to API '/api/account/send-email-code' to get 4-digt code (It expires in 10 minutes). Then user click on 'Verify' button.
			+ If It is invalid  => API return error 
			+ If it is valid => API return success => UI navigate to screen4			
	-Screen4: User click on 'I agreee to terms and conditions and the Privacy Policy', then click on 'Next' button, It will call to API '/api/account/agree' to update database value to true Then it navigate to screen5		
	-Screen5: User input 6-digit PIN then click on 'Next' button, It will call API '/api/account/create-pin' then navigate to screen6.
		
	-Screen6: This screen use to enable finger print and biometric.
		-Call to API '/api/account/biometric/face' to enable biometric
		-Call to API '/api/account/biometric/fingerprint' to enable fingerprint

Flow 2: 
	User input 'IC NUmber' then click on Login, It call to API '/api/account/login'	
		-If login success => It will must also move to screen by screen as above(Screen2, Screen3, Screen4 Screen5)  to complete steps and call APIs then goto profile page.
		-If login have error message => It will move to coresponding screen above to complete steps and call APIs
