# Auth0 Configuration for Admin User Seeding

This document explains how to configure Auth0 for secure admin user seeding.

## Configuration Steps

1. **Auth0 Management API Setup**
   - Go to your Auth0 Dashboard
   - Navigate to Applications > APIs > Auth0 Management API
   - Create a new Machine-to-Machine application
   - Grant the following scopes:
     - `read:users`
     - `read:user_app_metadata`
     - `read:user_idp_tokens`

2. **Update appsettings.json**
   ```json
   {
     "Auth0": {
       "Domain": "your-domain.auth0.com",
       "Audience": "your-api-audience",
       "Application": {
         "ClientId": "your-client-id",
         "ClientSecret": "your-client-secret"
       },
       "ManagementApi": {
         "ClientId": "your-management-api-client-id",
         "ClientSecret": "your-management-api-client-secret"
       }
     },
     "AdminUser": {
       "UserId": "auth0|your-admin-user-id",
       "EnableSeeding": true,
       "DefaultUsername": "Admin",
       "DefaultFirstName": "Admin",
       "DefaultLastName": "User"
     }
   }
   ```

3. **Production Configuration (recommended)**
   ```json
   {
     "Auth0": {
       "Domain": "your-domain.auth0.com",
       "Audience": "your-api-audience",
       "Application": {
         "ClientId": "your-client-id",
         "ClientSecret": "your-client-secret"
       },
       "ManagementApi": {
         "ClientId": "your-management-api-client-id",
         "ClientSecret": "your-management-api-client-secret"
       }
     },
     "AdminUser": {
       "UserId": "auth0|your-admin-user-id",
       "EnableSeeding": false,
       "DefaultUsername": "Admin",
       "DefaultFirstName": "Admin",
       "DefaultLastName": "User"
     }
   }
   ```

## Configuration Structure

### Auth0Options
- **Domain**: Your Auth0 domain (e.g., "your-domain.auth0.com")
- **Audience**: API audience for authentication
- **Application**: Nested object containing:
  - **ClientId**: Application client ID
  - **ClientSecret**: Application client secret
- **ManagementApi**: Nested object containing:
  - **ClientId**: Client ID for Auth0 Management API
  - **ClientSecret**: Client Secret for Auth0 Management API

### AdminUserOptions
- **UserId**: Auth0 user ID for the admin user
- **EnableSeeding**: Whether to enable admin user seeding
- **DefaultUsername**: Default username if not available from Auth0
- **DefaultFirstName**: Default first name if not available from Auth0
- **DefaultLastName**: Default last name if not available from Auth0

## Security Features

- **Mandatory Auth0 User Validation**: Always verifies the user exists in Auth0 before seeding (no opt-out)
- **Configurable Seeding**: Can be disabled in production environments
- **Audit Logging**: Logs all admin user operations
- **Real User Data**: Uses actual Auth0 user information for better data quality
- **Separation of Concerns**: Auth0 config separate from admin user config

## Environment Variables (Recommended for Production)

Instead of storing sensitive information in appsettings.json, use environment variables:

```bash
# PowerShell
$env:Auth0__Domain="your-domain.auth0.com"
$env:Auth0__Audience="your-api-audience"
$env:Auth0__Application__ClientId="your-client-id"
$env:Auth0__Application__ClientSecret="your-client-secret"
$env:Auth0__ManagementApi__ClientId="your-management-api-client-id"
$env:Auth0__ManagementApi__ClientSecret="your-management-api-client-secret"

$env:AdminUser__UserId="auth0|your-admin-user-id"
$env:AdminUser__EnableSeeding="false"
```

## How It Works

1. **On Application Startup**: `DatabaseSeeder` runs as a hosted service
2. **Configuration Check**: Validates if admin user seeding is enabled and if UserId is configured
3. **Mandatory Auth0 Validation**: Always calls Auth0 Management API to verify user exists
4. **User Information Retrieval**: Fetches user details from Auth0 (name, email, etc.)
5. **Database Seeding**: Creates a new user or updates an existing user with the admin role
6. **Audit Logging**: Logs all operations for security monitoring

## Benefits

- **Maximum Security**: User validation is always required - no way to bypass Auth0 verification
- **Data Quality**: Uses real Auth0 user data (name, email, etc.)
- **Auditability**: Full logging of admin user operations
- **Flexibility**: Can be disabled in production or configured per environment
- **Idempotent**: Safe to run multiple times without side effects
- **Separation of Concerns**: Clean separation between Auth0 and admin user configuration

# Auth0 Configuration for Finance API

## Admin User Seeding Setup

This document explains how to configure Auth0 for secure admin user seeding in the Finance API.

### Configuration Steps

1. **Auth0 Management API Setup**
   - Go to your Auth0 Dashboard
   - Navigate to Applications > APIs > Auth0 Management API
   - Create a new Machine-to-Machine application
   - Grant the following scopes:
     - `read:users`
     - `read:user_app_metadata`
     - `read:user_idp_tokens`

2. **Update appsettings.json**
   ```json
   {
     "Auth0": {
       "Domain": "your-domain.auth0.com",
       "Audience": "your-api-audience",
       "Application": {
         "ClientId": "your-client-id",
         "ClientSecret": "your-client-secret"
       },
       "ManagementApi": {
         "ClientId": "your-management-api-client-id",
         "ClientSecret": "your-management-api-client-secret"
       }
     },
     "AdminUser": {
       "UserId": "auth0|your-admin-user-id",
       "EnableSeeding": true,
       "DefaultUsername": "Admin",
       "DefaultFirstName": "Admin",
       "DefaultLastName": "User"
     }
   }
   ```

