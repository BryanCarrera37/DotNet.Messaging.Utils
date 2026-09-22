# DotNet.Messaging.Utils

**.NET package** that enables certain functionalities for working with **`RabbitMQ`**. These functionalities focus on:

- **Extension Method** for **`IServiceCollection`** for easy configuration.
- **Automatic Creation** of configured resources (Exchanges, Queues, etc.).
- Message publishing to **`RabbitMQ`** using the **Default Publisher**.
- Enablement of an Interface and **Extension Method** for configuring a **Custom Publisher**.
- Handling of **`Error Objects`** and **`Result Pattern`** for operation results.
- Helper for handling configured options. The functionalities provided by the Helper include:
  - GetExchangeByType
  - GetParkingLotFromRoutingKeyOfMainQueue
  - DeclareResourcesAsync
  - Among others
- Enablement of **`IConnectionManager`** for managing a single **AMQP Connection**.
- **Consumer** that can be used as a **Base Class** and take advantage of certain predefined operations.

## Table of Contents

- **Glossary**
- **How to use the package in an existing project?**
  - **Values to use in Exchanges `(For Property)`**
- **How to contribute to the package?**
- **Running Tests**
- **Dependencies**
  - **DotNet.Messaging.Utils Package**
  - **DotNet.Messaging.Utils.UnitTests**
  - **DotNet.Messaging.Utils.IntegrationTests**

## Glossary

- **Dead Letter Exchange (DLX):** **`RabbitMQ`** Exchange that redirects messages to queues for **dead-lettered** messages.
- **Dead Letter Queue (DLQ):** Queue used to store **dead-lettered** messages and handle **`TTL`** values.
- **Parking Lot Queue:** Queue where messages are stored after all allowed retries have been performed **(`RetryCountLimit` Property)**.

## How to use the package in an existing project?

1. Search for the **`DotNet.Messaging.Utils`** package using the **NuGet Package Manager**, or run the following command through the **Package Manager** console:

   ```
   Install-Package DotNet.Messaging.Utils
   ```

1. Once the package is installed, you must set the required values in your configuration file (e.g., `appsettings.json`):

   ```
   {
    "MessagingSettings": {
        "PayloadEncryptionKey": "",
        "RetryCountLimit": "", // Retry limit applying DLX and DLQ
        "Resources": {
            "VHost": "/vhost", // VHost configured for your project
            "Exchanges": [
                    {
                        "Name": "ex.name.direct",
                        "Type": "direct", // The Exchange type given the RabbitMQ values (direct, topic, etc.)
                        "For": "main" // Property related to the resources managed by this Exchange. See the Values to use in Exchanges section for more details
                    },
                    {
                        "Name": "dlx.name.direct",
                        "Type": "direct",
                        "For": "deadLetter"
                    },
                    {
                        "Name": "ex.name.parking",
                        "Type": "direct",
                        "For": "parkingLot"
                    }
                    ...
            ],
            "Queues": [
                    {
                        "Name": "q.notifications.email",
                        "RoutingKey": "notification.direct.email",
                        "ExchangeName": "ex.name.direct",
                        "DeadLetter": { // This is an OPTIONAL resource; you can omit this section if you do not use DLX
                            "Name": "dlq.notification.email",
                            "RoutingKey": "notification.direct.email",
                            "ExchangeName": "dlx.name.direct",
                            "TtlInSeconds": 5
                        },
                        "ParkingLot": { // This is an OPTIONAL resource; you can omit this section if you do not use a Parking Lot
                            "Name": "q.notification.email.parking",
                            "RoutingKey": "notification.email.parking"
                            "ExchangeName": "ex.name.parking",
                        }
                    }
            ]
        }
    }
   }
   ```

### Values to use in Exchanges `(For Property)`

The possible values are:

- main; exchange for main queues
- deadLetter; exchange for **DLQs**
- parkingLot; **Parking Lot Queues**

## How to contribute to the package?

1.  **Clone the repository:**

    ```
    git clone https://github.com/BryanCarrera37/DotNet.Messaging.Utils.git
    ```

2.  **Create your feature or fix branch:**

    ```
    git checkout -b feature/new-functionality
    ```

3.  **Create and Run Tests:**
    All implemented changes must have their respective tests, whether **`Unit Tests`** and **`Integration Tests`**. For more details, see the **Running Tests** section.
4.  **Publish Changes:**
    Once you have made your respective commit and published the branch to the **Remote Repository**, you can create a **Pull Request** in `GitHub` for reviewing and merging into the `develop` branch.

## Running Tests

**`Unit Tests`** must be created for new functionalities or bug fixes. Likewise, **`IntegrationTests`** must be considered for functionalities involving interaction with **`RabbitMQ`** (e.g., Queue creation, Exchanges, etc.). Make sure all tests pass before submitting your code.

You can use the `Test Explorer` in `Visual Studio` or run the following command in the terminal:

```
dotnet test
```

There are currently two test projects:

- **DotNet.Messaging.Utils.UnitTests**; specifically for Unit Tests.
- **DotNet.Messaging.Utils.IntegrationTests**; specifically for Integration Tests.

## Dependencies

### DotNet.Messaging.Utils Package

- DotNet.Messaging.OAuth
- BryanCM.AspNet.Utils
- Microsoft.Extensions.Configuration
- Microsoft.Extensions.Options
- Microsoft.Extensions.Options.ConfigurationExtensions

### DotNet.Messaging.Utils.UnitTests

- coverlet.collector
- Microsoft.NET.Test.Sdk
- xunit
- xunit.runner.visualstudio

### DotNet.Messaging.Utils.IntegrationTests

- coverlet.collector
- Microsoft.NET.Test.Sdk
- Testcontainers.Keycloak
- Testcontainers.RabbitMq
- xunit
- xunit.runner.visualstudio
