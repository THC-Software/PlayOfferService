# PlayOfferService

## Deploying Kubernetes Manifests Locally with Minikube

### Prerequisites

- [Install Minikube](https://minikube.sigs.k8s.io/docs/start/)
- [Install kubectl](https://kubernetes.io/docs/tasks/tools/install-kubectl/)

### Step-by-Step Guide

#### 1. Start Minikube

Start Minikube with sufficient resources.

```bash
minikube start --memory=4096 --cpus=2
```

#### 2. Apply kubernetes Manifests

Use kubectl to apply each of these YAML files. This will create the necessary Kubernetes resources.

```bash
kubectl apply -f redis.yml
kubectl apply -f pos_postgres_write.yml
kubectl apply -f pos_postgres_read.yml
kubectl apply -f pos_debezium.yml
kubectl apply -f pos_service.yml
```

#### 3. Check the status of pods/services

Check the status of your pods and services to ensure they are running correctly.

```bash
kubectl get pods
kubectl get services
```

#### 4. Access the service

Minikube provides a way to access services running inside the cluster using minikube service.

```bash
minikube service pos-service
```

You can also use `kubectl port-forward` to forward a port from your local machine to a port on a pod. For example:

```bash
kubectl port-forward deployment/pos-service 8080:8080
```

## Used patterns in microservice

### Saga

The Saga pattern is used to maintain data consistency in a microservice architecture. It is a sequence of local transactions where each transaction updates the data and publishes a message or event to trigger the next transaction in the sequence. If a transaction fails, the saga executes a series of compensating transactions that undo the changes that were made by the preceding transactions.

In the PlayOfferService the Saga Pattern is used in conjunction with the CourtService, to automatically create a Reservation if a PlayOffer is joined by a second Member. It includes the following Steps:

1. After `JoinPlayOfferCommand` is received, the PlayOfferService publishes a `PlayOfferJoinedEvent`
2. The CourtService listens to the `PlayOfferJoinedEvent` and tries to create a Reservation for the PlayOffer at the specified time in the PlayOfferJoinedEvent
3. The CourtService publishes one of three possible events, each containing the `EventId` of the `PlayOfferJoinedEvent` in the `CorrelationId`:
   - `ReservationCreatedEvent` if the Reservation was successfully created
   - `ReservationRejectedEvent` if the Reservation could not be created (e.g. no court available)
   - `ReservationLimitExceededEvent` if the Reservation could not be created due to a limit of Reservations per Member
4. The PlayOfferService listens to the events published by the CourtService and reacts depending on the event:
   - If a `ReservationCreatedEvent` is received it then triggers a `PlayOfferReservationAddedEvent` in the PlayOfferService to add the Reservation to the respective PlayOffer
   - If a `ReservationRejectedEvent` or `ReservationLimitExceededEvent` is received it then triggers a `PlayOfferOpponentRemovedEvent` to revert the changes of the `PlayOfferJoinedEvent`

The **compensation logic** for the Saga is implemented in the [ReservationEventHandler](./Application/Handlers/Events/ReservationEventHandler.cs) File in the functions in line _81_ and _102_.

### CQRS

The CQRS pattern is used to separate the read and write operations of a system. It allows for the creation of different models for reading and writing data, which can be optimized for their respective use cases. The CQRS pattern is used in the PlayOfferService to separate the read and write operations for PlayOffers.

The write operations are implemented using commands, which are located in the [Commands](./Application/Commands) folder. The read operations are implemented using queries, which are located in the [Queries](./Application/Queries) folder.

These commands and queries are then handled by their respective handlers, which are located in the root of the [Handlers](./Application/Handlers) folder. Each handler is responsible for executing the logic for a specific command or query.

#### Projection

The CQRS pattern is also used to implement projections in the PlayOfferService. Projections are used to transform the data from the write model to the read model. In the PlayOfferService, projections are implemented using the **Mediator Pattern** which is implemented in the [Events](./Application/Handlers/Events) folder.

Each Aggregate has a dedicated `RedisStreamReader` which subscribes to the Redis stream and listens to and parses the events for a specific aggregate, these can be found in the root of the [Application](./Application) folder.
Afterward each Event is handled by the respective `EventHandler` for the aggregates, which can be found in the [Events](./Application/Handlers/Events) folder. These `EventHandlers` then update the read model accordingly.

### Event Sourcing

TODO: Beschreiben wo events implementiert werden, besoderheiten WriteSide als EventStore + apply methoden in Aggregates

Event Sourcing is a pattern that involves storing the state of an application as a sequence of events. These events represent changes to the state of the application and can be used to reconstruct the state of the application at any point in time.

First a Request is received and the Controller creates a Command, which is then handled by the appropriate CommandHandler. The CommandHandler then creates an Event, which is stored in the Event Store.

This way the PostgreSQL DB acts as an Event Store, which stores the events for each Aggregate. The events are then used to reconstruct the state of the application by applying the events to the Aggregates.

The events are stored in the `events` table in the database, which contains the following columns:

- `event_id`: The unique identifier for the event
- `entity_id`: The unique identifier for the entity that the event belongs to
- `event_type`: The type of the event
- `entity_type`: The type of the entity that the event belongs to
- `event_data`: The data associated with the event
- `timestamp`: The timestamp when the event occurred
- `correlation_id`: The correlation id of the event

The events are then applied to the Aggregates by calling the `Apply` method on the Aggregate, which updates the state of the Aggregate based on the event.

The Implementation of the Apply method can be found in the Aggregates classes in the [Models](./Domain/Models) folder.

The Command Handlers `CancelPlayOfferHandler`, `CreatePlayOfferHandler` and `JoinPlayOfferHandler` are implemented in the [Commands](./Application/Handlers) folder and the Event Handlers are implemented in the [Events](./Application/Handlers/Events)

### Optimistic Locking

Optimistic Locking is a concurrency control mechanism that is used to prevent conflicts when multiple requests try to change or create data at the same time.

In the PlayOfferService, Optimistic Locking is implemented using the `EFCore` and its transaction mechanism. When a request is received, the current amount of events is read and incremented by one.

When the request is processed, the amount of events is read again and compared to the initial amount. If the amount of events has changed unexpectedly during the transaction, a concurrency exception is thrown and the transaction rolled back.

otherwise the transaction is committed and the changes are saved to the database.

The Optimistic Locking is implemented in the each CommandHandler in the [Commands](./Application/Handlers) folder.

- [`CancelPlayOfferHandler`](./Application/Handlers/CancelPlayOfferHandler.cs)
  - _Line 26:27_
  - _Line 67:75_
- [`JoinPlayOfferHandler`](./Application/Handlers/JoinPlayOfferHandler.cs)
  - _Line 29:30_
  - _Line 88:96_
- [`CreatePlayOfferHandler`](./Application/Handlers/CreatePlayOfferHandler.cs)
  - _Line 69:70_
  - _Line 75:83_

### Domain Driven Design

TODO: Design von Entitäten, enthalten business logic

Domain-Driven Design (DDD) is an approach to software development that focuses on the core domain and domain logic of the application. It is used to model complex domains in software by creating a shared understanding of the domain between technical and non-technical stakeholders.

In the PlayOfferService, DDD is used to model the core domain of the application, which includes the following entities:

- `PlayOffer`: Represents a play offer that is created by a member and can be joined by other members
- `Member`: Represents a member of the platform who can create and join play offers
- `Reservation`: Represents a reservation for a play offer that is created by the court service
- `Court`: Represents a court that can be reserved for a play offer
- `Club`: Represents a club that can have multiple courts and members

### Transaction Log Trailing

TODO: implementation debezium, für was verantwortlich --> projection von write zu read seite

Transaction Log Trailing is a technique used to capture changes to a database by reading the transaction log of the database. It is used to implement change data capture (CDC) in a microservice architecture.

In the PlayOfferService, Transaction Log Trailing is implemented using Debezium, which is an open-source platform for change data capture. Debezium captures changes to the PostgreSQL database and publishes them as events to a Redis Stream.

Debezium is a separate service that runs in the Kubernetes cluster and listens to the PostgreSQL database for changes. When a change occurs, Debezium captures the change and publishes it as an event to the Redis Stream.

the Debezium configuration can be found in the [pos_debezium.yml](./debezium-conf/application.properties) file.
