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
TODO: Add file/line numbers
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

The **compensation logic** for the Saga is implemented in the [ReservationEventHandler](./Application/Handlers/Events/ReservationEventHandler.cs) File in the functions in lines _81 - 102_.

### CQRS
TODO: Add file/line numbers
The CQRS pattern is used to separate the read and write operations of a system. It allows for the creation of different models for reading and writing data, which can be optimized for their respective use cases. The CQRS pattern is used in the PlayOfferService to separate the read and write operations for PlayOffers.

The write operations are implemented using commands, which are located in the [Commands](./Application/Commands) folder. The read operations are implemented using queries, which are located in the [Queries](./Application/Queries) folder.

These commands and queries are then handled by their respective handlers, which are located in the root of the [Handlers](./Application/Handlers) folder. Each handler is responsible for executing the logic for a specific command or query.

#### Projection
TODO: Add file/line numbers
The CQRS pattern is also used to implement projections in the PlayOfferService. Projections are used to transform the data from the write model to the read model. In the PlayOfferService, projections are implemented using the **Mediator Pattern** which is implemented in the [Events](./Application/Handlers/Events) folder.

Each Aggregate has a dedicated `RedisStreamReader` which subscribes to the Redis stream and listens to and parses the events for a specific aggregate, these can be found in the root of the [Application](./Application) folder.
Afterward each Event is handled by the respective `EventHandler` for the aggregates, which can be found in the [Events](./Application/Handlers/Events) folder. These `EventHandlers` then update the read model accordingly.

### Queries
TODO: Add file/line numbers, describe how it is implemented

### Commands
TODO: Add file/line numbers, describe how it is implemented

### Event Sourcing
The write side of the CQRS implementation is using a event sourcing pattern. In the PlayOfferService, events are used to represent changes to the state of Entities.
When a command is received, it is validated and then converted into one or more events, which are then stored in the write side database.

The events are structured with a hierarchy of event classes:
- `Technical[...]Event`: Represents a group of events that are used for a specific entity, these are used to route the events to the correct `EventHandler` in the read model. Implements the `BaseEvent` class.
    - [`TechnicalPlayOfferEvent.cs`](./Domain/Events/PlayOffer/TechnicalPlayOfferEvent.cs)(_Line 1:7_): Represents the events for the `PlayOffer` entity
    - [`TechnicalMemberEvent.cs`](./Domain/Events/Member/TechnicalMemberEvent.cs)(_Line 1:7_): Represents the events for the `Member` entity
    - [`TechnicalReservationEvent.cs`](./Domain/Events/Reservation/TechnicalReservationEvent.cs)(_Line 1:8_): Represents the events for the `Reservation` entity
    - [`TechnicalCourtEvent.cs`](./Domain/Events/Court/TechnicalCourtEvent.cs)(_Line 1:7_): Represents the events for the `Court` entity
    - [`TechnicalClubEvent.cs`](./Domain/Events/Club/TechnicalClubEvent.cs)(_Line 1:7_): Represents the events for the `Club` entity
- [`BaseEvent.cs`](./Domain/Events/BaseEvent.cs)(_Line 1:34_): Represents the whole event including the following metadata:
Each event class represents a specific type of event that can occur in the system.
  - `event_id`: The unique identifier for the event
  - `entity_id`: The unique identifier for the entity that the event belongs to
  - `event_type`: The type of the event
  - `entity_type`: The type of the entity that the event belongs to
  - `timestamp`: The timestamp when the event occurred
  - `correlation_id`: The correlation id of the event
- [`DomainEvent.cs`](./Domain/Events/DomainEvent.cs)(_Line 1:35_): Is used as the data type of the `eventData` property in the `BaseEvent` class. It is also used for json serialization and deserialization.

