# Wallet Management API

This API provides endpoints for managing wallets, allowing users to perform CRUD operations on their wallets.

## Endpoints

### Add Wallet

**POST** `/api/wallets`

Adds a new wallet to the user's account. Validates for:

- Duplicate wallet entries.
- A user can't have more than 5 wallets.

Payload:

```json
{
  "name": "My Wallet",
  "type": "momo",
  "accountNumber": "1234567890",
  "accountScheme": "mtn"
}
```

### Delete Wallet

**DELETE** `/api/wallets/{id}`

Removes a wallet by its ID from the user's account.

### Get Single Wallet

**GET** `/api/wallets/{id}`

Retrieves a single wallet by its ID.

### List All Wallets

**GET** `/api/wallets`

Retrieves a list of all wallets belonging to the authenticated user.

## Wallet Model

- **ID**: Unique identifier for the wallet.
- **Name**: Name given to the wallet.
- **Type**: Type of the wallet (`momo` or `card`).
- **Account Number**: Masked account number (stores only the first 6 digits).
- **Account Scheme**: Scheme of the wallet (`visa`, `mastercard`, `mtn`, `vodafone`, `airteltigo`).
- **Created At**: Date and time the wallet was created.
- **Owner**: Owenr's details.

## Usage

Ensure you have .NET 8 installed. Run the following commands to start the application:

```bash
dotnet restore
dotnet run
```

The swagger docs will be available at `{url}/swagger/index.html`.

## Tests

The project includes comprehensive tests covering all endpoints and functionalities. Run the following command to execute the tests:

```bash
dotnet test
```

---