3. **Production Configuration (recommended)**
   ```json
   {
     "Auth0": {
       "Domain": "your-domain.auth0.com",
       "Audience": "your-api-audience",
       "Application": {
         "ClientId": "your-client-id",
         "ClientSecret": "your-client-secret"
       },
       "ManagementApi": {
         "ClientId": "your-management-api-client-id",
         "ClientSecret": "your-management-api-client-secret"
       }
     },
     "AdminUser": {
       "UserId": "auth0|your-admin-user-id",
       "EnableSeeding": false,
       "DefaultUsername": "Admin",
       "DefaultFirstName": "Admin",
       "DefaultLastName": "User"
     }
   }
   ```

### Configuration Structure

#### Auth0Options
- **Domain**: Your Auth0 domain (e.g., "your-domain.auth0.com")
- **Audience**: API audience for authentication
- **Application**: Nested object containing:
  - **ClientId**: Application client ID
  - **ClientSecret**: Application client secret
- **ManagementApi**: Nested object containing:
  - **ClientId**: Client ID for Auth0 Management API
  - **ClientSecret**: Client Secret for Auth0 Management API

#### AdminUserOptions
- **UserId**: Auth0 user ID for the admin user
- **EnableSeeding**: Whether to enable admin user seeding
- **DefaultUsername**: Default username if not available from Auth0
- **DefaultFirstName**: Default first name if not available from Auth0
- **DefaultLastName**: Default last name if not available from Auth0

## Security Features

- **Mandatory Auth0 User Validation**: Always verifies the user exists in Auth0 before seeding (no opt-out)
- **Configurable Seeding**: Can be disabled in production environments
- **Audit Logging**: Logs all admin user operations
- **Real User Data**: Uses actual Auth0 user information for better data quality
- **Separation of Concerns**: Auth0 config separate from admin user config

## Environment Variables (Recommended for Production)

Instead of storing sensitive information in appsettings.json, use environment variables:

```bash
# PowerShell
$env:Auth0__Domain="your-domain.auth0.com"
$env:Auth0__Audience="your-api-audience"
$env:Auth0__Application__ClientId="your-client-id"
$env:Auth0__Application__ClientSecret="your-client-secret"
$env:Auth0__ManagementApi__ClientId="your-management-api-client-id"
$env:Auth0__ManagementApi__ClientSecret="your-management-api-client-secret"

$env:AdminUser__UserId="auth0|your-admin-user-id"
$env:AdminUser__EnableSeeding="false"
```

## How It Works

1. **On Application Startup**: `DatabaseSeeder` runs as a hosted service
2. **Configuration Check**: Validates if admin user seeding is enabled and if UserId is configured
3. **Mandatory Auth0 Validation**: Always calls Auth0 Management API to verify user exists
4. **User Information Retrieval**: Fetches user details from Auth0 (name, email, etc.)
5. **Database Seeding**: Creates a new user or updates an existing user with the admin role
6. **Audit Logging**: Logs all operations for security monitoring

## Benefits

- **Maximum Security**: User validation is always required - no way to bypass Auth0 verification
- **Data Quality**: Uses real Auth0 user data (name, email, etc.)
- **Auditability**: Full logging of admin user operations
- **Flexibility**: Can be disabled in production or configured per environment
- **Idempotent**: Safe to run multiple times without side effects
- **Separation of Concerns**: Clean separation between Auth0 and admin user configuration

## Authentication and Authorization

This API uses Auth0 for authentication and authorization. All endpoints are protected and require a valid JWT token from Auth0 with specific roles.

### Implementation Details

1. **Authentication Setup**
   - JWT Bearer authentication is configured in `ConfigExtensions.ConfigureAuth0()`
   - The API validates tokens against the Auth0 domain and audience specified in configuration

2. **Authorization Policy**
   - A policy named "AdminOrOwnerPolicy" has been defined
   - This policy uses database-driven authorization that:
     - Checks if the authenticated user exists in the database
     - Verifies the user has either the "Admin" or "Owner" role assigned in the database
     - Uses the Auth0 user ID from the JWT token to match with the database user

3. **Controller Protection**
   - All API controllers inherit from `SecuredApiController` which applies the AdminOrOwnerPolicy
   - This ensures that only users with admin or owner roles can access any endpoint

### Role Assignment

To give a user access to the API:

1. First, create the user in Auth0:
   - Go to your Auth0 Dashboard
   - Navigate to User Management > Users
   - Create or select the user you want to grant access to

2. Then, ensure the user is added to the application database with the appropriate role:
   - The user will be automatically added to the database on first login
   - An administrator must then assign either the "Admin" or "Owner" role to the user in the database
   - Alternatively, if admin user seeding is enabled, the configured admin user will be automatically assigned the Admin role

### Testing Authentication

To test if authentication is working:

1. Get a valid token from Auth0 using a user with the appropriate role
2. Make a request to any API endpoint with the header:
   ```
   Authorization: Bearer {your-jwt-token}
   ```
3. If the token is valid and has the correct roles, the request should succeed
4. If not, you'll receive a 403 Forbidden response