The smallest unit of events can be found in the [Events](./Domain/Events) folder. Each event class represents a specific type of event that can occur in the system and implements the `DomainEvent` class.
- **PlayOfferEvents**:
  - [`PlayOfferCreatedEvent.cs`](./Domain/Events/PlayOffer/PlayOfferCreatedEvent.cs)(_Line 1:26_): Represents the event when a PlayOffer is created
  - [`PlayOfferJoinedEvent.cs`](./Domain/Events/PlayOffer/PlayOfferJoinedEvent.cs)(_Line 1:9_): Represents the event when a Opponent joins a PlayOffer
  - [`PlayOfferCancelledEvent.cs`](./Domain/Events/PlayOffer/PlayOfferCancelledEvent.cs)(_Line 1:6_): Represents the event when a PlayOffer is canceled
  - [`PlayOfferReservationAddedEvent.cs`](./Domain/Events/PlayOffer/PlayOfferReservationAddedEvent.cs)(_Line 1:6_): Represents the event when a Reservation was created by the court service and was now added to the PlayOffer
  - [`PlayOfferOpponentRemovedEvent.cs`](./Domain/Events/PlayOffer/PlayOfferOpponentRemovedEvent.cs)(_Line 1:5_): Represents the event when no Reservation could be created by the court service and therefore the opponent was removed from the PlayOffer


- **MemberEvents**:
  - [`MemberCreatedEvent.cs`](./Domain/Events/Member/MemberCreatedEvent.cs)(_Line 1:13_): Represents the event when a Member is created
  - [`MemberDeletedEvent.cs`](./Domain/Events/Member/MemberDeletedEvent.cs)(_Line 1:5_): Represents the event when a Member is deleted
  - [`MemberEmailChangedEvent.cs`](./Domain/Events/Member/MemberEmailChangedEvent.cs)(_Line 1:6_): Represents the event when the email of a Member is changed
  - [`MemberFullNameChangedEvent.cs`](./Domain/Events/Member/MemberFullNameChangedEvent.cs)(_Line 1:8_): Represents the event when the name of a Member is changed
  - [`MemberLockedEvent.cs`](./Domain/Events/Member/MemberLockedEvent.cs)(_Line 1:6_): Represents the event when a Member is locked
  - [`MemberUnlockedEvent.cs`](./Domain/Events/Member/MemberUnlockedEvent.cs)(_Line 1:6_): Represents the event when a Member is unlocked


- **ReservationEvents**:
  - [`ReservationCreatedEvent.cs`](./Domain/Events/Reservation/ReservationCreatedEvent.cs)(_Line 1:21_): Represents the event when a Reservation is created
  - [`ReservationCancelledEvent.cs`](./Domain/Events/Reservation/ReservationCancelledEvent.cs)(_Line 1:5_): Represents the event when a Reservation is canceled
  - [`ReservationLimitExceededEvent.cs`](./Domain/Events/Reservation/ReservationLimitExceededEvent.cs)(_Line 1:21_): Represents the event when the limit of Reservations per Member is exceeded
  - [`ReservationRejectedEvent.cs`](./Domain/Events/Reservation/ReservationRejectedEvent.cs)(_Line 1:6_): Represents the event when a Reservation could not be created


- **CourtEvents**:
    - [`CourtCreatedEvent.cs`](./Domain/Events/Court/CourtCreatedEvent.cs)(_Line 1:14_): Represents the event when a Court is created
    - [`CourtUpdatedEvent.cs`](./Domain/Events/Court/CourtUpdatedEvent.cs)(_Line 1:12_): Represents the event when a Court is changed


- **ClubEvents**:
    - [`ClubCreatedEvent.cs`](./Domain/Events/Club/ClubCreatedEvent.cs)(_Line 1:13_): Represents the event when a Club is created
    - [`ClubDeletedEvent.cs`](./Domain/Events/Club/ClubDeletedEvent.cs)(_Line 1:16_): Represents the event when a Club is deleted
    - [`ClubNameChangedEvent.cs`](./Domain/Events/Club/ClubNameChangedEvent.cs)(_Line 1:6_): Represents the event when the name of a Club is changed
    - [`ClubLockedEvent.cs`](./Domain/Events/Club/ClubLockedEvent.cs)(_Line 1:6_): Represents the event when a Club is locked
    - [`ClubUnlockedEvent.cs`](./Domain/Events/Club/ClubUnlockedEvent.cs)(_Line 1:6_): Represents the event when a Club is unlocked


