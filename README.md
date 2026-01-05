# Grounds for Support

Grounds for Support is a full-stack web application designed to facilitate donations ("buying a coffee") using Stripe. It demonstrates a modern integration of a React frontend with an ASP.NET Core backend, handling secure payments, webhooks, and data persistence.

## Features

- **Secure Donations**: Integrated with Stripe Payment Intents for secure transaction processing.
- **Custom Amounts**: Users can specify their donation amount, name, and a personal message.
- **Real-time Feedback**: Immediate confirmation of payment status.
- **Recent Activity**: Displays a list of recent supporters.
- **Rate Limiting**: API endpoints are protected against abuse.
- **Responsive Design**: Works seamlessly on desktop and mobile.

## Tech Stack

### Frontend

- **Framework**: React 19 with TypeScript
- **Build Tool**: Vite
- **Styling**: CSS
- **Payments**: Stripe Elements (`@stripe/react-stripe-js`)
- **Testing**: Vitest, React Testing Library

### Backend

- **Framework**: ASP.NET Core (Minimal APIs)
- **Language**: C#
- **Database**: SQLite with Entity Framework Core
- **Payments**: Stripe.net SDK

### DevOps

- **Containerization**: Docker
- **CI/CD**: GitHub Actions (Build & Deploy)
- **Hosting**: Linux VPS (Ubuntu)

## Getting Started

### Prerequisites

- .NET SDK
- Node.js
- Stripe Account (for API keys)

### Local Development

1. **Clone the repository**

    ```bash
    git clone https://github.com/StevanFreeborn/groundsforsupport.stevanfreeborn.com.git
    cd groundsforsupport.stevanfreeborn.com
    ```

2. **Configure Environment**
    - Update `src/GroundsForSupport.Server/appsettings.Development.json` (or use User Secrets) with your Stripe keys:

        ```json
        "StripeOptions": {
          "ApiKey": "sk_test_...",
          "EventsWebhookSecret": "whsec_..."
        }
        ```

    - Create a `.env` file in `src/GroundsForSupport.Client` with your publishable key:

        ```txt
        VITE_STRIPE_API_KEY=pk_test_...
        ```

3. **Run the Application**
    The server project is configured to build the client automatically.

    ```bash
    cd src/GroundsForSupport.Server
    dotnet run
    ```

    The API will be available at `https://localhost:7071` (or similar), and it will serve the static frontend files.

    *Alternatively, run frontend separately for hot-reloading:*

    ```bash
    cd src/GroundsForSupport.Client
    npm install
    npm run dev
    ```

## Testing

- **Frontend Tests**:

    ```bash
    cd src/GroundsForSupport.Client
    npm test
    ```

- **Backend Tests**:

    ```bash
    dotnet test
    ```

## 📄 License

This project is licensed under the MIT License.