The events are applied to the entities in the `apply` methods, the implementation location can be found under [Domain Driven Design](#domain-driven-design).

#### Idempotent Events
In the PlayOfferService, the idempotency of all events is guaranteed!

All events which were read from the redis stream and were processed by the `EventHandlers` are saved into the `AppliedEvents` table in the read side database. This allows us to check if a received event was already processed and therefore can be ignored.
Therefore the outcome of all events won't change if they are processed multiple times.

### Authentication and Authorization
TODO: Add file/line numbers, describe how it is implemented


### Optimistic Locking
In the PlayOfferService, Optimistic Locking is implemented using the `EFCore` and its transaction mechanism. When a request is received, the current amount of events is read and incremented by one.

When the request is processed, the amount of events is read again and compared to the initial amount. If the amount of events has changed unexpectedly during the transaction, a concurrency exception is thrown and the transaction rolled back.

Otherwise the transaction is committed and the changes are saved to the database.

The Optimistic Locking is implemented in the each CommandHandler in the [Commands](./Application/Handlers) folder.

- [`CancelPlayOfferHandler.cs`](./Application/Handlers/CancelPlayOfferHandler.cs)
  - _Line 26:27_
  - _Line 67:75_
- [`JoinPlayOfferHandler.cs`](./Application/Handlers/JoinPlayOfferHandler.cs)
  - _Line 29:30_
  - _Line 88:96_
- [`CreatePlayOfferHandler.cs`](./Application/Handlers/CreatePlayOfferHandler.cs)
  - _Line 69:70_
  - _Line 75:83_

### Domain Driven Design
In the PlayOfferService, DDD is used to model the core domain of the application, which includes the following entities:

- [`PlayOffer.cs`](./Domain/Models/PlayOffer.cs)(_Line 1:81_): Represents a play offer that is created by a member and can be joined by other members
- [`Member.cs`](./Domain/Models/Member.cs)(_Line 1:81_): Represents a member of the platform who can create and join play offers
- [`Reservation.cs`](./Domain/Models/Reservation.cs)(_Line 1:51_): Represents a reservation for a play offer that is created by the court service
- [`Court.cs`](./Domain/Models/Court.cs)(_Line 1:45_): Represents a court that can be reserved for a play offer
- [`Club.cs`](./Domain/Models/Club.cs)(_Line 1:66_): Represents a club that can have multiple courts and members

Since event sourcing was also used each entity implements a `apply` method which is used to apply the events to the entity. It is important to note that the `apply` method is not allowed to fail, as it is used to reconstruct the state of the entity and the correctness of the events is guaranteed by the `CommandHandlers
`.
The implementation for the `apply` methods can be found here:
- [`PlayOffer.cs`](./Domain/Models/PlayOffer.cs)(_Line 23_)
- [`Member.cs`](./Domain/Models/Member.cs)(_Line 17_)
- [`Reservation.cs`](./Domain/Models/Reservation.cs)(_Line 18_)
- [`Court.cs`](./Domain/Models/Court.cs)(_Line 14_)
- [`Club.cs`](./Domain/Models/Club.cs)(_Line 14_)

However, we didn't implement a `process` method in each entity, since the processing of the events is done in the `CommandHandlers`.

### Transaction Log Trailing
In the PlayOfferService, Transaction Log Trailing is implemented using Debezium, which is an open-source platform for change data capture. Debezium captures changes to the PostgreSQL database and publishes them to a Redis Stream.

The Debezium configuration can be found in the [pos_debezium.yml](./debezium-conf/application.properties)(_Line 1:21_) file.
